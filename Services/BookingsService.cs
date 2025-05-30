using TicDrive.Context;

namespace TicDrive.Services
{
    public interface IBookingsService
    {
        void BookAService();
    }
    public class BookingsService : IBookingsService
    {
        private readonly TicDriveDbContext _context;
        public BookingsService(TicDriveDbContext context)
        {
            _context = context;
        }


        public void BookAService()
        {

        }
    }
}
