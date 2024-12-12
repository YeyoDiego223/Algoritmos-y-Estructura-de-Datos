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
    
    public partial class FormProveedores : Form
    {
        
        int posicion;
        string nombre, contacto, telefono, email;
        public FormProveedores()
        {
            InitializeComponent();
            llenarDataGridView();
            cbxNombre.Focus();
            llenarcombobox();
        }

         void eliminarbase()
        {
            posicion = dgvDetalle.CurrentRow.Index;
            var id = dgvDetalle[0, posicion].Value.ToString();
            // Conexion
            string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
            string query = @"DELETE FROM Proveedores WHERE ID_Proveedor = @idProveedor";

            // 1. Conexion y comando
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // 2. Parametros para evitar SQLL Injection
                    cmd.Parameters.AddWithValue("@idProveedor", id);

                    try
                    {
                        // 3. Abrir conexion y ejecutar comando
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Proveedor eliminado correctamente.");
                        dgvDetalle.Rows.RemoveAt(posicion);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar Proveedor: {ex.Message}");
                    }
                }
            }
        }

        void modificarbase()
        {
            posicion = dgvDetalle.CurrentRow.Index;
            var id = dgvDetalle[0, posicion].Value.ToString();
            var nombre = dgvDetalle[1, posicion].Value.ToString();
            var contacto = dgvDetalle[2, posicion].Value.ToString();
            var telefono = dgvDetalle[3, posicion].Value.ToString();
            var email = dgvDetalle[4, posicion].Value.ToString();
            // Conexion
            string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
            string query = @"
        UPDATE Proveedores
        SET Nombre_Proveedor = @nombre,
            Contacto = @contacto,
            Telefono = @telefono,
            Email = @email
        WHERE ID_Proveedor = @idProveedor";
            // 1. Conexion y comando
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                // 2. Parametros para evitar SQL Injection
                cmd.Parameters.AddWithValue("@idProveedor", id);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@contacto", contacto);
                cmd.Parameters.AddWithValue("@telefono", telefono);
                cmd.Parameters.AddWithValue("@email", email);

                try
                {
                    // 3. Abrir conexion y ejecutar comando
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Proveedor actualizado correctamente.");
                    dgvDetalle[1, posicion].Value = cbxNombre.Text;
                    dgvDetalle[2, posicion].Value = txtContacto.Text;
                    dgvDetalle[3, posicion].Value = txtTelefono.Text;
                    dgvDetalle[4, posicion].Value = txtEmail.Text;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al actualizar Proveedor: {ex.Message}");
                }
            }
        }

        void agregarbase()
        {
            nombre = cbxNombre.Text;
            contacto = txtContacto.Text;
            telefono = txtTelefono.Text;
            email = txtEmail.Text;
            if (nombre == "" || telefono == "" || email == "")
            {
                MessageBox.Show("No hay datos");
            }
            else
            {
                // Obtener el último valor de ID en el DataGridView
                int lastID = 0;
                if (dgvDetalle.Rows.Count > 0)
                {
                    // Obten el valor máximo de la columna "ID" (columna 0 en este caso)
                    lastID = dgvDetalle.Rows.Cast<DataGridViewRow>()
                                            .Max(row => Convert.ToInt32(row.Cells[0].Value));
                }
                // Nuevo ID  es el último ID + 1
                int newID = lastID + 1;
                // Agregar el nuevo producto al DataGridView con el nuevo ID

                string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
                string query = "INSERT INTO Proveedores (Nombre_Proveedor, Contacto, Telefono, Email)" +
                    "VALUES (@nombre, @contacto, @telefono, @email)";
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@contacto", contacto);
                    cmd.Parameters.AddWithValue("@telefono", telefono);
                    cmd.Parameters.AddWithValue("@email", email);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Proveedor agregado.");
                        dgvDetalle.Rows.Add(newID.ToString(), nombre, contacto, telefono, email);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al agregar proveedor {ex.Message}");
                    }
                }

            }
        }

        void llenarcombobox()
        {
            string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
            string query = "SELECT Nombre_Proveedor FROM Proveedores";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open(); // abre la conexion
                        SqlDataReader reader = cmd.ExecuteReader(); // Ejecuta la consulta

                        //Limpia el combobox antes de llenarlo
                        cbxNombre.Items.Clear();

                        //Itera por cada fila y añade los datos al combobox
                        while (reader.Read())
                        {
                            cbxNombre.Items.Add(reader["Nombre_Proveedor"].ToString());
                        }

                        // Seleciona el primer elemento por defecto
                        if (cbxNombre.Items.Count > 0)
                        {
                            cbxNombre.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al llenar el ComboBox: {ex.Message}");
            }
        }

        private void llenarDataGridView()
        {
            // Cadena de conexion
            string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";

            // Consulta SQL
            string query = "Select ID_Proveedor, Nombre_Proveedor, Contacto, Telefono, Email FROM Proveedores";

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
                            dgvDetalle.Rows[rowIndex].Cells["colCodigo"].Value = reader["ID_Proveedor"];
                            dgvDetalle.Rows[rowIndex].Cells["colNombre"].Value = reader["Nombre_Proveedor"];
                            dgvDetalle.Rows[rowIndex].Cells["colContacto"].Value = reader["Contacto"];
                            dgvDetalle.Rows[rowIndex].Cells["colTelefono"].Value = reader["Telefono"];
                            dgvDetalle.Rows[rowIndex].Cells["colEmail"].Value = reader["Email"];
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
            cbxNombre.Text = "";
            txtContacto.Text = "";
            txtTelefono.Text = "";
            txtEmail.Text = "";
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void FormProveedores_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormCompras compras = new FormCompras();
            compras.Show();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormClientes clientes = new FormClientes();
            clientes.Show();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
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

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void cbxNombre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                txtContacto.Focus(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtNombre_KeyDown(object sender, KeyEventArgs e)
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
                txtEmail.Focus(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtEmail_KeyDown(object sender, KeyEventArgs e)
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
                    cbxNombre.Text = dgvDetalle[1, posicion].Value.ToString();
                    txtContacto.Text = dgvDetalle[2, posicion].Value.ToString();
                    txtTelefono.Text = dgvDetalle[3, posicion].Value.ToString();
                    txtEmail.Text = dgvDetalle[4, posicion].Value.ToString();
                    btnAgregar.Enabled = false;
                    btnModificar.Enabled = true;
                    btnEliminar.Enabled = true;
                    cbxNombre.Focus();
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

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txtContacto_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvDetalle_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {

        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTelefono_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtNombre_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void txtContacto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                txtTelefono.Focus(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            agregarbase();
            limpiar();
            cbxNombre.Focus();
        }        

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Estás seguro de eliminar este proveedor?", "Confirmación", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                eliminarbase();
                limpiar();
                cbxNombre.Focus();
            }            
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            limpiar();
            btnAgregar.Enabled = true;
            cbxNombre.Focus();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Estás seguro que deseas modificar este proveedor", "Confirmacion", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                modificarbase();
                limpiar();
                cbxNombre.Focus();
            }            
        }
    }
}
