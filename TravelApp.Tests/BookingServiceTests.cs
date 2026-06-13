using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TravelApp.Models.Enums;
using TravelApp.Services.Booking;

namespace TravelApp.Tests
{
    [TestClass]
    public class BookingServiceTests
    {
        private readonly BookingService _service = new BookingService();

        [TestMethod]
        public void CalculatePrice_ForShortTrip_ReturnsExpectedQuote()
        {
            var quote = _service.CalculatePrice(1000000m, 2);

            Assert.AreEqual(1000000m, quote.GuideFee);
            Assert.AreEqual(2000000m, quote.HotelFee);
            Assert.AreEqual(0m, quote.Discount);
            Assert.AreEqual(150000m, quote.ServiceFee);
            Assert.AreEqual(3150000m, quote.Total);
        }

        [TestMethod]
        public void CalculatePrice_ForLongTrip_AppliesDiscountBeforeServiceFee()
        {
            var quote = _service.CalculatePrice(500000m, 7);

            Assert.AreEqual(3500000m, quote.GuideFee);
            Assert.AreEqual(3500000m, quote.HotelFee);
            Assert.AreEqual(700000m, quote.Discount);
            Assert.AreEqual(315000m, quote.ServiceFee);
            Assert.AreEqual(6615000m, quote.Total);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(31)]
        public void CalculatePrice_WithInvalidDays_Throws(int days)
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => _service.CalculatePrice(100000m, days));
        }

        [TestMethod]
        public void CalculatePrice_WithNegativeHotelPrice_Throws()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => _service.CalculatePrice(-1m, 1));
        }

        [DataTestMethod]
        [DataRow(BookingStatus.Pending, BookingStatus.Accepted, false, true)]
        [DataRow(BookingStatus.Pending, BookingStatus.Rejected, false, true)]
        [DataRow(BookingStatus.Pending, BookingStatus.Cancelled, false, true)]
        [DataRow(BookingStatus.Accepted, BookingStatus.Paid, false, true)]
        [DataRow(BookingStatus.Accepted, BookingStatus.Cancelled, false, true)]
        [DataRow(BookingStatus.Paid, BookingStatus.Completed, false, false)]
        [DataRow(BookingStatus.Paid, BookingStatus.Completed, true, true)]
        [DataRow(BookingStatus.Completed, BookingStatus.Pending, true, false)]
        public void CanChangeStatus_ReturnsExpectedResult(
            BookingStatus current,
            BookingStatus next,
            bool isAdmin,
            bool expected)
        {
            Assert.AreEqual(
                expected,
                BookingService.CanChangeStatus(current, next, isAdmin));
        }

        [TestMethod]
        public void CanChangeStatus_WithSameStatus_ReturnsTrue()
        {
            Assert.IsTrue(
                BookingService.CanChangeStatus(
                    BookingStatus.Cancelled,
                    BookingStatus.Cancelled,
                    false));
        }
    }
}
