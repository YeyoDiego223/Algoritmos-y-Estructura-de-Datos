﻿using System;
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
        string producto, precio, cantidad;
        int posicion;
        int i = 1;
        public FormVentas()
        {
            InitializeComponent();
            llenarDataGridView();
            txtProducto.Focus();
        }

        private void actualizarbase()
        {

        }

        private void llenarDataGridView()
        {
            // Cadena de conexion
            string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";

            // Consulta SQL
            string query = "Select ID_Venta, Fecha_Venta, ID_Cliente, Total_Venta FROM Ventas";

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
                            dgvDetalle.Rows[rowIndex].Cells["colProducto"].Value = reader["Fecha_Venta"];
                            dgvDetalle.Rows[rowIndex].Cells["colPrecio"].Value = reader["ID_Cliente"];
                            dgvDetalle.Rows[rowIndex].Cells["colCantidad"].Value = reader["Total_Venta"];
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
            txtProducto.Text = "";
            txtPrecio.Text = "";
            txtCantidad.Text = "";
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
            producto = txtProducto.Text;
            precio = txtPrecio.Text;
            cantidad = txtCantidad.Text;
            if (producto == "" || precio == "" || cantidad == "")
            {
                MessageBox.Show("No hay datos en algunos textos");
            }
            else
            {
                dgvDetalle.Rows.Add(i + "", producto, precio, cantidad);
                i = i + 1;
                
                limpiar();
                txtProducto.Focus();
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            producto = txtProducto.Text;
            precio = txtPrecio.Text;
            cantidad = txtCantidad.Text;
            dgvDetalle[1, posicion].Value = txtProducto.Text;
            dgvDetalle[2, posicion].Value = txtPrecio.Text;
            dgvDetalle[3, posicion].Value = txtCantidad.Text;
            var colCodigo = dgvDetalle[0, posicion].Value.ToString();
            actualizarbase();
            limpiar();
            txtProducto.Focus();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            dgvDetalle.Rows.RemoveAt(posicion);
            txtProducto.Focus();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            limpiar();
            btnAgregar.Enabled = true;
            txtProducto.Focus();
        }

        private void dgvDetalle_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void txtProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                txtPrecio.Focus(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtPrecio_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                txtCantidad.Focus(); // Pasa el foco al siguiente TextBox
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
            txtProducto.Text = dgvDetalle[1, posicion].Value.ToString();
            txtPrecio.Text = dgvDetalle[2, posicion].Value.ToString();
            txtCantidad.Text = dgvDetalle[3, posicion].Value.ToString();
            btnAgregar.Enabled = false;
            btnModificar.Enabled = true;
            btnEliminar.Enabled = true;
            txtProducto.Focus();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            posicion = dgvDetalle.CurrentRow.Index;
            txtProducto.Text = dgvDetalle[1, posicion].Value.ToString();
            txtPrecio.Text = dgvDetalle[2, posicion].Value.ToString();
            txtCantidad.Text = dgvDetalle[3, posicion].Value.ToString();
            btnAgregar.Enabled = false;
            btnModificar.Enabled = true;
            btnEliminar.Enabled = true;
            txtProducto.Focus();
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
                    txtProducto.Text = dgvDetalle[1, posicion].Value.ToString();
                    txtPrecio.Text = dgvDetalle[2, posicion].Value.ToString();
                    txtCantidad.Text = dgvDetalle[3, posicion].Value.ToString();
                    btnAgregar.Enabled = false;
                    btnModificar.Enabled = true;
                    btnEliminar.Enabled = true;
                    txtProducto.Focus();
                }
                else
                {
                    MessageBox.Show("La fila seleccionada contiene celdas vacías.");
                }

            }   
        }
    }
}
