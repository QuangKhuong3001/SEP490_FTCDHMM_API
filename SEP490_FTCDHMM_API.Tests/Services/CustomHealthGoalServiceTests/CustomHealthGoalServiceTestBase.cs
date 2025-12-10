using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Implementations.HealthGoalImplementations;

namespace SEP490_FTCDHMM_API.Tests.Services.CustomHealthGoalServiceTests
{
    public abstract class CustomHealthGoalServiceTestBase
    {
        protected readonly Mock<ICustomHealthGoalRepository> CustomHealthGoalRepositoryMock = new();
        protected readonly Mock<INutrientRepository> NutrientRepositoryMock = new();
        protected readonly Mock<IMapper> MapperMock;
        protected readonly CustomHealthGoalService Sut;

        protected CustomHealthGoalServiceTestBase()
        {
            MapperMock = new Mock<IMapper>();

            Sut = new CustomHealthGoalService(
                CustomHealthGoalRepositoryMock.Object,
                MapperMock.Object,
                NutrientRepositoryMock.Object
            );
        }
    }
}
