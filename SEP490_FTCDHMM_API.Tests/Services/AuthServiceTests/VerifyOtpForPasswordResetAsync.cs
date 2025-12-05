using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs;
using SEP490_FTCDHMM_API.Domain.Constants;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;
using SEP490_FTCDHMM_API.Shared.Utils;

namespace SEP490_FTCDHMM_API.Tests.Services.AuthServiceTests
{
    public class VerifyOtpForPasswordResetAsyncTests : AuthServiceTestBase
    {
        private VerifyOtpForPasswordResetRequest CreateDto(string email = "user@example.com", string code = "123456")
        {
            return new VerifyOtpForPasswordResetRequest
            {
                Email = email,
                Code = code
            };
        }

        private EmailOtp CreateOtp(Guid userId, bool disabled = false, int attempts = 0, int expireMinutes = 5)
        {
            return new EmailOtp
            {
                Id = Guid.NewGuid(),
                SentToId = userId,
                Code = HashHelper.ComputeSha256Hash("123456"),
                Purpose = OtpPurpose.ForgotPassword,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(expireMinutes),
                Attempts = attempts,
                IsDisabled = disabled
            };
        }

        [Fact]
        public async Task VerifyOtp_ShouldThrow_WhenUserNotFound()
        {
            var dto = CreateDto();

            UserManagerMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync((AppUser)null!);

            await Assert.ThrowsAsync<AppException>(() => Sut.VerifyOtpForPasswordResetAsync(dto));
        }

        [Fact]
        public async Task VerifyOtp_ShouldThrow_WhenOtpNotFound()
        {
            var dto = CreateDto();
            var user = CreateUser();

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            OtpRepositoryMock.Setup(o => o.GetLatestAsync(user.Id, OtpPurpose.ForgotPassword)).ReturnsAsync((EmailOtp)null!);

            await Assert.ThrowsAsync<AppException>(() => Sut.VerifyOtpForPasswordResetAsync(dto));
        }

        [Fact]
        public async Task VerifyOtp_ShouldThrow_WhenOtpIsDisabled()
        {
            var dto = CreateDto();
            var user = CreateUser();
            var otp = CreateOtp(user.Id, disabled: true);

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            OtpRepositoryMock.Setup(o => o.GetLatestAsync(user.Id, OtpPurpose.ForgotPassword)).ReturnsAsync(otp);

            await Assert.ThrowsAsync<AppException>(() => Sut.VerifyOtpForPasswordResetAsync(dto));
        }

        [Fact]
        public async Task VerifyOtp_ShouldThrow_WhenOtpExpired()
        {
            var dto = CreateDto();
            var user = CreateUser();
            var otp = CreateOtp(user.Id, expireMinutes: -1);

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            OtpRepositoryMock.Setup(o => o.GetLatestAsync(user.Id, OtpPurpose.ForgotPassword)).ReturnsAsync(otp);

            await Assert.ThrowsAsync<AppException>(() => Sut.VerifyOtpForPasswordResetAsync(dto));
        }

        [Fact]
        public async Task VerifyOtp_ShouldIncreaseAttempts_WhenOtpIncorrect()
        {
            var dto = CreateDto(code: "999999");
            var user = CreateUser();
            var otp = CreateOtp(user.Id, attempts: 0);

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            OtpRepositoryMock.Setup(o => o.GetLatestAsync(user.Id, OtpPurpose.ForgotPassword)).ReturnsAsync(otp);

            await Assert.ThrowsAsync<AppException>(() => Sut.VerifyOtpForPasswordResetAsync(dto));

            OtpRepositoryMock.Verify(o => o.UpdateAsync(It.IsAny<EmailOtp>()), Times.Once);
        }

        [Fact]
        public async Task VerifyOtp_ShouldDisableOtp_WhenAttemptsExceeded()
        {
            var dto = CreateDto(code: "999999");
            var user = CreateUser();
            var otp = CreateOtp(user.Id, attempts: OtpConstants.MaxAttempts - 1);

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            OtpRepositoryMock.Setup(o => o.GetLatestAsync(user.Id, OtpPurpose.ForgotPassword)).ReturnsAsync(otp);

            await Assert.ThrowsAsync<AppException>(() => Sut.VerifyOtpForPasswordResetAsync(dto));

            Assert.True(otp.IsDisabled);
            OtpRepositoryMock.Verify(o => o.UpdateAsync(It.IsAny<EmailOtp>()), Times.Once);
        }

        [Fact]
        public async Task VerifyOtp_ShouldDeleteOtp_WhenCorrect()
        {
            var dto = CreateDto();
            var user = CreateUser();
            var otp = CreateOtp(user.Id);

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            OtpRepositoryMock.Setup(o => o.GetLatestAsync(user.Id, OtpPurpose.ForgotPassword)).ReturnsAsync(otp);
            UserManagerMock.Setup(x => x.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("reset-token");

            var token = await Sut.VerifyOtpForPasswordResetAsync(dto);

            Assert.Equal("reset-token", token);
            OtpRepositoryMock.Verify(o => o.DeleteAsync(otp), Times.Once);
        }
    }
}
