using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace Sistema_de_Ventas
{
    class Conexion
    {
        // Cadena de conexión
        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";

        // Método para obtener una conexión
        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(connectionString);
        }

        // Metodo para cargar datos en un DataTable
        public DataTable ObtenerDatos(string query)
        {
            DataTable dt = new DataTable(); // Inicializa el DataTable aquí

            try
            {
                using (SqlConnection conn = ObtenerConexion())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Puedes registrar el error y devolver un DataTable vacío
                MessageBox.Show($"Error al obtener datos: {ex.Message}");
            }

            return dt; // El método siempre devolverá un DataTable, aunque esté vacío
        }

    }
}
