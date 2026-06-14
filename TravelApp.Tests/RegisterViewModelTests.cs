using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TravelApp.Models;
using TravelApp.Services.Contracts;
using TravelApp.ViewModels.Authentication;

namespace TravelApp.Tests
{
    [TestClass]
    public class RegisterViewModelTests
    {
        [TestMethod]
        public async Task RegisterCommand_UsesFullNameAndClearsSuccessfulForm()
        {
            var authService = new FakeAuthService();
            var viewModel = new RegisterViewModel(authService)
            {
                Email = "user@example.com",
                PhoneNumber = "0123456789",
                Password = "password123",
                FullName = "Test User"
            };

            await viewModel.RegisterCommand.ExecuteAsync(null);

            Assert.IsNotNull(authService.RegisteredUser);
            Assert.AreEqual("Test User", authService.RegisteredUser.FullName);
            Assert.AreEqual("password123", authService.RegisteredPassword);
            Assert.AreEqual(string.Empty, viewModel.Email);
            Assert.AreEqual(string.Empty, viewModel.PhoneNumber);
            Assert.AreEqual(string.Empty, viewModel.Password);
            Assert.AreEqual(string.Empty, viewModel.FullName);
        }

        [TestMethod]
        public async Task RegisterCommand_WithMissingFullName_DoesNotRegister()
        {
            var authService = new FakeAuthService();
            var viewModel = new RegisterViewModel(authService)
            {
                Email = "user@example.com",
                PhoneNumber = "0123456789",
                Password = "password123",
                FullName = " "
            };

            await viewModel.RegisterCommand.ExecuteAsync(null);

            Assert.IsNull(authService.RegisteredUser);
            StringAssert.Contains(viewModel.ErrorMessage, "họ tên");
        }

        private sealed class FakeAuthService : IAuthService
        {
            public UserModel RegisteredUser { get; private set; }
            public string RegisteredPassword { get; private set; }

            public Task<UserModel> LoginAsync(
                string emailOrPhone,
                string password)
            {
                return Task.FromResult<UserModel>(null);
            }

            public Task<bool> RegisterAsync(UserModel user, string password)
            {
                RegisteredUser = user;
                RegisteredPassword = password;
                return Task.FromResult(true);
            }
        }
    }
}
