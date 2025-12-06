using DemoProject.DAL;
using DemoProject.Repository;
using DemoProject.Repository.Implementation;
using DemoProject.Services;
using DemoProject.Services.Implimentation;

namespace DemoProject.Resolver
{
    public class RegisterServices
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IRegistrationServices, RegistrationServices>();
            services.AddScoped<IDalBase, DalBase>();
            services.AddScoped<IRegistrationRepository, RegistrationRepository>();

            // Other service registrations...
        }
    }
}
