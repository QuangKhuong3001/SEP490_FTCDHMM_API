using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Domain.Constants;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.UserServiceTests
{
    public class ChangeRoleAsyncTests : UserServiceTestBase
    {
        [Fact]
        public async Task ChangeRoleAsync_ShouldThrow_WhenRoleIsAdmin()
        {
            var userId = Guid.NewGuid();
            var req = new ChangeRoleRequest { RoleId = Guid.NewGuid() };

            var user = CreateUser(userId);

            var adminRole = new AppRole
            {
                Id = req.RoleId,
                Name = RoleConstants.Admin,
                IsActive = true
            };

            UserRepositoryMock
                .Setup(x => x.GetByIdAsync(userId, null))
                .ReturnsAsync(user);

            RoleRepositoryMock
                .Setup(x => x.GetByIdAsync(req.RoleId, It.IsAny<Func<IQueryable<AppRole>, IQueryable<AppRole>>?>()))
                .ReturnsAsync(adminRole);

            await Assert.ThrowsAsync<AppException>(() => Sut.ChangeRoleAsync(userId, req));
        }

        [Fact]
        public async Task ChangeRoleAsync_ShouldThrow_WhenRoleNotFoundOrInactive()
        {
            var userId = Guid.NewGuid();
            var req = new ChangeRoleRequest { RoleId = Guid.NewGuid() };

            var user = CreateUser(userId);

            UserRepositoryMock
                .Setup(x => x.GetByIdAsync(userId, null))
                .ReturnsAsync(user);

            RoleRepositoryMock
                .Setup(x => x.GetByIdAsync(req.RoleId, It.IsAny<Func<IQueryable<AppRole>, IQueryable<AppRole>>?>()))
                .ReturnsAsync((AppRole)null!);

            await Assert.ThrowsAsync<AppException>(() => Sut.ChangeRoleAsync(userId, req));
        }

        [Fact]
        public async Task ChangeRoleAsync_ShouldUpdate_WhenValid()
        {
            var userId = Guid.NewGuid();
            var req = new ChangeRoleRequest { RoleId = Guid.NewGuid() };

            var user = CreateUser(userId);

            var role = new AppRole
            {
                Id = req.RoleId,
                Name = "STAFF",
                IsActive = true
            };

            UserRepositoryMock
                .Setup(x => x.GetByIdAsync(userId, null))
                .ReturnsAsync(user);

            RoleRepositoryMock
                .Setup(x => x.GetByIdAsync(req.RoleId, It.IsAny<Func<IQueryable<AppRole>, IQueryable<AppRole>>?>()))
                .ReturnsAsync(role);

            UserRepositoryMock
                .Setup(x => x.UpdateAsync(user))
                .Returns(Task.CompletedTask);

            await Sut.ChangeRoleAsync(userId, req);

            Assert.Equal(req.RoleId, user.RoleId);
        }
    }
}
