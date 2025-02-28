using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace ProjectB
{
    class ProjectB
    {
        static async Task Main(string[] args)
        {
            // Handle nullable string example
            string? nullableString = GetNullableString();
            string nonNullableString = nullableString ?? "default value";
            Console.WriteLine(nonNullableString);

            // Run producer and consumer tasks
            var producerTask = Task.Run(() => RunProducer());
            var consumerTask = Task.Run(() => RunConsumer());

            await Task.WhenAll(producerTask, consumerTask);
        }

        static string? GetNullableString()
        {
            // Simulate a method that might return null
            return null;
        }

        static async Task RunProducer()
        {
            using (var pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.Out))
            {
                Console.WriteLine("Named pipe server waiting for connection...");
                pipeServer.WaitForConnection();
                Console.WriteLine("Client connected.");

                using (var writer = new StreamWriter(pipeServer))
                {
                    writer.AutoFlush = true;
                    for (int i = 0; i < 10; i++)
                    {
                        string message = $"Message {i}";
                        Console.WriteLine($"Sending: {message}");
                        await writer.WriteLineAsync(message);
                        await Task.Delay(500); // Simulate some work
                    }
                }
            }
        }

        static async Task RunConsumer()
        {
            using (var pipeClient = new NamedPipeClientStream(".", "testpipe", PipeDirection.In))
            {
                Console.WriteLine("Connecting to named pipe server...");
                pipeClient.Connect();
                Console.WriteLine("Connected to server.");

                using (var reader = new StreamReader(pipeClient))
                {
                    string? message;
                    while ((message = await reader.ReadLineAsync()) != null)
                    {
                        Console.WriteLine($"Received: {message}");
                    }
                }
            }
        }
    }
}