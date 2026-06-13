using Microsoft.VisualStudio.TestTools.UnitTesting;
using TravelApp.Utils;

namespace TravelApp.Tests
{
    [TestClass]
    public class ValidationHelperTests
    {
        [DataTestMethod]
        [DataRow("user@example.com")]
        [DataRow("first.last+tag@example.co.uk")]
        public void IsValidEmail_WithValidEmail_ReturnsTrue(string email)
        {
            Assert.IsTrue(ValidationHelper.IsValidEmail(email));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("user")]
        [DataRow("user@")]
        [DataRow("@example.com")]
        [DataRow("user@example")]
        public void IsValidEmail_WithInvalidEmail_ReturnsFalse(string email)
        {
            Assert.IsFalse(ValidationHelper.IsValidEmail(email));
        }

        [DataTestMethod]
        [DataRow("0123456789")]
        [DataRow("0987654321")]
        public void IsValidPhoneNumber_WithTenDigits_ReturnsTrue(string phone)
        {
            Assert.IsTrue(ValidationHelper.IsValidPhoneNumber(phone));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("123456789")]
        [DataRow("12345678901")]
        [DataRow("01234abcde")]
        [DataRow("012 345 6789")]
        public void IsValidPhoneNumber_WithInvalidValue_ReturnsFalse(
            string phone)
        {
            Assert.IsFalse(ValidationHelper.IsValidPhoneNumber(phone));
        }
    }
}
