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

        public IQueryable<FullServiceDto> GetServices(string workshopId, string? filter = null, string? languageCode = "en", int? fatherId = null)
        {
            var servicesQuery = _dbContext.Services.AsQueryable();

            if (!string.IsNullOrEmpty(workshopId))
            {
                var offeredServiceIds = _dbContext.OfferedServices
                    .Where(os => os.Workshop.Id == workshopId)
                    .Select(os => os.Service.Id)
                    .ToList();

                // Get a local cache of all services to walk the tree in memory
                var allServices = _dbContext.Services.ToList();

                var topLevelServiceIds = new HashSet<int>();

                foreach (var serviceId in offeredServiceIds)
                {
                    var current = allServices.FirstOrDefault(s => s.Id == serviceId);
                    while (current != null && current.FatherId != null)
                    {
                        current = allServices.FirstOrDefault(s => s.Id == current.FatherId);
                    }
                    if (current != null)
                    {
                        topLevelServiceIds.Add(current.Id);
                    }
                }

                servicesQuery = _dbContext.Services
                    .Where(s => topLevelServiceIds.Contains(s.Id));
            }
            else
            {
                servicesQuery = servicesQuery.Where(service => service.FatherId == fatherId);
            }

            var query = servicesQuery
                .Join(_dbContext.ServicesTranslations,
                    service => service.Id,
                    translation => translation.ServiceId,
                    (service, translation) => new { service, translation })
                .Join(_dbContext.Languages.Where(l => l.Code == languageCode),
                    st => st.translation.LanguageId,
                    language => language.Id,
                    (st, language) => new { st.service, translation = st.translation });

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(q => q.translation.Title.Contains(filter));
            }

            return query
                .OrderBy(q => q.service.Id)
                .Select(q => new FullServiceDto
                {
                    Id = q.service.Id,
                    Title = q.translation.Title,
                    Description = q.translation.Description,
                    Icon = q.service.Icon,
                    Bg_Image = q.service.Bg_Image,
                    FatherId = q.service.FatherId
                });
        }

        public bool ServiceHasChildren(int serviceId)
        {
            return _dbContext.Services.Any(s => s.FatherId == serviceId);
        }
    }
}
