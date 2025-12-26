using Microsoft.AspNetCore.Identity;
using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Implementations;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.AuthServiceTests
{
    public abstract class AuthServiceTestBase
    {
        protected readonly Mock<UserManager<AppUser>> UserManagerMock;
        protected readonly Mock<IRoleRepository> RoleRepositoryMock;
        protected readonly Mock<IOtpRepository> OtpRepositoryMock;
        protected readonly Mock<INotificationCommandService> NotificationCommandServiceMock;
        protected readonly Mock<IMailService> MailServiceMock;
        protected readonly Mock<IJwtAuthService> JwtServiceMock;
        protected readonly Mock<IGoogleAuthService> GoogleAuthServiceMock;
        protected readonly Mock<IGoogleProvisioningService> GoogleProvisioningServiceMock;
        protected readonly Mock<IEmailTemplateService> EmailTemplateServiceMock;
        protected readonly Mock<IUserMealSlotInitializer> UserMealSlotInitializerMock;

        protected readonly AuthService Sut;

        protected AuthServiceTestBase()
        {
            UserManagerMock = CreateUserManagerMock();

            RoleRepositoryMock = new Mock<IRoleRepository>();
            OtpRepositoryMock = new Mock<IOtpRepository>();
            NotificationCommandServiceMock = new Mock<INotificationCommandService>();
            MailServiceMock = new Mock<IMailService>();
            JwtServiceMock = new Mock<IJwtAuthService>();
            GoogleAuthServiceMock = new Mock<IGoogleAuthService>();
            GoogleProvisioningServiceMock = new Mock<IGoogleProvisioningService>();
            EmailTemplateServiceMock = new Mock<IEmailTemplateService>();
            UserMealSlotInitializerMock = new Mock<IUserMealSlotInitializer>();

            Sut = new AuthService(
                UserManagerMock.Object,
                RoleRepositoryMock.Object,
                OtpRepositoryMock.Object,
                NotificationCommandServiceMock.Object,
                MailServiceMock.Object,
                JwtServiceMock.Object,
                GoogleProvisioningServiceMock.Object,
                GoogleAuthServiceMock.Object,
                UserMealSlotInitializerMock.Object,
                EmailTemplateServiceMock.Object
            );
        }

        protected static AppUser CreateUser(Guid? roleId = null, bool emailConfirmed = true)
        {
            return new AppUser
            {
                Id = Guid.NewGuid(),
                FirstName = "Nguyen",
                LastName = "Tra",
                UserName = "user",
                Email = "user@example.com",
                EmailConfirmed = emailConfirmed,
                Gender = Gender.Male,
                DateOfBirth = DateTime.UtcNow.AddYears(-20),
                RoleId = roleId ?? Guid.NewGuid()
            };
        }

        protected static AppRole CreateRole(string name = "Customer")
        {
            return new AppRole
            {
                Id = Guid.NewGuid(),
                Name = name
            };
        }

        protected static EmailOtp CreateOtp(Guid userId, OtpPurpose purpose)
        {
            return new EmailOtp
            {
                Id = Guid.NewGuid(),
                Code = "123456",
                Purpose = purpose,
                SentToId = userId,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(5),
                CreatedAtUtc = DateTime.UtcNow
            };
        }

        protected static Mock<UserManager<AppUser>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<AppUser>>();

            return new Mock<UserManager<AppUser>>(
                store.Object,
                null!,
                null!,
                Array.Empty<IUserValidator<AppUser>>(),
                Array.Empty<IPasswordValidator<AppUser>>(),
                null!,
                null!,
                null!,
                null!
            );
        }
    }
}
