using System;
using System.Threading;
using Xunit;

namespace ProjectATests
{
    public class ReservationTests
    {
        [Fact]
        public void ConcurrencyTest()
        {
            // Arrange
            bool[] tableReserved = new bool[10];
            Mutex[] tableMutexes = new Mutex[10];
            for (int i = 0; i < tableMutexes.Length; i++)
            {
                tableMutexes[i] = new Mutex();
            }

            // Act
            Thread[] threads = new Thread[10];
            for (int i = 0; i < threads.Length; i++)
            {
                int threadId = i;
                threads[i] = new Thread(() =>
                {
                    MakeReservation(threadId, tableReserved, tableMutexes);
                });
                threads[i].Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            // Assert
            int totalReservations = 0;
            foreach (bool reserved in tableReserved)
            {
                if (reserved) totalReservations++;
            }
            Assert.Equal(10, totalReservations); // Each thread reserves 1 table
        }

        [Fact]
        public void SynchronizationValidationTest()
        {
            // Arrange
            bool[] tableReserved = new bool[10];
            Mutex[] tableMutexes = new Mutex[10];
            for (int i = 0; i < tableMutexes.Length; i++)
            {
                tableMutexes[i] = new Mutex();
            }

            // Act
            Thread[] threads = new Thread[10];
            for (int i = 0; i < threads.Length; i++)
            {
                int threadId = i;
                threads[i] = new Thread(() =>
                {
                    MakeReservation(threadId, tableReserved, tableMutexes);
                });
                threads[i].Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            // Assert
            for (int i = 0; i < tableReserved.Length; i++)
            {
                Assert.True(tableReserved[i], $"Table {i} was not reserved."); // Each table should be reserved once
            }
        }

        [Fact]
        public void StressTest()
        {
            // Arrange
            bool[] tableReserved = new bool[10];
            Mutex[] tableMutexes = new Mutex[10];
            for (int i = 0; i < tableMutexes.Length; i++)
            {
                tableMutexes[i] = new Mutex();
            }

            // Act
            Thread[] threads = new Thread[100];
            for (int i = 0; i < threads.Length; i++)
            {
                int threadId = i;
                threads[i] = new Thread(() =>
                {
                    MakeReservation(threadId, tableReserved, tableMutexes);
                });
                threads[i].Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            // Assert
            int totalReservations = 0;
            foreach (bool reserved in tableReserved)
            {
                if (reserved) totalReservations++;
            }
            Assert.Equal(10, totalReservations); // Each table should be reserved once
        }

        private void MakeReservation(int id, bool[] tableReserved, Mutex[] tableMutexes)
        {
            Random random = new Random();
            bool reserved = false;

            while (!reserved)
            {
                int table = random.Next(0, 10);

                if (tableMutexes[table].WaitOne(1000))
                {
                    try
                    {
                        if (!tableReserved[table])
                        {
                            tableReserved[table] = true;
                            reserved = true;
                        }
                    }
                    finally
                    {
                        tableMutexes[table].ReleaseMutex();
                    }
                }
            }
        }
    }
}