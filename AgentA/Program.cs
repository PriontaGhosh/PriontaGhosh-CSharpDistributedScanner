using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

class Program
{
// here I will store the word counts
static Dictionary<string, Dictionary<string, int>> fileWordCounts = new();

// used to signal when reading is finished
static ManualResetEvent dataReady = new(false);

// to keep the folder path user gives
static string folderPath = "";

    static void Main(string[] args)
    {
        // run this agent on CPU core 0
        Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << 0);
        Console.WriteLine("Enter the folder path that contains .txt files:");
        folderPath = Console.ReadLine();  // ✅ now updates the global variable

        if (string.IsNullOrWhiteSpace(folderPath))
        {
            Console.WriteLine("No folder path entered. Exiting.");
            return;
        }

// this thread reads all the txt files and counts words
static void ReadFilesAndCountWords()
{
    bool isAgentA = true; // this is AgentA
    string[] allFiles = Directory.GetFiles(folderPath, "*.txt");
    int total = allFiles.Length;
    int mid = total / 2;

    // AgentA will take first half of files
    string[] filesToProcess = isAgentA
        ? allFiles[..mid]
        : allFiles[mid..];

    foreach (string file in filesToProcess)
    {
        string fileName = Path.GetFileName(file);
        string[] words = File.ReadAllText(file)
            .ToLower()
            .Split(new[] { ' ', '\r', '\n', '.', ',', ';', ':', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

        Dictionary<string, int> wordCounts = new();
        foreach (string word in words)
        {
            if (wordCounts.ContainsKey(word))
                wordCounts[word]++;
            else
                wordCounts[word] = 1;
        }

        fileWordCounts[fileName] = wordCounts;
    }

    // let the other thread know that reading is done
    dataReady.Set();
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
