using System;
using System.Threading;

namespace ProjectA
{
    class ProjectA
    {
        private static readonly Mutex[] tableMutexes = new Mutex[10];
        private static readonly bool[] tableReserved = new bool[10];

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

        private static readonly ThreadLocal<Random> threadLocalRandom = new ThreadLocal<Random>(() => new Random());

        static void MakeReservation(object? threadId)
        {
            int id = (int)threadId!;
            Random random = threadLocalRandom.Value!;            int table = random.Next(0, 10);

            Console.WriteLine($"Thread {id} attempting to reserve table {table}");

            if (tableMutexes[table].WaitOne(1000))
            {
                try
                {
                    // Check if the table is already reserved
                    if (!tableReserved[table])
                    {
                        // Simulate reservation process
                        Console.WriteLine($"Thread {id} reserved table {table}");
                        tableReserved[table] = true;
                    }
                    else
                    {
                        Console.WriteLine($"Thread {id} found table {table} already reserved");
                    }
                }
                finally
                {
                    tableMutexes[table].ReleaseMutex();
                }
            }
            else
            {
                Console.WriteLine($"Thread {id} timeout waiting for table {table}");
            }
        }
    }
}