using Moq;
using SEP490_FTCDHMM_API.Domain.Constants;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RoleServiceTests
{
    public class DeleteRoleAsyncTests : RoleServiceTestBase
    {
        [Fact]
        public async Task DeleteRoleAsync_ShouldThrow_WhenNotFound()
        {
            RoleRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
                .ReturnsAsync((AppRole)null!);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.DeleteRoleAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task DeleteRoleAsync_ShouldThrow_WhenAdmin()
        {
            var role = new AppRole { Id = Guid.NewGuid(), Name = RoleConstants.Admin };

            RoleRepoMock
                .Setup(r => r.GetByIdAsync(role.Id, null))
                .ReturnsAsync(role);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.DeleteRoleAsync(role.Id));
        }

        [Fact]
        public async Task DeleteRoleAsync_ShouldThrow_WhenRoleInUse()
        {
            var id = Guid.NewGuid();
            var role = new AppRole { Id = id, Name = "Manager" };

            RoleRepoMock.Setup(r => r.GetByIdAsync(id, null)).ReturnsAsync(role);
            UserRepoMock.Setup(r => r.ExistsAsync(u => u.RoleId == id)).ReturnsAsync(true);

            await Assert.ThrowsAsync<AppException>(() => Sut.DeleteRoleAsync(id));
        }

        [Fact]
        public async Task DeleteRoleAsync_ShouldDelete_WhenValid()
        {
            var id = Guid.NewGuid();
            var role = new AppRole { Id = id, Name = "Manager" };

            RoleRepoMock.Setup(r => r.GetByIdAsync(id, null)).ReturnsAsync(role);
            UserRepoMock.Setup(r => r.ExistsAsync(u => u.RoleId == id)).ReturnsAsync(false);

            RoleRepoMock.Setup(r => r.DeleteAsync(role)).Returns(Task.CompletedTask);

            await Sut.DeleteRoleAsync(id);

            RoleRepoMock.Verify(r => r.DeleteAsync(role), Times.Once);
        }
    }
}
