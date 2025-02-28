using System;
using System.Threading;

namespace ProjectA
{
    class ProjectA
    {
        private static readonly Mutex[] tableMutexes = new Mutex[10];
        private static readonly int[] tables = new int[10];

        static void Main(string[] args)
        {
            // Initialize mutexes for each table
            for (int i = 0; i < tableMutexes.Length; i++)
            {
                tableMutexes[i] = new Mutex();
            }

            // Create and start 10 threads for reservations
            Thread[] threads = new Thread[10];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(MakeReservation);
                threads[i].Start(i);
            }

            // Wait for all threads to complete
            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            Console.WriteLine("All reservations completed.");
        }

        static void MakeReservation(object? threadId)
        {
            int id = (int)threadId!;
            Random random = new Random();
            int table1 = random.Next(0, 10);
            int table2 = random.Next(0, 10);

            // Ensure different tables are selected
            while (table2 == table1)
            {
                table2 = random.Next(0, 10);
            }

            Console.WriteLine($"Thread {id} attempting to reserve tables {table1} and {table2}");

            // Acquire locks in a consistent order to prevent deadlocks
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
                            // Simulate reservation process
                            Console.WriteLine($"Thread {id} reserved tables {table1} and {table2}");
                            lock (tables)
                            {
                                tables[table1]++;
                                tables[table2]++;
                            }
                        }
                        finally
                        {
                            tableMutexes[secondTable].ReleaseMutex();
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Thread {id} timeout waiting for table {secondTable}");
                    }
                }
                finally
                {
                    tableMutexes[firstTable].ReleaseMutex();
                }
            }
            else
            {
                Console.WriteLine($"Thread {id} timeout waiting for table {firstTable}");
            }
        }
    }
}