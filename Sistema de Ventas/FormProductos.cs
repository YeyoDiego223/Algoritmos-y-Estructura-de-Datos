using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sistema_de_Ventas
{
    public partial class FormProductos : Form
    {
        int i = 1;
        int posicion;
        string producto, descripcion, precio, stock, categoria;
        public FormProductos()
        {
            InitializeComponent();
            string query = "SELECT * FROM Productos";
            Conexion conexion = new Conexion();
            try
            {
                DataTable datos = conexion.ObtenerDatos(query);
                dgvDetalle.DataSource = datos;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error: {ex.Message}");
            }
        }

        void limpiar()
        {
            btnEliminar.Enabled = false;
            btnModificar.Enabled = false;
            txtProducto.Text = "";
            txtDescripcion.Text = "";
            txtPrecio.Text = "";
            txtStock.Text = "";
            cbxCategoria.Text = "";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
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
            FormVentas ventas = new FormVentas();
            ventas.Show();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormDetalles detalles = new FormDetalles();
            detalles.Show();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormPrincipal principal = new FormPrincipal();
            principal.Show();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            producto = txtProducto.Text;
            descripcion = txtDescripcion.Text;
            precio = txtPrecio.Text;
            stock = txtStock.Text;
            categoria = cbxCategoria.Text;
            if (producto == "" && descripcion == "" && precio == "" && stock == "" && categoria == "")
            {
                MessageBox.Show("No hay datos");
            }
            else
            {
                dgvDetalle.Rows.Add(i + "", producto, descripcion, precio, stock, categoria);
                i = i + 1;
                limpiar();
                txtProducto.Focus();
            }
        }

        private void cbxCategoria_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                btnAgregar.PerformClick(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                txtDescripcion.Focus(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtDescripcion_KeyDown(object sender, KeyEventArgs e)
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

        }

        private void txtStock_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            producto = txtProducto.Text;
            descripcion = txtDescripcion.Text;
            precio = txtPrecio.Text;
            stock = txtStock.Text;
            categoria = cbxCategoria.Text;
            dgvDetalle[1, posicion].Value = txtProducto.Text;
            dgvDetalle[2, posicion].Value = txtDescripcion.Text;
            dgvDetalle[3, posicion].Value = txtPrecio.Text;
            dgvDetalle[4, posicion].Value = txtStock.Text;
            dgvDetalle[5, posicion].Value = cbxCategoria.Text;
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
            btnAgregar.Enabled = false;
            txtProducto.Focus();
        }

        private void dgvDetalle_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Verifica que no sea el encabezado
            {
                DataGridViewRow filaSeleccionada = dgvDetalle.Rows[e.RowIndex];
                if (filaSeleccionada.Cells[0].Value != null && filaSeleccionada.Cells[1].Value != null)
                {
                    posicion = dgvDetalle.CurrentRow.Index;
                    txtProducto.Text = dgvDetalle[1, posicion].Value.ToString();
                    txtDescripcion.Text = dgvDetalle[2, posicion].Value.ToString();
                    txtPrecio.Text = dgvDetalle[3, posicion].Value.ToString();
                    txtStock.Text = dgvDetalle[4, posicion].Value.ToString();
                    cbxCategoria.Text = dgvDetalle[5, posicion].Value.ToString();
                    btnAgregar.Enabled = false;
                    btnModificar.Enabled = true;
                    btnEliminar.Enabled = true;
                    txtPrecio.Focus();
                }
                else
                {
                    MessageBox.Show("La fila seleccionada contiene celdas vacias.");
                }
            }
        }
    }
}
