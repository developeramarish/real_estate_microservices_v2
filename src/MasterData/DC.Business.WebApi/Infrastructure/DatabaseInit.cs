//using DbUp;
using DbUp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;

namespace DC.Business.WebApi.Infrastructure
{
    public class DatabaseInit : IStartupFilter
    {
        private readonly DatabaseConfig _config;
        private readonly DbLogger<DatabaseInit> _logger;

        public DatabaseInit(DatabaseConfig config, DbLogger<DatabaseInit> logger)
        {
            _config = config;
            _logger = logger;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            var connectionString = _config.ConnectionString;

            EnsureDatabase.For.MySqlDatabase(connectionString);

            var dbUpgradeEngineBuilder = DeployChanges.To
                .MySqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(typeof(Program).Assembly)
                .WithTransaction()
                .LogTo(_logger);

            var dbUpgradeEngine = dbUpgradeEngineBuilder.Build();
            if (dbUpgradeEngine.IsUpgradeRequired())
            {
                _logger.WriteInformation("Upgrades have been detected. Upgrading database now...");
                var operation = dbUpgradeEngine.PerformUpgrade();
                if (operation.Successful)
                {
                    _logger.WriteInformation("Upgrade completed successfully");
                }

                _logger.WriteInformation("Error happened in the upgrade. Please check the logs");
            }

            return next;
        }
    }
}
