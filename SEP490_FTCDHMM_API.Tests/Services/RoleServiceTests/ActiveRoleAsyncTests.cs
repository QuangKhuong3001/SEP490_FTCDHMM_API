using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RoleServiceTests
{
    public class ActiveRoleAsyncTests : RoleServiceTestBase
    {
        [Fact]
        public async Task ActiveRoleAsync_ShouldThrow_WhenNotFound()
        {
            RoleRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
                .ReturnsAsync((AppRole)null!);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.ActiveRoleAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task ActiveRoleAsync_ShouldThrow_WhenAlreadyActive()
        {
            var role = new AppRole { Id = Guid.NewGuid(), IsActive = true };

            RoleRepoMock.Setup(r => r.GetByIdAsync(role.Id, null)).ReturnsAsync(role);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.ActiveRoleAsync(role.Id));
        }

        [Fact]
        public async Task ActiveRoleAsync_ShouldActive_WhenValid()
        {
            var role = new AppRole { Id = Guid.NewGuid(), IsActive = false };

            RoleRepoMock.Setup(r => r.GetByIdAsync(role.Id, null)).ReturnsAsync(role);
            RoleRepoMock.Setup(r => r.UpdateAsync(role)).Returns(Task.CompletedTask);

            await Sut.ActiveRoleAsync(role.Id);

            Assert.True(role.IsActive);
        }
    }
}
