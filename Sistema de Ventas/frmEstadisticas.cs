using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Windows.Forms.DataVisualization.Charting;

namespace Sistema_de_Ventas
{
    public partial class frmEstadisticas : Form
    {
        public frmEstadisticas()
        {
            InitializeComponent();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            chartVentas.Series["Series1"].ChartType = SeriesChartType.Column;
            chartVentas.ChartAreas[0].AxisX.Title = "Mes";
            chartVentas.ChartAreas[0].AxisY.Title = "Ingresos ($)";
        }

        string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";

        private void CargarEstadisticas()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // 1. Obtener resumen (Total Ventas e Ingreso Total)
                string queryResumen = "SELECT COUNT(*) AS TotalVentas, SUM(Total_Venta) AS IngresoTotal FROM Ventas";
                using (SqlCommand command = new SqlCommand(queryResumen, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        lblTotalVentas.Text = $"Total de Ventas: {reader["TotalVentas"]}";
                        lblIngresoTotal.Text = $"Ingreso Total: ${reader["IngresoTotal"]}";
                    }
                    reader.Close();
                }

                // 2. Obtener ventas por mes y año
                string queryVentasPorMes = @"
                SELECT 
                    FORMAT(Fecha_Venta, 'yyyy-MM') AS MesAnio, 
                    SUM(Total_Venta) AS VentasMensuales
                FROM Ventas
                GROUP BY FORMAT(Fecha_Venta, 'yyyy-MM')
                ORDER BY MesAnio";
                using (SqlCommand command = new SqlCommand(queryVentasPorMes, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    chartVentas.Series["Series1"].Points.Clear();
                    while (reader.Read())
                    {
                        string mesAnio = reader["MesAnio"].ToString(); // Ejemplo: "2024-10"
                        decimal total = Convert.ToDecimal(reader["VentasMensuales"]);
                        chartVentas.Series["Series1"].Points.AddXY(mesAnio, total);
                    }
                    reader.Close();
                }


                // 3. Obtener detalles de ventas
                string queryDetalles = "SELECT ID_Venta, Fecha_Venta, Total_Venta FROM Ventas ORDER BY Fecha_Venta DESC";
                using (SqlCommand command = new SqlCommand(queryDetalles, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridViewDetalles.DataSource = dt;
                }
            }
        }

        private void pctbxProductos_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormProductos productos = new FormProductos();
            productos.Show();
        }

        private void pctbxProveedores_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormProveedores proveedores = new FormProveedores();
            proveedores.Show();
        }

        private void pctbxClientes_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormClientes clientes = new FormClientes();
            clientes.Show();
        }

        private void pctbxVentas_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormVentas ventas = new FormVentas();
            ventas.Show();
        }

        private void pctbxCompras_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormCompras compras = new FormCompras();
            compras.Show();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormPrincipal principal = new FormPrincipal();
            principal.Show();
        }

        private void frmEstadisticas_Load(object sender, EventArgs e)
        {
            CargarEstadisticas();
        }

        private void chartVentas_Click(object sender, EventArgs e)
        {

        }
    }
}
