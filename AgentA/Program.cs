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
