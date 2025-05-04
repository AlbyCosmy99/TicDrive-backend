using System.ComponentModel.DataAnnotations.Schema;

namespace TicDrive.Models
{
    public class OfferedServices
    {
        public int Id { get; set; }
        public required int ServiceId { get; set; }
        [ForeignKey(nameof(ServiceId))]
        public Service Service { get; set; }
        private User _workshop;
        public required string WorkshopId { get; set; }
        [ForeignKey(nameof(WorkshopId))]
        public User Workshop
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
        public decimal? Price { get; set; } = 0; //TODO: to make it required once the workshops can add prices on their own
        public char? Currency { get; set; } = '€';
        public decimal? Discount { get; set; } = 0;
    } 
}
