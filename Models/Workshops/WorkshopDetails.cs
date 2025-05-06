using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using TicDrive.Enums;

namespace TicDrive.Models.Workshops
{
    public class WorkshopDetails
    {
        public int Id { get; set; }
        [Required]
        public string WorkshopId { get; set; }
        private User _workshop;
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
        public string WorkshopName { get; set; }
        public string? PersonalPhoneNumber { get; set; }
        public string? PersonalEmail { get; set; }
        public bool? OffersHomeServices { get; set; } = false;
        public int MaxDailyVehicles { get; set; } = 2;
        public string? Description { get; set; }
        public int LaborWarrantyMonths { get; set; } = 0;
        public string SignatureName { get; set; }
        public string SignatureSurname { get; set; }
        public System.DateTime SignatureDate { get; set; } = System.DateTime.UtcNow;
    }
}
