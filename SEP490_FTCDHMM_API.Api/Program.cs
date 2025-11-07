using Hangfire;
using SEP490_FTCDHMM_API.Api.Configurations;
using SEP490_FTCDHMM_API.Api.Middleware;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Infrastructure.Hubs;
using SEP490_FTCDHMM_API.Infrastructure.Persistence.SeedData;
using SEP490_FTCDHMM_API.Infrastructure.Security;
var builder = WebApplication.CreateBuilder(args);

const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "https://sep-490-ftcdhmm-ui.vercel.app")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddServices(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new GenderJsonConverter());
    });
builder.Services.AddSignalR(options =>
{
    // Configure keepalive to prevent timeout
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60); // 60s - timeout nếu client không respond
    options.KeepAliveInterval = TimeSpan.FromSeconds(15); // 15s - gửi keepalive ping
    options.HandshakeTimeout = TimeSpan.FromSeconds(10); // 10s - handshake timeout
});
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SEP490 FTCDHMM API",
        Version = "v1",
        Description = "API for SEP490 FTCDHMM Project"
    });

    // Define the security scheme
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // Make sure swagger UI requires a Bearer token to be specified
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var config = services.GetRequiredService<IConfiguration>();

    await DataSeeder.SeedAdminAsync(services, config);
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthFilter() }
});

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();
app.MapHub<RecipeHub>("/hubs/recipe");
app.MapHub<CommentHub>("/hubs/comments");
app.MapControllers();

app.Run();
