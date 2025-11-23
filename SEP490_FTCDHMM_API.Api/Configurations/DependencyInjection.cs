using System.Text;
using Amazon.S3;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SEP490_FTCDHMM_API.Api.Authorization;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Jobs.Implementations;
using SEP490_FTCDHMM_API.Application.Jobs.Interfaces;
using SEP490_FTCDHMM_API.Application.Services.Implementations;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.Interfaces;
using SEP490_FTCDHMM_API.Infrastructure.Data;
using SEP490_FTCDHMM_API.Infrastructure.ModelSettings;
using SEP490_FTCDHMM_API.Infrastructure.Persistence;
using SEP490_FTCDHMM_API.Infrastructure.Repositories;
using SEP490_FTCDHMM_API.Infrastructure.Services;
using SEP490_FTCDHMM_API.Shared.Exceptions;

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


            //cache
            //services.AddSingleton<IConnectionMultiplexer>(sp =>
            //{
            //    var redisConnection = configuration.GetConnectionString("Redis");
            //    return ConnectionMultiplexer.Connect(redisConnection!);
            //});


            //hangfire
            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                      .UseSimpleAssemblyNameTypeSerializer()
                      .UseRecommendedSerializerSettings()
                      .UseSqlServerStorage(configuration.GetConnectionString("MyCnn"), new SqlServerStorageOptions
                      {
                          CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                          SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                          QueuePollInterval = TimeSpan.Zero,
                          UseRecommendedIsolationLevel = true,
                          DisableGlobalLocks = true
                      });
            });
            services.AddHangfireServer();

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
                // Lockout configuration
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(365);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // Password policy configuration
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;  // Special characters: @#$%^&*-_
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            });

            // Auto Mapping
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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

            //batch/job module
            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                      .UseSimpleAssemblyNameTypeSerializer()
                      .UseRecommendedSerializerSettings()
                      .UseSqlServerStorage(configuration.GetConnectionString("MyCnn"), new SqlServerStorageOptions
                      {
                          CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                          SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                          QueuePollInterval = TimeSpan.Zero,
                          UseRecommendedIsolationLevel = true,
                          DisableGlobalLocks = true
                      });
            });

            //            RecurringJob.AddOrUpdate<IngredientPopularityJob>(
            //    job => job.ExecuteAsync(),
            //    Cron.Daily(2, 0)
            //);

            //bind settings value
            services.Configure<AwsS3Settings>(configuration.GetSection("AWS"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<AppSettings>(configuration.GetSection("Application"));
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            services.Configure<GoogleAuthSettings>(configuration.GetSection("GoogleAuth"));

            //job
            services.AddScoped<IExpireUserDietRestrictionsJob, ExpireUserDietRestrictionsJob>();

            // DI External Service

            //redis
            //services.AddScoped<ICacheService, RedisCacheService>();

            //usda
            //services.AddScoped<IUsdaApiService, UsdaApiService>();

            //rollback
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Mail
            services.AddScoped<IMailService, SEP490_FTCDHMM_API.Infrastructure.Services.MailService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();

            //Jwt&&Identity
            services.AddScoped<IJwtAuthService, JwtAuthService>();

            //S3
            services.AddScoped<IS3ImageService, S3ImageService>();
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();

            //Google
            services.AddHttpClient<IGoogleAuthService, GoogleAuthService>();
            services.AddScoped<IGoogleProvisioningService, GoogleProvisioningService>();

            //gemini
            services.AddScoped<IGeminiIngredientDetectionService, GeminiIngredientDetectionService>();

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

            //user interaction
            services.AddScoped<IUserFollowRepository, UserFollowRepository>();

            //ingredient
            services.AddScoped<IIngredientRepository, IngredientRepository>();
            services.AddScoped<IIngredientService, IngredientService>();

            //ingredientCategory
            services.AddScoped<IIngredientCategoryRepository, IngredientCategoryRepository>();
            services.AddScoped<IIngredientCategoryService, IngredientCategoryService>();

            //nutrient
            services.AddScoped<INutrientRepository, NutrientRepository>();
            services.AddScoped<INutrientService, NutrientService>();

            //label
            services.AddScoped<ILabelRepository, LabelRepository>();
            services.AddScoped<ILabelService, LabelService>();

            //recipe
            services.AddScoped<IRecipeRepository, RecipeRepository>();
            services.AddScoped<IRecipeService, RecipeService>();

            //userRecipeView
            services.AddScoped<IUserRecipeViewRepository, UserRecipeViewRepository>();

            //userSaveRecipe
            services.AddScoped<IUserSaveRecipeRepository, UserSaveRecipeRepository>();

            //userFavoriteRecipe
            services.AddScoped<IUserFavoriteRecipeRepository, UserFavoriteRecipeRepository>();

            //cookingstep
            services.AddScoped<ICookingStepRepository, CookingStepRepository>();

            //image
            services.AddScoped<IImageRepository, ImageRepository>();

            //ingredientDetection
            services.AddScoped<IIngredientDetectionService, IngredientDetectionService>();
            //comment rating
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IRatingService, RatingService>();

            //healthgoal
            services.AddScoped<IHealthGoalRepository, HealthGoalRepository>();
            services.AddScoped<IHealthGoalService, HealthGoalService>();


            //userhealthgoal
            services.AddScoped<IUserHealthGoalRepository, UserHealthGoalRepository>();
            services.AddScoped<IUserHealthGoalService, UserHealthGoalService>();

            //customhealthgoal
            services.AddScoped<ICustomHealthGoalRepository, CustomHealthGoalRepository>();
            services.AddScoped<ICustomHealthGoalService, CustomHealthGoalService>();

            //RecipeScore
            services.AddScoped<IRecipeGoalAnalysisService, RecipeGoalAnalysisService>();
            services.AddScoped<IRecipeNutritionAggregator, RecipeNutritionAggregator>();


            //userhealthgoal
            services.AddScoped<IUserHealthGoalRepository, UserHealthGoalRepository>();
            services.AddScoped<IUserHealthGoalService, UserHealthGoalService>();

            //customhealthgoal
            services.AddScoped<ICustomHealthGoalRepository, CustomHealthGoalRepository>();
            services.AddScoped<ICustomHealthGoalService, CustomHealthGoalService>();

            //healthgoalconflict
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IRatingRepository, RatingRepository>();
            services.AddScoped<IRealtimeNotifier, SignalRNotifierService>();

            //userhealthmetric
            services.AddScoped<IUserHealthMetricRepository, UserHealthMetricRepository>();
            services.AddScoped<IUserHealthMetricService, UserHealthMetricService>();

            //dietRestriction
            services.AddScoped<IUserDietRestrictionRepository, UserDietRestrictionRepository>();
            services.AddScoped<IUserDietRestrictionService, UserDietRestrictionService>();

            //recipeUserTag
            services.AddScoped<IRecipeUserTagRepository, RecipeUserTagRepository>();

            //draftRecipe
            services.AddScoped<IDraftRecipeUserTagRepository, DraftRecipeUserTagRepository>();
            services.AddScoped<IDraftRecipeIngredientRepository, DraftRecipeIngredientRepository>();
            services.AddScoped<IDraftCookingStepRepository, DraftCookingStepRepository>();
            services.AddScoped<IDraftCookingStepImageRepository, DraftCookingStepImageRepository>();
            services.AddScoped<IDraftRecipeRepository, DraftRecipeRepository>();

            services.AddScoped<IDraftRecipeService, DraftRecipeService>();

        }
    }
}
