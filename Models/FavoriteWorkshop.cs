using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicDrive.Models
{
    public class FavoriteWorkshop
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; }
        private User _customer;
        [ForeignKey(nameof(CustomerId))]
        public virtual User Customer
        {
            get => _customer;
            set
            {
                if(value.UserType != Enums.UserType.Customer)
                {
                    throw new ArgumentException("Customer must have UserType 1.", nameof(User));
                }
                _customer = value;
                CustomerId = value.Id;
            }
        }

        [Required]
        public string WorkshopId { get; set; }
        private User _workshop;
        [ForeignKey(nameof(WorkshopId))]
        public virtual User Workshop
        {
            get => _workshop;
            set
            {
                if (value.UserType != Enums.UserType.Workshop)
                {
                    throw new ArgumentException("Workshop must have UserType 2.", nameof(User));
                }
                _workshop = value;
                WorkshopId = value.Id;
            }
        }
    }
}
