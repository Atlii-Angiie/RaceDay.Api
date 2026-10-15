
using RaceDay.Api.Services;
using Xunit;

namespace RaceDay.Api.Tests
{
    public class PasswordServiceTests
    {
        private readonly PasswordService _passwordService = new();

        [Fact]
        public void HashPasswordSamePasswordTwiceShouldProduceDifferentHashes()
        {
            // Arrange
            string password = "TestPassword#2026";

            // Act
            string firstHash = _passwordService.HashPassword(password);
            string secondHash = _passwordService.HashPassword(password);

            // Assert
            Assert.NotEqual(firstHash, secondHash);
        }


        [Fact]
        public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
        {
            // Arrange
            string password = "TestPassword#2026";

            string hashedPassword =
                _passwordService.HashPassword(password);

            // Act
            bool result = _passwordService.VerifyPassword(
                password,
                hashedPassword);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
        {
            // Arrange
            string password = "TestPassword#2026";

            string hashedPassword =
                _passwordService.HashPassword(password);

            // Act
            bool result = _passwordService.VerifyPassword(
                "WrongPassword123",
                hashedPassword);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_WithEmptyPassword_ShouldReturnFalse()
        {
            // Arrange
            string password = "TestPassword#2026";
            string hashedPassword = _passwordService.HashPassword(password);

            // Act
            bool result = _passwordService.VerifyPassword("", hashedPassword);

            // Assert
            Assert.False(result);
        }


        [Fact]
        public void HashPassword_TwoDifferentHashes_ShouldBothVerify()
        {
            // Arrange
            string password = "TestPassword#2026";

            // Act
            string firstHash = _passwordService.HashPassword(password);
            string secondHash = _passwordService.HashPassword(password);

            // Assert
            Assert.True(_passwordService.VerifyPassword(password, firstHash));
            Assert.True(_passwordService.VerifyPassword(password, secondHash));
        }
    }
}

