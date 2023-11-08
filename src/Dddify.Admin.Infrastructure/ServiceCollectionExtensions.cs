using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JwtBearerOptions = Dddify.Admin.Infrastructure.Services.JwtBearerOptions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:Configuration"];
            options.InstanceName = configuration["Redis:InstanceName"];
        });

        services.AddOptions<JwtBearerOptions>().BindConfiguration(JwtBearerOptions.SectionName);

        services.AddSingleton<IJwtHelper, JwtHelper>();
        services.AddSingleton<IPasswordHelper, PasswordHelper>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Authentication:JwtBearer:Issuer"],
                ValidAudience = configuration["Authentication:JwtBearer:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:JwtBearer:Secret"]!))
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Add("TOKEN-EXPIRED", "true");
                    }

                    return Task.CompletedTask;
                }
            };
        });

        services.AddDbContext<ApplicationDbContext>(
            options => options
            .UseMySql(configuration.GetConnectionString("Default"), ServerVersion.Parse("8.0.34"), c => c.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors());

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}