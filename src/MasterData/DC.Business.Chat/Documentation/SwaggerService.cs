using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DC.Business.Chat.Documentation
{
    internal sealed class SwaggerService
    {
        public static void Configure(IServiceCollection servicesContainer)
        {
            servicesContainer.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DC Server Api" });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);


                c.AddSecurityDefinition("Bearer",
                                                         new OpenApiSecurityScheme
                                                         {
                                                             Description = "JWT Authorization header using the Bearer scheme. Insert in value field the following: Bearer {token}",
                                                             Name = "Authorization"
                                                             //  In = "header",
                                                             //  Type = "apiKey"
                                                         });
                //  c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> { { "Bearer", Array.Empty<string>() } });
            });
        }
    }
}
