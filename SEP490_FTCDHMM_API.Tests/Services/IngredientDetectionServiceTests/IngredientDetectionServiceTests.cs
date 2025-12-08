using Microsoft.AspNetCore.Http;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos.IngredientDetection;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Implementations;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services
{
    public class IngredientDetectionServiceTests
    {
        private readonly Mock<IGeminiIngredientDetectionService> GeminiMock;
        private readonly Mock<IIngredientRepository> IngredientRepoMock;
        private readonly IngredientDetectionService Sut;

        public IngredientDetectionServiceTests()
        {
            GeminiMock = new Mock<IGeminiIngredientDetectionService>(MockBehavior.Strict);
            IngredientRepoMock = new Mock<IIngredientRepository>(MockBehavior.Strict);

            Sut = new IngredientDetectionService(
                GeminiMock.Object,
                IngredientRepoMock.Object
            );
        }

        private IFormFile CreateFakeFile(string fileName = "test.png")
        {
            var bytes = new byte[] { 1, 2, 3 };
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            };
        }

        private Image CreateFakeImage()
        {
            return new Image
            {
                Id = Guid.NewGuid(),
                Key = "img/apple.png",
                ContentType = "image/png",
                CreatedAtUTC = DateTime.UtcNow
            };
        }

        [Fact]
        public async Task DetectIngredients_ShouldReturnEmpty_WhenGeminiReturnsEmpty()
        {
            var req = new IngredientDetectionUploadRequest
            {
                Image = CreateFakeFile()
            };

            GeminiMock
                .Setup(g => g.DetectIngredientsAsync(req.Image))
                .ReturnsAsync(new List<IngredientDetectionResult>());

            var result = await Sut.DetectIngredientsAsync(req);

            Assert.Empty(result);

            GeminiMock.VerifyAll();
        }

        [Fact]
        public async Task DetectIngredients_ShouldReturnEmpty_WhenNoDetectedIngredientExistsInDb()
        {
            var req = new IngredientDetectionUploadRequest
            {
                Image = CreateFakeFile()
            };

            var geminiResults = new List<IngredientDetectionResult>
            {
                new() { Ingredient = "Apple", Confidence = 0.9 },
                new() { Ingredient = "Banana", Confidence = 0.8 }
            };

            GeminiMock
                .Setup(g => g.DetectIngredientsAsync(req.Image))
                .ReturnsAsync(geminiResults);

            IngredientRepoMock
                .Setup(r => r.GetAllAsync(null, null))
                .ReturnsAsync(new List<Ingredient>());

            var result = await Sut.DetectIngredientsAsync(req);

            Assert.Empty(result);

            GeminiMock.VerifyAll();
            IngredientRepoMock.VerifyAll();
        }

        [Fact]
        public async Task DetectIngredients_ShouldReturnMatches_SortedByConfidenceDesc()
        {
            var req = new IngredientDetectionUploadRequest
            {
                Image = CreateFakeFile()
            };

            var geminiResults = new List<IngredientDetectionResult>
            {
                new() { Ingredient = "Carrot", Confidence = 0.5 },
                new() { Ingredient = "Potato", Confidence = 0.9 },
                new() { Ingredient = "Onion", Confidence = 0.7 }
            };

            var dbIngredients = new List<Ingredient>
            {
                new() { Id = Guid.NewGuid(), Name = "Potato", Image = CreateFakeImage()  },
                new() { Id = Guid.NewGuid(), Name = "Onion", Image = CreateFakeImage() }
            };

            GeminiMock
                .Setup(g => g.DetectIngredientsAsync(req.Image))
                .ReturnsAsync(geminiResults);

            IngredientRepoMock
                .Setup(r => r.GetAllAsync(null, null))
                .ReturnsAsync(dbIngredients);

            var result = (await Sut.DetectIngredientsAsync(req)).ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal("Potato", result[0].Ingredient);
            Assert.Equal("Onion", result[1].Ingredient);
            Assert.True(result[0].Confidence > result[1].Confidence);

            GeminiMock.VerifyAll();
            IngredientRepoMock.VerifyAll();
        }

        [Fact]
        public async Task DetectIngredients_ShouldCallGeminiAndRepoOnce()
        {
            var req = new IngredientDetectionUploadRequest
            {
                Image = CreateFakeFile()
            };

            var geminiResults = new List<IngredientDetectionResult>
            {
                new() { Ingredient = "Apple", Confidence = 0.9 }
            };

            var dbIngredients = new List<Ingredient>
            {
                new() { Id = Guid.NewGuid(), Name = "Apple", Image = CreateFakeImage()  }
            };

            GeminiMock
                .Setup(g => g.DetectIngredientsAsync(req.Image))
                .ReturnsAsync(geminiResults);

            IngredientRepoMock
                .Setup(r => r.GetAllAsync(null, null))
                .ReturnsAsync(dbIngredients);

            var result = await Sut.DetectIngredientsAsync(req);

            Assert.Single(result);

            GeminiMock.Verify(g => g.DetectIngredientsAsync(req.Image), Times.Once);
            IngredientRepoMock.Verify(r => r.GetAllAsync(null, null), Times.Once);
        }
    }
}
