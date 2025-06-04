// En DatabaseInitializer.cs
using Microsoft.Data.Sqlite;
using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms; // Asegúrate de tener este using para MessageBox

public static class DatabaseInitializer
{
    public static void InitializeDatabase()
    {
        string connectionString = ConfigurationManager.ConnectionStrings["SQLiteDB"].ConnectionString;
        string dbFilePath = string.Empty;
        var csBuilder = new SqliteConnectionStringBuilder(connectionString);
        dbFilePath = csBuilder.DataSource;

        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                // Asegúrate de que el nombre del archivo aquí coincida con tu archivo SQL
                string schemaFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schema.sql");

                if (File.Exists(schemaFilePath))
                {
                    string script = File.ReadAllText(schemaFilePath);
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = script;
                        command.ExecuteNonQuery();
                        // Console.WriteLine("Base de datos inicializada correctamente."); // Puedes dejarlo
                        MessageBox.Show("Script de inicialización de BD ejecutado SIN errores.", "Inicialización BD", MessageBoxButtons.OK, MessageBoxIcon.Information); // NUEVO
                    }
                }
                else
                {
                    // Console.WriteLine("El archivo de esquema no existe: " + schemaFilePath); // Puedes dejarlo
                    MessageBox.Show("¡ATENCIÓN! El archivo de esquema no existe: " + schemaFilePath, "Error de Esquema", MessageBoxButtons.OK, MessageBoxIcon.Error); // NUEVO
                }
            }
        }
        catch (Exception ex)
        {
            // Console.WriteLine("Error al inicializar la base de datos: " + ex.Message); // Puedes dejarlo
            MessageBox.Show("Error GRAVE al inicializar la base de datos: " + ex.Message, "Error Inicialización BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}