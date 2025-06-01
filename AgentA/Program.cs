using System;
using System.IO;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter the folder path that contains .txt files:");
        string? folderPath = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(folderPath))
        {
            Console.WriteLine("Folder path is empty. Exiting.");
            return;
        }

        string[] txtFiles = Directory.GetFiles(folderPath, "*.txt");

        Console.WriteLine("Found " + txtFiles.Length + " .txt files");

        foreach (string file in txtFiles)
        {
            Console.WriteLine($"\nReading file: {file}");

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

            Console.WriteLine("Word frequencies in " + Path.GetFileName(file) + ":");
            foreach (var pair in wordCount)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}");
            }
        }
    }
}
