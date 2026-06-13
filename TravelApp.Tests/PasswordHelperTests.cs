using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TravelApp.Utils;

namespace TravelApp.Tests
{
    [TestClass]
    public class PasswordHelperTests
    {
        [TestMethod]
        public void HashPassword_WithValidPassword_CreatesVerifiableHash()
        {
            const string password = "StrongPassword123!";

            var hash = PasswordHelper.HashPassword(password);

            Assert.IsTrue(hash.StartsWith("PBKDF2$10000$"));
            Assert.IsTrue(PasswordHelper.VerifyPassword(password, hash));
        }

        [TestMethod]
        public void HashPassword_UsesRandomSalt()
        {
            const string password = "StrongPassword123!";

            var firstHash = PasswordHelper.HashPassword(password);
            var secondHash = PasswordHelper.HashPassword(password);

            Assert.AreNotEqual(firstHash, secondHash);
        }

        [TestMethod]
        public void HashPassword_WithEmptyPassword_Throws()
        {
            Assert.ThrowsException<ArgumentException>(
                () => PasswordHelper.HashPassword(string.Empty));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("not-a-password-hash")]
        [DataRow("PBKDF2$invalid$salt$hash")]
        [DataRow("OTHER$10000$c2FsdA==$aGFzaA==")]
        public void VerifyPassword_WithMalformedHash_ReturnsFalse(string hash)
        {
            Assert.IsFalse(PasswordHelper.VerifyPassword("password", hash));
        }

        [TestMethod]
        public void VerifyPassword_WithWrongPassword_ReturnsFalse()
        {
            var hash = PasswordHelper.HashPassword("correct-password");

            Assert.IsFalse(
                PasswordHelper.VerifyPassword("wrong-password", hash));
        }
    }
}
