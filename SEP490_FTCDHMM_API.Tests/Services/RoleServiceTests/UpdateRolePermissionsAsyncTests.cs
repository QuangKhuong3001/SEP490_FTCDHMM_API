using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RoleDtos;
using SEP490_FTCDHMM_API.Domain.Constants;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RoleServiceTests
{
    public class UpdateRolePermissionsAsyncTests : RoleServiceTestBase
    {
        [Fact]
        public async Task UpdateRolePermissionsAsync_ShouldThrow_WhenRoleNotFound()
        {
            RoleRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<AppRole>, IQueryable<AppRole>>>()))
                .ReturnsAsync((AppRole)null!);

            var dto = new RolePermissionSettingRequest();

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateRolePermissionsAsync(Guid.NewGuid(), dto));
        }

        [Fact]
        public async Task UpdateRolePermissionsAsync_ShouldThrow_WhenAdminRole()
        {
            var role = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = RoleConstants.Admin,
                RolePermissions = new List<AppRolePermission>()
            };

            RoleRepoMock
                .Setup(r => r.GetByIdAsync(role.Id, It.IsAny<Func<IQueryable<AppRole>, IQueryable<AppRole>>>()))
                .ReturnsAsync(role);

            var dto = new RolePermissionSettingRequest();

            var ex = await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateRolePermissionsAsync(role.Id, dto));

            Assert.Equal("Không được quyền chỉnh sửa tài khoàn admin", ex.Message);
        }

        [Fact]
        public async Task UpdateRolePermissionsAsync_ShouldThrow_WhenPermissionNotExist()
        {
            var role = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = "Manager",
                RolePermissions = new List<AppRolePermission>
                {
                    new AppRolePermission
                    {
                        PermissionActionId = Guid.NewGuid(),
                        IsActive = false
                    }
                }
            };

            RoleRepoMock
                .Setup(r => r.GetByIdAsync(role.Id, It.IsAny<Func<IQueryable<AppRole>, IQueryable<AppRole>>>()))
                .ReturnsAsync(role);

            // gửi permissionActionId không tồn tại trong role
            var dto = new RolePermissionSettingRequest
            {
                Permissions =
                {
                    new PermissionToggleRequest
                    {
                        PermissionActionId = Guid.NewGuid(),
                        IsActive = true
                    }
                }
            };

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateRolePermissionsAsync(role.Id, dto));
        }

        [Fact]
        public async Task UpdateRolePermissionsAsync_ShouldUpdatePermissions_WhenValid()
        {
            var perm1 = Guid.NewGuid();
            var perm2 = Guid.NewGuid();

            var role = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = "Manager",
                RolePermissions = new List<AppRolePermission>
                {
                    new AppRolePermission
                    {
                        PermissionActionId = perm1,
                        IsActive = false
                    },
                    new AppRolePermission
                    {
                        PermissionActionId = perm2,
                        IsActive = false
                    }
                }
            };

            RoleRepoMock
                .Setup(r => r.GetByIdAsync(role.Id, It.IsAny<Func<IQueryable<AppRole>, IQueryable<AppRole>>>()))
                .ReturnsAsync(role);

            var dto = new RolePermissionSettingRequest
            {
                Permissions =
                {
                    new PermissionToggleRequest
                    {
                        PermissionActionId = perm1,
                        IsActive = true
                    },
                    new PermissionToggleRequest
                    {
                        PermissionActionId = perm2,
                        IsActive = false
                    }
                }
            };

            RoleRepoMock.Setup(r => r.UpdateAsync(role)).Returns(Task.CompletedTask);

            await Sut.UpdateRolePermissionsAsync(role.Id, dto);

            Assert.True(role.RolePermissions.First(x => x.PermissionActionId == perm1).IsActive);
            Assert.False(role.RolePermissions.First(x => x.PermissionActionId == perm2).IsActive);

            RoleRepoMock.Verify(r => r.UpdateAsync(role), Times.Once);
        }
    }
}
