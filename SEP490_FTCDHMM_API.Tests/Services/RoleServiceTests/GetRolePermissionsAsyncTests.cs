using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.RoleServiceTests
{
    public class GetRolePermissionsAsyncTests : RoleServiceTestBase
    {
        [Fact]
        public async Task GetRolePermissionsAsync_ShouldReturnPermissions()
        {
            var roleId = Guid.NewGuid();

            var domains = new List<PermissionDomain>
            {
                new PermissionDomain
                {
                    Name = "Recipe",
                    Actions = new List<PermissionAction>
                    {
                        new PermissionAction { Id = Guid.NewGuid(), Name = "View" }
                    }
                }
            };

            PermissionDomainRepoMock
                .Setup(r => r.GetAllAsync(null, It.IsAny<Func<IQueryable<PermissionDomain>, IQueryable<PermissionDomain>>>()))
                .ReturnsAsync(domains);

            RolePermissionRepoMock
                .Setup(r => r.GetAllAsync(rp => rp.RoleId == roleId, null))
                .ReturnsAsync(new List<AppRolePermission>());

            var result = await Sut.GetRolePermissionsAsync(roleId);

            Assert.Single(result);
            Assert.Single(result.First().Actions);
        }
    }
}
