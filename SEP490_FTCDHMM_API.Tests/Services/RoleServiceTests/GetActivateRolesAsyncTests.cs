using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.RoleServiceTests
{
    public class GetActivateRolesAsyncTests : RoleServiceTestBase
    {
        [Fact]
        public async Task GetActivateRolesAsync_ShouldReturnActiveRoles()
        {
            var roles = new List<AppRole>
            {
                new AppRole { Id = Guid.NewGuid(), Name = "Manager", IsActive = true }
            };

            RoleRepoMock
                .Setup(r => r.GetAllAsync(r => r.IsActive, null))
                .ReturnsAsync(roles);

            var result = await Sut.GetActivateRolesAsync();

            Assert.Single(result);
        }
    }
}
