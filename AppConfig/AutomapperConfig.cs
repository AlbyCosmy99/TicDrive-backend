using AutoMapper;
using TicDrive.Dto.CarDto.CarMakeDto;
using TicDrive.Dto.CarDto.CarModelDto;
using TicDrive.Dto.ReviewDto;
using TicDrive.Dto.ServiceDto;
using TicDrive.Models;

namespace TicDrive.AppConfig
{
    public class AutomapperConfig : Profile
    {
        public AutomapperConfig() 
        {
            CreateMap<Service, FullServiceDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Icon, opt => opt.MapFrom(src => src.Icon));

            CreateMap<CarMake, FullCarMakeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<CarModel, FullCarModelDto>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year))
               .ForMember(dest => dest.CarMakeId, opt => opt.MapFrom(src => src.CarMakeId));

            CreateMap<Review, FullReviewDto>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
               .ForMember(dest => dest.WorkshopId, opt => opt.MapFrom(src => src.Workshop.Id))
               .ForMember(dest => dest.WhenPublished, opt => opt.MapFrom(src => src.WhenPublished))
               .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
               .ForMember(dest => dest.Stars, opt => opt.MapFrom(src => src.Stars));
        }
    }
}
