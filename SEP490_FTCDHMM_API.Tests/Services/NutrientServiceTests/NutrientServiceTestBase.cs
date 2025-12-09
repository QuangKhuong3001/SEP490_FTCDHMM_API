using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Implementations;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.NutrientServiceTests
{
    public abstract class NutrientServiceTestBase
    {
        protected Mock<INutrientRepository> NutrientRepositoryMock { get; }
        protected Mock<IMapper> MapperMock { get; }
        protected NutrientService Sut { get; }

        protected NutrientServiceTestBase()
        {
            NutrientRepositoryMock = new Mock<INutrientRepository>(MockBehavior.Strict);
            MapperMock = new Mock<IMapper>(MockBehavior.Strict);
            Sut = new NutrientService(MapperMock.Object, NutrientRepositoryMock.Object);
        }

        protected Nutrient CreateNutrient(Guid id, bool isMacro = false)
        {
            return new Nutrient
            {
                Id = id,
                Name = "Protein",
                VietnameseName = "Protein",
                Description = "desc",
                IsMacroNutrient = isMacro,
                UnitId = Guid.NewGuid(),
                Unit = new NutrientUnit
                {
                    Id = Guid.NewGuid(),
                    Name = "Gram",
                    Symbol = "g",
                    Description = "gram"
                }
            };
        }
    }
}
