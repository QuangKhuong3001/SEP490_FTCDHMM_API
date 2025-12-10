using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.LabelDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.LabelServiceTests
{
    public class GetLabelsAsyncTests : LabelServiceTestBase
    {
        [Fact]
        public async Task GetLabels_ShouldReturnMappedList()
        {
            var dto = new LabelSearchDropboxRequest { Keyword = "food" };

            var list = new List<Label> { CreateLabel(), CreateLabel() };

            LabelRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Label, bool>>>(),
                    It.IsAny<Func<IQueryable<Label>, IQueryable<Label>>?>()
                ))
                .ReturnsAsync(list);

            var mapped = new List<LabelResponse>
            {
                new LabelResponse{ Id = list[0].Id, Name = list[0].Name }
            };

            MapperMock
                .Setup(m => m.Map<IEnumerable<LabelResponse>>(It.IsAny<IEnumerable<Label>>()))
                .Returns(mapped);

            var result = await Sut.GetLabelsAsync(dto);

            Assert.Single(result);

            LabelRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }
    }
}
