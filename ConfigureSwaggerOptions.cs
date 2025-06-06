using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Compellio.DCAP.Web
{
    public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
    {
        public static string ApiKeyName = "x-session-key";
        private readonly IApiVersionDescriptionProvider provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            this.provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateVersionInfo(description));
            }

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        }

        public void Configure(string name, SwaggerGenOptions options)
        {
            Configure(options);
        }

        private OpenApiInfo CreateVersionInfo(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = "IETF SATP Registry API",
                Version = description.ApiVersion.ToString(),
                Description = "This is a sample Registry API as defined in the Asset Profile Architecture RFC - IETF SATP. The API is based on the OpenAPI 3.0 specification",
                TermsOfService = new Uri("https://gateway.compell.io/registry/terms"),
                Contact = new OpenApiContact
                {
                    Name = "Contact the developer",
                    Email = "hello@compell.io",
                },
                License = new OpenApiLicense
                {
                    Name = "Example License",
                    Url = new Uri("https://gateway.compell.io/registry/licenses/license.html")
                },
            };

            if (description.IsDeprecated)
            {
                info.Description += " deprecated API.";
            }

            return info;
        }
    }
}
