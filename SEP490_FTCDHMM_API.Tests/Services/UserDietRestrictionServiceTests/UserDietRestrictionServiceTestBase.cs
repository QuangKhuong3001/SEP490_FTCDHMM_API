using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Mappings;
using SEP490_FTCDHMM_API.Application.Services.Implementations;

namespace SEP490_FTCDHMM_API.Tests.Services.UserDietRestrictionServiceTests
{
    public abstract class UserDietRestrictionServiceTestBase
    {
        protected readonly Mock<IUserDietRestrictionRepository> _mockRepo;
        protected readonly Mock<IIngredientCategoryRepository> _mockCategoryRepo;
        protected readonly Mock<IIngredientRepository> _mockIngredientRepo;
        protected readonly IMapper _mapper;
        protected readonly UserDietRestrictionService _service;

        protected UserDietRestrictionServiceTestBase()
        {
            _mockRepo = new Mock<IUserDietRestrictionRepository>();
            _mockCategoryRepo = new Mock<IIngredientCategoryRepository>();
            _mockIngredientRepo = new Mock<IIngredientRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new UserIngredientRestrictionMappingProfile());
            });

            _mapper = config.CreateMapper();

            _service = new UserDietRestrictionService(
                _mockRepo.Object,
                _mapper,
                _mockCategoryRepo.Object,
                _mockIngredientRepo.Object
            );
        }
    }
}
