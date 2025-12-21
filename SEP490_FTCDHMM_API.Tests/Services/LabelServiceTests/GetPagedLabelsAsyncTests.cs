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
            var labels = new List<Label>
            {
                CreateLabel(),
                CreateLabel()
            };

            CacheServiceMock
                .Setup(c => c.GetAsync<PagedResult<LabelResponse>>(
                    It.IsAny<string>()))
                .ReturnsAsync((PagedResult<LabelResponse>?)null);

            LabelRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    1,
                    10,
                    It.IsAny<Expression<Func<Label, bool>>>(),
                    It.IsAny<Func<IQueryable<Label>, IOrderedQueryable<Label>>>(),
                    null,
                    null,
                    null))
                .ReturnsAsync((labels, labels.Count));

            MapperMock
                .Setup(m => m.Map<List<LabelResponse>>(labels))
                .Returns(labels.Select(l => new LabelResponse
                {
                    Id = l.Id,
                    Name = l.Name
                }).ToList());

            CacheServiceMock
                .Setup(c => c.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<PagedResult<LabelResponse>>(),
                    It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            var request = new LabelFilterRequest
            {
                Keyword = "FOOD",
                PaginationParams = new PaginationParams
                {
                    PageNumber = 1,
                    PageSize = 10
                }
            };

            var result = await Sut.GetPagedLabelsAsync(request);

            Assert.Equal(2, result.Items.Count());
            Assert.Equal(2, result.TotalCount);
        }

    }
}
