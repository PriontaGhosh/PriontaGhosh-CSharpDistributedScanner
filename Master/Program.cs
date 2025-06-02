using System;
using System.IO.Pipes;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("agentApipe", PipeDirection.In))
            {
                Console.WriteLine("Waiting for AgentA to connect...");

                pipeServer.WaitForConnection();
                Console.WriteLine("AgentA connected.");

                byte[] buffer = new byte[1024];
                int bytesRead = pipeServer.Read(buffer, 0, buffer.Length);

                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received from AgentA: " + receivedMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in Master: " + ex.Message);
        }
    }
}
