using Microsoft.AspNetCore.Identity;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.AuthServiceTests
{
    public class ChangePasswordAsyncTests : AuthServiceTestBase
    {
        private ChangePasswordRequest CreateValidDto()
        {
            return new ChangePasswordRequest
            {
                CurrentPassword = "OldPass123@",
                NewPassword = "NewPass123@"
            };
        }

        [Fact]
        public async Task ChangePassword_ShouldThrow_WhenPasswordsAreSame()
        {
            var dto = CreateValidDto();
            dto.NewPassword = dto.CurrentPassword;

            var user = CreateUser();

            UserManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            UserManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.NewPassword))
                .ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<AppException>(() =>
                Sut.ChangePasswordAsync(user.Id.ToString(), dto));

            Assert.Equal(AppResponseCode.PASSWORD_CANNOT_BE_SAME_AS_OLD, ex.ResponseCode);
        }

        [Fact]
        public async Task ChangePassword_ShouldThrow_WhenUserNotFound()
        {
            var dto = CreateValidDto();

            UserManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((AppUser)null!);

            var ex = await Assert.ThrowsAsync<AppException>(() =>
                Sut.ChangePasswordAsync("invalid-id", dto));

            Assert.Equal(AppResponseCode.INVALID_ACCOUNT_INFORMATION, ex.ResponseCode);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnErrors_WhenIdentityFails()
        {
            var dto = CreateValidDto();
            var user = CreateUser();

            UserManagerMock.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            UserManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.NewPassword))
                .ReturnsAsync(false);

            UserManagerMock.Setup(x => x.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Too weak" }));

            var (success, errors) = await Sut.ChangePasswordAsync(user.Id.ToString(), dto);

            Assert.False(success);
            Assert.Single(errors);
            Assert.Contains("Too weak", errors.First());
        }

        [Fact]
        public async Task ChangePassword_ShouldSucceed_WhenValid()
        {
            var dto = CreateValidDto();
            var user = CreateUser();

            UserManagerMock.Setup(x => x.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            UserManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.NewPassword))
                .ReturnsAsync(false);

            UserManagerMock.Setup(x => x.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            var (success, errors) = await Sut.ChangePasswordAsync(user.Id.ToString(), dto);

            Assert.True(success);
            Assert.Empty(errors);
        }
    }
}
