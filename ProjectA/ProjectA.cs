using System;
using System.Collections.Generic;
using System.Threading;

namespace ProjectA
{
    public class ReservationSystem
    {
        private static Mutex[] tableMutexes = new Mutex[10];
        private static bool[] tableReserved = new bool[10];
        private static HashSet<int> reservedTables = new HashSet<int>(); 
        private static readonly object lockObj = new object(); 
        private static readonly ThreadLocal<Random> threadLocalRandom = new(() => new Random());

        static ReservationSystem()
        {
            Initialize();
        }

        private static void Initialize()
        {
            for (int i = 0; i < tableMutexes.Length; i++)
            {
                tableMutexes[i] = new Mutex();
                tableReserved[i] = false;
            }

            lock (lockObj)
            {
                reservedTables.Clear();
            }
        }

        public static void ResetSystem()
        {
            lock (lockObj)
            {
                Initialize();
            }
        }

        public static void MakeReservation(object? threadId)
        {
            int id = (int)threadId!;
            Random random = threadLocalRandom.Value!;

            while (true) // Keep trying until a reservation succeeds
            {
                int table;
                lock (lockObj)
                {
                    List<int> availableTables = new List<int>();
                    for (int i = 0; i < tableReserved.Length; i++)
                    {
                        if (!tableReserved[i])
                        {
                            availableTables.Add(i);
                        }
                    }

                    if (availableTables.Count == 0)
                    {
                        Console.WriteLine($"Thread {id} found no available tables.");
                        return; // Exit if all tables are already reserved
                    }

                    table = availableTables[random.Next(availableTables.Count)];
                }

                Console.WriteLine($"Thread {id} attempting to reserve table {table}");

                if (tableMutexes[table].WaitOne()) // Ensure mutex lock
                {
                    try
                    {
                        lock (lockObj)
                        {
                            if (!tableReserved[table])
                            {
                                tableReserved[table] = true;
                                reservedTables.Add(table);
                                Console.WriteLine($"Thread {id} successfully reserved table {table}");
                                return; // Exit loop after successful reservation
                            }
                        }
                    }
                    finally
                    {
                        tableMutexes[table].ReleaseMutex();
                    }
                }
            }
        }

        public static bool[] GetTableReservations()
        {
            lock (lockObj)
            {
                return (bool[])tableReserved.Clone();
            }
        }

        public static int GetTotalReservations()
        {
            lock (lockObj)
            {
                return reservedTables.Count;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Restaurant Reservation System...\n");

            // Create and start 10 threads for reservations
            Thread[] threads = new Thread[10];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(ReservationSystem.MakeReservation);
                threads[i].Start(i);
            }

            // Wait for all threads to complete
            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            Console.WriteLine("\nAll reservations completed.");
            Console.WriteLine($"Total tables reserved: {ReservationSystem.GetTotalReservations()}");

            // Display final reservation status
            bool[] reservations = ReservationSystem.GetTableReservations();
            for (int i = 0; i < reservations.Length; i++)
            {
                Console.WriteLine($"Table {i}: {(reservations[i] ? "Reserved" : "Available")}");
            }
        }
    }
}
