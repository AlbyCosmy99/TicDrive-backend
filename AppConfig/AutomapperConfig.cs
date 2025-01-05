using AutoMapper;
using TicDrive.Dto.CarDto.CarMakeDto;
using TicDrive.Dto.CarDto.CarModelDto;
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
        }
    }
}
