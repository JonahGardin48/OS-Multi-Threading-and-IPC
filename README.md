# OS Multi-Threading & IPC Simulator

A C# / .NET project demonstrating two core operating systems concepts — **multi-threading with synchronization** and **inter-process communication (IPC)** — through practical, scenario-based implementations built and tested independently.

## Projects

### Project A — Multi-Threading Implementation
Simulates a restaurant reservation system where multiple threads compete for shared resources. Demonstrates:
- **Concurrent thread execution** — multiple threads performing simultaneous operations
- **Mutex-based resource protection** — preventing race conditions on shared reservation data
- **Deadlock creation** — intentionally inducing a deadlock to illustrate the conditions under which they occur
- **Deadlock resolution** — implementing strategies to detect and recover from deadlocked states

### Project B — Inter-Process Communication (IPC)
Implements a producer-consumer communication pattern between two separate processes using **named pipes**:
- **Producer process** — writes messages to a named pipe
- **Consumer process** — reads and processes messages from the pipe
- Demonstrates how independent processes communicate without shared memory

## What It Demonstrates

- Thread lifecycle management and synchronization primitives (mutexes)
- The four necessary conditions for deadlock and how to break them
- IPC via named pipes as an alternative to shared memory
- Unit testing concurrent systems in .NET (`ProjectATests`, `ProjectBTests`)

## Language & Tools

- **Language:** C#
- **Platform:** .NET 8.0
- **IDE:** Visual Studio / VS Code
- **OS:** Linux (Ubuntu via WSL) recommended

## How to Run

### Prerequisites

1. **WSL (Windows Subsystem for Linux)** — open PowerShell and run:
   ```
   wsl --install
   ```
   Restart when prompted.

2. **Ubuntu** — available on the Microsoft Store (easiest Linux environment for WSL).

3. **Install .NET SDK 8.0** — in your Ubuntu terminal:
   ```bash
   sudo apt update
   sudo apt install -y wget apt-transport-https
   wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
   sudo dpkg -i packages-microsoft-prod.deb
   sudo apt update
   sudo apt install -y dotnet-sdk-8.0
   ```

4. **Install Git:**
   ```bash
   sudo apt install -y git
   ```

### Clone & Run

```bash
git clone https://github.com/JonahGardin48/OS-Multi-Threading-and-IPC.git
cd OS-Multi-Threading-and-IPC
```

**Run Project A (Multi-Threading):**
```bash
cd ProjectA
dotnet build
dotnet run
```

**Run Project B (IPC):**
```bash
cd ProjectB
dotnet build
dotnet run
```

### Running Unit Tests

**Test Project A:**
```bash
cd ProjectATests
dotnet build
dotnet test
```

**Test Project B:**
```bash
cd ProjectBTests
dotnet build
dotnet test
```
