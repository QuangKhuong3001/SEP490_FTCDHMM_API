using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.RoleServiceTests
{
    public class GetRolesAsyncTests : RoleServiceTestBase
    {
        [Fact]
        public async Task GetRolesAsync_ShouldReturnPagedResult()
        {
            var pagination = new PaginationParams { PageNumber = 1, PageSize = 10 };

            var roles = new List<AppRole>
            {
                new AppRole { Id = Guid.NewGuid(), Name = "Manager" }
            };

            RoleRepoMock
                .Setup(r => r.GetPagedAsync(1, 10, null, null, null, null, null))
                .ReturnsAsync((roles, 1));

            var result = await Sut.GetRolesAsync(pagination);

            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);
        }
    }
}
