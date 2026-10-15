
using Xunit;
using RaceDay.Api.Services;

namespace RaceDay.Api.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void HashPassword_DoesNotStorePlainText()
        {
            // Arrange
            var passwordService = new PasswordService();
            string password = "SecurePassword123!";

            // Act
            string hashedPassword =
                passwordService.HashPassword(password);

            // Assert
            Assert.NotEqual(password, hashedPassword);
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
        {
            // Arrange
            var passwordService = new PasswordService();
            string password = "SecurePassword123!";

            string hashedPassword =
                passwordService.HashPassword(password);

            // Act
            bool result = passwordService.VerifyPassword(
                hashedPassword,
                password);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
        {
            // Arrange
            var passwordService = new PasswordService();

            string hashedPassword =
                passwordService.HashPassword("CorrectPassword123!");

            // Act
            bool result = passwordService.VerifyPassword(
                hashedPassword,
                "WrongPassword123!");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HashPassword_SamePasswordProducesDifferentHashes()
        {
            // Arrange
            var passwordService = new PasswordService();
            string password = "SecurePassword123!";

            // Act
            string firstHash =
                passwordService.HashPassword(password);

            string secondHash =
                passwordService.HashPassword(password);

            // Assert
            Assert.NotEqual(firstHash, secondHash);
        }

        [Fact]
        public void VerifyPassword_WithEmptyPassword_ReturnsFalse()
        {
            // Arrange
            var passwordService = new PasswordService();

            string hashedPassword =
                passwordService.HashPassword("SecurePassword123!");

            // Act
            bool result = passwordService.VerifyPassword(
                hashedPassword,
                "");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_TwoDifferentHashesBothVerify()
        {
            // Arrange
            var passwordService = new PasswordService();
            string password = "SecurePassword123!";

            string firstHash =
                passwordService.HashPassword(password);

            string secondHash =
                passwordService.HashPassword(password);

            // Act
            bool firstResult = passwordService.VerifyPassword(
                firstHash,
                password);

            bool secondResult = passwordService.VerifyPassword(
                secondHash,
                password);

            // Assert
            Assert.True(firstResult);
            Assert.True(secondResult);
        }
    }
}
