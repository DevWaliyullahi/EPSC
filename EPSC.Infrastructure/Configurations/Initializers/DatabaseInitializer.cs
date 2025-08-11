using EPSC.Infrastructure.Identity.Auth;
using Microsoft.AspNetCore.Identity;

namespace EPSC.Infrastructure.Configurations.Initializers
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly RoleManager<EPSAuthRole> _roleManager;
        private readonly UserManager<EPSAuthUser> _userManager;

        public DatabaseInitializer(RoleManager<EPSAuthRole> roleManager, UserManager<EPSAuthUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            string[] roles = { "Admin", "Employer", "Contributor" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new EPSAuthRole { Name = role });
            }

            var adminEmail = "admin@epsc.local";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new EPSAuthUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "EPSC Admin"
                };

                // Get admin password from environment 
                var adminPassword = Environment.GetEnvironmentVariable("EPSC_ADMIN_PASSWORD");
                if (string.IsNullOrWhiteSpace(adminPassword))
                {
                    adminPassword = "Admin@123"; // fallback
                }

                var result = await _userManager.CreateAsync(user, adminPassword);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    // Log errors 
                    throw new Exception("Failed to create admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
