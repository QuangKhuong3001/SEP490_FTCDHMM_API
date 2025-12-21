using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.LabelDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.LabelServiceTests
{
    public class GetLabelsAsyncTests : LabelServiceTestBase
    {
        [Fact]
        public async Task GetLabels_ShouldReturnFromCache_WhenCacheExists()
        {
            var request = new LabelSearchDropboxRequest { Keyword = "food" };

            var cached = new List<LabelResponse>
            {
                new LabelResponse { Id = Guid.NewGuid(), Name = "Food" }
            };

            CacheServiceMock
                .Setup(c => c.GetAsync<IEnumerable<LabelResponse>>(It.IsAny<string>()))
                .ReturnsAsync(cached);

            var result = await Sut.GetLabelsAsync(request);

            Assert.Single(result);

            LabelRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<Expression<Func<Label, bool>>>(), null), Times.Never);
            CacheServiceMock.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<LabelResponse>>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        [Fact]
        public async Task GetLabels_ShouldReturnEmpty_WhenNoData()
        {
            var request = new LabelSearchDropboxRequest { Keyword = "food" };

            CacheServiceMock
                .Setup(c => c.GetAsync<IEnumerable<LabelResponse>>(It.IsAny<string>()))
                .ReturnsAsync((IEnumerable<LabelResponse>?)null);

            LabelRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Label, bool>>>(),
                    null))
                .ReturnsAsync(new List<Label>());

            MapperMock
                .Setup(m => m.Map<IEnumerable<LabelResponse>>(It.IsAny<object>()))
                .Returns(new List<LabelResponse>());

            CacheServiceMock
                .Setup(c => c.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<LabelResponse>>(),
                    It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            var result = await Sut.GetLabelsAsync(request);

            Assert.Empty(result);

            LabelRepositoryMock.Verify(
                r => r.GetAllAsync(It.IsAny<Expression<Func<Label, bool>>>(), null),
                Times.Once);

            MapperMock.Verify(
                m => m.Map<IEnumerable<LabelResponse>>(It.IsAny<object>()),
                Times.Once);

            CacheServiceMock.Verify(
                c => c.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<LabelResponse>>(),
                    It.IsAny<TimeSpan>()),
                Times.Once);
        }


        [Fact]
        public async Task GetLabels_ShouldReturnMappedResult_WhenDataExists()
        {
            var request = new LabelSearchDropboxRequest { Keyword = "food" };

            var labels = new List<Label>
            {
                new Label { Id = Guid.NewGuid(), Name = "B", NormalizedName = "b" },
                new Label { Id = Guid.NewGuid(), Name = "A", NormalizedName = "a" }
            };

            CacheServiceMock
                .Setup(c => c.GetAsync<IEnumerable<LabelResponse>>(It.IsAny<string>()))
                .ReturnsAsync((IEnumerable<LabelResponse>?)null);

            LabelRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Label, bool>>>(), null))
                .ReturnsAsync(labels);

            MapperMock
                .Setup(m => m.Map<IEnumerable<LabelResponse>>(It.IsAny<IEnumerable<Label>>()))
                .Returns((IEnumerable<Label> src) =>
                    src.Select(l => new LabelResponse { Id = l.Id, Name = l.Name }).ToList()
                );

            CacheServiceMock
                .Setup(c => c.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<LabelResponse>>(),
                    It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            var result = (await Sut.GetLabelsAsync(request)).ToList();

            Assert.Equal(2, result.Count);

            LabelRepositoryMock.Verify(
                r => r.GetAllAsync(It.IsAny<Expression<Func<Label, bool>>>(), null),
                Times.Once);

            MapperMock.Verify(
                m => m.Map<IEnumerable<LabelResponse>>(It.IsAny<IEnumerable<Label>>()),
                Times.Once);

            CacheServiceMock.Verify(
                c => c.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<LabelResponse>>(),
                    It.IsAny<TimeSpan>()),
                Times.Once);
        }

    }
}
