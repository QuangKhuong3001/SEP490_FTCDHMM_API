using Microsoft.AspNetCore.Http;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Domain.Constants;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.UserServiceTests
{
    public class UpdateProfileAsyncTests : UserServiceTestBase
    {
        private UpdateProfileRequest CreateValidDto()
        {
            return new UpdateProfileRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                DateOfBirth = DateTime.UtcNow.AddYears(-20),
                Address = "123 Street",
                Bio = "Hello Bio",
                Avatar = null
            };
        }

        [Fact]
        public async Task UpdateProfile_ShouldThrow_WhenAgeInvalid()
        {
            var dto = CreateValidDto();
            dto.DateOfBirth = DateTime.UtcNow.AddYears(-(AuthConstants.MIN_REGISTER_AGE - 1)); // too young

            var userId = Guid.NewGuid();

            await Assert.ThrowsAsync<AppException>(() => Sut.UpdateProfileAsync(userId, dto));
        }

        [Fact]
        public async Task UpdateProfile_ShouldUpdateFields_WhenAvatarIsNull()
        {
            var userId = Guid.NewGuid();
            var user = CreateUser(userId);

            var dto = CreateValidDto();
            dto.Avatar = null;

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    userId,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            await Sut.UpdateProfileAsync(userId, dto);

            Assert.Equal(dto.FirstName, user.FirstName);
            Assert.Equal(dto.LastName, user.LastName);
            Assert.Equal(dto.Address, user.Address);
            Assert.Equal(dto.Bio, user.Bio);
            Assert.Equal(dto.DateOfBirth, user.DateOfBirth);
            Assert.Equal(Gender.From(dto.Gender), user.Gender);

            UserRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateProfile_ShouldUploadAvatar_WhenNoPreviousAvatar()
        {
            var userId = Guid.NewGuid();
            var user = CreateUser(userId);
            user.AvatarId = null;

            var dto = CreateValidDto();
            dto.Avatar = Mock.Of<IFormFile>(f => f.Length > 0);

            var uploadedImg = new Image { Id = Guid.NewGuid(), Key = "img1.png" };

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    userId,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            S3ImageServiceMock
                .Setup(s => s.UploadImageAsync(dto.Avatar, StorageFolder.AVATARS, user))
                .ReturnsAsync(uploadedImg);

            await Sut.UpdateProfileAsync(userId, dto);

            Assert.Equal(uploadedImg.Id, user.AvatarId);

            S3ImageServiceMock.Verify(s => s.UploadImageAsync(dto.Avatar, StorageFolder.AVATARS, user), Times.Once);
            S3ImageServiceMock.Verify(s => s.DeleteImageAsync(It.IsAny<Guid>()), Times.Never);

            UserRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateProfile_ShouldReplaceAvatar_WhenPreviousAvatarExists()
        {
            var userId = Guid.NewGuid();
            var oldAvatarId = Guid.NewGuid();

            var user = CreateUser(userId);
            user.AvatarId = oldAvatarId;

            var dto = CreateValidDto();
            dto.Avatar = Mock.Of<IFormFile>(f => f.Length > 0);

            var uploadedImg = new Image { Id = Guid.NewGuid(), Key = "new.png" };

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    userId,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            S3ImageServiceMock
                 .Setup(s => s.UploadImageAsync(dto.Avatar, StorageFolder.AVATARS, user))
                 .ReturnsAsync(uploadedImg);

            await Sut.UpdateProfileAsync(userId, dto);

            Assert.Equal(uploadedImg.Id, user.AvatarId);

            S3ImageServiceMock.Verify(s => s.DeleteImageAsync(oldAvatarId), Times.Once);
            S3ImageServiceMock.Verify(s => s.UploadImageAsync(dto.Avatar, StorageFolder.AVATARS, user), Times.Once);

            UserRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }
    }
}
