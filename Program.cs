using Asp.Versioning;
using System.Text.Json.Serialization;
using Compellio.DCAP.Web.RestApi.Models.V1;

namespace Compellio.DCAP.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Configuration.AddEnvironmentVariables();
            TAR.ChainId = builder.Configuration.GetSection("DCAP").GetValue<string>("BlockchainId");
            TAR.RPCEndPoint = builder.Configuration.GetSection("DCAP").GetValue<string>("BlockchainUrl");
            TAR.PrivateKey = builder.Configuration.GetSection("DCAP").GetValue<string>("PrivateKey");
            TAR.StoragePath = builder.Configuration.GetSection("DCAP").GetValue<string>("StoragePath");
            TAR.UriPrefix = builder.Configuration.GetSection("DCAP").GetValue<string>("UriPrefix");

            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(2, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            builder.Services.AddControllers()
                .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowAny",
                    x =>
                    {
                        x.AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed(isOriginAllowed: _ => true)
                        .AllowCredentials();
                    });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseStaticFiles();
                app.UseCors("AllowAny");

                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.ConfigObject.AdditionalItems.Add("x-session-key", "x-session-key");
                    var descriptions = app.DescribeApiVersions();

                    foreach (var description in descriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                });
            }

            app.UseHttpsRedirection();
            app.MapControllers();
            app.Run();
        }
    }
}
