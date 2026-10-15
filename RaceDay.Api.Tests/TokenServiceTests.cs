
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using RaceDay.Api.Models;
using RaceDay.Api.Services;
using Xunit;

namespace RaceDay.Api.Tests
{
    public class TokenServiceTests
    {
        // This is a test-only key.
        // Never use it as your application's production secret.
        private const string TestJwtKey =
            "RaceDayTestSigningKeyForUnitTestsOnly2026Secure12345";

        private TokenService CreateTokenService()
        {
            var settings = new Dictionary<string, string?>
            {
                { "Jwt:Key", TestJwtKey },
                { "Jwt:Issuer", "RaceDay.Tests" },
                { "Jwt:Audience", "RaceDay.Tests.Users" }
            };

            IConfiguration configuration =
                new ConfigurationBuilder()
                    .AddInMemoryCollection(settings)
                    .Build();

            return new TokenService(configuration);
        }

        private User CreateTestUser(string role)
        {
            return new User
            {
                Id = 42,
                Email = "testuser@example.com",
                Role = role
            };
        }

        [Fact]
        public void GenerateToken_ReturnsNonEmptyToken()
        {
            // Arrange
            var tokenService = CreateTokenService();
            var user = CreateTestUser("Participant");

            // Act
            string token = tokenService.GenerateToken(user);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        [Fact]
        public void GenerateToken_ContainsCorrectUserId()
        {
            // Arrange
            var tokenService = CreateTokenService();
            var user = CreateTestUser("Participant");

            // Act
            string token = tokenService.GenerateToken(user);

            var jwt = new JwtSecurityTokenHandler()
                .ReadJwtToken(token);

            // Assert
            Assert.Contains(
                jwt.Claims,
                claim =>
                    claim.Type == ClaimTypes.NameIdentifier &&
                    claim.Value == "42");
        }

        [Fact]
        public void GenerateToken_ContainsCorrectEmail()
        {
            // Arrange
            var tokenService = CreateTokenService();
            var user = CreateTestUser("Participant");

            // Act
            string token = tokenService.GenerateToken(user);

            var jwt = new JwtSecurityTokenHandler()
                .ReadJwtToken(token);

            // Assert
            Assert.Contains(
                jwt.Claims,
                claim =>
                    claim.Type == ClaimTypes.Name &&
                    claim.Value == "testuser@example.com");
        }

        [Fact]
        public void GenerateToken_ContainsParticipantRole()
        {
            // Arrange
            var tokenService = CreateTokenService();
            var user = CreateTestUser("Participant");

            // Act
            string token = tokenService.GenerateToken(user);

            var jwt = new JwtSecurityTokenHandler()
                .ReadJwtToken(token);

            // Assert
            Assert.Contains(
                jwt.Claims,
                claim =>
                    claim.Type == ClaimTypes.Role &&
                    claim.Value == "Participant");
        }

        [Fact]
        public void GenerateToken_ContainsOrganiserRole()
        {
            // Arrange
            var tokenService = CreateTokenService();
            var user = CreateTestUser("Organiser");

            // Act
            string token = tokenService.GenerateToken(user);

            var jwt = new JwtSecurityTokenHandler()
                .ReadJwtToken(token);

            // Assert
            Assert.Contains(
                jwt.Claims,
                claim =>
                    claim.Type == ClaimTypes.Role &&
                    claim.Value == "Organiser");
        }

        [Fact]
        public void GenerateToken_ExpiresInApproximatelyOneHour()
        {
            // Arrange
            var tokenService = CreateTokenService();
            var user = CreateTestUser("Participant");

            DateTime beforeGeneration = DateTime.UtcNow;

            // Act
            string token = tokenService.GenerateToken(user);

            DateTime afterGeneration = DateTime.UtcNow;

            var jwt = new JwtSecurityTokenHandler()
                .ReadJwtToken(token);

            // Assert
            Assert.True(
                jwt.ValidTo >= beforeGeneration.AddHours(1).AddSeconds(-2));

            Assert.True(
                jwt.ValidTo <= afterGeneration.AddHours(1).AddSeconds(2));
        }
    }
}
