using System.ComponentModel.DataAnnotations.Schema;

namespace TicDrive.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public System.DateTime DateTime { get; set; }
        private User _customer;
        public required string CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public User Customer
        {
            get => _customer;
            set
            {
                if (value.UserType != Enums.UserType.Customer)
                {
                    throw new ArgumentException("Customer must have UserType 1.", nameof(User));
                }
            }
        }
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
        public required int ServiceId { get; set; }
        [ForeignKey(nameof(ServiceId))]
        public Service Service { get; set; }
        public required int CustomerCarId { get; set; }
        [ForeignKey(nameof(CustomerCarId))]
        public CustomerCar CustomerCar { get; set; }
        public decimal FinalPrice { get; set; }
    }
}
