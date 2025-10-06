using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace DataProcessorService;

public class DataProcessor : IDisposable
{
    private readonly string _connectionString;
    private SqliteConnection? _connection;

    public DataProcessor(string connectionString)
    {
        _connectionString = connectionString
            ?? throw new InvalidOperationException("SQLite connection string not found.");
    }

    /// <summary>
    /// Database initialization and connection
    /// </summary>
    public void Init()
    {
        // SqliteOpenMode.ReadWriteCreate by default
        _connection = new SqliteConnection(_connectionString);
        _connection.Open();

        // Create simple table if need
        var createTableCmd = _connection.CreateCommand();
        createTableCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Status (
                ModuleCategoryID TEXT PRIMARY KEY,
                ModuleState TEXT NOT NULL
            );
        ";
        createTableCmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Add or Update new data record
    /// </summary>
    public int WriteData(string moduleCategoryId, string moduleState)
    {
        if (_connection == null)
            throw new InvalidOperationException("The database connection is not initialized. Call Init()");

        using var cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Status (ModuleCategoryID, ModuleState)
            VALUES ($id, $state)
            ON CONFLICT(ModuleCategoryID)
            DO UPDATE SET ModuleState = $state;
        ";
        cmd.Parameters.AddWithValue("$id", moduleCategoryId);
        cmd.Parameters.AddWithValue("$state", moduleState);
        
        return cmd.ExecuteNonQuery();
    }

    public void Dispose()
    {
        _connection?.Close();
        _connection?.Dispose();
    }
}