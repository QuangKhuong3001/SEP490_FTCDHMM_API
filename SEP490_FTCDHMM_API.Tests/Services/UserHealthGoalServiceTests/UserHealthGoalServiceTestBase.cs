using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Implementations.HealthGoalImplementations;

namespace SEP490_FTCDHMM_API.Tests.Services.UserHealthGoalServiceTests
{
    public abstract class UserHealthGoalServiceTestBase
    {
        protected readonly Mock<IUserHealthGoalRepository> UserHealthGoalRepositoryMock;
        protected readonly Mock<IHealthGoalRepository> HealthGoalRepositoryMock;
        protected readonly Mock<ICustomHealthGoalRepository> CustomHealthGoalRepositoryMock;
        protected readonly Mock<IMapper> MapperMock;
        protected readonly UserHealthGoalService Sut;

        protected UserHealthGoalServiceTestBase()
        {
            UserHealthGoalRepositoryMock = new(MockBehavior.Strict);
            HealthGoalRepositoryMock = new(MockBehavior.Strict);
            CustomHealthGoalRepositoryMock = new(MockBehavior.Strict);
            MapperMock = new(MockBehavior.Strict);

            Sut = new UserHealthGoalService(
                UserHealthGoalRepositoryMock.Object,
                HealthGoalRepositoryMock.Object,
                MapperMock.Object,
                CustomHealthGoalRepositoryMock.Object
            );
        }

        protected Guid NewId() => Guid.NewGuid();
    }
}
