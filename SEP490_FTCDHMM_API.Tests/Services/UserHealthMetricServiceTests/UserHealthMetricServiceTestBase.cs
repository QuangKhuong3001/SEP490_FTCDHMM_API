using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Implementations;

public abstract class UserHealthMetricServiceTestBase
{
    protected readonly Mock<IUserHealthMetricRepository> MetricRepoMock;
    protected readonly Mock<IUserRepository> UserRepoMock;
    protected readonly Mock<IMapper> MapperMock;
    protected readonly UserHealthMetricService Sut;
    protected readonly Mock<ICacheService> CacheServiceMock;

    protected UserHealthMetricServiceTestBase()
    {
        MetricRepoMock = new Mock<IUserHealthMetricRepository>();
        UserRepoMock = new Mock<IUserRepository>();
        MapperMock = new Mock<IMapper>();
        CacheServiceMock = new Mock<ICacheService>();

        Sut = new UserHealthMetricService(
            MetricRepoMock.Object,
            MapperMock.Object,
            CacheServiceMock.Object,
            UserRepoMock.Object
        );
    }
}
