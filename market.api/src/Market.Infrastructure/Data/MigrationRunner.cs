using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;


namespace Market.Infrastructure.Data
{
    public class MigrationRunner
{
    private readonly string _connectionString;
    private readonly string _migrationsFolder;

    public MigrationRunner(string connectionString, string migrationsFolder)
    {
        _connectionString = connectionString;
        _migrationsFolder = migrationsFolder;
    }

    public async Task RunAsync()
    {
        await EnsureMigrationsTableAsync();

        var scripts = Directory.GetFiles(_migrationsFolder, "*.sql")
                                .OrderBy(f => f)
                                .ToList();

        var applied = await GetAppliedMigrationsAsync();

        foreach (var scriptPath in scripts)
        {
            var scriptName = Path.GetFileName(scriptPath);

            if (applied.Contains(scriptName))
            {
                Console.WriteLine($"✔ Migration {scriptName} already applied.");
                continue;
            }

            Console.WriteLine($"Running {scriptName} ...");
            var sql = await File.ReadAllTextAsync(scriptPath);

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var command = new NpgsqlCommand(sql, connection, transaction);
                await command.ExecuteNonQueryAsync();

                var insert = new NpgsqlCommand("INSERT INTO \"_Migrations\" (\"MigrationName\") VALUES (@name)", connection, transaction);
                insert.Parameters.AddWithValue("@name", scriptName);
                await insert.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
                Console.WriteLine($"{scriptName} applied successfully!");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error in {scriptName}: {ex.Message}");
                throw;
            }
        }

        Console.WriteLine("All migrations are up to date.");
    }

    private async Task EnsureMigrationsTableAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var command = new NpgsqlCommand(@"
            CREATE TABLE IF NOT EXISTS ""_Migrations"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""MigrationName"" VARCHAR(200) NOT NULL,
                ""AppliedOn"" TIMESTAMP NOT NULL DEFAULT NOW()
            );", connection);

        await command.ExecuteNonQueryAsync();
    }

    private async Task<HashSet<string>> GetAppliedMigrationsAsync()
    {
        var applied = new HashSet<string>();

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var command = new NpgsqlCommand("SELECT \"MigrationName\" FROM \"_Migrations\"", connection);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            applied.Add(reader.GetString(0));
        }

        return applied;
    }
}
}