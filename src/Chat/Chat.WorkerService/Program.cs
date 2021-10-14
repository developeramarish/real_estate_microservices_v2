using Chat.Domain;
using Chat.Infrastructure.IRepositories.Interfaces;
using Chat.Infrastructure.Repositories;
using Chat.WorkerService.Services;
using Chat.WorkerService.Services.Interfaces;
using Common.Bootstrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.WorkerService
{
    public class Program
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
                                                .SetBasePath(Directory.GetCurrentDirectory())
                                                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                                .AddEnvironmentVariables()
                                                .Build();
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.RegisterRabbitMQPublisherHandler(hostContext.Configuration);

                    // Mongo
                    services.Configure<MongoConnectionAppSettings>(Configuration.GetSection("MongoConnectionString"));
                    services.AddScoped<IUserRepository, UserRepository>();
                    services.AddScoped<IChatRoomRepository, ChatRoomRepository>();
                    services.AddScoped<ICreateChatRoomService, CreateChatRoomService>();

                    services.AddHostedService<Worker>();
                    services.AddLogging(builder =>
                      builder.AddDebug()
                         .AddConsole()
                         .AddConfiguration(Configuration.GetSection("Logging"))
                         .SetMinimumLevel(LogLevel.Information));
                });
    }
}
