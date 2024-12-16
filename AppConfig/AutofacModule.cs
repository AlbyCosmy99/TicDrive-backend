using Autofac;
using TicDrive.Services;

namespace TicDrive.AppConfig
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ServicesService>().As<IServicesService>();
        }
    }
}
