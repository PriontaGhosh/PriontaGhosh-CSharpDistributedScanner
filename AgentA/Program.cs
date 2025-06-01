using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter the folder path that contains .txt files:");
        string folderPath = Console.ReadLine();

        string[] txtFiles = Directory.GetFiles(folderPath, "*.txt");

        Console.WriteLine("Found " + txtFiles.Length + " .txt files:");
        foreach (string file in txtFiles)
        {
            Console.WriteLine(file);
        }
    }
}
