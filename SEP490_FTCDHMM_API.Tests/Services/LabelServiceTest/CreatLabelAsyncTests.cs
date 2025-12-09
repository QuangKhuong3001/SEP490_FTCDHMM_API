using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.LabelDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.LabelServiceTests
{
    public class CreatLabelAsyncTests : LabelServiceTestBase
    {
        [Fact]
        public async Task CreatLabel_ShouldThrowExists_WhenLabelAlreadyExists()
        {
            var dto = new CreateLabelRequest { Name = "Food", ColorCode = "#000" };

            LabelRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Label, bool>>>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AppException>(() => Sut.CreatLabelAsync(dto));

            LabelRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task CreatLabel_ShouldCreateSuccessfully_WhenValid()
        {
            var dto = new CreateLabelRequest { Name = "Healthy", ColorCode = "#111" };

            LabelRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Label, bool>>>()))
                .ReturnsAsync(false);

            LabelRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Label>()))
                .ReturnsAsync((Label l) => l);

            await Sut.CreatLabelAsync(dto);

            LabelRepositoryMock.VerifyAll();
        }
    }
}
