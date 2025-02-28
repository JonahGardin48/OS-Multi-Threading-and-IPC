using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ProjectBTests
{
    public class IPCTests
    {
        [Fact]
        public async Task DataIntegrityTest()
        {
            string expectedMessage = "Test Message";
            string actualMessage = string.Empty;

            using (var server = new NamedPipeServerStream("TestPipe", PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
            using (var client = new NamedPipeClientStream(".", "TestPipe", PipeDirection.In, PipeOptions.Asynchronous))
            {
                var serverTask = Task.Run(async () =>
                {
                    await server.WaitForConnectionAsync();
                    byte[] messageBytes = Encoding.UTF8.GetBytes(expectedMessage + "\n");
                    await server.WriteAsync(messageBytes, 0, messageBytes.Length);
                    await server.FlushAsync();
                });

                var clientTask = Task.Run(async () =>
                {
                    await client.ConnectAsync();
                    byte[] buffer = new byte[1024];
                    int bytesRead = await client.ReadAsync(buffer, 0, buffer.Length);
                    actualMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                });

                await Task.WhenAll(serverTask, clientTask);
            }

            Assert.Equal(expectedMessage, actualMessage);
        }

       [Fact]
public async Task ErrorHandlingTest()
{
    string errorMessage = string.Empty;

    using (var server = new NamedPipeServerStream("ErrorPipe", PipeDirection.Out, 1, PipeTransmissionMode.Byte))
    using (var client = new NamedPipeClientStream(".", "ErrorPipe", PipeDirection.In))
    {
        var serverTask = Task.Run(async () =>
        {
            await server.WaitForConnectionAsync();
            byte[] message = Encoding.UTF8.GetBytes("Partial Message");
            await server.WriteAsync(message, 0, message.Length); // Send partial data
            await Task.Delay(50); // Give the client a moment to start reading
            server.Close(); // Hard-close the pipe
        });

        var clientTask = Task.Run(async () =>
        {
            try
            {
                await client.ConnectAsync();
                byte[] buffer = new byte[1024];
                int bytesRead = await client.ReadAsync(buffer, 0, buffer.Length); // This should fail
                errorMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
            catch (IOException ex)
            {
                errorMessage = ex.Message; // Capture error message
            }
        });

        await Task.WhenAll(serverTask, clientTask);
    }

    Assert.False(string.IsNullOrEmpty(errorMessage)); // Ensure an error occurs
}



        [Fact]
        public async Task PerformanceBenchmarkingTest()
        {
            byte[] data = new byte[1024 * 1024]; // 1MB of random data
            new Random().NextBytes(data);
            byte[] receivedData = new byte[data.Length];

            using (var server = new NamedPipeServerStream("BenchmarkPipe", PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
            using (var client = new NamedPipeClientStream(".", "BenchmarkPipe", PipeDirection.In, PipeOptions.Asynchronous))
            {
                var stopwatch = Stopwatch.StartNew();

                var serverTask = Task.Run(async () =>
                {
                    await server.WaitForConnectionAsync();
                    await server.WriteAsync(data, 0, data.Length);
                    await server.FlushAsync();
                });

                var clientTask = Task.Run(async () =>
                {
                    await client.ConnectAsync();
                    int bytesRead = 0;
                    while (bytesRead < data.Length)
                    {
                        bytesRead += await client.ReadAsync(receivedData, bytesRead, data.Length - bytesRead);
                    }
                });

                await Task.WhenAll(serverTask, clientTask);
                stopwatch.Stop();

                Assert.Equal(data, receivedData);
                Console.WriteLine($"Performance Test Time: {stopwatch.ElapsedMilliseconds} ms");
            }
        }
    }
}
