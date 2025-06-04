using System;
using System.Configuration;
using Microsoft.Data.Sqlite;
public class DatabaseConnector
{
    public static SqliteConnection GetConnection()
    {
        // Leer configuraciones desde app.config
        string[] ips = ConfigurationManager.AppSettings["BackupIPs"].Split(',');
        string database = ConfigurationManager.AppSettings["Database"];
        string user = ConfigurationManager.AppSettings["User"];
        string password = ConfigurationManager.AppSettings["Password"];

        SqliteConnection connection = null;

        foreach (string ip in ips)
        {
            string connectionString = $"Server={ip};Database={database};User Id={user};Password={password};Encrypt=False;TrustServerCertificate=True;";

            try
            {
                connection = new SqliteConnection(connectionString);
                connection.Open(); // Intenta abrir la conexión
                return connection; // Si es exitosa, retorna la conexión
            }
            catch (SqliteException)
            {
                // Si falla, cierra la conexión y prueba la siguiente IP
                connection?.Dispose();
            }
        }

        // Si todas las IPs fallan, lanza una excepción
        throw new InvalidOperationException("No se pudo conectar a ninguna IP configurada.");
    }
}