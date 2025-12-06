using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.AuthServiceTests
{
    public class ResendVerifyAccountEmailOtpAsync : AuthServiceTestBase
    {
        private ResendOtpRequest CreateRequest()
        {
            return new ResendOtpRequest
            {
                Email = "user@example.com"
            };
        }

        [Fact]
        public async Task ResendOtp_ShouldThrow_WhenUserNotFound()
        {
            var dto = new ResendOtpRequest { Email = "abc@example.com" };

            UserManagerMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync((AppUser)null!);

            await Assert.ThrowsAsync<AppException>(() => Sut.ResendVerifyAccountEmailOtpAsync(dto));
        }

        [Fact]
        public async Task ResendOtp_ShouldThrow_WhenEmailAlreadyConfirmed_AndPurposeIsVerify()
        {
            var dto = CreateRequest();
            var user = CreateUser(emailConfirmed: true);

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
                           .ReturnsAsync(user);

            var ex = await Assert.ThrowsAsync<AppException>(() =>
                Sut.ResendVerifyAccountEmailOtpAsync(dto));

            Assert.Equal(AppResponseCode.INVALID_ACTION, ex.ResponseCode);
        }

        [Fact]
        public async Task ResendOtp_ShouldDeleteOldOtp_WhenExists()
        {
            var dto = CreateRequest();
            var user = CreateUser(emailConfirmed: false);
            var oldOtp = CreateOtp(user.Id, OtpPurpose.VerifyAccountEmail);

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);

            OtpRepositoryMock
                .Setup(x => x.GetLatestAsync(user.Id, OtpPurpose.VerifyAccountEmail))
                .ReturnsAsync(oldOtp);

            EmailTemplateServiceMock
                .Setup(x => x.RenderTemplateAsync(It.IsAny<EmailTemplateType>(), It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("html");

            await Sut.ResendVerifyAccountEmailOtpAsync(dto);

            OtpRepositoryMock.Verify(x => x.DeleteAsync(oldOtp), Times.Once);
        }

        [Fact]
        public async Task ResendOtp_ShouldNotDeleteOldOtp_WhenNoneExists()
        {
            var dto = CreateRequest();
            var user = CreateUser(emailConfirmed: false);

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            OtpRepositoryMock.Setup(x => x.GetLatestAsync(user.Id, OtpPurpose.VerifyAccountEmail))
                             .ReturnsAsync((EmailOtp?)null);

            EmailTemplateServiceMock
                .Setup(x => x.RenderTemplateAsync(It.IsAny<EmailTemplateType>(), It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("html");

            await Sut.ResendVerifyAccountEmailOtpAsync(dto);

            OtpRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<EmailOtp>()), Times.Never);
        }

        [Fact]
        public async Task ResendOtp_ShouldCreateNewOtp()
        {
            var dto = CreateRequest();
            var user = CreateUser(emailConfirmed: false);

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            OtpRepositoryMock.Setup(x => x.GetLatestAsync(user.Id, OtpPurpose.VerifyAccountEmail))
                             .ReturnsAsync((EmailOtp?)null);

            EmailTemplateServiceMock
                .Setup(x => x.RenderTemplateAsync(It.IsAny<EmailTemplateType>(), It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("html");

            await Sut.ResendVerifyAccountEmailOtpAsync(dto);

            OtpRepositoryMock.Verify(x => x.AddAsync(It.Is<EmailOtp>(otp =>
                otp.Purpose == OtpPurpose.VerifyAccountEmail &&
                otp.SentToId == user.Id)), Times.Once);
        }

        [Fact]
        public async Task ResendOtp_ShouldRenderTemplate_VerifyAccountEmail()
        {
            var dto = CreateRequest();
            var user = CreateUser(emailConfirmed: false);

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            OtpRepositoryMock.Setup(x => x.GetLatestAsync(user.Id, OtpPurpose.VerifyAccountEmail))
                             .ReturnsAsync((EmailOtp?)null);

            EmailTemplateServiceMock
                .Setup(e => e.RenderTemplateAsync(EmailTemplateType.VerifyAccountEmail, It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("html");

            await Sut.ResendVerifyAccountEmailOtpAsync(dto);

            EmailTemplateServiceMock.Verify(
                e => e.RenderTemplateAsync(EmailTemplateType.VerifyAccountEmail, It.IsAny<Dictionary<string, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ResendOtp_ShouldSendEmail()
        {
            var dto = CreateRequest();
            var user = CreateUser(emailConfirmed: false);

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            OtpRepositoryMock.Setup(x => x.GetLatestAsync(user.Id, OtpPurpose.VerifyAccountEmail))
                             .ReturnsAsync((EmailOtp?)null);

            EmailTemplateServiceMock
                .Setup(e => e.RenderTemplateAsync(It.IsAny<EmailTemplateType>(), It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("html");

            await Sut.ResendVerifyAccountEmailOtpAsync(dto);

            MailServiceMock.Verify(
                m => m.SendEmailAsync(dto.Email, It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
        }
    }
}
