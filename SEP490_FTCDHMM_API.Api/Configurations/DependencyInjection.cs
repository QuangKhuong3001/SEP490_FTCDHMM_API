using System.Text;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SEP490_FTCDHMM_API.Api.Authorization;
using SEP490_FTCDHMM_API.Application.Interfaces;
using SEP490_FTCDHMM_API.Application.Services.Implementations;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;
using SEP490_FTCDHMM_API.Infrastructure.ModelSettings;
using SEP490_FTCDHMM_API.Infrastructure.Repositories;
using SEP490_FTCDHMM_API.Infrastructure.Services;
using SEP490_FTCDHMM_API.Shared.Exceptions;
using ApiMapping = SEP490_FTCDHMM_API.Api.Mappings;
using ApplicationMapping = SEP490_FTCDHMM_API.Application.Mappings;

namespace SEP490_FTCDHMM_API.Api.Configurations
{
    public static class DependencyInjection
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var domain = jwtSettings["Issuer"];

            // Connect with SQL Server
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("MyCnn")));

            // Config Identity
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(365);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();


            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(365);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;
            });

            // Auto Mapping
            services.AddAutoMapper(typeof(ApiMapping.AuthMappingProfile).Assembly,
                typeof(ApiMapping.CommonMappingProfile).Assembly,
                typeof(ApiMapping.UserMappingProfile).Assembly,
                typeof(ApiMapping.RoleMappingProfile).Assembly,
                typeof(ApplicationMapping.UserMappingProfile).Assembly,
                typeof(ApplicationMapping.RoleMappingProfile).Assembly);

            // Config JWT
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = domain,
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
                    )
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        throw new AppException(AppResponseCode.UNAUTHORIZED);
                    },

                    OnForbidden = context =>
                    {
                        throw new AppException(AppResponseCode.FORBIDDEN);
                    }
                };
            });

            //Role and Permission
            services.AddSingleton<IAuthorizationHandler, ModulePermissionHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, ModulePolicyProvider>();

            //bind settings value
            services.Configure<AwsS3Settings>(configuration.GetSection("AWS"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<AppSettings>(configuration.GetSection("Application"));
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

            // DI External Service

            //Mail
            services.AddScoped<SEP490_FTCDHMM_API.Application.Interfaces.IMailService, SEP490_FTCDHMM_API.Infrastructure.Services.MailService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();

            //Jwt&&Identity
            services.AddScoped<IJwtAuthService, JwtAuthService>();

            //S3
            services.AddScoped<IS3ImageService, S3ImageService>();
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();

            // DI Internal Service
            // Auth
            services.AddScoped<IAuthService, AuthService>();

            //Otp
            services.AddScoped<IOtpRepository, OtpRepository>();

            // User
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            //role
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            //permission
            services.AddScoped<IPermissionActionRepository, PermissionActionRepository>();
            services.AddScoped<IPermissionDomainRepository, PermissionDomainRepository>();

            //role-permission
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

        }
    }
}
