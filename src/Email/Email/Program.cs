using Common.Bootstrap;
using Email.Interfaces;
using Email.SendGrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Email
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
                    // configure strongly typed settings objects
                    var appSettingsSection = Configuration.GetSection("SendGrid");
                    services.Configure<Models.SendGrid>(appSettingsSection);

                    services.RegisterRabbitMQPublisherHandler(hostContext.Configuration);

                    var appSettings = appSettingsSection.Get<Models.SendGrid>();
                    var _sendGridKey = appSettings.ApiKey;

                    services.AddTransient<ISendGridService, SendGridService>(x => new SendGridService(_sendGridKey));

                    services.AddHostedService<EmailManager>();
                    services.AddLogging(builder =>
                    builder.AddDebug()
                       .AddConsole()
                       .AddConfiguration(Configuration.GetSection("Logging"))
                       .SetMinimumLevel(LogLevel.Information));
                });
    }
}
