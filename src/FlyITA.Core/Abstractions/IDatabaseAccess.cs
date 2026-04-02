namespace FlyITA.Core.Abstractions;

public interface IDatabaseAccess
{
    Dictionary<string, object?>? ExecuteStoredProcedure(string spName, Dictionary<string, object?> parameters);
    int ExecuteNonQuery(string spName, Dictionary<string, object?> parameters);
    List<Dictionary<string, object?>> ExecuteStoredProcedureList(string spName, Dictionary<string, object?> parameters);
}
