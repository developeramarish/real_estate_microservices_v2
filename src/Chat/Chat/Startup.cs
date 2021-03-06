using Chat.Chat;
using Chat.Domain;
using Chat.Hubs;
using Chat.Infrastructure.IRepositories.Interfaces;
using Chat.Infrastructure.Repositories;
using Common.Entities;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSignalR();
            services.AddSingleton<UserInfoInMemory>();
            //services.AddMediatR(typeof(Startup));
            var assembly = AppDomain.CurrentDomain.Load("Chat.Business");
            services.AddMediatR(assembly);
            
            services.AddCors(options =>
            {
                options.AddPolicy("AllowMyOrigins",
                    builder =>
                    {
                        builder
                         .AllowAnyHeader()
                         .AllowAnyMethod()
                         //.AllowAnyOrigin()
                         .AllowCredentials()
                        //.AllowCredentials()
                        //.AllowAnyHeader()
                        //.SetIsOriginAllowedToAllowWildcardSubdomains()
                        //.AllowAnyMethod()
                         .WithOrigins("http://localhost:4200", "https://localhost:4200");
                    });
            });

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Path.Value.StartsWith("/chat")
                            && context.Request.Query.TryGetValue("token", out StringValues token)
                        )
                        {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        var te = context.Exception;
                        return Task.CompletedTask;
                    }
                };
            });

            // Mongo
            services.Configure<MongoConnectionAppSettings>(Configuration.GetSection("MongoConnectionString"));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IChatRoomRepository, ChatRoomRepository>();

            // SwaggerService.Configure(services);

            services.AddSwaggerGen(c =>
            {
                // add JWT Authentication
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // must be lower case
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {securityScheme, new string[] { }}
                });

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIs", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "APIs");
                c.RoutePrefix = string.Empty;
            });

            //app.UseHttpsRedirection();
            app.UseCors("AllowMyOrigins");

            //app.UseCors(options =>
            //{
            //    // options.WithOrigins("http://localhost:4200");
            //    options.AllowAnyHeader();
            //    options.AllowAnyMethod();
            //    options.AllowAnyOrigin();
            //});

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    //c.SwaggerEndpoint("/swagger/v1/swagger.json", "APIs");
            //    c.SwaggerEndpoint(this.Configuration["VirtualDirectory"] + "../swagger/v1/swagger.json", "DC Server Api");
            //    c.RoutePrefix = "api";
            //    c.DefaultModelsExpandDepth(-1);
            //    //c.DocExpansion(DocExpansion.None);
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MainHub>("/chat");
            });
        }
    }
}
