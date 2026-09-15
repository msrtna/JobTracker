using JobTracker.Infrastructure.Services;

namespace JobTracker.Tests.UnitTests
{
    public class PasswordHasherServiceTests
    {
        private readonly PasswordHasherService _service;

        public PasswordHasherServiceTests()
        {
            _service = new PasswordHasherService();
        }


        [Fact]
        public void HashPassword_ReturnsHash()
        {
            // Arrange
            var password = "Password123!";

            // Act
            var hash = _service.HashPassword(password);

            // Assert
            Assert.NotNull(hash);
            Assert.NotEqual(password, hash);
            Assert.NotEmpty(hash);
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
        {
            // Arrange
            var password = "Password123!";
            var hash = _service.HashPassword(password);

            // Act
            var result = _service.VerifyPassword(password, hash);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
        {
            // Arrange
            var password = "Password123!";
            var wrongPassword = "WrongPassword123!";
            var hash = _service.HashPassword(password);

            // Act
            var result = _service.VerifyPassword(wrongPassword, hash);

            // Assert
            Assert.False(result);
        }
    }
}
