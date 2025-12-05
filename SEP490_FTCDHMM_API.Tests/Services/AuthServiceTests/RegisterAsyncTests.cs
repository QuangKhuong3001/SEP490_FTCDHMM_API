using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs;
using SEP490_FTCDHMM_API.Domain.Constants;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.AuthServiceTests
{
    public class AuthService_RegisterTests : AuthServiceTestBase
    {
        private RegisterRequest CreateValidRegisterDto()
        {
            return new RegisterRequest
            {
                FirstName = "Nguyen",
                LastName = "Tra",
                Email = "user@example.com",
                Gender = "Male",
                DateOfBirth = DateTime.UtcNow.AddYears(-20),
                Password = "12345@aA",
                RePassword = "12345@aA"
            };
        }

        [Fact]
        public async Task Register_ShouldThrow_WhenAgeLessThanMin()
        {
            var dto = CreateValidRegisterDto();
            dto.DateOfBirth = DateTime.UtcNow.AddYears(-(AuthConstants.MIN_REGISTER_AGE - 1));

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.RegisterAsync(dto));

            Assert.Equal(AppResponseCode.INVALID_ACTION, ex.ResponseCode);
        }

        [Fact]
        public async Task Register_ShouldThrow_WhenAgeGreaterThanMax()
        {
            var dto = CreateValidRegisterDto();
            dto.DateOfBirth = DateTime.UtcNow.AddYears(-(AuthConstants.MAX_REGISTER_AGE + 1));

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.RegisterAsync(dto));

            Assert.Equal(AppResponseCode.INVALID_ACTION, ex.ResponseCode);
        }

        [Fact]
        public async Task Register_ShouldThrow_WhenEmailAlreadyExists()
        {
            var dto = CreateValidRegisterDto();
            var existing = CreateUser();

            UserManagerMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync(existing);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.RegisterAsync(dto));

            Assert.Equal(AppResponseCode.EXISTS, ex.ResponseCode);
        }

        [Fact]
        public async Task Register_ShouldThrow_WhenCustomerRoleNotFound()
        {
            var dto = CreateValidRegisterDto();

            UserManagerMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync((AppUser)null!);

            RoleRepositoryMock
                .Setup(r => r.FindByNameAsync(RoleValue.Customer.Name))
                .ReturnsAsync((AppRole)null!);

            await Assert.ThrowsAsync<NullReferenceException>(() => Sut.RegisterAsync(dto));
        }

        [Fact]
        public async Task Register_ShouldUseExtractedUsername_WhenNotExists()
        {
            var dto = CreateValidRegisterDto();

            UserManagerMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync((AppUser)null!);

            RoleRepositoryMock
                .Setup(r => r.FindByNameAsync(RoleValue.Customer.Name))
                .ReturnsAsync(CreateRole());

            UserRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
                .ReturnsAsync(false);

            UserManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            EmailTemplateServiceMock
                .Setup(e => e.RenderTemplateAsync(It.IsAny<EmailTemplateType>(), It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("html");

            var (success, _) = await Sut.RegisterAsync(dto);

            Assert.True(success);

            UserRepositoryMock.Verify(
                x => x.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Register_ShouldIncrementUsername_WhenExists()
        {
            var dto = CreateValidRegisterDto();

            UserManagerMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync((AppUser)null!);

            RoleRepositoryMock
                .Setup(r => r.FindByNameAsync(RoleValue.Customer.Name))
                .ReturnsAsync(CreateRole());

            UserRepositoryMock
                .SetupSequence(r => r.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
                .ReturnsAsync(true)
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            UserManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            EmailTemplateServiceMock
                .Setup(e => e.RenderTemplateAsync(It.IsAny<EmailTemplateType>(), It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("html");

            var (success, _) = await Sut.RegisterAsync(dto);

            Assert.True(success);

            UserRepositoryMock.Verify(
                r => r.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()),
                Times.Exactly(3)
            );
        }

        [Fact]
        public async Task Register_ShouldReturnFalse_WhenIdentityCreateFailed()
        {
            var dto = CreateValidRegisterDto();

            UserManagerMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync((AppUser)null!);

            RoleRepositoryMock
                .Setup(r => r.FindByNameAsync(RoleValue.Customer.Name))
                .ReturnsAsync(CreateRole());

            UserRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
                .ReturnsAsync(false);

            var identityResult = IdentityResult.Failed(new IdentityError { Description = "Weak" });

            UserManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), dto.Password))
                .ReturnsAsync(identityResult);

            var (success, errors) = await Sut.RegisterAsync(dto);

            Assert.False(success);
            Assert.Single(errors);
            Assert.Contains("Weak", errors.First());
        }

        [Fact]
        public async Task Register_ShouldAddOtp_WhenUserCreatedSuccessfully()
        {
            var dto = CreateValidRegisterDto();

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync((AppUser)null!);
            RoleRepositoryMock.Setup(r => r.FindByNameAsync(RoleValue.Customer.Name)).ReturnsAsync(CreateRole());
            UserRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
                .ReturnsAsync(false);
            UserManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), dto.Password))
                .Callback<AppUser, string>((u, _) => u.Id = Guid.NewGuid())
                .ReturnsAsync(IdentityResult.Success);

            EmailTemplateServiceMock.Setup(e =>
                e.RenderTemplateAsync(It.IsAny<EmailTemplateType>(), It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("html");

            var (success, _) = await Sut.RegisterAsync(dto);

            Assert.True(success);

            OtpRepositoryMock.Verify(o =>
                o.AddAsync(It.Is<EmailOtp>(otp =>
                    otp.Purpose == OtpPurpose.VerifyAccountEmail &&
                    otp.SentToId != Guid.Empty
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task Register_ShouldCall_RenderTemplateAsync()
        {
            var dto = CreateValidRegisterDto();

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync((AppUser)null!);
            RoleRepositoryMock.Setup(r => r.FindByNameAsync(RoleValue.Customer.Name)).ReturnsAsync(CreateRole());
            UserRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
                .ReturnsAsync(false);
            UserManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            EmailTemplateServiceMock.Setup(e =>
                e.RenderTemplateAsync(It.IsAny<EmailTemplateType>(), It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("html");

            var result = await Sut.RegisterAsync(dto);

            Assert.True(result.Success);

            EmailTemplateServiceMock.Verify(
                e => e.RenderTemplateAsync(
                    EmailTemplateType.VerifyAccountEmail,
                    It.IsAny<Dictionary<string, string>>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Register_ShouldCall_SendEmail()
        {
            var dto = CreateValidRegisterDto();

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync((AppUser)null!);
            RoleRepositoryMock.Setup(r => r.FindByNameAsync(RoleValue.Customer.Name)).ReturnsAsync(CreateRole());
            UserRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
                .ReturnsAsync(false);
            UserManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            EmailTemplateServiceMock.Setup(e =>
                e.RenderTemplateAsync(It.IsAny<EmailTemplateType>(), It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("html");

            var result = await Sut.RegisterAsync(dto);

            Assert.True(result.Success);

            MailServiceMock.Verify(
                m => m.SendEmailAsync(dto.Email, It.IsAny<string>(), "Chào mừng bạn đến với FitFood Tracker"),
                Times.Once
            );
        }

        [Fact]
        public async Task Register_ShouldCall_AddNotification()
        {
            var dto = CreateValidRegisterDto();

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync((AppUser)null!);
            RoleRepositoryMock.Setup(r => r.FindByNameAsync(RoleValue.Customer.Name)).ReturnsAsync(CreateRole());
            UserRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
                .ReturnsAsync(false);
            UserManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            EmailTemplateServiceMock.Setup(e =>
                e.RenderTemplateAsync(It.IsAny<EmailTemplateType>(), It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("html");

            var result = await Sut.RegisterAsync(dto);

            Assert.True(result.Success);

            NotificationRepositoryMock.Verify(n =>
                n.AddNotification(
                    null,
                    It.IsAny<Guid>(),
                    NotificationType.System,
                    "Chào mừng bạn đến với FitFood Tracker",
                    null
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task Register_ShouldReturnSuccess_WhenEverythingValid()
        {
            var dto = CreateValidRegisterDto();

            UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync((AppUser)null!);
            RoleRepositoryMock.Setup(r => r.FindByNameAsync(RoleValue.Customer.Name)).ReturnsAsync(CreateRole());
            UserRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
                .ReturnsAsync(false);
            UserManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            EmailTemplateServiceMock.Setup(e =>
                e.RenderTemplateAsync(It.IsAny<EmailTemplateType>(), It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("html");

            var (success, errors) = await Sut.RegisterAsync(dto);

            Assert.True(success);
            Assert.Empty(errors);
        }
    }
}
