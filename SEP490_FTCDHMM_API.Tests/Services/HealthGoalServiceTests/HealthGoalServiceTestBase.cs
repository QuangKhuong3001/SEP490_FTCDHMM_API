using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Implementations.HealthGoalImplementations;
using SEP490_FTCDHMM_API.Application.Services.Implementations.SEP490_FTCDHMM_API.Application.Interfaces;

namespace SEP490_FTCDHMM_API.Tests.Services.HealthGoalServiceTests
{
    public abstract class HealthGoalServiceTestBase
    {
        protected Mock<IHealthGoalRepository> HealthGoalRepositoryMock { get; }
        protected Mock<IHealthGoalTargetRepository> HealthGoalTargetRepositoryMock { get; }
        protected Mock<ICustomHealthGoalRepository> CustomHealthGoalRepositoryMock { get; }
        protected Mock<INutrientRepository> NutrientRepositoryMock { get; }
        protected Mock<ICacheService> CacheServiceMock = new();
        protected Mock<IMapper> MapperMock { get; }

        protected HealthGoalService Sut { get; }

        protected HealthGoalServiceTestBase()
        {
            HealthGoalRepositoryMock = new(MockBehavior.Strict);
            HealthGoalTargetRepositoryMock = new(MockBehavior.Strict);
            CustomHealthGoalRepositoryMock = new(MockBehavior.Strict);
            NutrientRepositoryMock = new(MockBehavior.Strict);
            CacheServiceMock = new(MockBehavior.Strict);
            MapperMock = new(MockBehavior.Strict);

            Sut = new HealthGoalService(
                HealthGoalRepositoryMock.Object,
                NutrientRepositoryMock.Object,
                HealthGoalTargetRepositoryMock.Object,
                MapperMock.Object,
                CacheServiceMock.Object,
                CustomHealthGoalRepositoryMock.Object
            );
        }

        protected Guid NewId() => Guid.NewGuid();
    }
}
