
using Xunit;
using RaceDay.Api.Services;

namespace RaceDay.Api.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void HashPassword_DoesNotStorePlainText()
        {
            var passwordService = new PasswordService();
            string password = "SecurePassword123!";

            string hashedPassword =
                passwordService.HashPassword(password);

            Assert.NotEqual(password, hashedPassword);
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
        {
            var passwordService = new PasswordService();
            string password = "SecurePassword123!";

            string hashedPassword =
                passwordService.HashPassword(password);

            // Correct order: password first, hash second.
            bool result = passwordService.VerifyPassword(
                password,
                hashedPassword);

            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
        {
            var passwordService = new PasswordService();

            string hashedPassword =
                passwordService.HashPassword("CorrectPassword123!");

            // Correct order: password first, hash second.
            bool result = passwordService.VerifyPassword(
                "WrongPassword123!",
                hashedPassword);

            Assert.False(result);
        }

        [Fact]
        public void HashPassword_SamePasswordProducesDifferentHashes()
        {
            var passwordService = new PasswordService();
            string password = "SecurePassword123!";

            string firstHash =
                passwordService.HashPassword(password);

            string secondHash =
                passwordService.HashPassword(password);

            Assert.NotEqual(firstHash, secondHash);
        }

        [Fact]
        public void VerifyPassword_WithEmptyPassword_ReturnsFalse()
        {
            var passwordService = new PasswordService();

            string hashedPassword =
                passwordService.HashPassword("SecurePassword123!");

            // Correct order: password first, hash second.
            bool result = passwordService.VerifyPassword(
                "",
                hashedPassword);

            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_TwoDifferentHashesBothVerify()
        {
            var passwordService = new PasswordService();
            string password = "SecurePassword123!";

            string firstHash =
                passwordService.HashPassword(password);

            string secondHash =
                passwordService.HashPassword(password);

            // Correct order: password first, hash second.
            bool firstResult = passwordService.VerifyPassword(
                password,
                firstHash);

            bool secondResult = passwordService.VerifyPassword(
                password,
                secondHash);

            Assert.True(firstResult);
            Assert.True(secondResult);
        }

        [Fact]
        public void HashPassword_WithValidPassword_ReturnsNonEmptyHash()
        {
            var passwordService = new PasswordService();
            string password = "SecurePassword123!";

            string hashedPassword =
                passwordService.HashPassword(password);

            Assert.False(string.IsNullOrWhiteSpace(hashedPassword));
        }
    }
}
