using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TicDrive.Models.Workshops
{
    public class WorkshopSpecialization
    {
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
                    throw new ArgumentException("Workshop must have UserType 2.", nameof(User));
                }
            }
        }
        public int SpecializationId { get; set; }
        [ForeignKey(nameof(SpecializationId))]
        public Specialization Specialization { get; set; }
    }
}
