using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.LabelServiceTests.UserServiceTests
{
    public class GetFollowersAsyncTests : UserServiceTestBase
    {
        private AppUser CreateUser(Guid id, string firstName, string lastName, string email)
        {
            return new AppUser
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = $"{firstName.ToLower()}.{lastName.ToLower()}",
                Gender = Gender.Male
            };
        }

        [Fact]
        public async Task ShouldReturnSortedFollowers_WhenTheyExist()
        {
            var userId = Guid.NewGuid();

            var followerC = CreateUser(Guid.NewGuid(), "John", "Smith", "john@a.com");
            var followerA = CreateUser(Guid.NewGuid(), "Adam", "Brown", "adam@a.com");
            var followerA2 = CreateUser(Guid.NewGuid(), "Adam", "Adams", "adams@a.com");

            var relations = new List<UserFollow>
            {
                new UserFollow { Follower = followerC, FollowerId = followerC.Id, FolloweeId = userId },
                new UserFollow { Follower = followerA, FollowerId = followerA.Id, FolloweeId = userId },
                new UserFollow { Follower = followerA2, FollowerId = followerA2.Id, FolloweeId = userId }
            };

            UserFollowRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<UserFollow, bool>>>(),
                    It.IsAny<Func<IQueryable<UserFollow>, IQueryable<UserFollow>>>()))
                .ReturnsAsync(relations);

            MapperMock
                .Setup(m => m.Map<IEnumerable<UserInteractionResponse>>(It.IsAny<object>()))
                .Returns((IEnumerable<AppUser> users) =>
                    users.Select(u => new UserInteractionResponse
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email!,
                        UserName = u.UserName!,
                        AvatarUrl = u.Avatar?.Key ?? null
                    }));

            var result = await Sut.GetFollowersAsync(userId);
            var list = result.ToList();

            Assert.Equal(3, list.Count);
            Assert.Equal(followerA2.Id, list[0].Id);
            Assert.Equal(followerA.Id, list[1].Id);
            Assert.Equal(followerC.Id, list[2].Id);
        }

        [Fact]
        public async Task ShouldReturnEmpty_WhenNoFollowers()
        {
            var userId = Guid.NewGuid();

            UserFollowRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<UserFollow, bool>>>(),
                    It.IsAny<Func<IQueryable<UserFollow>, IQueryable<UserFollow>>>()))
                .ReturnsAsync(new List<UserFollow>());

            MapperMock
                .Setup(m => m.Map<IEnumerable<UserInteractionResponse>>(It.IsAny<object>()))
                .Returns(new List<UserInteractionResponse>());

            var result = await Sut.GetFollowersAsync(userId);

            Assert.Empty(result);
        }
    }
}
