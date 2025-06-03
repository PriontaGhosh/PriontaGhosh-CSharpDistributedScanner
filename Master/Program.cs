using System;
using System.IO.Pipes;
using System.Text;

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
}
