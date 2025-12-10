using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RoleDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RoleServiceTests
{
    public class CreateRoleAsyncTests : RoleServiceTestBase
    {
        [Fact]
        public async Task CreateRoleAsync_ShouldThrow_WhenRoleNameExists()
        {
            var dto = new CreateRoleRequest { Name = "Manager" };

            RoleRepoMock
                .Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<AppRole, bool>>>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AppException>(() => Sut.CreateRoleAsync(dto));
        }

        [Fact]
        public async Task CreateRoleAsync_ShouldCreateRole_WhenValid()
        {
            var dto = new CreateRoleRequest { Name = "Manager" };

            RoleRepoMock
                .Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<AppRole, bool>>>()))
                .ReturnsAsync(false);

            PermissionActionRepoMock
                .Setup(r => r.GetAllAsync(null, null))
                .ReturnsAsync(new List<PermissionAction>
                {
                    new PermissionAction { Id = Guid.NewGuid(), Name = "VIEW" },
                    new PermissionAction { Id = Guid.NewGuid(), Name = "UPDATE" }
                });

            AppRole createdRole = null!;

            RoleRepoMock
                .Setup(r => r.AddAsync(It.IsAny<AppRole>()))
                .Callback<AppRole>(r => createdRole = r)
                .ReturnsAsync(() => createdRole);

            RolePermissionRepoMock
                .Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<AppRolePermission>>()))
                .Returns(Task.CompletedTask);

            await Sut.CreateRoleAsync(dto);

            Assert.NotNull(createdRole);
            Assert.Equal(dto.Name, createdRole.Name);
            Assert.True(createdRole.IsActive);
        }
    }
}
