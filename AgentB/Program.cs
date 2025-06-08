using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        // run AgentB on CPU core 1
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
    }
}
