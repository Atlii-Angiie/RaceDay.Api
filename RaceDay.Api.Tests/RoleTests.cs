
using Xunit;
using RaceDay.Api.Constants;

namespace RaceDay.Api.Tests
{
    public class RoleTests
    {
        [Fact]
        public void OrganiserRole_ShouldHaveCorrectValue()
        {
            Assert.Equal("Organiser", Roles.Organiser);
        }

        [Fact]
        public void ParticipantRole_ShouldHaveCorrectValue()
        {
            Assert.Equal("Participant", Roles.Participant);
        }

        [Fact]
        public void OrganiserAndParticipantRoles_ShouldBeDifferent()
        {
            Assert.NotEqual(Roles.Organiser, Roles.Participant);
        }
    }
}
