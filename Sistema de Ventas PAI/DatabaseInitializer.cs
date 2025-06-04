using Microsoft.Data.Sqlite;
using System.Configuration;
using System.IO;

public static class DatabaseInitializer
{
    public static void InitializeDatabase()
    {
        string connectionString = ConfigurationManager.ConnectionStrings["SQLiteDB"].ConnectionString;
        string dbPath = connectionString.Split('=')[1].Split(';')[0].Trim();

        if (!File.Exists(dbPath))
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string script = File.ReadAllText("schema.sql");
                SqliteCommand command = new SqliteCommand(script, connection);
                command.ExecuteNonQuery();
            }
        }
    }
}