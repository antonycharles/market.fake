using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using Market.Domain.Entities;
using Market.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Market.Domain.Enums;
using Market.Domain.Settings;
using Microsoft.Extensions.Options;

namespace Market.Infrastructure.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly string _connectionString;

        public MemberRepository(IOptions<ProjectSettings> options)
        {
            _connectionString = options.Value.ConnectionString;
        }

        public async Task<Member> GetByIdAsync(Guid id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new NpgsqlCommand(@"
                SELECT ""Id"", ""UserId"", ""ProjectId"", ""CreatedAt"", ""UpdatedAt"", ""Status""
                FROM ""Member""
                WHERE ""Id"" = @id AND ""DeletedAt"" IS NULL", connection);

            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapMember(reader);
            }

            return null;
        }

        public async Task<IEnumerable<Member>> GetByProjectIdAsync(Guid projectId)
        {
            var members = new List<Member>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new NpgsqlCommand(@"
                SELECT ""Id"", ""UserId"", ""ProjectId"", ""CreatedAt"", ""UpdatedAt"", ""Status""
                FROM ""Member""
                WHERE ""ProjectId"" = @projectId AND ""DeletedAt"" IS NULL", connection);

            command.Parameters.AddWithValue("@projectId", projectId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                members.Add(MapMember(reader));
            }

            return members;
        }

        public async Task AddAsync(Member member)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new NpgsqlCommand(@"
                INSERT INTO ""Member"" 
                (""Id"", ""UserId"", ""ProjectId"", ""CreatedAt"", ""UpdatedAt"", ""Status"")
                VALUES
                (@id, @userId, @projectId, @createdAt, @updatedAt, @status)", connection);

            command.Parameters.AddWithValue("@id", member.Id);
            command.Parameters.AddWithValue("@userId", member.UserId);
            command.Parameters.AddWithValue("@projectId", member.ProjectId);
            command.Parameters.AddWithValue("@createdAt", member.CreatedAt);
            command.Parameters.AddWithValue("@updatedAt", member.UpdatedAt);
            command.Parameters.AddWithValue("@status", (int)StatusEnum.Active);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new NpgsqlCommand(@"
                UPDATE ""Member""
                SET ""DeletedAt"" = @deletedAt
                WHERE ""Id"" = @id", connection);

            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@deletedAt", DateTime.UtcNow);

            await command.ExecuteNonQueryAsync();
        }

        private Member MapMember(NpgsqlDataReader reader)
        {
            return new Member
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                ProjectId = reader.GetGuid(reader.GetOrdinal("ProjectId")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                Status = (StatusEnum)reader.GetInt32(reader.GetOrdinal("Status"))
            };
        }
    }
}