using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.GoogleAuthDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.AuthServiceTests
{
    public class GoogleLoginWithIdTokenAsyncTests : AuthServiceTestBase
    {
        [Fact]
        public async Task GoogleLogin_ShouldReturnJwt_WhenSuccess()
        {
            var req = new GoogleIdTokenLoginRequest
            {
                IdToken = "valid-google-id-token"
            };

            var payload = new GoogleTokenPayload
            {
                Email = "user@example.com",
                EmailVerified = true,
                Name = "Test User",
                Subject = "12345"
            };

            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                Email = payload.Email,
                FirstName = "Test",
                LastName = "User",
                Role = new AppRole { Name = "Customer" }
            };

            GoogleAuthServiceMock
                .Setup(x => x.ValidateIdTokenAsync(req.IdToken))
                .ReturnsAsync(payload);

            GoogleProvisioningServiceMock
                .Setup(x => x.FindOrProvisionFromGoogleAsync(
                    It.Is<GoogleProvisionRequest>(r => r.Payload.Email == payload.Email)))
                .ReturnsAsync(user);

            JwtServiceMock
                .Setup(x => x.GenerateToken(user, user.Role))
                .Returns("jwt-token");

            var result = await Sut.GoogleLoginWithIdTokenAsync(req);

            Assert.Equal("jwt-token", result);

            GoogleAuthServiceMock.Verify(x => x.ValidateIdTokenAsync(req.IdToken), Times.Once);
            GoogleProvisioningServiceMock.Verify(x => x.FindOrProvisionFromGoogleAsync(It.IsAny<GoogleProvisionRequest>()), Times.Once);
            JwtServiceMock.Verify(x => x.GenerateToken(user, user.Role), Times.Once);
        }


        [Fact]
        public async Task GoogleLogin_ShouldThrow_WhenIdTokenInvalid()
        {
            var req = new GoogleIdTokenLoginRequest { IdToken = "invalid-token" };

            GoogleAuthServiceMock
                .Setup(x => x.ValidateIdTokenAsync(req.IdToken))
                .ThrowsAsync(new Exception("invalid token"));

            await Assert.ThrowsAsync<Exception>(() => Sut.GoogleLoginWithIdTokenAsync(req));

            GoogleAuthServiceMock.Verify(x => x.ValidateIdTokenAsync(req.IdToken), Times.Once);
        }


        [Fact]
        public async Task GoogleLogin_ShouldThrow_WhenProvisionFails()
        {
            var req = new GoogleIdTokenLoginRequest { IdToken = "valid-token" };

            var payload = new GoogleTokenPayload
            {
                Email = "user@example.com",
                EmailVerified = true,
                Subject = "abc"
            };

            GoogleAuthServiceMock
                .Setup(x => x.ValidateIdTokenAsync(req.IdToken))
                .ReturnsAsync(payload);

            GoogleProvisioningServiceMock
                .Setup(x => x.FindOrProvisionFromGoogleAsync(It.IsAny<GoogleProvisionRequest>()))
                .ThrowsAsync(new Exception("provision failed"));

            await Assert.ThrowsAsync<Exception>(() => Sut.GoogleLoginWithIdTokenAsync(req));

            GoogleAuthServiceMock.Verify(x => x.ValidateIdTokenAsync(req.IdToken), Times.Once);
            GoogleProvisioningServiceMock.Verify(x => x.FindOrProvisionFromGoogleAsync(It.IsAny<GoogleProvisionRequest>()), Times.Once);
        }
    }
}
