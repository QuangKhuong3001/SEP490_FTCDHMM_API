using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Implementations;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.UserServiceTests
{
    public abstract class UserServiceTestBase
    {
        protected readonly Mock<IUserRepository> UserRepositoryMock;
        protected readonly Mock<IMapper> MapperMock;
        protected readonly Mock<IRoleRepository> RoleRepositoryMock;
        protected readonly Mock<IMailService> MailServiceMock;
        protected readonly Mock<IEmailTemplateService> EmailTemplateServiceMock;
        protected readonly Mock<IS3ImageService> S3ImageServiceMock;
        protected readonly Mock<IUserFollowRepository> UserFollowRepositoryMock;

        protected readonly UserService Sut;

        protected UserServiceTestBase()
        {
            UserRepositoryMock = new Mock<IUserRepository>();
            MapperMock = new Mock<IMapper>();
            RoleRepositoryMock = new Mock<IRoleRepository>();
            MailServiceMock = new Mock<IMailService>();
            EmailTemplateServiceMock = new Mock<IEmailTemplateService>();
            S3ImageServiceMock = new Mock<IS3ImageService>();
            UserFollowRepositoryMock = new Mock<IUserFollowRepository>();

            Sut = new UserService(
                UserRepositoryMock.Object,
                MapperMock.Object,
                RoleRepositoryMock.Object,
                MailServiceMock.Object,
                EmailTemplateServiceMock.Object,
                S3ImageServiceMock.Object,
                UserFollowRepositoryMock.Object
            );
        }

        protected static AppUser CreateUser(Guid? id = null, ActivityLevel? activityLevel = null)
        {
            return new AppUser
            {
                Id = id ?? Guid.NewGuid(),
                FirstName = "Nguyen",
                LastName = "Tra",
                UserName = "user001",
                Email = "user@example.com",
                Gender = Gender.Male,
                DateOfBirth = DateTime.UtcNow.AddYears(-20),
                ActivityLevel = activityLevel ?? ActivityLevel.From("SEDENTARY")
            };
        }
    }
}
