using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.GoogleAuthDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.AuthServiceTests
{
    public class GoogleLoginWithCodeAsyncTests : AuthServiceTestBase
    {
        private GoogleCodeLoginRequest CreateDto()
        {
            return new GoogleCodeLoginRequest
            {
                Code = "sample-code",
                CodeVerifier = "sample-verifier"
            };
        }

        [Fact]
        public async Task GoogleLoginWithCode_ShouldThrow_WhenExchangeTokenReturnsNull()
        {
            var dto = CreateDto();

            GoogleAuthServiceMock
                .Setup(x => x.ExchangeCodeForTokenAsync(It.IsAny<GoogleCodeLoginRequest>()))
                .ReturnsAsync((GoogleTokenResponse)null!);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.GoogleLoginWithCodeAsync(dto));

            Assert.Equal(AppResponseCode.SERVICE_NOT_AVAILABLE, ex.ResponseCode);
        }

        [Fact]
        public async Task GoogleLoginWithCode_ShouldCall_ValidateIdToken()
        {
            var dto = CreateDto();

            var tokens = new GoogleTokenResponse
            {
                IdToken = "id-token",
                AccessToken = "access-token",
                RefreshToken = "refresh-token"
            };

            GoogleAuthServiceMock
                .Setup(x => x.ExchangeCodeForTokenAsync(It.IsAny<GoogleCodeLoginRequest>()))
                .ReturnsAsync(tokens);

            GoogleAuthServiceMock
                .Setup(x => x.ValidateIdTokenAsync(tokens.IdToken))
                .ReturnsAsync(new GoogleTokenPayload());

            GoogleAuthServiceMock
                .Setup(x => x.FetchUserInfoWithPeopleApiAsync(tokens.AccessToken))
                .ReturnsAsync(new GoogleUserInfo());

            GoogleProvisioningServiceMock
                .Setup(x => x.FindOrProvisionFromGoogleAsync(It.IsAny<GoogleProvisionRequest>()))
                .ReturnsAsync(CreateUser());

            JwtServiceMock
                .Setup(x => x.GenerateToken(It.IsAny<AppUser>(), It.IsAny<AppRole>()))
                .Returns("jwt-token");

            var result = await Sut.GoogleLoginWithCodeAsync(dto);

            Assert.Equal("jwt-token", result);

            GoogleAuthServiceMock.Verify(
                x => x.ValidateIdTokenAsync(tokens.IdToken), Times.Once);
        }

        [Fact]
        public async Task GoogleLoginWithCode_ShouldProvisionUser()
        {
            var dto = CreateDto();

            var tokens = new GoogleTokenResponse
            {
                IdToken = "id",
                AccessToken = "access",
                RefreshToken = "refresh"
            };

            var payload = new GoogleTokenPayload();
            var userInfo = new GoogleUserInfo();
            var user = CreateUser();

            GoogleAuthServiceMock.Setup(x => x.ExchangeCodeForTokenAsync(It.IsAny<GoogleCodeLoginRequest>()))
                .ReturnsAsync(tokens);

            GoogleAuthServiceMock.Setup(x => x.ValidateIdTokenAsync(tokens.IdToken))
                .ReturnsAsync(payload);

            GoogleAuthServiceMock.Setup(x => x.FetchUserInfoWithPeopleApiAsync(tokens.AccessToken))
                .ReturnsAsync(userInfo);

            GoogleProvisioningServiceMock
                .Setup(x => x.FindOrProvisionFromGoogleAsync(It.IsAny<GoogleProvisionRequest>()))
                .ReturnsAsync(user);

            JwtServiceMock.Setup(x => x.GenerateToken(user, user.Role))
                .Returns("token");

            var result = await Sut.GoogleLoginWithCodeAsync(dto);

            Assert.Equal("token", result);

            GoogleProvisioningServiceMock.Verify(
                x => x.FindOrProvisionFromGoogleAsync(It.IsAny<GoogleProvisionRequest>()),
                Times.Once);
        }

        [Fact]
        public async Task GoogleLoginWithCode_ShouldReturnToken_WhenSuccess()
        {
            var dto = CreateDto();

            var tokens = new GoogleTokenResponse
            {
                IdToken = "valid",
                AccessToken = "access",
                RefreshToken = "refresh"
            };

            var payload = new GoogleTokenPayload();
            var info = new GoogleUserInfo();
            var user = CreateUser();

            GoogleAuthServiceMock.Setup(x => x.ExchangeCodeForTokenAsync(It.IsAny<GoogleCodeLoginRequest>()))
                .ReturnsAsync(tokens);

            GoogleAuthServiceMock.Setup(x => x.ValidateIdTokenAsync(tokens.IdToken))
                .ReturnsAsync(payload);

            GoogleAuthServiceMock.Setup(x => x.FetchUserInfoWithPeopleApiAsync(tokens.AccessToken))
                .ReturnsAsync(info);

            GoogleProvisioningServiceMock.Setup(x =>
                x.FindOrProvisionFromGoogleAsync(It.IsAny<GoogleProvisionRequest>()))
                .ReturnsAsync(user);

            JwtServiceMock.Setup(x => x.GenerateToken(user, user.Role))
                .Returns("jwt-123");

            var result = await Sut.GoogleLoginWithCodeAsync(dto);

            Assert.Equal("jwt-123", result);
        }
    }
}
