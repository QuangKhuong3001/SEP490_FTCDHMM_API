using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Implementations;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.LabelServiceTests
{
    public abstract class LabelServiceTestBase
    {
        protected Mock<ILabelRepository> LabelRepositoryMock { get; }
        protected Mock<IMapper> MapperMock { get; }
        protected Mock<ICacheService> CacheServiceMock { get; }
        protected LabelService Sut { get; }

        protected LabelServiceTestBase()
        {
            LabelRepositoryMock = new Mock<ILabelRepository>(MockBehavior.Strict);
            MapperMock = new Mock<IMapper>(MockBehavior.Strict);
            CacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);

            Sut = new LabelService(
                LabelRepositoryMock.Object,
                MapperMock.Object,
                CacheServiceMock.Object);
        }

        protected Label CreateLabel(Guid? id = null)
        {
            return new Label
            {
                Id = id ?? Guid.NewGuid(),
                Name = "Label",
                UpperName = "LABEL",
                NormalizedName = "label",
                ColorCode = "#FFFFFF",
            };
        }
    }
}
