using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

class Program
{
    // stores the final result of word frequencies per file
    static Dictionary<string, Dictionary<string, int>> results = new();

    static void CountWordsFromFiles(string folderPath)
    {
        Console.WriteLine("AgentB reading files...");

        // get all .txt files
        string[] allFiles = Directory.GetFiles(folderPath, "*.txt");

        // split work: AgentB will take the second half
        int midpoint = allFiles.Length / 2;
        string[] filesToRead = allFiles.Skip(midpoint).ToArray();

        foreach (var file in filesToRead)
        {
            string fileName = Path.GetFileName(file);
            string content = File.ReadAllText(file).ToLower();

            string[] words = content.Split(new char[] {
                ' ', '\n', '\r', '.', ',', ';', ':', '!', '?'
            }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, int> wordFreq = new();

            foreach (string word in words)
            {
                if (!wordFreq.TryAdd(word, 1))
                    wordFreq[word]++;
            }

            results[fileName] = wordFreq;
        }

        Console.WriteLine("Word counting done by AgentB.");
    }

    static void Main(string[] args)
    {
        // run AgentB on CPU core 1
        Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << 1);
        Console.WriteLine("Hello from AgentB (running on CPU core 1)");

        Console.Write("Enter path to .txt files: ");
        string? folder = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(folder))
        {
            Console.WriteLine("No folder path entered. Exiting.");
            return;
        }

        Console.WriteLine("You entered: " + folder);
        CountWordsFromFiles(folder); // call file reading
    }
}
