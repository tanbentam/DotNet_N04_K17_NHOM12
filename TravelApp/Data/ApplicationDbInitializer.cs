using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using TravelApp.Data.Migrations;

namespace TravelApp.Data
{
    internal sealed class ApplicationDbInitializer :
        IDatabaseInitializer<ApplicationDbContext>
    {
        private const string ContextKey =
            "TravelApp.Data.ApplicationDbContext";
        private const string MigrationId =
            "202606121648265_AddGuideCancellationRequests";

        private readonly MigrateDatabaseToLatestVersion<
            ApplicationDbContext,
            Configuration> _migrationInitializer =
                new MigrateDatabaseToLatestVersion<
                    ApplicationDbContext,
                    Configuration>();

        public void InitializeDatabase(ApplicationDbContext context)
        {
            RepairGuideCancellationMigrationHistory(context);
            _migrationInitializer.InitializeDatabase(context);
        }

        private static void RepairGuideCancellationMigrationHistory(
            ApplicationDbContext context)
        {
            var connection = context.Database.Connection;
            var shouldClose = connection.State == ConnectionState.Closed;

            try
            {
                if (shouldClose)
                {
                    connection.Open();
                }

                if (!HasCompleteGuideCancellationSchema(connection) ||
                    !HasMigrationHistoryTable(connection) ||
                    HasMigrationHistoryRecord(connection))
                {
                    return;
                }

                var metadata = (IMigrationMetadata)
                    new AddGuideCancellationRequests();
                var targetModel = Convert.FromBase64String(metadata.Target);

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
INSERT INTO `__MigrationHistory`
    (`MigrationId`, `ContextKey`, `Model`, `ProductVersion`)
SELECT
    @migrationId,
    @contextKey,
    @model,
    @productVersion
WHERE NOT EXISTS (
    SELECT 1
    FROM `__MigrationHistory`
    WHERE `MigrationId` = @migrationId
      AND `ContextKey` = @contextKey
);";

                    AddParameter(command, "@migrationId", MigrationId);
                    AddParameter(command, "@contextKey", ContextKey);
                    AddParameter(command, "@model", targetModel);
                    AddParameter(command, "@productVersion", "6.5.2");
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (shouldClose && connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        private static bool HasCompleteGuideCancellationSchema(
            IDbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT COUNT(DISTINCT `COLUMN_NAME`)
FROM `INFORMATION_SCHEMA`.`COLUMNS`
WHERE `TABLE_SCHEMA` = DATABASE()
  AND `TABLE_NAME` = 'Bookings'
  AND `COLUMN_NAME` IN (
      'GuideCancellationRequestedAt',
      'GuideCancellationReason',
      'GuideCancellationResolvedAt',
      'GuideCancellationApproved'
  );";

                return Convert.ToInt32(command.ExecuteScalar()) == 4;
            }
        }

        private static bool HasMigrationHistoryTable(IDbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT COUNT(*)
FROM `INFORMATION_SCHEMA`.`TABLES`
WHERE `TABLE_SCHEMA` = DATABASE()
  AND `TABLE_NAME` = '__MigrationHistory';";

                return Convert.ToInt32(command.ExecuteScalar()) == 1;
            }
        }

        private static bool HasMigrationHistoryRecord(IDbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT COUNT(*)
FROM `__MigrationHistory`
WHERE `MigrationId` = @migrationId
  AND `ContextKey` = @contextKey;";

                AddParameter(command, "@migrationId", MigrationId);
                AddParameter(command, "@contextKey", ContextKey);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        private static void AddParameter(
            IDbCommand command,
            string name,
            object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }
    }
}
