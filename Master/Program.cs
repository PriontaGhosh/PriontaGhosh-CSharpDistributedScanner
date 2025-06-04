using System;
using System.IO.Pipes;
using System.Text;
using System.Threading;

class Program
{
   static void Main(string[] args)
{
    // this will hold all word data from both agents
    var mergedWordCounts = new Dictionary<string, Dictionary<string, int>>();

    // to avoid problems when 2 threads update at same time
    object lockObj = new object();

// run 2 threads for agentA and agentB to listen to their pipes
    Thread agentAThread = new Thread(() => HandlePipe("agentApipe", "AgentA", mergedWordCounts, lockObj));
    Thread agentBThread = new Thread(() => HandlePipe("agentBpipe", "AgentB", mergedWordCounts, lockObj));

    agentAThread.Start();
    agentBThread.Start();

    agentAThread.Join();
    agentBThread.Join();


// final result after both agents done sending
    Console.WriteLine("\n=== Final Merged Word Count ===");
    foreach (var fileEntry in mergedWordCounts)
    {
        foreach (var wordEntry in fileEntry.Value)
        {
            Console.WriteLine($"{fileEntry.Key}:{wordEntry.Key}:{wordEntry.Value}");
        }
    }
}


// this method will run in a thread and listen to a named pipe for one agent
// it takes pipe name (like "agentApipe") and a label to know which agent is sending
static void HandlePipe(string pipeName, string agentLabel, Dictionary<string, Dictionary<string, int>> sharedData, object lockObj)
{
    try
    {
        // this will open a pipe server for receiving data
        using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In))
        {
            Console.WriteLine($"{agentLabel}: Waiting for connection on pipe '{pipeName}'...");
            pipeServer.WaitForConnection();
            Console.WriteLine($"{agentLabel}: Connected.");

            // read the incoming data from the agent
            byte[] buffer = new byte[4096];
            int bytesRead = pipeServer.Read(buffer, 0, buffer.Length);

            // convert the byte data into a string message
            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            // split the message into lines, each line has file:word:count
            string[] lines = receivedMessage.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            // use lock to avoid data conflicts when both threads update the same dictionary
            lock (lockObj)
            {
                foreach (string line in lines)
                {
                    // split line into file, word, and count
                    string[] parts = line.Trim().Split(':');
                    if (parts.Length == 3)
                    {
                        string file = parts[0];
                        string word = parts[1];

                        // convert count to number (ignore line if it's not a number)
                        if (!int.TryParse(parts[2], out int count)) continue;

                        // if this file is not in the dictionary, add it
                        if (!sharedData.ContainsKey(file))
                            sharedData[file] = new Dictionary<string, int>();

                        // add or update the word count
                        if (sharedData[file].ContainsKey(word))
                            sharedData[file][word] += count;
                        else
                            sharedData[file][word] = count;
                    }
                }
            }

            Console.WriteLine($"{agentLabel}: Data processed.");
        }
    }
    catch (Exception ex)
    {
        // if anything goes wrong, print the error
        Console.WriteLine($"{agentLabel} Error: {ex.Message}");
    }
}


}
