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

        [TestMethod]
        public void RefundStatusDisplay_WhenRequestPending_ReturnsWaiting()
        {
            var booking = new BookingModel
            {
                RefundRequestedAt = DateTime.Now
            };

            Assert.IsTrue(booking.HasPendingRefundRequest);
            Assert.AreEqual("Đang chờ duyệt", booking.RefundStatusDisplay);
        }

        [DataTestMethod]
        [DataRow(true, "Đã hoàn tiền")]
        [DataRow(false, "Đã từ chối")]
        public void RefundStatusDisplay_WhenResolved_ReturnsExpected(
            bool approved,
            string expected)
        {
            var booking = new BookingModel
            {
                RefundRequestedAt = DateTime.Now.AddHours(-1),
                RefundResolvedAt = DateTime.Now,
                RefundApproved = approved
            };

            Assert.IsFalse(booking.HasPendingRefundRequest);
            Assert.AreEqual(expected, booking.RefundStatusDisplay);
        }

        [TestMethod]
        public void CanUserCancel_PaidFutureBookingWithoutRequest_ReturnsTrue()
        {
            var booking = new BookingModel
            {
                Status = BookingStatus.Paid,
                StartDate = DateTime.Today.AddDays(1)
            };

            Assert.IsTrue(booking.CanUserCancel);
            Assert.AreEqual(
                "Yêu cầu hoàn tiền",
                booking.UserCancellationAction);
        }

        [TestMethod]
        public void CanUserCancel_WhenRefundPending_ReturnsFalse()
        {
            var booking = new BookingModel
            {
                Status = BookingStatus.Paid,
                StartDate = DateTime.Today.AddDays(1),
                RefundRequestedAt = DateTime.Now
            };

            Assert.IsFalse(booking.CanUserCancel);
        }
    }
}
