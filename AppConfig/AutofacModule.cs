using Autofac;
using TicDrive.Services;

namespace TicDrive.AppConfig
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ServicesService>().As<IServicesService>();
            builder.RegisterType<AuthService>().As<IAuthService>();
            builder.RegisterType<EmailService>().As<IEmailService>();
            builder.RegisterType<CarsService>().As<ICarsService>();
            builder.RegisterType<WorkshopsService>().As<IWorkshopsService>();
            builder.RegisterType<ReviewsService>().As<IReviewsService>();
            builder.RegisterType<CustomerService>().As<ICustomerService>();
        }
    }
}
