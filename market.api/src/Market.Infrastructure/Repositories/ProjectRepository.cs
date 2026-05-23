using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Market.Domain.Interfaces;
using Market.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Market.Domain.Settings;
using Microsoft.Extensions.Options;
using Messaging.Abstractions;
using Messaging.Contracts.Events;

namespace Market.Infrastructure.Repositories
{
    public class ProjectRepository: IProjectRepository
    {
        private readonly string _connectionString;
        private readonly IEventBus _eventBus;

        public ProjectRepository(IOptions<ProjectSettings> options, IEventBus eventBus)
        {
            _connectionString = options.Value.ConnectionString;
            _eventBus = eventBus;
        }

        public async Task<Domain.Entities.Project> GetByIdAsync(Guid id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new NpgsqlCommand(@"
                SELECT ""Id"", ""Name"", ""Description"", ""UserCreatedId"", ""CreatedAt"", ""UpdatedAt"", ""Status""
                FROM ""Project""
                WHERE ""Id"" = @id AND ""DeletedAt"" IS NULL", connection);

            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapProject(reader);
            }

            return null;
        }

        public async Task<IEnumerable<Domain.Entities.Project>> GetAllAsync()
        {
            var projects = new List<Domain.Entities.Project>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new NpgsqlCommand(@"
                SELECT ""Id"", ""Name"", ""Description"", ""UserCreatedId"", ""CreatedAt"", ""UpdatedAt"", ""Status""
                FROM ""Project""
                WHERE ""DeletedAt"" IS NULL", connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                projects.Add(MapProject(reader));
            }

            return projects;
        }

        public async Task AddAsync(Domain.Entities.Project Project)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new NpgsqlCommand(@"
                INSERT INTO ""Project"" 
                (""Id"", ""Name"", ""Description"", ""UserCreatedId"", ""CreatedAt"", ""UpdatedAt"", ""Status"")
                VALUES
                (@id, @name, @description, @userCreatedId, @createdAt, @updatedAt, @status)", connection);

            command.Parameters.AddWithValue("@id", Project.Id);
            command.Parameters.AddWithValue("@name", Project.Name);
            command.Parameters.AddWithValue("@description", (object?)Project.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@userCreatedId", Project.UserCreatedId);
            command.Parameters.AddWithValue("@createdAt", Project.CreatedAt);
            command.Parameters.AddWithValue("@updatedAt", Project.UpdatedAt);
            command.Parameters.AddWithValue("@status", (int)Project.Status);

            await command.ExecuteNonQueryAsync();

            var item = new Project_Created_Event(Project.Id, new Guid(), Project.Name, Project.Status.ToString());

            await _eventBus.PublishAsync(item);
        }

        public async Task UpdateAsync(Domain.Entities.Project Project)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new NpgsqlCommand(@"
                UPDATE ""Project""
                SET ""Name"" = @name,
                    ""Description"" = @description,
                    ""Status"" = @status,
                    ""UpdatedAt"" = @updatedAt
                WHERE ""Id"" = @id", connection);

            command.Parameters.AddWithValue("@id", Project.Id);
            command.Parameters.AddWithValue("@name", Project.Name);
            command.Parameters.AddWithValue("@description", (object?)Project.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@updatedAt", Project.UpdatedAt);
            command.Parameters.AddWithValue("@status", (int)Project.Status);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new NpgsqlCommand(@"
                UPDATE ""Project""
                SET ""DeletedAt"" = @deletedAt
                WHERE ""Id"" = @id", connection);

            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@deletedAt", DateTime.UtcNow);

            await command.ExecuteNonQueryAsync();
        }

        private Domain.Entities.Project MapProject(NpgsqlDataReader reader)
        {
            return new Domain.Entities.Project
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                UserCreatedId = reader.GetGuid(reader.GetOrdinal("UserCreatedId")),
                Status = (Market.Domain.Enums.StatusEnum)reader.GetInt32(reader.GetOrdinal("Status")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                // Members: não populado aqui, depende de join ou consulta separada
            };
        }
    }
}