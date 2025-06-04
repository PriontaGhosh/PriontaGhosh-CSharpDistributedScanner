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


    static void HandlePipe(string pipeName, string agentLabel)
    {
        try
        {
            using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In))
            {
                Console.WriteLine($"{agentLabel}: Waiting for connection on pipe '{pipeName}'...");
                pipeServer.WaitForConnection();
                Console.WriteLine($"{agentLabel}: Connected.");

                byte[] buffer = new byte[4096];
                int bytesRead = pipeServer.Read(buffer, 0, buffer.Length);
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                Console.WriteLine($"{agentLabel}: Received Data:\n{receivedMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{agentLabel} Error: {ex.Message}");
        }
    }
}
