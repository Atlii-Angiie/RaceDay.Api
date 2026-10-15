
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

            bool result = passwordService.VerifyPassword(
                hashedPassword,
                password);

            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
        {
            var passwordService = new PasswordService();

            string hashedPassword =
                passwordService.HashPassword("CorrectPassword123!");

            bool result = passwordService.VerifyPassword(
                hashedPassword,
                "WrongPassword123!");

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

            bool result = passwordService.VerifyPassword(
                hashedPassword,
                "");

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

            bool firstResult = passwordService.VerifyPassword(
                firstHash,
                password);

            bool secondResult = passwordService.VerifyPassword(
                secondHash,
                password);

            Assert.True(firstResult);
            Assert.True(secondResult);
        }

        // NEW TEST
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
