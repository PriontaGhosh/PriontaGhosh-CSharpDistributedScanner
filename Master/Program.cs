using System;
using System.IO.Pipes;
using System.Text;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        Thread agentAThread = new Thread(() => HandlePipe("agentApipe", "AgentA"));
        Thread agentBThread = new Thread(() => HandlePipe("agentBpipe", "AgentB"));

        agentAThread.Start();
        agentBThread.Start();

        agentAThread.Join();
        agentBThread.Join();
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
