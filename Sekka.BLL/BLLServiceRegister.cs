using Microsoft.Extensions.DependencyInjection;
using Sekka.BLL.Classes;
using Sekka.BLL.Interfaces;
using Sekka.BLL.Services;
using Sekka.BLL.Profiles;
using Sekka.DAL.Repositories.Classes;
using Sekka.DAL.Repositories.Interfaces;

namespace Sekka.BLL
{
	public static class BLLServiceRegister
	{

		public static IServiceCollection AddBLLServices(this IServiceCollection services)
		{
			services.AddScoped<IComplaintService, ComplaintService>();
			services.AddScoped<IComplaintRepository, ComplaintRepository>();
			services.AddScoped<IAccountService, AccountService>();
			services.AddScoped<IDriverService, DriverService>();
			services.AddScoped<IAttachmentService, AttachmentService>();
			services.AddScoped<IAdminService, AdminService>();
			services.AddScoped<ITripService, TripService>();
			services.AddAutoMapper(typeof(DriverProfile));

			return services;
		}

	}
}