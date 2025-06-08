### CSharp Distributed File Scanner

## GitHub Repo:

https://github.com/PriontaGhosh/PriontaGhosh-CSharpDistributedScanner

## Project Overview

This project is my final submission for the Object-Oriented Programming course at Vilnius University Šiauliai Academy. As a beginner in C#, I built a distributed file scanner system using three console applications: two agents (AgentA and AgentB) and a Master process. It was hard for me at first because I never used C# before, but I followed everything step by step and committed every part of the progress.

## Objective

To create a distributed system using:

-> Two agent programs to read and count word frequencies in .txt files from a given folder.

-> A master program to collect and merge word counts from both agents using named pipes and multithreading.

-> Each component runs on a separate CPU core using ProcessorAffinity.

## Technologies Used

-> C# (.NET 8.0)

-> Console Applications

-> Named Pipes (NamedPipeClientStream, NamedPipeServerStream)

-> Threads (System.Threading.Thread)

-> CPU Core Assignment (Processor.ProcessorAffinity)

-> Visual Studio Code on Windows

## My Learning Process

I started this project without any C# experience, so every step was new to me. I created the solution using "dotnet new sln", and then added each console app step by step. At first, I had trouble understanding how Named Pipes and threads work, but after testing many times and asking questions, I figured them out.

I also made mistakes like editing the wrong file, forgetting to commit, or missing using directives. But I fixed every issue step by step and learned from them. I also committed each small change and added beginner-style comments in my code.

## Project Structure

CSharpDistributedScanner

-> AgentA (First agent application)

-> AgentB (Second agent application)

-> Master (Master aggregator application)

-> CSharpDistributedScanner.sln

## Communication Flow

Agents (AgentA and AgentB):

-> Ask user for folder path

-> Count word frequencies in their half of .txt files

-> Send results to Master using Named Pipes

Master:

-> Waits on two Named Pipes using threads

-> Receives and merges word data

-> Prints final word index result

## Multithreading

AgentA and AgentB:

-> One thread reads files

-> Another thread sends data after reading is finished

Master:

-> Two threads listen to pipes from AgentA and AgentB

-> Uses "lock" to protect shared dictionary when updating it

## CPU Affinity

Each component runs on a separate CPU core:

-> AgentA → Core 0

-> AgentB → Core 1

-> Master → Core 2

This was done using this line:
Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << X);

## Screenshots

-> I took screenshots of:

-> Program outputs

-> CPU core assignment in Task Manager

-> Each .exe process

-> Final results in terminal

These screenshots are included in the final test report.

## Testing Summary

-> Tested with 2 .txt files

-> Later tested with 4+ files split between both agents

-> Verified final merged results printed correctly

-> CPU affinity checked in Task Manager

-> Verified pipe communication is correct

## Challenges I Faced

-> I didn’t know anything in C# when I started

-> Struggled setting up sln and csproj

-> Made mistakes editing wrong Program.cs

-> Sometimes forgot to commit

-> Named pipe connection errors

-> Missed exe files in Task Manager until scrolling

-> Needed to manually assign CPU cores

-> Got confused about how to divide files between agents

But I fixed everything after lots of testing and following instructions.

## Final Status

-> AgentA and AgentB scan files using threads

-> Master receives and merges correctly using threads

-> Named Pipes working

-> CPU affinity working

-> Everything committed in small parts to GitHub

## Submission Checklist

-> Project zipped with /bin/Debug/ included

-> GitHub link added

-> Test report written with screenshots

-> UML diagram made

-> README file written (this one)
