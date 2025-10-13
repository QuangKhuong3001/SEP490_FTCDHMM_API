using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.GoogleAuthDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class GoogleProvisioningService : IGoogleProvisioningService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IRoleRepository _roleRepository;
        private readonly IS3ImageService _s3ImageService;

        public GoogleProvisioningService(UserManager<AppUser> userManager,
            IRoleRepository roleRepository,
            IS3ImageService s3ImageService)
        {
            _userManager = userManager;
            _roleRepository = roleRepository;
            _s3ImageService = s3ImageService;
        }

        public async Task<AppUser> FindOrProvisionFromGoogleAsync(GoogleProvisionRequest req)
        {
            var p = req.Payload;

            var user = await _userManager.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == p.Email);

            var firstName = req.UserInfo?.GivenName ?? p.GivenName ?? p.Name?.Split(' ').FirstOrDefault() ?? "";
            var lastName = req.UserInfo?.FamilyName ?? p.FamilyName ?? p.Name?.Split(' ').LastOrDefault() ?? "";
            var picture = req.UserInfo?.PictureUrl ?? p.PictureUrl;

            Gender gender = Gender.Other;
            if (!string.IsNullOrWhiteSpace(req.UserInfo?.Gender))
            {
                gender = req.UserInfo.Gender.ToLower() switch
                {
                    "male" => Gender.Male,
                    "female" => Gender.Female,
                    _ => Gender.Other
                };
            }

            var phone = req.UserInfo?.PhoneNumber;

            if (user == null)
            {
                user = new AppUser
                {
                    UserName = p.Email,
                    Email = p.Email,
                    EmailConfirmed = p.EmailVerified,
                    FirstName = firstName,
                    LastName = lastName,
                    CreatedAtUtc = DateTime.UtcNow,
                    Gender = gender,
                    PhoneNumber = phone,
                };

                var customerRole = await _roleRepository.FindByNameAsync(RoleValue.Customer.Name);
                if (customerRole != null)
                    user.RoleId = customerRole.Id;

                var create = await _userManager.CreateAsync(user);
                if (!create.Succeeded)
                    throw new Exception(string.Join("; ", create.Errors.Select(e => e.Description)));

                var loginInfo = new UserLoginInfo("Google", p.Subject ?? p.Email, "Google");
                await _userManager.AddLoginAsync(user, loginInfo);
            }

            if (!string.IsNullOrWhiteSpace(picture) && user.AvatarId == null)
            {
                var avatar = await _s3ImageService.MirrorExternalImageAsync(StorageFolder.AVATARS, picture, user.Id);
                user.Avatar = avatar;
                await _userManager.UpdateAsync(user);
            }

            return user;
        }
    }
}
