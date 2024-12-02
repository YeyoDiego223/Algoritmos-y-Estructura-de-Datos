using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Sistema_de_Ventas
{
    public partial class FormCompras : Form
    {
        int i = 1;
        int posicion;
        string proveedor, fecha, total;
        public FormCompras()
        {
            InitializeComponent();
            cbxProveedor.DropDownStyle = ComboBoxStyle.DropDownList;
            llenarDataGridView();
            cbxProveedor.Focus();
        }

        void modificarbase()
        {

            var idCompra = dgvDetalle[0, posicion].Value.ToString();
            fecha = dtpFecha.Text;
            total = txtTotal.Text;
            proveedor = cbxProveedor.Text;

            string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
            string getIdProveedorQuery = "SELECT ID_Proveedor FROM Proveedores WHERE Nombre_Proveedor = @nombreProveedor";
            int idProveedor;

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(getIdProveedorQuery, conn))
            {
                cmd.Parameters.AddWithValue("@nombreProveedor", proveedor);

                try
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Proveedor no encontrado");
                        return;
                    }
                    idProveedor = (int)result;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al obtener ID del proveedor: {ex.Message}");
                    return;
                }
            }

            // Actualizar la tabla de compras
            string updateQuery = "UPDATE Compras SET ID_Proveedor = @idProveedor, Fecha_Compra = @fechaCompra, Total_Compra = @totalCompra WHERE ID_Compra = @idCompra";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
            {
                cmd.Parameters.AddWithValue("@idProveedor", proveedor);
                cmd.Parameters.AddWithValue("@fechaCompra", fecha);
                cmd.Parameters.AddWithValue("@totalCompra", total);
                cmd.Parameters.AddWithValue("@idCompra", idCompra);

                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if(rowsAffected > 0)
                    {
                        MessageBox.Show("Registro modificado correctamente.");
                        llenarDataGridView();
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el registro para modificar.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al modificar: {ex.Message}");
                }
            }
        }



        private void llenarDataGridView()
        {
            // Cadena de conexion
            string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";

            // Consulta SQL
            string query = @"
            SELECT 
                c.ID_Compra, 
                p.Nombre_Proveedor,
                c.Fecha_Compra,
                c.Total_Compra
            FROM Compras c
            INNER JOIN Proveedores p 
            ON c.ID_Proveedor = p.ID_Proveedor";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Crear una nueva fila y asignar los valores manualmente
                            int rowIndex = dgvDetalle.Rows.Add();
                            dgvDetalle.Rows[rowIndex].Cells["colCodigo"].Value = reader["ID_Compra"];
                            dgvDetalle.Rows[rowIndex].Cells["colProveedor"].Value = reader["Nombre_Proveedor"];
                            dgvDetalle.Rows[rowIndex].Cells["colFecha"].Value = reader["Fecha_Compra"];
                            dgvDetalle.Rows[rowIndex].Cells["colTotal"].Value = reader["Total_Compra"];
                            i = i + 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al llenar el DataGridView: {ex.Message}");
                }
            }
        }

        void limpiar()
        {
            btnAgregar.Enabled = true;
            btnEliminar.Enabled = false;
            btnModificar.Enabled = false;
            cbxProveedor.Text = "";
            dtpFecha.Text = "";
            txtTotal.Text = "";
        }
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormProveedores proveedores = new FormProveedores();
            proveedores.Show();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormVentas ventas = new FormVentas();
            ventas.Show();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormDetalles detalles = new FormDetalles();
            detalles.Show();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormProductos configuracion = new FormProductos();
            configuracion.Show();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormClientes clientes = new FormClientes();
            clientes.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            proveedor = cbxProveedor.Text;
            fecha = dtpFecha.Text;
            total = txtTotal.Text;
            if (proveedor == "" || fecha == "" || total == "")
            {
                MessageBox.Show("No hay datos");
            }
            else
            {
                dgvDetalle.Rows.Add(i + "", proveedor, fecha, total);
                i = i + 1;
                limpiar();
                cbxProveedor.Focus();
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormPrincipal principal = new FormPrincipal();
            principal.Show();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            limpiar();
            btnAgregar.Enabled = true;
            cbxProveedor.Focus();
        }

        private void dgvDetalle_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Verifica que no sea el encabezado
            {
                
                // Obtener la fila seleccionada
                DataGridViewRow row = dgvDetalle.Rows[e.RowIndex];
                if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                {
                    // Llenar los TextBox con los valores correspondientes
                    dtpFecha.Text = row.Cells["colFecha"].Value?.ToString();
                    txtTotal.Text = row.Cells["colTotal"].Value?.ToString();

                    // Obtener el nombre del proveedor
                    proveedor = row.Cells["colProveedor"].Value?.ToString();

                    // Verificar si el ComboBox contiene el proveedor
                    if (cbxProveedor.Items.Contains(proveedor))
                    {
                        cbxProveedor.Text = proveedor;
                    }
                    else
                    {
                        // Si no esta en la lista, agregarlo y selecionarlo
                        cbxProveedor.Items.Add(proveedor);
                        cbxProveedor.Text = proveedor;
                    }
                    btnAgregar.Enabled = false;
                    btnModificar.Enabled = true;
                    btnEliminar.Enabled = true;
                    cbxProveedor.Focus();
                }
                else
                {
                    MessageBox.Show("La fila seleccionada contiene celdas vacías.");
                }

            }
        }

        private void cbxProveedor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                dtpFecha.Focus(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void dtpFecha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                txtTotal.Focus(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtTotal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                btnAgregar.PerformClick(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void dgvDetalle_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            dgvDetalle.Rows.RemoveAt(posicion);
            cbxProveedor.Focus();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            proveedor = cbxProveedor.Text;
            fecha = dtpFecha.Text;
            total = txtTotal.Text;

            dgvDetalle[1, posicion].Value = cbxProveedor.Text;
            dgvDetalle[2, posicion].Value = dtpFecha.Text;
            dgvDetalle[3, posicion].Value = txtTotal.Text;
            modificarbase();
            limpiar();
            cbxProveedor.Focus();
        }
    }
}
