using Microsoft.AspNetCore.Http;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.UserServiceTests
{
    public class UpdateProfileAsyncTests : UserServiceTestBase
    {
        private static IFormFile CreateAvatarFile()
        {
            var bytes = new byte[] { 1, 2, 3 };
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, "avatar", "avatar.jpg");
        }

        private static UpdateProfileRequest CreateValidDtoWithAvatar()
        {
            return new UpdateProfileRequest
            {
                FirstName = "New",
                LastName = "Name",
                Gender = "FEMALE",
                DateOfBirth = DateTime.UtcNow.AddYears(-20),
                Bio = "Bio",
                Address = "Address",
                Avatar = CreateAvatarFile()
            };
        }

        private static UpdateProfileRequest CreateValidDtoWithoutAvatar()
        {
            return new UpdateProfileRequest
            {
                FirstName = "New",
                LastName = "Name",
                Gender = "FEMALE",
                DateOfBirth = DateTime.UtcNow.AddYears(-20),
                Bio = "Bio",
                Address = "Address",
                Avatar = null
            };
        }

        [Fact]
        public async Task UpdateProfile_ShouldUploadAvatar_WhenNoPreviousAvatar()
        {
            var userId = Guid.NewGuid();
            var user = CreateUser(userId);
            user.AvatarId = null;

            var dto = CreateValidDtoWithAvatar();

            var uploadedImg = new Image
            {
                Id = Guid.NewGuid(),
                Key = "avatars/new-avatar.jpg"
            };

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>?>()))
                .ReturnsAsync(user);

            S3ImageServiceMock
                .Setup(s => s.UploadImageAsync(dto.Avatar!, StorageFolder.AVATARS))
                .ReturnsAsync(uploadedImg);

            UserRepositoryMock
                .Setup(r => r.UpdateAsync(user))
                .Returns(Task.CompletedTask);

            await Sut.UpdateProfileAsync(userId, dto);

            Assert.Equal(uploadedImg.Id, user.AvatarId);
            S3ImageServiceMock.Verify(s => s.UploadImageAsync(dto.Avatar!, StorageFolder.AVATARS), Times.Once);
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

            var dto = CreateValidDtoWithAvatar();

            var uploadedImg = new Image
            {
                Id = Guid.NewGuid(),
                Key = "avatars/replaced-avatar.jpg"
            };

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>?>()))
                .ReturnsAsync(user);

            S3ImageServiceMock
                .Setup(s => s.UploadImageAsync(dto.Avatar!, StorageFolder.AVATARS))
                .ReturnsAsync(uploadedImg);

            S3ImageServiceMock
                .Setup(s => s.DeleteImageAsync(oldAvatarId))
                .Returns(Task.CompletedTask);

            UserRepositoryMock
                .Setup(r => r.UpdateAsync(user))
                .Returns(Task.CompletedTask);

            await Sut.UpdateProfileAsync(userId, dto);

            Assert.Equal(uploadedImg.Id, user.AvatarId);
            S3ImageServiceMock.Verify(s => s.DeleteImageAsync(oldAvatarId), Times.Once);
            S3ImageServiceMock.Verify(s => s.UploadImageAsync(dto.Avatar!, StorageFolder.AVATARS), Times.Once);
            UserRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateProfile_ShouldUpdateFields_WhenAvatarIsNull()
        {
            var userId = Guid.NewGuid();
            var user = CreateUser(userId);

            var dto = CreateValidDtoWithoutAvatar();

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>?>()))
                .ReturnsAsync(user);

            UserRepositoryMock
                .Setup(r => r.UpdateAsync(user))
                .Returns(Task.CompletedTask);

            await Sut.UpdateProfileAsync(userId, dto);

            Assert.Equal(dto.FirstName, user.FirstName);
            Assert.Equal(dto.LastName, user.LastName);
            Assert.Equal(Gender.From(dto.Gender), user.Gender);
            Assert.Equal(dto.DateOfBirth, user.DateOfBirth);
            Assert.Equal(dto.Bio, user.Bio);
            Assert.Equal(dto.Address, user.Address);
            S3ImageServiceMock.Verify(s => s.UploadImageAsync(It.IsAny<IFormFile>(), It.IsAny<StorageFolder>()), Times.Never);
            S3ImageServiceMock.Verify(s => s.DeleteImageAsync(It.IsAny<Guid>()), Times.Never);
            UserRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateProfile_ShouldThrow_WhenAgeInvalid()
        {
            var userId = Guid.NewGuid();

            var dto = new UpdateProfileRequest
            {
                FirstName = "A",
                LastName = "B",
                Gender = "MALE",
                DateOfBirth = DateTime.UtcNow
            };

            await Assert.ThrowsAsync<AppException>(() => Sut.UpdateProfileAsync(userId, dto));
        }
    }
}
