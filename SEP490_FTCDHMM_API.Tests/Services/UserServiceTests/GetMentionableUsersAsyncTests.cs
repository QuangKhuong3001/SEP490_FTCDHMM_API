using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos.Mention;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.UserServiceTests
{
    public class GetMentionableUsersAsyncTests : UserServiceTestBase
    {
        [Fact]
        public async Task GetMentionableUsers_ShouldReturnMappedUsers()
        {
            var userId = Guid.NewGuid();
            string keyword = "tra";

            var users = new List<AppUser>
            {
                new AppUser { Id = Guid.NewGuid(), FirstName = "Nguyen", LastName = "Tra" },
                new AppUser { Id = Guid.NewGuid(), FirstName = "Le", LastName = "Tra" }
            };

            UserRepositoryMock
                .Setup(r => r.GetTaggableUsersAsync(userId, keyword))
                .ReturnsAsync(users);

            var mapped = new List<MentionUserResponse>
            {
                new MentionUserResponse { Id = users[0].Id, FirstName = "Nguyen", LastName = "Tra" },
                new MentionUserResponse { Id = users[1].Id, FirstName = "Le", LastName = "Tra" }
            };

            MapperMock
                .Setup(m => m.Map<IEnumerable<MentionUserResponse>>(users))
                .Returns(mapped);

            var result = await Sut.GetMentionableUsersAsync(userId, keyword);

            Assert.Equal(2, result.Count());
            Assert.Equal("Nguyen", result.First().FirstName);

            UserRepositoryMock.Verify(r => r.GetTaggableUsersAsync(userId, keyword), Times.Once);
        }

        [Fact]
        public async Task GetMentionableUsers_ShouldReturnEmptyList_WhenNoUsersFound()
        {
            var userId = Guid.NewGuid();
            string? keyword = null;

            var empty = new List<AppUser>();

            UserRepositoryMock
                .Setup(r => r.GetTaggableUsersAsync(userId, keyword))
                .ReturnsAsync(empty);

            MapperMock
                .Setup(m => m.Map<IEnumerable<MentionUserResponse>>(empty))
                .Returns(new List<MentionUserResponse>());

            var result = await Sut.GetMentionableUsersAsync(userId, keyword);

            Assert.Empty(result);

            UserRepositoryMock.Verify(r => r.GetTaggableUsersAsync(userId, keyword), Times.Once);
        }
    }
}
