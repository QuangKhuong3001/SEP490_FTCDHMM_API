using Microsoft.AspNetCore.Identity;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.AuthServiceTests
{
    public class ResetPasswordWithTokenAsyncTests : AuthServiceTestBase
    {
        private ResetPasswordWithTokenDto CreateDto(
            string email = "user@example.com",
            string token = "reset-token",
            string newPass = "NewPass123!")
        {
            return new ResetPasswordWithTokenDto
            {
                Email = email,
                Token = token,
                NewPassword = newPass
            };
        }

        [Fact]
        public async Task ResetPassword_ShouldThrow_WhenUserNotFound()
        {
            var dto = CreateDto();

            UserManagerMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync((AppUser)null!);

            await Assert.ThrowsAsync<AppException>(
                () => Sut.ResetPasswordWithTokenAsync(dto)
            );
        }

        [Fact]
        public async Task ResetPassword_ShouldThrow_WhenNewPasswordSameAsOld()
        {
            var dto = CreateDto();
            var user = CreateUser();

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);

            UserManagerMock
                .Setup(x => x.CheckPasswordAsync(user, dto.NewPassword))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AppException>(
                () => Sut.ResetPasswordWithTokenAsync(dto)
            );
        }

        [Fact]
        public async Task ResetPassword_ShouldReturnFalse_WhenIdentityFails()
        {
            var dto = CreateDto();
            var user = CreateUser();

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            UserManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.NewPassword)).ReturnsAsync(false);

            var identityErrors = new IdentityError { Description = "Invalid token" };
            var identityResult = IdentityResult.Failed(identityErrors);

            UserManagerMock
                .Setup(x => x.ResetPasswordAsync(user, dto.Token, dto.NewPassword))
                .ReturnsAsync(identityResult);

            var (success, errors) = await Sut.ResetPasswordWithTokenAsync(dto);

            Assert.False(success);
            Assert.Single(errors);
            Assert.Contains("Invalid token", errors.First());
        }

        [Fact]
        public async Task ResetPassword_ShouldReturnSuccess_WhenValid()
        {
            var dto = CreateDto();
            var user = CreateUser();

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            UserManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.NewPassword)).ReturnsAsync(false);

            UserManagerMock
                .Setup(x => x.ResetPasswordAsync(user, dto.Token, dto.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            var (success, errors) = await Sut.ResetPasswordWithTokenAsync(dto);

            Assert.True(success);
            Assert.Empty(errors);
        }
    }
}
