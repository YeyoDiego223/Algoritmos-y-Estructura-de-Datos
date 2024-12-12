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
    public partial class FormClientes : Form
    {
        int posicion;
        string nombre, apellido, email, tel, direcc;
        public FormClientes()
        {
            InitializeComponent();
            llenarDataGridView();
            txtNombre.Focus();
        }

        void eliminarbase()
        {
            posicion = dgvDetalle.CurrentRow.Index;
            var id = dgvDetalle[0, posicion].Value.ToString();
            // Conexion
            string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
            string query = @"DELETE FROM Clientes WHERE ID_Cliente = @idCliente";

            // 1. Conexion y comando
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // 2. Parámetros para evitar SQL Injection
                    cmd.Parameters.AddWithValue("@idCliente", id);

                    try
                    {
                        // 3. Abrír conexión y ejecutar comando

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Cliente eliminado correctamente.");
                        dgvDetalle.Rows.RemoveAt(posicion);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar Cliente: {ex.Message}");
                    }

                }

            }
        }

        void modificarbase()
        {
            posicion = dgvDetalle.CurrentRow.Index;
            var id = dgvDetalle[0, posicion].Value.ToString();
            var nombre = dgvDetalle[1, posicion].Value.ToString();
            var apellido = dgvDetalle[2, posicion].Value.ToString();
            var email = dgvDetalle[3, posicion].Value.ToString();
            var tel = dgvDetalle[4, posicion].Value.ToString();
            var direcc = dgvDetalle[5, posicion].Value.ToString();
            // Conexión
            string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
            string query = @"
        UPDATE Clientes 
        SET Nombre = @nombre, 
            Apellido = @apellido, 
            Email = @email, 
            Telefono = @telefono, 
            Direccion = @direccion
        WHERE ID_Cliente = @idCliente";
            // 1. Conexión y comando
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                // 2. Parámetros para evitar SQL Injection
                cmd.Parameters.AddWithValue("@idCliente", id);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@apellido", apellido);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@telefono", tel);
                cmd.Parameters.AddWithValue("@direccion", direcc);

                try
                {
                    // 3. Abrír conexión y ejecutar comando
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cliente actualizado correctamente.");
                    dgvDetalle[1, posicion].Value = txtNombre.Text;
                    dgvDetalle[2, posicion].Value = txtApellido.Text;
                    dgvDetalle[3, posicion].Value = txtEmail.Text;
                    dgvDetalle[4, posicion].Value = txtTelefono.Text;
                    dgvDetalle[5, posicion].Value = txtDireccion.Text;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al actualizar Cliente: {ex.Message}");
                }
            }
        }

        void agregarbase()
        {
            nombre = txtNombre.Text;
            apellido = txtApellido.Text;
            email = txtEmail.Text;
            tel = txtTelefono.Text;
            direcc = txtDireccion.Text;
            if (nombre == "" || apellido == "" || email == "" || tel == "" || direcc == "")
            {
                MessageBox.Show("No hay datos");
            }
            else
            {
                int lastID = 0;
                if (dgvDetalle.Rows.Count > 0)
                {
                    lastID = dgvDetalle.Rows.Cast<DataGridViewRow>()
                                        .Max(row => Convert.ToInt32(row.Cells[0].Value));
                }
                int newID = lastID + 1;
                string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
                string query = "INSERT INTO Clientes (Nombre, Apellido, Email, Telefono, Direccion) " +
                       "VALUES (@nombre, @apellido, @email, @telefono, @direccion)";
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@apellido", apellido);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@telefono", tel);
                    cmd.Parameters.AddWithValue("@direccion", direcc);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Clientes agregado.");
                        dgvDetalle.Rows.Add(newID.ToString(), nombre, apellido, email, tel, direcc);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al agregar cliente {ex.Message}");
                    }
                }

            }
        }

        private void llenarDataGridView()
        {
            // Cadena de conexion
            string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";

            // Consulta SQL
            string query = "Select ID_Cliente, Nombre, Apellido, Email, Telefono, Direccion FROM Clientes";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int lastID = 0;
                        if (dgvDetalle.Rows.Count > 0)
                        {
                            lastID = dgvDetalle.Rows.Cast<DataGridViewRow>()
                                                    .Max(row => Convert.ToInt32(row.Cells["colCodigo"].Value));
                        }
                        while (reader.Read())
                        {
                            lastID++;
                            // Crear una nueva fila y asignar los valores manualmente
                            int rowIndex = dgvDetalle.Rows.Add();
                            dgvDetalle.Rows[rowIndex].Cells["colCodigo"].Value = reader["ID_Cliente"];
                            dgvDetalle.Rows[rowIndex].Cells["colNombre"].Value = reader["Nombre"];
                            dgvDetalle.Rows[rowIndex].Cells["colApellido"].Value = reader["Apellido"];
                            dgvDetalle.Rows[rowIndex].Cells["colEmail"].Value = reader["Email"];
                            dgvDetalle.Rows[rowIndex].Cells["colTelefono"].Value = reader["Telefono"];
                            dgvDetalle.Rows[rowIndex].Cells["colDireccion"].Value = reader["Direccion"];
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
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtEmail.Text = "";
            txtTelefono.Text = "";
            txtDireccion.Text = "";
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormDetalles detalles = new FormDetalles();
            detalles.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormVentas ventas = new FormVentas();
            ventas.Show();
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

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormProductos configuracion = new FormProductos();
            configuracion.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            agregarbase();
            limpiar();
            txtNombre.Focus();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Estas seguro que deseas eliminar este cliente?",
                                                    "Confirmacion", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                eliminarbase();
                limpiar();
                txtNombre.Focus();
            }
            
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Estas seguro de modificar este cliente?", "Confirmación", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                modificarbase();
                limpiar();
                txtNombre.Focus();
            }            
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            limpiar();
            btnAgregar.Enabled = true;
            txtNombre.Focus();
        }

        private void dgvDetalle_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void txtNombre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                txtApellido.Focus(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtApellido_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                txtEmail.Focus(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                txtTelefono.Focus(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtTelefono_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                txtDireccion.Focus(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtDireccion_KeyDown(object sender, KeyEventArgs e)
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
            if (e.RowIndex >= 0) // Verifica que no sea el encabezado
            {
                DataGridViewRow filaSeleccionada = dgvDetalle.Rows[e.RowIndex];
                if (filaSeleccionada.Cells[0].Value != null && filaSeleccionada.Cells[1].Value != null)
                {
                    posicion = dgvDetalle.CurrentRow.Index;
                    txtNombre.Text = dgvDetalle[1, posicion].Value.ToString();
                    txtApellido.Text = dgvDetalle[2, posicion].Value.ToString();
                    txtEmail.Text = dgvDetalle[3, posicion].Value.ToString();
                    txtTelefono.Text = dgvDetalle[4, posicion].Value.ToString();
                    txtDireccion.Text = dgvDetalle[5, posicion].Value.ToString();
                    btnAgregar.Enabled = false;
                    btnModificar.Enabled = true;
                    btnEliminar.Enabled = true;
                    txtNombre.Focus();
                }
                else
                {
                    MessageBox.Show("La fila seleccionada contiene celdas vacías.");
                }
                
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormPrincipal principal = new FormPrincipal();
            principal.Show();
        }
    }
}
