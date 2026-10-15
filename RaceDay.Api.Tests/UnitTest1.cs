
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
            // Correct order: password first, hash second.
            bool result = passwordService.VerifyPassword(
                password,
                hashedPassword);

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
            // Correct order: password first, hash second.
            bool result = passwordService.VerifyPassword(
                "WrongPassword123!",
                hashedPassword);

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
            // Correct order: password first, hash second.
            bool result = passwordService.VerifyPassword(
                "",
                hashedPassword);

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
            // Correct order: password first, hash second.
            bool firstResult = passwordService.VerifyPassword(
                password,
                firstHash);

            bool secondResult = passwordService.VerifyPassword(
                password,
                secondHash);

            // Assert
            Assert.True(firstResult);
            Assert.True(secondResult);
        }

        [Fact]
        public void HashPassword_WithValidPassword_ReturnsNonEmptyHash()
        {
            // Arrange
            var passwordService = new PasswordService();
            string password = "SecurePassword123!";

            // Act
            string hashedPassword =
                passwordService.HashPassword(password);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(hashedPassword));
        }
    }
}
