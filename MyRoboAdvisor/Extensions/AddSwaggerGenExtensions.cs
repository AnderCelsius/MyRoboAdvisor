using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace MyRoboAdvisor.Extensions;

public static class AddSwaggerGenExtensions
{
    /// <summary>
    /// Sets up Swagger schema generation with additional settings.
    /// </summary>
    /// <param name="services">IServiceCollection.</param>
    /// <returns>services.</returns>
    public static IServiceCollection AddSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(opt =>
        {
            opt.SupportNonNullableReferenceTypes();
            opt.UseAllOfToExtendReferenceSchemas();
            opt.SchemaFilter<MakeNotNullableRequiredSchemaFilter>();

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
            opt.IncludeXmlComments(xmlPath);

            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Robo Advisor API",
                Description = "", //TODO: Update description of the project here
                Version = "v1"
            });

            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer",
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                  new OpenApiSecurityScheme
                  {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer", },
                  },
                  new List<string>()
                },
            });
        });

        return services;
    }


    /// <summary>
    /// Finds all schema properties that are not nullable, and marks them as required.
    /// This removes the "|undefined" from the generated typescript client.
    /// </summary>
    public class MakeNotNullableRequiredSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties == null)
            {
                return;
            }

            var notNullableProperties = schema
              .Properties
              .Where(x => !x.Value.Nullable && !schema.Required.Contains(x.Key))
              .ToList();

            foreach (var property in notNullableProperties)
            {
                schema.Required.Add(property.Key);
            }
        }
    }
}
