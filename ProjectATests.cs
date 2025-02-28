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
            int[] tables = new int[10];
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
                    MakeReservation(threadId, tables, tableMutexes);
                });
                threads[i].Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            // Assert
            int totalReservations = 0;
            foreach (int table in tables)
            {
                totalReservations += table;
            }
            Assert.Equal(20, totalReservations); // Each thread reserves 2 tables
        }

        [Fact]
        public void SynchronizationValidationTest()
        {
            // Arrange
            int[] tables = new int[10];
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
                    MakeReservation(threadId, tables, tableMutexes);
                });
                threads[i].Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            // Assert
            for (int i = 0; i < tables.Length; i++)
            {
                Assert.True(tables[i] <= 2); // No table should be reserved more than twice
            }
        }

        [Fact]
        public void StressTest()
        {
            // Arrange
            int[] tables = new int[10];
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
                    MakeReservation(threadId, tables, tableMutexes);
                });
                threads[i].Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            // Assert
            int totalReservations = 0;
            foreach (int table in tables)
            {
                totalReservations += table;
            }
            Assert.Equal(200, totalReservations); // Each thread reserves 2 tables
        }

        private void MakeReservation(int id, int[] tables, Mutex[] tableMutexes)
        {
            Random random = new Random();
            int table1 = random.Next(0, 10);
            int table2 = random.Next(0, 10);

            while (table2 == table1)
            {
                table2 = random.Next(0, 10);
            }

            int firstTable = Math.Min(table1, table2);
            int secondTable = Math.Max(table1, table2);

            if (tableMutexes[firstTable].WaitOne(1000))
            {
                try
                {
                    if (tableMutexes[secondTable].WaitOne(1000))
                    {
                        try
                        {
                            tables[table1]++;
                            tables[table2]++;
                        }
                        finally
                        {
                            tableMutexes[secondTable].ReleaseMutex();
                        }
                    }
                }
                finally
                {
                    tableMutexes[firstTable].ReleaseMutex();
                }
            }
        }
    }
}