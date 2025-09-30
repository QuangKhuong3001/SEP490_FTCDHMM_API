using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.SeedData
{
    public static class ModelBuilderExtensions
    {
        public static void SeedRoles(this ModelBuilder builder)
        {
            var roleType = typeof(RoleValue);

            var roles = roleType
                .GetFields(System.Reflection.BindingFlags.Public |
                           System.Reflection.BindingFlags.Static |
                           System.Reflection.BindingFlags.DeclaredOnly)
                .Where(f => f.FieldType == typeof(RoleValue))
                .Select(f => (RoleValue)f.GetValue(null)!)
                .ToList();

            var rows = roles.Select(r => new AppRole
            {
                Id = DeterministicGuid(r.Name),
                Name = r.Name,
                NormalizedName = r.Name.ToUpperInvariant(),
                IsActive = true
            }).ToArray();

            builder.Entity<AppRole>().HasData(rows);
        }

        public static void SeedPermissions(this ModelBuilder builder)
        {
            var permissionType = typeof(PermissionValue);

            var permissionValues = permissionType
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(f => f.FieldType == typeof(PermissionValue))
                .Select(f => (PermissionValue)f.GetValue(null)!)
                .ToList();

            var domains = permissionValues
                .GroupBy(p => p.Domain)
                .ToList();

            var domainEntities = new List<PermissionDomain>();
            var actionEntities = new List<PermissionAction>();

            foreach (var domainGroup in domains)
            {
                var domainId = DeterministicGuid(domainGroup.Key);

                domainEntities.Add(new PermissionDomain
                {
                    Id = domainId,
                    Name = domainGroup.Key
                });

                foreach (var perm in domainGroup)
                {
                    actionEntities.Add(new PermissionAction
                    {
                        Id = DeterministicGuid($"{perm.Domain}:{perm.Action}"),
                        Name = perm.Action,
                        PermissionDomainId = domainId
                    });
                }
            }

            builder.Entity<PermissionDomain>().HasData(domainEntities);
            builder.Entity<PermissionAction>().HasData(actionEntities);
        }


        public static void SeedRolePermissions(this ModelBuilder builder)
        {
            var roles = new[]
            {
                new { Id = DeterministicGuid(RoleValue.Admin.Name), Name = RoleValue.Admin.Name },
                new { Id = DeterministicGuid(RoleValue.Moderator.Name), Name = RoleValue.Moderator.Name },
                new { Id = DeterministicGuid(RoleValue.Customer.Name), Name = RoleValue.Customer.Name }
            };

            var permissions = typeof(PermissionValue)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(f => f.FieldType == typeof(PermissionValue))
                .Select(f => (PermissionValue)f.GetValue(null)!)
                .ToList();

            var rolePermissions = new List<AppRolePermission>();

            foreach (var role in roles)
            {
                foreach (var p in permissions)
                {
                    rolePermissions.Add(new AppRolePermission
                    {
                        RoleId = role.Id,
                        PermissionActionId = DeterministicGuid($"{p.Domain}:{p.Action}"),
                        IsActive = role.Name == RoleValue.Admin.Name,
                    });
                }
            }

            builder.Entity<AppRolePermission>().HasData(rolePermissions);
        }


        private static Guid DeterministicGuid(string text)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(text));
            return new Guid(hash);
        }
    }
}
