using System;
using System.Threading;
using Xunit;
using ProjectA;

namespace ProjectATests
{
    public class ReservationTests
    {
        [Fact]
        public void ConcurrencyTest()
        {
            // Arrange
            ReservationSystem.ResetSystem();

            // Act
            Thread[] threads = new Thread[10];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(ReservationSystem.MakeReservation);
                threads[i].Start(i);
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            // Assert
            Assert.Equal(10, ReservationSystem.GetTotalReservations()); // Expect 10 successful reservations
        }

        [Fact]
        public void SynchronizationValidationTest()
        {
            // Arrange
            ReservationSystem.ResetSystem();

            // Act
            Thread[] threads = new Thread[10];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(ReservationSystem.MakeReservation);
                threads[i].Start(i);
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            // Assert
            bool[] reservations = ReservationSystem.GetTableReservations();
            int reservedTables = 0;
            for (int i = 0; i < reservations.Length; i++)
            {
                if (reservations[i]) reservedTables++;
            }
            Assert.Equal(10, reservedTables); // Expect exactly 10 reservations
        }

        [Fact]
        public void StressTest()
        {
            // Arrange
            ReservationSystem.ResetSystem();

            // Act
            Thread[] threads = new Thread[100];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(ReservationSystem.MakeReservation);
                threads[i].Start(i);
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            // Assert
            Assert.Equal(10, ReservationSystem.GetTotalReservations()); // Maximum tables available is 10
        }
    }
}
