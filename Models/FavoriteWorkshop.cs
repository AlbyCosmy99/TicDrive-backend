namespace TicDrive.Models
{
    public class FavoriteWorkshop
    {
        public int Id { get; set; }
        private User _customer;
        public User Customer
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
        public User Workshop
        {
            get => _workshop;
            set
            {
                if (value.UserType != Enums.UserType.Workshop)
                {
                    throw new ArgumentException("Workshop must have UserType 2.", nameof(User));
                }
                _workshop = value;
            }
        }
    }
}
