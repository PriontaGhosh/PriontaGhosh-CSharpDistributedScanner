using System;
using System.IO.Pipes;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "agentApipe", PipeDirection.Out))
            {
                Console.WriteLine("Connecting to Master...");
                pipeClient.Connect();

                string testMessage = "Hello from AgentA!";
                byte[] messageBytes = Encoding.UTF8.GetBytes(testMessage);

                pipeClient.Write(messageBytes, 0, messageBytes.Length);
                Console.WriteLine("Message sent to Master.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
