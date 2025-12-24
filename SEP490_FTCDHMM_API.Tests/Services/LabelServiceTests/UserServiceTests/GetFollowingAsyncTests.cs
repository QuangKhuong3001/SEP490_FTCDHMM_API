using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.LabelServiceTests.UserServiceTests
{
    public class GetFollowingAsyncTests : UserServiceTestBase
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
                Gender = Gender.Female
            };
        }

        [Fact]
        public async Task ShouldReturnSortedFollowing_WhenTheyExist()
        {
            var userId = Guid.NewGuid();

            var f3 = CreateUser(Guid.NewGuid(), "Zara", "Alpha", "zara@a.com");
            var f1 = CreateUser(Guid.NewGuid(), "Adam", "Beta", "beta@a.com");
            var f2 = CreateUser(Guid.NewGuid(), "Adam", "Zen", "zen@a.com");

            var relations = new List<UserFollow>
            {
                new UserFollow { Followee = f3, FolloweeId = f3.Id, FollowerId = userId },
                new UserFollow { Followee = f1, FolloweeId = f1.Id, FollowerId = userId },
                new UserFollow { Followee = f2, FolloweeId = f2.Id, FollowerId = userId }
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

            var result = await Sut.GetFollowingAsync(userId);
            var list = result.ToList();

            Assert.Equal(3, list.Count);
            Assert.Equal(f1.Id, list[0].Id);
            Assert.Equal(f2.Id, list[1].Id);
            Assert.Equal(f3.Id, list[2].Id);
        }

        [Fact]
        public async Task ShouldReturnEmpty_WhenNoFollowing()
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

            var result = await Sut.GetFollowingAsync(userId);

            Assert.Empty(result);
        }
    }
}
