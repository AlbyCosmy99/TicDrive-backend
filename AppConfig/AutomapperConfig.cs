using AutoMapper;
using TicDrive.Dto.CarDto.CarMakeDto;
using TicDrive.Dto.CarDto.CarModelDto;
using TicDrive.Dto.CarDto.CarModelVersionDto;
using TicDrive.Dto.FavoriteWorkshopDto;
using TicDrive.Dto.OfferedServicesDto;
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
               .ForMember(dest => dest.CarMakeId, opt => opt.MapFrom(src => src.CarMakeId));

            CreateMap<FavoriteWorkshop, FullFavoriteWorkshopDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.WorkshopId, opt => opt.MapFrom(src => src.WorkshopId));


            CreateMap<OfferedServices, FullOfferedServicesDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.ServiceId))
                .ForMember(dest => dest.WorkshopId, opt => opt.MapFrom(src => src.WorkshopId))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency));

            CreateMap<CarModelVersion, FullCarModelVersionDto>()
                  .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                  .ForMember(dest => dest.CarModelId, opt => opt.MapFrom(src => src.CarModelId))
                  .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year));
        }
    }
}
