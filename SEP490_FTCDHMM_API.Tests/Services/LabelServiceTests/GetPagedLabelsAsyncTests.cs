using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.LabelDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.LabelServiceTests
{
    public class GetPagedLabelsAsyncTests : LabelServiceTestBase
    {
        [Fact]
        public async Task GetPagedLabels_ShouldReturnPagedResult_WhenDataExists()
        {
            var dto = new LabelFilterRequest
            {
                Keyword = "food",
                PaginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 }
            };

            var list = new List<Label> { CreateLabel(), CreateLabel() };

            LabelRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    dto.PaginationParams.PageNumber,
                    dto.PaginationParams.PageSize,
                    It.IsAny<Expression<Func<Label, bool>>>(),
                    It.IsAny<Func<IQueryable<Label>, IOrderedQueryable<Label>>>(),
                    It.IsAny<string?>(),
                    It.IsAny<string[]?>(),
                    It.IsAny<Func<IQueryable<Label>, IQueryable<Label>>?>()
                ))
                .ReturnsAsync((list, list.Count));

            var mapped = new List<LabelResponse>
            {
                new LabelResponse{ Id = list[0].Id, Name = list[0].Name },
                new LabelResponse{ Id = list[1].Id, Name = list[1].Name }
            };

            MapperMock
                .Setup(m => m.Map<List<LabelResponse>>(list))
                .Returns(mapped);

            var result = await Sut.GetPagedLabelsAsync(dto);

            Assert.Equal(2, result.Items.Count());
            Assert.Equal(2, result.TotalCount);

            LabelRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }
    }
}
