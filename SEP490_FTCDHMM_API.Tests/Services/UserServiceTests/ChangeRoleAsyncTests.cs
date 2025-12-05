using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.UserServiceTests
{
    public class ChangeRoleAsync_Tests : UserServiceTestBase
    {
        private static ChangeRoleRequest CreateDto(Guid roleId)
        {
            return new ChangeRoleRequest
            {
                RoleId = roleId
            };
        }

        [Fact]
        public async Task ChangeRole_ShouldThrow_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var dto = CreateDto(roleId);

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync((AppUser?)null);

            var ex = await Assert.ThrowsAsync<AppException>(() =>
                Sut.ChangeRoleAsync(userId, dto));

            Assert.Equal(AppResponseCode.INVALID_ACCOUNT_INFORMATION, ex.ResponseCode);
        }

        [Fact]
        public async Task ChangeRole_ShouldThrow_WhenRoleNotFound()
        {
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var dto = CreateDto(roleId);

            var user = CreateUser(userId);

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            RoleRepositoryMock
                .Setup(r => r.GetByIdAsync(roleId, It.IsAny<Func<IQueryable<AppRole>, IQueryable<AppRole>>>()))
                .ReturnsAsync((AppRole?)null);

            var ex = await Assert.ThrowsAsync<AppException>(() =>
                Sut.ChangeRoleAsync(userId, dto));

            Assert.Equal(AppResponseCode.INVALID_ACCOUNT_INFORMATION, ex.ResponseCode);
        }

        [Fact]
        public async Task ChangeRole_ShouldThrow_WhenRoleInactive()
        {
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var dto = CreateDto(roleId);

            var user = CreateUser(userId);
            var role = new AppRole { Id = roleId, IsActive = false };

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            RoleRepositoryMock
                .Setup(r => r.GetByIdAsync(roleId, It.IsAny<Func<IQueryable<AppRole>, IQueryable<AppRole>>>()))
                .ReturnsAsync(role);

            var ex = await Assert.ThrowsAsync<AppException>(() =>
                Sut.ChangeRoleAsync(userId, dto));

            Assert.Equal(AppResponseCode.INVALID_ACCOUNT_INFORMATION, ex.ResponseCode);
        }

        [Fact]
        public async Task ChangeRole_ShouldUpdateRole_WhenValid()
        {
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var dto = CreateDto(roleId);

            var user = CreateUser(userId);
            user.RoleId = Guid.NewGuid();

            var role = new AppRole { Id = roleId, IsActive = true };

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            RoleRepositoryMock
                .Setup(r => r.GetByIdAsync(roleId, It.IsAny<Func<IQueryable<AppRole>, IQueryable<AppRole>>>()))
                .ReturnsAsync(role);

            UserRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<AppUser>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await Sut.ChangeRoleAsync(userId, dto);

            Assert.Equal(roleId, user.RoleId);
            UserRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }
    }
}
