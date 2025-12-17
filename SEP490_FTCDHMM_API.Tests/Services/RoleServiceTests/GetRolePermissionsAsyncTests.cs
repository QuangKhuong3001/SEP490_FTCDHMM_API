using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RoleServiceTests
{
    public class GetRolePermissionsAsyncTests : RoleServiceTestBase
    {
        [Fact]
        public async Task GetRolePermissionsAsync_ShouldThrow_WhenRoleNotFound()
        {
            var roleId = Guid.NewGuid();

            RoleRepoMock
                .Setup(r => r.GetByIdAsync(
                    roleId,
                    It.IsAny<Func<IQueryable<AppRole>, IQueryable<AppRole>>>()))
                .ReturnsAsync((AppRole)null!);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.GetRolePermissionsAsync(roleId));

            PermissionDomainRepoMock.Verify(
                r => r.GetAllAsync(
                    It.IsAny<Expression<Func<PermissionDomain, bool>>>(),
                    It.IsAny<Func<IQueryable<PermissionDomain>, IQueryable<PermissionDomain>>>()),
                Times.Never);
        }

        [Fact]
        public async Task GetRolePermissionsAsync_ShouldReturnPermissions()
        {
            var roleId = Guid.NewGuid();
            var actionId = Guid.NewGuid();

            var role = new AppRole
            {
                Id = roleId,
                RolePermissions = new List<AppRolePermission>
                {
                    new AppRolePermission
                    {
                        PermissionActionId = actionId,
                        IsActive = true
                    }
                }
            };

            RoleRepoMock
                .Setup(r => r.GetByIdAsync(
                    roleId,
                    It.IsAny<Func<IQueryable<AppRole>, IQueryable<AppRole>>>()))
                .ReturnsAsync(role);

            var domains = new List<PermissionDomain>
            {
                new PermissionDomain
                {
                    Name = "Recipe",
                    Actions = new List<PermissionAction>
                    {
                        new PermissionAction
                        {
                            Id = actionId,
                            Name = "View"
                        }
                    }
                }
            };

            PermissionDomainRepoMock
                .Setup(r => r.GetAllAsync(
                    null,
                    It.IsAny<Func<IQueryable<PermissionDomain>, IQueryable<PermissionDomain>>>()))
                .ReturnsAsync(domains);

            var result = await Sut.GetRolePermissionsAsync(roleId);

            Assert.NotNull(result);
            Assert.Single(result.Domains);

            var domain = result.Domains.First();
            Assert.Equal("Recipe", domain.DomainName);
            Assert.Single(domain.Actions);

            var action = domain.Actions.First();
            Assert.Equal(actionId, action.ActionId);
            Assert.Equal("View", action.ActionName);
            Assert.True(action.IsActive);
        }
    }
}
