using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Implementations;

namespace SEP490_FTCDHMM_API.Tests.Services.UserMealSlotServiceTests
{
    public abstract class UserMealSlotServiceTestBase
    {
        protected Mock<IMealSlotRepository> MealSlotRepositoryMock { get; }
        protected Mock<IMapper> MapperMock { get; }

        protected UserMealSlotService Sut { get; }

        protected UserMealSlotServiceTestBase()
        {
            MealSlotRepositoryMock = new(MockBehavior.Strict);
            MapperMock = new(MockBehavior.Strict);

            Sut = new UserMealSlotService(
                MapperMock.Object,
                MealSlotRepositoryMock.Object
            );
        }

        protected Guid NewId() => Guid.NewGuid();
    }
}
