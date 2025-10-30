using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class IngredientMappingProfile : Profile
    {
        public IngredientMappingProfile()
        {
            CreateMap<Ingredient, IngredientNameResponse>();

            CreateMap<Ingredient, IngredientResponse>()
                .ForMember(dest => dest.CategoryNames,
                    opt => opt.MapFrom(src => src.Categories));

            CreateMap<Ingredient, IngredientDetailsResponse>()
                // ImageUrl sẽ được gán ở đây nhưng sau đó sẽ bị override ở IngredientService.GetDetails()
                // để tạo Signed URL thay vì dùng URL trực tiếp, bucket S3 được thiết lập Private,
                // nên URL trực tiếp sẽ trả về 403 Forbidden. Service layer sẽ xử lý việc ký URL để đảm bảo kiểm soát truy cập đúng cách
                .ForMember(dest => dest.ImageUrl,
                    opt => opt.MapFrom(src => src.Image != null ? $"https://sep490-images.s3.amazonaws.com/{src.Image.Key}" : string.Empty))
                .ForMember(dest => dest.Nutrients,
                    opt => opt.MapFrom(src => src.IngredientNutrients));
        }
    }
}
