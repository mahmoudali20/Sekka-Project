using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sekka.DAL.Models;

namespace Sekka.DAL.Context
{
    public class IdentityDataSeeding
    {
        public static async Task SeedIdentityData(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger logger, CancellationToken ct = default)
        {
            try
            {
                bool hasUsers = await userManager.Users.AnyAsync(ct);

                // =========================
                // Roles
                // =========================

                var roles = new List<string>
                {
                    "SuperAdmin",
                    "Admin",
                    "Driver",
                    "Passenger"
                };

                foreach (var roleName in roles)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        var role = new IdentityRole(roleName);

                        var roleResult = await roleManager.CreateAsync(role);

                        if (!roleResult.Succeeded)
                        {
                            logger.LogError(
                                "Failed to add role {RoleName}",
                                roleName);
                        }
                    }
                }

                // =========================
                // Users
                // =========================

                if (!hasUsers)
                {
                    var mainAdmin = new ApplicationUser
                    {
                        FullName = "Mahmoud Ali",
                        Email = "Mahmoud@gmail.com",
                        UserName = "MahmoudAli",
                        PhoneNumber = "01156542833",
                        Address = "Cairo"
                    };

                    var mainAdminResult = await userManager.CreateAsync(mainAdmin, "P@ssw0rd");

                    if (mainAdminResult.Succeeded)
                        await userManager.AddToRoleAsync(mainAdmin, "SuperAdmin");



                    var admin = new ApplicationUser
                    {
                        FullName = "Nour Ahmed",
                        Email = "Nour@gmail.com",
                        UserName = "NourAhmed",
                        PhoneNumber = "01035220185",
                        Address = "Cairo"
                    };

                    var adminResult = await userManager.CreateAsync(admin, "P@ssw0rd");

                    if (adminResult.Succeeded)
                        await userManager.AddToRoleAsync(admin, "Admin");


                    logger.LogInformation("Identity Seeded Successfully");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while seeding Identity data");
            }
        }
    }
}