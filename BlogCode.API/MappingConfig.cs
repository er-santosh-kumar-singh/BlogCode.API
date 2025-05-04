using AutoMapper;
using BlogCode.API.Models.Domain;
using BlogCode.API.Models.DTO;

namespace BlogCode.API
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps() 
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<UpdateBlogPostRequestDto, BlogPost>()
      .ForMember(dest => dest.Categories, opt => opt.Ignore())
      .ReverseMap();
                config.CreateMap<CategoryDto, Category>().ReverseMap();
                config.CreateMap<BlogPostDto, BlogPost>().ReverseMap();

                config.CreateMap<BlogPostDto, BlogPost>()
    .ForMember(dest => dest.Categories, opt => opt.Ignore())
    .ReverseMap();
                // config.CreateMap<UpdateBlogPostRequestDto, BlogPost>().ReverseMap();
            });
            return mappingConfig;
        }
    }
    
}
