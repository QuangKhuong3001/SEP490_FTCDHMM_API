using Microsoft.AspNetCore.Identity;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.AuthServiceTests
{
    public class RegisterAsyncTests : AuthServiceTestBase
    {
        [Fact]
        public async Task Register_ShouldThrow_WhenAgeInvalid()
        {
            var dto = new RegisterRequest
            {
                Email = "x@gmail.com",
                Password = "Password1!",
                FirstName = "A",
                LastName = "B",
                Gender = "MALE",
                DateOfBirth = DateTime.UtcNow
            };

            await Assert.ThrowsAsync<AppException>(() => Sut.RegisterAsync(dto));
        }

        [Fact]
        public async Task Register_ShouldThrow_WhenEmailExists()
        {
            var dto = new RegisterRequest
            {
                Email = "x@gmail.com",
                Password = "Password1!",
                FirstName = "A",
                LastName = "B",
                Gender = "MALE",
                DateOfBirth = DateTime.UtcNow.AddYears(-20)
            };

            UserManagerMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync(new AppUser
                {
                    FirstName = "A",
                    LastName = "B",
                    UserName = "u1",
                    Email = dto.Email,
                    Gender = Gender.From("MALE"),
                    DateOfBirth = dto.DateOfBirth
                });

            await Assert.ThrowsAsync<AppException>(() => Sut.RegisterAsync(dto));
        }

        [Fact]
        public async Task Register_ShouldUseExtractedUserName_WhenNotExists()
        {
            var dto = new RegisterRequest
            {
                Email = "john.doe@gmail.com",
                Password = "Password1!",
                FirstName = "John",
                LastName = "Doe",
                Gender = "MALE",
                DateOfBirth = DateTime.UtcNow.AddYears(-20)
            };

            var role = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = RoleValue.Customer.Name,
                IsActive = true
            };

            AppUser captured = null!;

            RoleRepositoryMock
                .Setup(x => x.FindByNameAsync(RoleValue.Customer.Name))
                .ReturnsAsync(role);

            UserManagerMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync((AppUser)null!);

            UserManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<AppUser, string>((u, _) => captured = u);

            OtpRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<EmailOtp>()))
                .ReturnsAsync(new EmailOtp
                {
                    Code = "code",
                    Purpose = OtpPurpose.VerifyAccountEmail,
                    SentToId = Guid.NewGuid(),
                    ExpiresAtUtc = DateTime.UtcNow.AddMinutes(5)
                });

            EmailTemplateServiceMock
                .Setup(x => x.RenderTemplateAsync(EmailTemplateType.VerifyAccountEmail, It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("html");

            MailServiceMock
                .Setup(x => x.SendEmailAsync(dto.Email, "html", It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            NotificationCommandServiceMock
                .Setup(x => x.CreateAndSendNotificationAsync(
                    null,
                    It.IsAny<Guid>(),
                    NotificationType.System,
                    null))
                .Returns(Task.CompletedTask);

            var result = await Sut.RegisterAsync(dto);

            Assert.True(result.Success);
            Assert.StartsWith("john.doe", captured.UserName);
        }

        [Fact]
        public async Task Register_ShouldRetry_WhenDuplicateUserName()
        {
            var dto = new RegisterRequest
            {
                Email = "retry@gmail.com",
                Password = "Password1!",
                FirstName = "A",
                LastName = "B",
                Gender = "MALE",
                DateOfBirth = DateTime.UtcNow.AddYears(-20)
            };

            var role = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = RoleValue.Customer.Name,
                IsActive = true
            };

            int counter = 0;

            RoleRepositoryMock
                .Setup(x => x.FindByNameAsync(RoleValue.Customer.Name))
                .ReturnsAsync(role);

            UserManagerMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync((AppUser)null!);

            UserManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), dto.Password))
                .ReturnsAsync(() =>
                {
                    counter++;
                    if (counter == 1)
                        return IdentityResult.Failed(new IdentityError { Code = "DuplicateUserName" });
                    return IdentityResult.Success;
                });

            OtpRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<EmailOtp>()))
                .ReturnsAsync(new EmailOtp
                {
                    Code = "code",
                    Purpose = OtpPurpose.VerifyAccountEmail,
                    SentToId = Guid.NewGuid(),
                    ExpiresAtUtc = DateTime.UtcNow.AddMinutes(5)
                });

            EmailTemplateServiceMock
                .Setup(x => x.RenderTemplateAsync(EmailTemplateType.VerifyAccountEmail, It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync("html");

            MailServiceMock
                .Setup(x => x.SendEmailAsync(dto.Email, "html", It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            NotificationCommandServiceMock
                .Setup(x => x.CreateAndSendNotificationAsync(
                    null,
                    It.IsAny<Guid>(),
                    NotificationType.System,
                    null))
                .Returns(Task.CompletedTask);

            var result = await Sut.RegisterAsync(dto);

            Assert.True(result.Success);
            Assert.Equal(2, counter);
        }
    }
}
