using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sekka.DAL.Context;
using Sekka.DAL.Repositories.Classes;
using Sekka.DAL.Repositories.Interfaces;

namespace Sekka.DAL
{
    public static class DALServiceRegister
    {

        public static IServiceCollection AddDALServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<SekkaDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });






            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDriverRepository, DriverRepository>();
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            return services;
        }
    }
}
