using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.NutrientServiceTests
{
    public class GetRequiredNutrientsAsyncTests : NutrientServiceTestBase
    {
        [Fact]
        public async Task GetRequiredNutrientsAsync_ShouldReturnMappedResult()
        {
            var n1 = CreateNutrient(Guid.NewGuid(), isMacro: true);
            var n2 = CreateNutrient(Guid.NewGuid(), isMacro: true);
            var list = new List<Nutrient> { n1, n2 };

            NutrientRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Nutrient, bool>>>(),
                    It.IsAny<Func<IQueryable<Nutrient>, IQueryable<Nutrient>>>()))
                .ReturnsAsync(list);

            var mapped = new List<NutrientNameResponse>
            {
                new NutrientNameResponse(),
                new NutrientNameResponse()
            };

            MapperMock
                .Setup(m => m.Map<IEnumerable<NutrientNameResponse>>(list))
                .Returns(mapped);

            var result = await Sut.GetRequiredNutrientsAsync();

            Assert.Equal(2, result.Count());

            NutrientRepositoryMock.Verify(r => r.GetAllAsync(
                It.IsAny<Expression<Func<Nutrient, bool>>>(),
                It.IsAny<Func<IQueryable<Nutrient>, IQueryable<Nutrient>>>()), Times.Once);

            MapperMock.Verify(m => m.Map<IEnumerable<NutrientNameResponse>>(list), Times.Once);
        }
    }
}
