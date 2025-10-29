using Microsoft.OpenApi.Models;

namespace App.ServiceExtensions {
    public static class SwaggerGenExtension {
        public static void ConfigureSwagger(this IServiceCollection services) {
            services.AddSwaggerGen(opt => {
                opt.SwaggerDoc("v1", new OpenApiInfo {
                    Version = "v1",
                    Title = "SoftwaresMarket.Api",
                    Description = "Rest Api for Softwares Market",
                    Contact = new OpenApiContact(),
                    License = new OpenApiLicense(),
                    TermsOfService = new Uri("https://www.arogroup.ru/")
                });
            });
        }
    }
}
