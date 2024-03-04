using Microsoft.AspNetCore.Identity;
using MyRoboAdvisor.Configuration;
using MyRoboAdvisor.Database.Data;
using MyRoboAdvisor.Database.Repositories;
using MyRoboAdvisor.Domain.Entities;
using MyRoboAdvisor.Services.Implementations;
using MyRoboAdvisor.Services.Interfaces;
using System.Text.Json.Serialization;

namespace MyRoboAdvisor.Extensions;

/// <summary>
/// 
/// </summary>
public static class HostingExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    public static void AddServices(this WebApplicationBuilder builder)
    {
        // Register Services
        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        builder.Services.AddScoped<IAuthService, AuthService>();

        builder.Services.AddControllers().AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            opts.JsonSerializerOptions.DefaultIgnoreCondition =
                JsonIgnoreCondition.WhenWritingNull;
        });

        builder.Services.AddProblemDetails();

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen();

        builder.Services.AddHealthChecks()
            .AddDbContextCheck<RoboAdvisorDbContext>();

        builder.Services.AddDatabase<RoboAdvisorDbContext>(builder.Configuration
            .GetRequiredSection("Database").Get<DatabaseConfiguration>());

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<RoboAdvisorDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddCors(c =>
        {
            c.AddPolicy("AllowOrigin", options =>
            {
                options.AllowAnyOrigin();
                options.AllowAnyHeader();
                options.AllowAnyMethod();
            });
        });
    }

    /// <summary>
    /// Configures the HTTP request pipeline.
    /// </summary>
    /// <param name="app">Web Application</param>
    public static async Task ConfigurePipeline(this WebApplication app)
    {
        if (!app.Environment.IsProduction())
        {
            await app.Services.MigrateDatabaseToLatestVersion<RoboAdvisorDbContext>();
            app.UseDeveloperExceptionPage();
            app.UseSwagger().UseSwaggerUI();
        }

        app.UseHealthChecks("/health");

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
    }
}