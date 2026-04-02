using System.Data;
using FlyITA.Core.Abstractions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FlyITA.Infrastructure.Data;

public class DatabaseAccess : IDatabaseAccess
{
    private readonly string _connectionString;

    public DatabaseAccess(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");
    }

    public Dictionary<string, object?>? ExecuteStoredProcedure(string spName, Dictionary<string, object?> parameters)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(spName, connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        foreach (var param in parameters)
        {
            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
        }

        connection.Open();
        using var reader = command.ExecuteReader();

        if (!reader.Read())
        {
            return null;
        }

        var result = new Dictionary<string, object?>();
        for (var i = 0; i < reader.FieldCount; i++)
        {
            result[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
        }

        return result;
    }

    public int ExecuteNonQuery(string spName, Dictionary<string, object?> parameters)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(spName, connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        foreach (var param in parameters)
        {
            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
        }

        connection.Open();
        return command.ExecuteNonQuery();
    }
}
