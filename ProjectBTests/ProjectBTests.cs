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
            // Arrange
            string expectedMessage = "Hello, this is a test message!";
            string actualMessage = string.Empty;

            using (var server = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
            using (var client = new AnonymousPipeClientStream(PipeDirection.In, server.GetClientHandleAsString()))
            {
                // Act
                var serverTask = Task.Run(() =>
                {
                    using (var writer = new StreamWriter(server))
                    {
                        writer.AutoFlush = true;
                        writer.WriteLine(expectedMessage);
                    }
                });

                var clientTask = Task.Run(() =>
                {
                    using (var reader = new StreamReader(client))
                    {
                        actualMessage = reader.ReadLine() ?? string.Empty;
                    }
                });

                await Task.WhenAll(serverTask, clientTask);
            }

            // Assert
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public async Task ErrorHandlingTest()
        {
            // Arrange
            string expectedMessage = "Hello, this is a test message!";
            string actualMessage = string.Empty;

            using (var server = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
            using (var client = new AnonymousPipeClientStream(PipeDirection.In, server.GetClientHandleAsString()))
            {
                // Act
                var serverTask = Task.Run(() =>
                {
                    using (var writer = new StreamWriter(server))
                    {
                        writer.AutoFlush = true;
                        writer.WriteLine(expectedMessage);
                                                // Simulate pipe closure
                        #if WINDOWS
                                                server.WaitForPipeDrain();
                        #endif
                        server.Dispose();
                    }
                });

                var clientTask = Task.Run(() =>
                {
                    try
                    {
                        using (var reader = new StreamReader(client))
                        {
                            actualMessage = reader.ReadLine() ?? string.Empty;
                        }
                    }
                    catch (IOException ex)
                    {
                        actualMessage = ex.Message;
                    }
                });

                await Task.WhenAll(serverTask, clientTask);
            }

            // Assert
            Assert.Contains("pipe is broken", actualMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task PerformanceBenchmarkingTest()
        {
            // Arrange
            byte[] data = new byte[1024 * 1024]; // 1 MB of data
            new Random().NextBytes(data);
            byte[] receivedData = new byte[data.Length];

            using (var server = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
            using (var client = new AnonymousPipeClientStream(PipeDirection.In, server.GetClientHandleAsString()))
            {
                // Act
                var stopwatch = Stopwatch.StartNew();

                var serverTask = Task.Run(() =>
                {
                    server.Write(data, 0, data.Length);
                });

                var clientTask = Task.Run(() =>
                {
                    int bytesRead = 0;
                    while (bytesRead < data.Length)
                    {
                        bytesRead += client.Read(receivedData, bytesRead, data.Length - bytesRead);
                    }
                });

                await Task.WhenAll(serverTask, clientTask);

                stopwatch.Stop();

                // Assert
                Assert.Equal(data, receivedData);
                Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
            }
        }
    }
}