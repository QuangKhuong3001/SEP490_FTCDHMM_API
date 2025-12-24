using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
namespace SEP490_FTCDHMM_API.Tests.Services.LabelServiceTests.UserServiceTests
{

    public class GetUserListAsyncTests : UserServiceTestBase
    {
        private UserFilterRequest CreateRequest(string? keyword = null, Guid? roleId = null)
        {
            return new UserFilterRequest
            {
                Keyword = keyword,
                RoleId = roleId,
                PaginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 }
            };
        }

        [Fact]
        public async Task ShouldReturnAllUsers_WhenNoFilter()
        {
            var users = new List<AppUser>
            {
                CreateUser(Guid.NewGuid()),
                CreateUser(Guid.NewGuid()),
            };

            UserRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<AppUser, bool>>>(),
                    It.IsAny<Func<IQueryable<AppUser>, IOrderedQueryable<AppUser>>>(),
                    null,
                    null,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync((users, users.Count));

            MapperMock
                .Setup(m => m.Map<List<UserResponse>>(It.IsAny<List<AppUser>>()))
                .Returns(new List<UserResponse>
                    {
                    new UserResponse(), new UserResponse()
                    });

            var request = CreateRequest();

            var result = await Sut.GetUserListAsync(request);

            Assert.Equal(2, result.Items.Count());
            Assert.Equal(2, result.TotalCount);
        }

        [Fact]
        public async Task ShouldFilterByKeyword()
        {
            var users = new List<AppUser>
            {
                CreateUser(Guid.NewGuid()),
            };

            UserRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<AppUser, bool>>>(),
                    It.IsAny<Func<IQueryable<AppUser>, IOrderedQueryable<AppUser>>>(),
                    null,
                    null,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync((users, 1));

            MapperMock
                .Setup(m => m.Map<List<UserResponse>>(users))
                .Returns(new List<UserResponse> { new UserResponse() });

            var request = CreateRequest(keyword: "Tra");

            var result = await Sut.GetUserListAsync(request);

            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);
        }

        [Fact]
        public async Task ShouldFilterByRoleId()
        {
            var roleId = Guid.NewGuid();

            var users = new List<AppUser>
            {
                new AppUser { Id = Guid.NewGuid(), RoleId = roleId, FirstName = "John", LastName = "Doe", Email = "a@a.com" }
            };

            UserRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<AppUser, bool>>>(),
                    It.IsAny<Func<IQueryable<AppUser>, IOrderedQueryable<AppUser>>>(),
                    null,
                    null,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync((users, 1));

            MapperMock
                .Setup(m => m.Map<List<UserResponse>>(users))
                .Returns(new List<UserResponse> { new UserResponse() });

            var request = CreateRequest(roleId: roleId);

            var result = await Sut.GetUserListAsync(request);

            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);
        }


        [Fact]
        public async Task ShouldFilterByKeywordAndRoleId()
        {
            var roleId = Guid.NewGuid();

            var users = new List<AppUser>
            {
                new AppUser
                {
                    Id = Guid.NewGuid(),
                    RoleId = roleId,
                    FirstName = "Nguyen",
                    LastName = "Tra",
                    Email = "user@gmail.com"
                }
            };

            UserRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<AppUser, bool>>>(),
                    It.IsAny<Func<IQueryable<AppUser>, IOrderedQueryable<AppUser>>>(),
                    null,
                    null,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync((users, 1));

            MapperMock
                .Setup(m => m.Map<List<UserResponse>>(users))
                .Returns(new List<UserResponse> { new UserResponse() });

            var request = CreateRequest(keyword: "Tra", roleId: roleId);

            var result = await Sut.GetUserListAsync(request);

            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);
        }

        [Fact]
        public async Task ShouldMapUsersCorrectly()
        {
            var users = new List<AppUser> { CreateUser() };

            UserRepositoryMock
                .Setup(r => r.GetPagedAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<AppUser, bool>>>(),
                    It.IsAny<Func<IQueryable<AppUser>, IOrderedQueryable<AppUser>>>(),
                    null,
                    null,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync((users, users.Count));

            MapperMock
                .Setup(m => m.Map<List<UserResponse>>(users))
                .Returns(new List<UserResponse> { new UserResponse() });

            var request = CreateRequest();

            var result = await Sut.GetUserListAsync(request);

            MapperMock.Verify(m => m.Map<List<UserResponse>>(users), Times.Once);
        }
    }
}