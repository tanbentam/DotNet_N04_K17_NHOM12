using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TravelApp.Data.Repositories;
using TravelApp.Models;
using TravelApp.Services;
using TravelApp.Utils;

namespace TravelApp.Tests
{
    [TestClass]
    public class AuthServiceTests
    {
        [TestMethod]
        public async Task LoginAsync_WithCorrectPassword_ReturnsUser()
        {
            var expectedUser = new UserModel
            {
                Id = 7,
                Email = "user@example.com",
                PasswordHash = PasswordHelper.HashPassword("correct-password")
            };
            var repository = new FakeUserRepository
            {
                UserToFind = expectedUser
            };
            var service = new AuthService(repository);

            var result = await service.LoginAsync(
                expectedUser.Email,
                "correct-password");

            Assert.AreSame(expectedUser, result);
            Assert.AreEqual(expectedUser.Email, repository.LastIdentifier);
        }

        [TestMethod]
        public async Task LoginAsync_WithWrongPassword_ReturnsNull()
        {
            var repository = new FakeUserRepository
            {
                UserToFind = new UserModel
                {
                    PasswordHash =
                        PasswordHelper.HashPassword("correct-password")
                }
            };
            var service = new AuthService(repository);

            var result = await service.LoginAsync(
                "user@example.com",
                "wrong-password");

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task LoginAsync_WithMissingCredentials_DoesNotQueryRepository()
        {
            var repository = new FakeUserRepository();
            var service = new AuthService(repository);

            var result = await service.LoginAsync(" ", "password");

            Assert.IsNull(result);
            Assert.AreEqual(0, repository.FindCallCount);
        }

        [TestMethod]
        public async Task RegisterAsync_NormalizesUserAndHashesPassword()
        {
            var repository = new FakeUserRepository
            {
                CreateResult = true
            };
            var service = new AuthService(repository);
            var user = new UserModel
            {
                Email = "  USER@Example.COM ",
                Phone = " 0123456789 ",
                FullName = "  Test User "
            };

            var result = await service.RegisterAsync(user, "password123");

            Assert.IsTrue(result);
            Assert.AreSame(user, repository.CreatedUser);
            Assert.AreEqual("user@example.com", user.Email);
            Assert.AreEqual("0123456789", user.Phone);
            Assert.AreEqual("Test User", user.FullName);
            Assert.IsTrue(
                PasswordHelper.VerifyPassword("password123", user.PasswordHash));
        }

        [TestMethod]
        public async Task RegisterAsync_WithMissingRequiredField_ReturnsFalse()
        {
            var repository = new FakeUserRepository();
            var service = new AuthService(repository);
            var user = new UserModel
            {
                Email = "user@example.com",
                Phone = string.Empty,
                FullName = "Test User"
            };

            var result = await service.RegisterAsync(user, "password123");

            Assert.IsFalse(result);
            Assert.AreEqual(0, repository.CreateCallCount);
        }

        private sealed class FakeUserRepository : IUserRepository
        {
            public UserModel UserToFind { get; set; }
            public UserModel CreatedUser { get; private set; }
            public string LastIdentifier { get; private set; }
            public bool CreateResult { get; set; }
            public int FindCallCount { get; private set; }
            public int CreateCallCount { get; private set; }

            public Task<IReadOnlyList<UserModel>> GetAllAsync()
            {
                return Task.FromResult<IReadOnlyList<UserModel>>(
                    new List<UserModel>());
            }

            public Task<UserModel> FindByIdentifierAsync(string emailOrPhone)
            {
                FindCallCount++;
                LastIdentifier = emailOrPhone;
                return Task.FromResult(UserToFind);
            }

            public Task<bool> CreateAsync(UserModel user)
            {
                CreateCallCount++;
                CreatedUser = user;
                return Task.FromResult(CreateResult);
            }

            public Task<bool> UpdateAsync(
                UserModel user,
                string passwordHash)
            {
                return Task.FromResult(false);
            }

            public Task<bool> DeleteAsync(int userId)
            {
                return Task.FromResult(false);
            }
        }
    }
}
