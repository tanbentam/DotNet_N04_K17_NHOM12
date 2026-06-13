using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TravelApp.Models;
using TravelApp.Models.Enums;

namespace TravelApp.Tests
{
    [TestClass]
    public class BookingModelTests
    {
        [TestMethod]
        public void CompletionDate_IncludesLastTourDay()
        {
            var booking = new BookingModel
            {
                StartDate = new DateTime(2026, 6, 10),
                Nights = 3
            };

            Assert.AreEqual(
                new DateTime(2026, 6, 12),
                booking.CompletionDate);
        }

        [TestMethod]
        public void CompletionDate_WithInvalidDuration_UsesOneDay()
        {
            var booking = new BookingModel
            {
                StartDate = new DateTime(2026, 6, 10),
                Nights = 0
            };

            Assert.AreEqual(
                new DateTime(2026, 6, 10),
                booking.CompletionDate);
        }

        [TestMethod]
        public void HasPendingGuideCancellation_WhenUnresolved_ReturnsTrue()
        {
            var booking = new BookingModel
            {
                GuideCancellationRequestedAt = DateTime.Now,
                GuideCancellationResolvedAt = null
            };

            Assert.IsTrue(booking.HasPendingGuideCancellation);
        }

        [TestMethod]
        public void CanGuideRequestCancellation_ForFutureAcceptedBooking_ReturnsTrue()
        {
            var booking = new BookingModel
            {
                Status = BookingStatus.Accepted,
                StartDate = DateTime.Today.AddDays(1)
            };

            Assert.IsTrue(booking.CanGuideRequestCancellation);
        }

        [TestMethod]
        public void CanGuideRequestCancellation_WhenRequestPending_ReturnsFalse()
        {
            var booking = new BookingModel
            {
                Status = BookingStatus.Accepted,
                StartDate = DateTime.Today.AddDays(1),
                GuideCancellationRequestedAt = DateTime.Now
            };

            Assert.IsFalse(booking.CanGuideRequestCancellation);
        }
    }
}
