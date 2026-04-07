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
        _connectionString = configuration.GetConnectionString("Default") ?? string.Empty;
    }

    public Dictionary<string, object?>? ExecuteStoredProcedure(string spName, Dictionary<string, object?> parameters)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(spName, connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        AddParameters(command, parameters);

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

        AddParameters(command, parameters);

        connection.Open();
        return command.ExecuteNonQuery();
    }

    public List<Dictionary<string, object?>> ExecuteStoredProcedureList(string spName, Dictionary<string, object?> parameters)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(spName, connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        AddParameters(command, parameters);

        connection.Open();
        using var reader = command.ExecuteReader();

        var results = new List<Dictionary<string, object?>>();
        while (reader.Read())
        {
            var row = new Dictionary<string, object?>();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            }
            results.Add(row);
        }

        return results;
    }

    private static void AddParameters(SqlCommand command, Dictionary<string, object?> parameters)
    {
        foreach (var param in parameters)
        {
            var sqlParam = new SqlParameter
            {
                ParameterName = param.Key,
                Value = param.Value ?? DBNull.Value
            };

            // Set explicit SqlDbType for common types to avoid AddWithValue inference issues
            if (param.Value is string s)
            {
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                sqlParam.Size = Math.Max(s.Length, 1);
            }
            else if (param.Value is int)
            {
                sqlParam.SqlDbType = SqlDbType.Int;
            }
            else if (param.Value is long)
            {
                sqlParam.SqlDbType = SqlDbType.BigInt;
            }
            else if (param.Value is bool)
            {
                sqlParam.SqlDbType = SqlDbType.Bit;
            }
            else if (param.Value is DateTime)
            {
                sqlParam.SqlDbType = SqlDbType.DateTime2;
            }
            else if (param.Value is decimal)
            {
                sqlParam.SqlDbType = SqlDbType.Decimal;
            }

            command.Parameters.Add(sqlParam);
        }
    }
}
