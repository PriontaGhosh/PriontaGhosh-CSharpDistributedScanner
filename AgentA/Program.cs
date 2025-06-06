﻿using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        // run this agent on CPU core 0
Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << 0);
        Console.WriteLine("Enter the folder path that contains .txt files:");
        string? folderPath = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(folderPath))
        {
            Console.WriteLine("No folder path entered. Exiting.");
            return;
        }
        // split the file list between AgentA and AgentB so both don't scan same files

      bool isAgentA = true; // manually define this for AgentA

string[] allFiles = Directory.GetFiles(folderPath, "*.txt");
int total = allFiles.Length;
int mid = total / 2;

string[] filesToProcess;

if (isAgentA)
    filesToProcess = allFiles.Take(mid).ToArray(); // AgentA takes first half
else
    filesToProcess = allFiles.Skip(mid).ToArray();


        Dictionary<string, Dictionary<string, int>> fileWordCounts = new Dictionary<string, Dictionary<string, int>>();

        foreach (string file in filesToProcess)
        {
            string content = File.ReadAllText(file);
            string[] words = content.Split(new char[] { ' ', '.', ',', '?', '!', ';', ':', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, int> wordCount = new Dictionary<string, int>();

            foreach (string word in words)
            {
                string cleanedWord = word.ToLower();
                if (wordCount.ContainsKey(cleanedWord))
                    wordCount[cleanedWord]++;
                else
                    wordCount[cleanedWord] = 1;
            }

            fileWordCounts[Path.GetFileName(file)] = wordCount;
        }

        try
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "agentApipe", PipeDirection.Out))
            {
                Console.WriteLine("Connecting to Master...");
                pipeClient.Connect();

                StringBuilder sb = new StringBuilder();
                foreach (var fileEntry in fileWordCounts)
                {
                    foreach (var wordEntry in fileEntry.Value)
                    {
                        sb.AppendLine($"{fileEntry.Key}:{wordEntry.Key}:{wordEntry.Value}");
                    }
                }

                byte[] messageBytes = Encoding.UTF8.GetBytes(sb.ToString());
                pipeClient.Write(messageBytes, 0, messageBytes.Length);
                Console.WriteLine("Word count data sent to Master.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
