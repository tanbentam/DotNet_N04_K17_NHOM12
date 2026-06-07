using System;
using System.Security.Cryptography;

namespace TravelApp.Utils
{
    public static class PasswordHelper
    {
        private const int Iterations = 10000;
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const string Algorithm = "PBKDF2";

        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be empty");
            }

            var salt = new byte[SaltSize];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }

            var hash = DeriveHash(password, salt, Iterations);
            return string.Join(
                "$",
                Algorithm,
                Iterations,
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash));
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(storedHash))
            {
                return false;
            }

            var parts = storedHash.Split('$');
            if (parts.Length != 4 ||
                !string.Equals(parts[0], Algorithm, StringComparison.Ordinal) ||
                !int.TryParse(parts[1], out var iterations) ||
                iterations <= 0)
            {
                return false;
            }

            try
            {
                var salt = Convert.FromBase64String(parts[2]);
                var expectedHash = Convert.FromBase64String(parts[3]);
                var actualHash = DeriveHash(password, salt, iterations);

                return FixedTimeEquals(actualHash, expectedHash);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static byte[] DeriveHash(
            string password,
            byte[] salt,
            int iterations)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(HashSize);
            }
        }

        private static bool FixedTimeEquals(byte[] left, byte[] right)
        {
            if (left == null || right == null || left.Length != right.Length)
            {
                return false;
            }

            var difference = 0;
            for (var index = 0; index < left.Length; index++)
            {
                difference |= left[index] ^ right[index];
            }

            return difference == 0;
        }
    }
}
