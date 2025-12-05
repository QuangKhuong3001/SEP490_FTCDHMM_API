using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.AuthServiceTests
{
    public class ForgotPasswordRequestAsyncTests : AuthServiceTestBase
    {
        private ForgotPasswordRequest CreateDto()
        {
            return new ForgotPasswordRequest
            {
                Email = "user@example.com"
            };
        }

        [Fact]
        public async Task ForgotPassword_ShouldDoNothing_WhenUserNotFound()
        {
            var dto = CreateDto();

            UserManagerMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync((AppUser)null!);

            await Sut.ForgotPasswordRequestAsync(dto);

            OtpRepositoryMock.Verify(o => o.AddAsync(It.IsAny<EmailOtp>()), Times.Never);
            EmailTemplateServiceMock.Verify(e =>
                e.RenderTemplateAsync(It.IsAny<EmailTemplateType>(), It.IsAny<Dictionary<string, string>>()),
                Times.Never);
            MailServiceMock.Verify(m =>
                m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task ForgotPassword_ShouldCreateOtp_WhenUserExists()
        {
            var dto = CreateDto();
            var user = CreateUser();

            UserManagerMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync(user);

            EmailTemplateServiceMock
                .Setup(e => e.RenderTemplateAsync(It.IsAny<EmailTemplateType>(), It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("html-body");

            await Sut.ForgotPasswordRequestAsync(dto);

            OtpRepositoryMock.Verify(o =>
                o.AddAsync(It.Is<EmailOtp>(otp =>
                    otp.SentToId == user.Id &&
                    otp.Purpose == OtpPurpose.ForgotPassword
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task ForgotPassword_ShouldRenderTemplate()
        {
            var dto = CreateDto();
            var user = CreateUser();

            UserManagerMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync(user);

            EmailTemplateServiceMock
                .Setup(e => e.RenderTemplateAsync(EmailTemplateType.ForgotPassword, It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("html-body");

            await Sut.ForgotPasswordRequestAsync(dto);

            EmailTemplateServiceMock.Verify(e =>
                e.RenderTemplateAsync(
                    EmailTemplateType.ForgotPassword,
                    It.IsAny<Dictionary<string, string>>()),
                Times.Once
            );
        }

        [Fact]
        public async Task ForgotPassword_ShouldSendEmail()
        {
            var dto = CreateDto();
            var user = CreateUser();

            UserManagerMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync(user);

            EmailTemplateServiceMock
                .Setup(e => e.RenderTemplateAsync(It.IsAny<EmailTemplateType>(), It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("html-body");

            await Sut.ForgotPasswordRequestAsync(dto);

            MailServiceMock.Verify(m =>
                m.SendEmailAsync(
                    dto.Email,
                    "html-body",
                    "Đặt lại mật khẩu tài khoản FitFood Tracker"),
                Times.Once
            );
        }
    }
}
