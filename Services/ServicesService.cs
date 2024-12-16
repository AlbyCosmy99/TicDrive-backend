using TicDrive.Models;

namespace TicDrive.Services
{
    public interface IServicesService
    {
        List<Service> GetServices();
    }
    public class ServicesService : IServicesService
    {
        public List<Service> GetServices()
        {
            return new List<Service>
            {
                new Service
                {
                    Id = 1,
                    Title = "Oil change",
                    Description = "Your vehicle will get its oil changed."
                },
                new Service
                {
                    Id = 2,
                    Title = "Vehicle inspection",
                    Description = "Assess the vehicle's condition and identify potential issues or maintenance needs."
                },
                new Service
                {
                    Id = 3,
                    Title = "Service",
                    Description = "Comprehensive vehicle maintenance for performance and safety."
                },
                new Service
                {
                    Id = 4,
                    Title = "Tires",
                    Description = "Check and replace worn tires for optimal safety and performance."
                },
                new Service
                {
                    Id = 5,
                    Title = "Air conditioning",
                    Description = "Ensure your vehicle's A/C system is functioning properly for comfort."
                },
                new Service
                {
                    Id = 6,
                    Title = "Battery",
                    Description = "Test and replace the vehicle's battery to ensure reliable performance."
                }
            };
        }
    }
}
