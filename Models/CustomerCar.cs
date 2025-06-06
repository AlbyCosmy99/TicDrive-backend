using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TicDrive.AbstractClasses;

namespace TicDrive.Models
{
    public class CustomerCar : SoftDeletableEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        private User _customer;
        public virtual User Customer
        {
            get => _customer;
            set
            {
                if (value.UserType != Enums.UserType.Customer)
                {
                    throw new ArgumentException("Customer must have UserType 1.", nameof(User));
                }
                _customer = value;
                CustomerId = value.Id;
            }
        }

        public int CarId { get; set; }
        [ForeignKey(nameof(CarId))]
        public Car Car { get; set; }
        public int? Km { get; set; }
        public string? Name { get; set; }
    }
}
