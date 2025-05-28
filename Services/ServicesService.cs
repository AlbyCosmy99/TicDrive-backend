using TicDrive.Context;
using TicDrive.Dto.ServiceDto;
using TicDrive.Models;

namespace TicDrive.Services
{
    public interface IServicesService
    {
        IQueryable<FullServiceDto> GetServices(string workshopId, string? filter = null, string? languageCode = "en", int? fatherId = null);
        bool ServiceHasChildren(int serviceId);
    }

    public class ServicesService : IServicesService
    {
        private readonly TicDriveDbContext _dbContext;

        public ServicesService(TicDriveDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<FullServiceDto> GetServices(string workshopId, string? filter = null, string? languageCode = "en",int? fatherId = null)
        {
            var query = _dbContext.Services
                .Where(service => service.FatherId ==  fatherId)
                .Join(_dbContext.ServicesTranslations,
                    service => service.Id,
                    serviceTranslation => serviceTranslation.ServiceId,
                    (service, serviceTranslation) => new { service, serviceTranslation })
                .Join(_dbContext.Languages
                    .Where(language => language.Code == languageCode),
                    sst => sst.serviceTranslation.LanguageId,
                    language => language.Id,
                    (sst, language) => new { sst.service, sst.serviceTranslation, language })
                .AsQueryable();


            if (!string.IsNullOrEmpty(workshopId))
            {
                query = query.Join(
                    _dbContext.OfferedServices.Where(os => os.Workshop.Id == workshopId),
                    sstl => sstl.service.Id,
                    offeredService => offeredService.Service.Id,
                    (service, offeredService) => service
                );
            }

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(sstl => sstl.serviceTranslation.Title.Contains(filter));
            }

            return query.OrderBy(sstl => sstl.service.Id)
                .Select(sstl => new FullServiceDto
                {
                    Id = sstl.service.Id,
                    Title = sstl.serviceTranslation.Title,
                    Description = sstl.serviceTranslation.Description,
                    Icon = sstl.service.Icon,
                    Bg_Image = sstl.service.Bg_Image,
                    FatherId = sstl.service.FatherId,
                });
        }

        public bool ServiceHasChildren(int serviceId)
        {
            return _dbContext.Services.Any(s => s.FatherId == serviceId);
        }
    }
}
