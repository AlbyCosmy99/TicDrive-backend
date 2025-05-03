using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TicDrive.Models.Workshops
{
    public class WorkshopNonWorkingDay
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string WorkshopId { get; set; }

        private User _workshop;

        [ForeignKey(nameof(WorkshopId))]
        public required User Workshop
        {
            get => _workshop;
            set
            {
                if (value.UserType != Enums.UserType.Workshop)
                {
                    throw new ArgumentException("The associated user must be of type 'Workshop' (UserType = 2).", nameof(Workshop));
                }
                _workshop = value;
            }
        }

        [Required]
        public System.DateTime Date { get; set; }
    }
}
