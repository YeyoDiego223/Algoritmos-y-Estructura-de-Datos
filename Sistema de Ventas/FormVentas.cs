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
    
    public partial class FormVentas : Form
    {
        string fecha, cliente, total;
        int posicion;
        int i = 1;
        public FormVentas()
        {
            InitializeComponent();
            llenarDataGridView();
            txtFecha.Focus();
        }

        private void actualizarbase()
        {

        }

        private void llenarDataGridView()
        {
            // Cadena de conexion
            string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";

            // Consulta SQL
            string query = @"
            SELECT 
                v.ID_Venta,
                v.Fecha_Venta,
                c.Nombre,
                v.Total_Venta
            FROM Ventas v
            INNER JOIN Clientes c
            ON v.ID_Cliente = c.ID_Cliente";

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
                            dgvDetalle.Rows[rowIndex].Cells["colCodigo"].Value = reader["ID_Venta"];
                            dgvDetalle.Rows[rowIndex].Cells["colFecha"].Value = reader["Fecha_Venta"];
                            dgvDetalle.Rows[rowIndex].Cells["colCliente"].Value = reader["Nombre"];
                            dgvDetalle.Rows[rowIndex].Cells["colTotal"].Value = reader["Total_Venta"];
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
            btnEliminar.Enabled = false;
            btnModificar.Enabled = false;
            txtFecha.Text = "";
            txtCliente.Text = "";
            txtTotal.Text = "";
        }
        private void pictureBox7_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormCompras compras = new FormCompras();
            compras.Show();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormProveedores proveedores = new FormProveedores();
            proveedores.Show();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormDetalles detalles = new FormDetalles();
            detalles.Show();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            fecha = txtFecha.Text;
            cliente = txtCliente.Text;
            total = txtTotal.Text;
            if (fecha == "" || cliente == "" || total == "")
            {
                MessageBox.Show("No hay datos en algunos textos");
            }
            else
            {
                dgvDetalle.Rows.Add(i + "", fecha, cliente, total);
                i = i + 1;
                
                limpiar();
                txtFecha.Focus();
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            fecha = txtFecha.Text;
            cliente = txtCliente.Text;
            total = txtTotal.Text;
            dgvDetalle[1, posicion].Value = txtFecha.Text;
            dgvDetalle[2, posicion].Value = txtCliente.Text;
            dgvDetalle[3, posicion].Value = txtTotal.Text;
            var colCodigo = dgvDetalle[0, posicion].Value.ToString();
            actualizarbase();
            limpiar();
            txtFecha.Focus();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            dgvDetalle.Rows.RemoveAt(posicion);
            txtFecha.Focus();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            limpiar();
            btnAgregar.Enabled = true;
            txtFecha.Focus();
        }

        private void dgvDetalle_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void txtProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                txtCliente.Focus(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtPrecio_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                txtTotal.Focus(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtCantidad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                btnAgregar.PerformClick(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void dgvDetalle_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            posicion = dgvDetalle.CurrentRow.Index;
            txtFecha.Text = dgvDetalle[1, posicion].Value.ToString();
            txtCliente.Text = dgvDetalle[2, posicion].Value.ToString();
            txtTotal.Text = dgvDetalle[3, posicion].Value.ToString();
            btnAgregar.Enabled = false;
            btnModificar.Enabled = true;
            btnEliminar.Enabled = true;
            txtFecha.Focus();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            posicion = dgvDetalle.CurrentRow.Index;
            txtFecha.Text = dgvDetalle[1, posicion].Value.ToString();
            txtCliente.Text = dgvDetalle[2, posicion].Value.ToString();
            txtTotal.Text = dgvDetalle[3, posicion].Value.ToString();
            btnAgregar.Enabled = false;
            btnModificar.Enabled = true;
            btnEliminar.Enabled = true;
            txtFecha.Focus();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormProductos configuracion = new FormProductos();
            configuracion.Show();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormPrincipal principal = new FormPrincipal();
            principal.Show();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormClientes clientes = new FormClientes();
            clientes.Show();
        }

        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Verifica que no sea el encabezado
            {
                DataGridViewRow filaSeleccionada = dgvDetalle.Rows[e.RowIndex];
                if (filaSeleccionada.Cells[0].Value != null && filaSeleccionada.Cells[1].Value != null)
                {
                    posicion = dgvDetalle.CurrentRow.Index;
                    txtFecha.Text = dgvDetalle[1, posicion].Value.ToString();
                    txtCliente.Text = dgvDetalle[2, posicion].Value.ToString();
                    txtTotal.Text = dgvDetalle[3, posicion].Value.ToString();
                    btnAgregar.Enabled = false;
                    btnModificar.Enabled = true;
                    btnEliminar.Enabled = true;
                    txtFecha.Focus();
                }
                else
                {
                    MessageBox.Show("La fila seleccionada contiene celdas vacías.");
                }

            }   
        }
    }
}
