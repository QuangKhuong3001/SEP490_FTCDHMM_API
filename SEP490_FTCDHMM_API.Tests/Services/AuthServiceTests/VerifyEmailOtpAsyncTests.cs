using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs;
using SEP490_FTCDHMM_API.Domain.Constants;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;
using SEP490_FTCDHMM_API.Shared.Utils;

namespace SEP490_FTCDHMM_API.Tests.Services.AuthServiceTests
{
    public class AuthService_VerifyEmailOtpTests : AuthServiceTestBase
    {
        private OtpVerifyRequest CreateValidOtpRequest()
        {
            return new OtpVerifyRequest
            {
                Email = "user@example.com",
                Code = "123456"
            };
        }

        private EmailOtp CreateOtp(Guid userId, string hashedCode, bool disabled = false, int attempts = 0, int minutesOffset = +5)
        {
            return new EmailOtp
            {
                Id = Guid.NewGuid(),
                SentToId = userId,
                Code = hashedCode,
                Purpose = OtpPurpose.VerifyAccountEmail,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(minutesOffset),
                Attempts = attempts,
                IsDisabled = disabled
            };
        }

        [Fact]
        public async Task VerifyEmailOtp_ShouldThrow_WhenUserNotFound()
        {
            var dto = CreateValidOtpRequest();

            UserManagerMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync((AppUser?)null);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.VerifyEmailOtpAsync(dto));

            Assert.Equal(AppResponseCode.INVALID_ACCOUNT_INFORMATION, ex.ResponseCode);
        }

        [Fact]
        public async Task VerifyEmailOtp_ShouldThrow_WhenOtpNotFound()
        {
            var dto = CreateValidOtpRequest();
            var user = CreateUser();

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            OtpRepositoryMock.Setup(x => x.GetLatestAsync(user.Id, OtpPurpose.VerifyAccountEmail))
                             .ReturnsAsync((EmailOtp?)null);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.VerifyEmailOtpAsync(dto));

            Assert.Equal(AppResponseCode.NOT_FOUND, ex.ResponseCode);
        }

        [Fact]
        public async Task VerifyEmailOtp_ShouldThrow_WhenOtpIsDisabled()
        {
            var dto = CreateValidOtpRequest();
            var user = CreateUser();

            var otp = CreateOtp(user.Id, "HASHED", disabled: true);

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            OtpRepositoryMock.Setup(x => x.GetLatestAsync(user.Id, OtpPurpose.VerifyAccountEmail))
                             .ReturnsAsync(otp);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.VerifyEmailOtpAsync(dto));

            Assert.Equal(AppResponseCode.OTP_INVALID, ex.ResponseCode);
        }

        [Fact]
        public async Task VerifyEmailOtp_ShouldThrow_WhenOtpExpired()
        {
            var dto = CreateValidOtpRequest();
            var user = CreateUser();
            var otp = CreateOtp(user.Id, "HASHED", minutesOffset: -1);

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            OtpRepositoryMock.Setup(x => x.GetLatestAsync(user.Id, OtpPurpose.VerifyAccountEmail))
                             .ReturnsAsync(otp);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.VerifyEmailOtpAsync(dto));

            Assert.Equal(AppResponseCode.OTP_INVALID, ex.ResponseCode);
            Assert.True(otp.IsDisabled);

            OtpRepositoryMock.Verify(x => x.UpdateAsync(otp), Times.Once);
        }

        [Fact]
        public async Task VerifyEmailOtp_ShouldThrow_WhenOtpIncorrect_AndIncreaseAttempts()
        {
            var dto = CreateValidOtpRequest();
            var user = CreateUser();

            string wrongHash = HashHelper.ComputeSha256Hash("000000");
            var otp = CreateOtp(user.Id, wrongHash, attempts: 0);

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            OtpRepositoryMock.Setup(x => x.GetLatestAsync(user.Id, OtpPurpose.VerifyAccountEmail))
                             .ReturnsAsync(otp);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.VerifyEmailOtpAsync(dto));

            Assert.Equal(AppResponseCode.OTP_INVALID, ex.ResponseCode);
            Assert.Equal(1, otp.Attempts);

            OtpRepositoryMock.Verify(x => x.UpdateAsync(otp), Times.Once);
        }

        [Fact]
        public async Task VerifyEmailOtp_ShouldDisable_WhenAttemptsReachLimit()
        {
            var dto = CreateValidOtpRequest();
            var user = CreateUser();

            string wrongHash = HashHelper.ComputeSha256Hash("000000");
            int maxAttempts = OtpConstants.MaxAttempts;

            var otp = CreateOtp(user.Id, wrongHash, attempts: maxAttempts - 1);

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            OtpRepositoryMock.Setup(x => x.GetLatestAsync(user.Id, OtpPurpose.VerifyAccountEmail))
                             .ReturnsAsync(otp);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.VerifyEmailOtpAsync(dto));

            Assert.Equal(AppResponseCode.OTP_INVALID, ex.ResponseCode);
            Assert.True(otp.IsDisabled);

            OtpRepositoryMock.Verify(x => x.UpdateAsync(otp), Times.Once);
        }

        [Fact]
        public async Task VerifyEmailOtp_ShouldSuccess_WhenOtpCorrect()
        {
            var dto = CreateValidOtpRequest();
            var user = CreateUser();

            string hashedCorrect = HashHelper.ComputeSha256Hash(dto.Code);
            var otp = CreateOtp(user.Id, hashedCorrect);

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            OtpRepositoryMock.Setup(x => x.GetLatestAsync(user.Id, OtpPurpose.VerifyAccountEmail))
                             .ReturnsAsync(otp);

            var result = Sut.VerifyEmailOtpAsync(dto);

            await result;

            Assert.True(user.EmailConfirmed);

            OtpRepositoryMock.Verify(x => x.DeleteAsync(otp), Times.Once);
            UserManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);
        }
    }
}
