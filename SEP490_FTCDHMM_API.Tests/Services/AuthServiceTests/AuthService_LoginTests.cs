using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.AuthServiceTests
{
    public class AuthService_LoginTests : AuthServiceTestBase
    {
        private LoginRequest CreateValidLoginRequest()
        {
            return new LoginRequest
            {
                Email = "user@example.com",
                Password = "12345@aA"
            };
        }

        [Fact]
        public async Task Login_ShouldThrow_WhenUserNotFound()
        {
            var dto = CreateValidLoginRequest();

            UserManagerMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync((AppUser)null!);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.Login(dto));

            Assert.Equal(AppResponseCode.INVALID_ACCOUNT_INFORMATION, ex.ResponseCode);
        }

        [Fact]
        public async Task Login_ShouldThrow_WhenPasswordInvalid()
        {
            var dto = CreateValidLoginRequest();
            var user = CreateUser();

            UserManagerMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync(user);

            UserManagerMock
                .Setup(x => x.CheckPasswordAsync(user, dto.Password))
                .ReturnsAsync(false);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.Login(dto));

            Assert.Equal(AppResponseCode.INVALID_ACCOUNT_INFORMATION, ex.ResponseCode);
        }

        [Fact]
        public async Task Login_ShouldThrow_WhenAccountLocked()
        {
            var dto = CreateValidLoginRequest();
            var user = CreateUser();

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            UserManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);
            UserManagerMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.Login(dto));

            Assert.Equal(AppResponseCode.ACCOUNT_LOCKED, ex.ResponseCode);
        }

        [Fact]
        public async Task Login_ShouldThrow_WhenEmailNotConfirmed()
        {
            var dto = CreateValidLoginRequest();
            var user = CreateUser(emailConfirmed: false);

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            UserManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);
            UserManagerMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.Login(dto));

            Assert.Equal(AppResponseCode.EMAIL_NOT_CONFIRMED, ex.ResponseCode);
        }

        [Fact]
        public async Task Login_ShouldThrow_WhenRoleNotFound()
        {
            var dto = CreateValidLoginRequest();
            var user = CreateUser();

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            UserManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);
            UserManagerMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);

            RoleRepositoryMock
                .Setup(r => r.GetRoleWithPermissionsAsync(user.RoleId))
                .ReturnsAsync((AppRole)null!);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.Login(dto));

            Assert.Equal(AppResponseCode.NOT_FOUND, ex.ResponseCode);
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenValid()
        {
            var dto = CreateValidLoginRequest();
            var roleId = Guid.NewGuid();
            var user = CreateUser(roleId: roleId, emailConfirmed: true);
            var role = new AppRole { Id = roleId, Name = "Customer" };

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            UserManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);
            UserManagerMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);

            RoleRepositoryMock
                .Setup(r => r.GetRoleWithPermissionsAsync(user.RoleId))
                .ReturnsAsync(role);

            var expectedToken = "fake-token";

            JwtServiceMock
                .Setup(j => j.GenerateToken(user, role))
                .Returns(expectedToken);

            var result = await Sut.Login(dto);

            Assert.Equal(expectedToken, result);
            JwtServiceMock.Verify(j => j.GenerateToken(user, role), Times.Once);
        }
    }
}
