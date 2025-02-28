using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectB
{
    class ProjectB
    {
        static async Task Main()
        {
            var producerTask = Task.Run(RunProducer);
            var consumerTask = Task.Run(RunConsumer);

            await Task.WhenAll(producerTask, consumerTask);
        }

        static async Task RunProducer()
        {
            using (var pipeServer = new NamedPipeServerStream("ProjectBPipe", PipeDirection.Out, 1, PipeTransmissionMode.Message))
            {
                Console.WriteLine("Producer: Waiting for connection...");
                pipeServer.WaitForConnection();
                Console.WriteLine("Producer: Client connected.");

                using (var writer = new StreamWriter(pipeServer, Encoding.UTF8, leaveOpen: true))
                {
                    writer.AutoFlush = true;
                    for (int i = 1; i <= 10; i++)
                    {
                        string message = $"Message {i}";
                        Console.WriteLine($"Producer: Sending - {message}");
                        await writer.WriteLineAsync(message);
                        await Task.Delay(200); // Simulate processing delay
                    }
                }
            }
        }

        static async Task RunConsumer()
        {
            using (var pipeClient = new NamedPipeClientStream(".", "ProjectBPipe", PipeDirection.In))
            {
                Console.WriteLine("Consumer: Connecting to producer...");
                pipeClient.Connect();
                Console.WriteLine("Consumer: Connected to producer.");

                using (var reader = new StreamReader(pipeClient, Encoding.UTF8))
                {
                    string? message;
                    while ((message = await reader.ReadLineAsync()) != null)
                    {
                        Console.WriteLine($"Consumer: Received - {message}");
                    }
                }
            }
        }
    }
}
