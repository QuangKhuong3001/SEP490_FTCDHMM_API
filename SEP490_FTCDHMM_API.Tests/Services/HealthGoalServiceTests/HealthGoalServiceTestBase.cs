using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Implementations.HealthGoalImplementations;

namespace SEP490_FTCDHMM_API.Tests.Services.HealthGoalServiceTests
{
    public abstract class HealthGoalServiceTestBase
    {
        protected Mock<IHealthGoalRepository> HealthGoalRepositoryMock { get; }
        protected Mock<IHealthGoalTargetRepository> HealthGoalTargetRepositoryMock { get; }
        protected Mock<ICustomHealthGoalRepository> CustomHealthGoalRepositoryMock { get; }
        protected Mock<INutrientRepository> NutrientRepositoryMock { get; }
        protected Mock<IMapper> MapperMock { get; }

        protected HealthGoalService Sut { get; }

        protected HealthGoalServiceTestBase()
        {
            HealthGoalRepositoryMock = new Mock<IHealthGoalRepository>(MockBehavior.Strict);
            HealthGoalTargetRepositoryMock = new Mock<IHealthGoalTargetRepository>(MockBehavior.Strict);
            CustomHealthGoalRepositoryMock = new Mock<ICustomHealthGoalRepository>(MockBehavior.Strict);
            NutrientRepositoryMock = new Mock<INutrientRepository>(MockBehavior.Strict);
            MapperMock = new Mock<IMapper>(MockBehavior.Strict);

            Sut = new HealthGoalService(
                HealthGoalRepositoryMock.Object,
                NutrientRepositoryMock.Object,
                HealthGoalTargetRepositoryMock.Object,
                MapperMock.Object,
                CustomHealthGoalRepositoryMock.Object
            );
        }

        protected Guid NewId() => Guid.NewGuid();
    }
}
