using System;
using System.IO;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter the folder path that contains .txt files:");
        string folderPath = Console.ReadLine();

        string[] txtFiles = Directory.GetFiles(folderPath, "*.txt");

        foreach (string file in txtFiles)
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

            Console.WriteLine("\nFile: " + Path.GetFileName(file));
            foreach (var pair in wordCount)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}");
            }
        }
    }
}
