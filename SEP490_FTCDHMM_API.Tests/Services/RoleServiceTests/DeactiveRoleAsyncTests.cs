using Moq;
using SEP490_FTCDHMM_API.Domain.Constants;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RoleServiceTests
{
    public class DeactiveRoleAsyncTests : RoleServiceTestBase
    {
        [Fact]
        public async Task DeactiveRoleAsync_ShouldThrow_WhenNotFound()
        {
            RoleRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
                .ReturnsAsync((AppRole)null!);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.DeactiveRoleAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task DeactiveRoleAsync_ShouldThrow_WhenAlreadyInactive()
        {
            var role = new AppRole { Id = Guid.NewGuid(), IsActive = false };

            RoleRepoMock.Setup(r => r.GetByIdAsync(role.Id, null)).ReturnsAsync(role);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.DeactiveRoleAsync(role.Id));
        }

        [Fact]
        public async Task DeactiveRoleAsync_ShouldThrow_WhenAdmin()
        {
            var role = new AppRole { Id = Guid.NewGuid(), Name = RoleConstants.Admin, IsActive = true };

            RoleRepoMock.Setup(r => r.GetByIdAsync(role.Id, null)).ReturnsAsync(role);

            await Assert.ThrowsAsync<AppException>(() => Sut.DeactiveRoleAsync(role.Id));
        }

        [Fact]
        public async Task DeactiveRoleAsync_ShouldThrow_WhenRoleInUse()
        {
            var id = Guid.NewGuid();
            var role = new AppRole { Id = id, Name = "Manager", IsActive = true };

            RoleRepoMock.Setup(r => r.GetByIdAsync(id, null)).ReturnsAsync(role);
            UserRepoMock.Setup(r => r.ExistsAsync(u => u.RoleId == id)).ReturnsAsync(true);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.DeactiveRoleAsync(id));
        }

        [Fact]
        public async Task DeactiveRoleAsync_ShouldDeactive_WhenValid()
        {
            var id = Guid.NewGuid();
            var role = new AppRole { Id = id, IsActive = true, Name = "Manager" };

            RoleRepoMock.Setup(r => r.GetByIdAsync(id, null)).ReturnsAsync(role);
            UserRepoMock.Setup(r => r.ExistsAsync(u => u.RoleId == id)).ReturnsAsync(false);

            RoleRepoMock.Setup(r => r.UpdateAsync(role)).Returns(Task.CompletedTask);

            await Sut.DeactiveRoleAsync(id);

            Assert.False(role.IsActive);
        }
    }
}
