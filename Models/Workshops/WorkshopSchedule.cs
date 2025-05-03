using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicDrive.Models.DateTime;

namespace TicDrive.Models.Workshops
{
    public class WorkshopSchedule
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

        public int DayId { get; set; }

        [ForeignKey(nameof(DayId))]
        public required Day Day { get; set; }

        public TimeOnly? MorningStartTime { get; set; }
        public TimeOnly? MorningEndTime { get; set; }
        public TimeOnly? AfternoonStartTime { get; set; }
        public TimeOnly? AfternoonEndTime { get; set; }
    }
}
