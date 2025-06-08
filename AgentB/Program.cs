using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Collections.Generic;


class Program
{
    // stores the final result of word frequencies per file
    static Dictionary<string, Dictionary<string, int>> results = new();

    static ManualResetEvent dataReady = new(false);

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

        /*
             // print what was counted for each file
        foreach (var file in results)
        {
            Console.WriteLine($"\nFile: {file.Key}");
            foreach (var word in file.Value)
            {
                Console.WriteLine($"{word.Key}: {word.Value}");
            }
        }
        */


        Console.WriteLine("Word counting done by AgentB.");


        // signal the sender thread that is done
        dataReady.Set();
    }

    static void SendDataToMaster()
    {
        Console.WriteLine("Connecting to Master...");


        // wait until reading finishes
        dataReady.WaitOne();


        try
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "agentBpipe", PipeDirection.Out))
            {
                pipeClient.Connect();

                StringBuilder sb = new StringBuilder();

                foreach (var fileEntry in results)
                {
                    foreach (var wordEntry in fileEntry.Value)
                    {
                        sb.AppendLine($"{fileEntry.Key}:{wordEntry.Key}:{wordEntry.Value}");
                    }
                }

                byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
                pipeClient.Write(buffer, 0, buffer.Length);

                Console.WriteLine("AgentB sent word data to Master.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to send data: " + ex.Message);
        }
    }

static void Main(string[] args)
{
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

    Thread readThread = new Thread(() => CountWordsFromFiles(folder));
    Thread sendThread = new Thread(SendDataToMaster);

    readThread.Start();
    sendThread.Start();

    readThread.Join();
    sendThread.Join();
}


}
