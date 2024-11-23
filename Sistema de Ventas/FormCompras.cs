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
    public partial class FormCompras : Form
    {
        int i = 1;
        int posicion;
        string proveedor, fecha, total;
        public FormCompras()
        {
            InitializeComponent();
            cbxProveedor.DropDownStyle = ComboBoxStyle.DropDownList;
            string query = "SELECT * FROM Compras";
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
            if (proveedor == "" && fecha == "" && total == "")
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
                DataGridViewRow filasSeleccionada = dgvDetalle.Rows[e.RowIndex];
                if (filasSeleccionada.Cells[0].Value != null && filasSeleccionada.Cells[1].Value != null)
                {
                    posicion = dgvDetalle.CurrentRow.Index;
                    cbxProveedor.Text = dgvDetalle[1, posicion].Value.ToString();
                    dtpFecha.Text = dgvDetalle[2, posicion].Value.ToString();
                    txtTotal.Text = dgvDetalle[3, posicion].Value.ToString();
                    btnAgregar.Enabled = false;
                    btnModificar.Enabled = true;
                    btnEliminar.Enabled = true;
                    cbxProveedor.Focus();
                }
                else
                {
                    MessageBox.Show("La fila seleccionada contiene celdas vacias");
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
            limpiar();
            cbxProveedor.Focus();
        }
    }
}
