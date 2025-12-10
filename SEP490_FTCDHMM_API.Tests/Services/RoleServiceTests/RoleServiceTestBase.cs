using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Mappings;
using SEP490_FTCDHMM_API.Application.Services.Implementations;

namespace SEP490_FTCDHMM_API.Tests.Services.RoleServiceTests
{
    public abstract class RoleServiceTestBase
    {
        protected readonly Mock<IRoleRepository> RoleRepoMock = new();
        protected readonly Mock<IUserRepository> UserRepoMock = new();
        protected readonly Mock<IPermissionActionRepository> PermissionActionRepoMock = new();
        protected readonly Mock<IPermissionDomainRepository> PermissionDomainRepoMock = new();
        protected readonly Mock<IRolePermissionRepository> RolePermissionRepoMock = new();

        protected readonly IMapper Mapper;
        protected readonly RoleService Sut;

        protected RoleServiceTestBase()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<RoleMappingProfile>();
            });

            Mapper = mapperConfig.CreateMapper();

            Sut = new RoleService(
                RoleRepoMock.Object,
                UserRepoMock.Object,
                PermissionActionRepoMock.Object,
                PermissionDomainRepoMock.Object,
                RolePermissionRepoMock.Object,
                Mapper
            );
        }
    }
}
