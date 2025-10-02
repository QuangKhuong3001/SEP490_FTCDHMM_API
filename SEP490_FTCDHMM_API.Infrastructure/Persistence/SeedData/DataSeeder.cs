using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Infrastructure.ModelSettings;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.SeedData
{
    public static class DataSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider services, IConfiguration config)
        {
            using var scope = services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

            var adminConfig = config.GetSection("AdminAccount").Get<AdminAccountSettings>();
            if (adminConfig == null)
                throw new AppException(AppResponseCode.MISSING_ADMIN_ACCOUNT_CONFIG);

            var adminRole = await roleManager.FindByNameAsync(RoleValue.Admin.Name);
            if (adminRole == null)
            {
                adminRole = new AppRole
                {
                    IsActive = true,
                    Name = RoleValue.Admin.Name,
                    NormalizedName = RoleValue.Admin.Name.ToUpper()
                };
                var roleResult = await roleManager.CreateAsync(adminRole);
                if (!roleResult.Succeeded)
                {
                    throw new AppException(AppResponseCode.NOT_FOUND);
                }
            }

            var admin = await userManager.FindByEmailAsync(adminConfig.Email);
            if (admin == null)
            {
                var newAdmin = new AppUser
                {
                    UserName = adminConfig.Email,
                    Email = adminConfig.Email,
                    FirstName = adminConfig.FirstName,
                    LastName = adminConfig.LastName,
                    Gender = Gender.Other,
                    EmailConfirmed = true,
                    RoleId = adminRole.Id
                };

                var result = await userManager.CreateAsync(newAdmin, adminConfig.Password);
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create Admin user: {string.Join(";", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                if (admin.RoleId != adminRole.Id)
                {
                    admin.RoleId = adminRole.Id;
                    await userManager.UpdateAsync(admin);
                }
            }
        }
    }

}
