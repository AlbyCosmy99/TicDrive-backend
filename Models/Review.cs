namespace TicDrive.Models
{
    public class Review
    {
        public int Id { get; set; }
        private User _customer;
        public required User Customer
        {
            get => _customer;
            set
            {
                if(value.UserType != Enums.UserType.Customer)
                {
                    throw new ArgumentException("Customer must have UserType 1.", nameof(User));
                }
                _customer = value;
            }
        }
        private User _workshop;
        public required User Workshop {
            get => _workshop;
            set
            {
                if(value.UserType != Enums.UserType.Workshop)
                {
                    throw new ArgumentException("Workshop must have UserType 2.", nameof(User));
                }
            }
        }

        public required string Text { get; set; }

        public DateTime WhenPublished { get; set; } = DateTime.Now;

        private double _stars;

        public required double Stars
        {
            get => _stars;
            set
            {
                if (value < 1.0 || value > 5.0 || (value * 2) % 1 != 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Stars), "Stars must be between 1.0 and 5.0 in increments of 0.5.");
                }
                _stars = value;
            }
        }
    }
}
