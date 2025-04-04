using System.ComponentModel.DataAnnotations.Schema;

namespace TicDrive.Models
{
    public class OfferedServices
    {
        public required int Id { get; set; }
        public required int ServiceId { get; set; }
        [ForeignKey(nameof(ServiceId))]
        public required Service Service { get; set; }
        private User _workshop;
        public required string WorkshopId { get; set; }
        [ForeignKey(nameof(WorkshopId))]
        public required User Workshop
        {
            get => _workshop;
            set
            {
                if (value.UserType != Enums.UserType.Workshop)
                {
                    throw new ArgumentException("Workshop must have UserType 2.", nameof(User));
                }
            }
        }
        public required decimal Price { get; set; }
        public required char Currency { get; set; } = '€';
        public decimal? Discount { get; set; } = 0;
    } 
}
