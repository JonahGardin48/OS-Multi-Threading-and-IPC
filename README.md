# OS-Multi-Threading-and-IPC

## Overview

This project demonstrates the concepts of multi-threading and inter-process communication (IPC) using C# and .NET. It consists of two main parts:

1. **Project A: Multi-Threading Implementation**
   - This project showcases the use of threads to perform concurrent operations, resource protection using mutexes, deadlock creation, and deadlock resolution in a restaurant reservation scenario.

2. **Project B: Inter-Process Communication (IPC)**
   - This project demonstrates communication between processes using named pipes. It implements a basic producer-consumer pattern where the producer writes messages to a pipe and the consumer reads messages from the pipe.

## Project A: Multi-Threading Implementation


### Instructions (Windows 10)

1. **Enable WSL**
    - Open your computer's Command Prompt Such As PowerShell
    - Run the following Command and Restart as needed: wsl --install

2. **Install your Virtualization for a Linux Environment**
    *Ubuntu is the easiest option, as it is on the microsoft store

3. **Install the compiler onto the virtualization setup**
    *This repo uses C#, so install .NET SDK by
        *In the Virtualization Terminal, run these commands
            * sudo apt update
            * sudo apt install -y wget apt-transport-https
            * wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
            * sudo dpkg -i packages-microsoft-prod.deb
            * sudo apt update
            *sudo apt install -y dotnet-sdk-8.0

4. **Cloning the Repository**
    *In the Virtualization Terminal, run these commands
        *sudo apt install -y git
        *git clone https://github.com/your-repository/OS-Multi-Threading-and-IPC.git    

5. **Build the Project**
    *cd into the directory with : cd OS-Multi-Threading-and-IPC
    *Build and Run Project A
        *Navigate in the terminal : cd ProjectA
        *Build the Project: dotnet build
        *Run the Project: dotnet Run
    *Build and Run Project b
        *Navigate in the terminal : cd ProjectB
        *Build the Project: dotnet build
        *Run the Project: dotnet Run

6. **Running Unit Tests**
    *Build and Test Project A
        *Navigate in the terminal: cd ../ProjectATests
        *Build the tests: dotnet build
        *Run the tests: dotnet test
    *Build and Test Project B
        *Navigate in the terminal: cd ../ProjectBTests
        *Build the tests: dotnet build
        *Run the tests: dotnet test   