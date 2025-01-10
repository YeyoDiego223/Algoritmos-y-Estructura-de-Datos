using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing.Imaging;

namespace Sistema_de_Ventas
{
    public partial class FormAdministrarUsuarios : Form
    {
        int posicion, cuenta;
        string cargo, usuario, pass, connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
        public FormAdministrarUsuarios()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;            
            InitializeComponent();
            limpiar();
            cbxCargo.DropDownStyle = ComboBoxStyle.DropDownList;
            llenarDataGridVie();
        }

        void limpiar()
        {
            btnCrear.Enabled = true;
            btnEditar.Enabled = false;
            btnEliminar.Enabled = false;
            txtUsuario.Text = "";
            txtContraseña.Text = "";
            cbxCargo.Focus();
            cbxCargo.Select();
        }

        void llenarDataGridVie()
        {
            // Consulta SQL
            string query = "Select Cuenta, Cargo, Usuario, Contraseña FROM Usuarios";

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
                            dgvDetalle.Rows[rowIndex].Cells["colCodigo"].Value = reader["Cuenta"];
                            dgvDetalle.Rows[rowIndex].Cells["colCargo"].Value = reader["Cargo"];
                            dgvDetalle.Rows[rowIndex].Cells["colUsuario"].Value = reader["Usuario"];
                            dgvDetalle.Rows[rowIndex].Cells["colPass"].Value = reader["Contraseña"];
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al llenar el DataGridView: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void modificarbase()
        {
            posicion = dgvDetalle.CurrentRow.Index;
            cuenta = Convert.ToInt32(dgvDetalle[0, posicion].Value.ToString());
            cargo = cbxCargo.Text;
            usuario = txtUsuario.Text;
            pass = txtContraseña.Text;

            // Rstriccion campos vacíos
            if (cargo == "" || usuario == "" || pass == "")
            {
                MessageBox.Show("No debe haber campos vacios", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }            
            // Conexión
            string query = @"
        UPDATE Usuarios 
        SET Cargo = @cargo, 
            Usuario = @usuario, 
            Contraseña = @pass
        WHERE Cuenta = @cuenta";
            // 1. Conexión y comando
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                // 2. Parámetros para evitar SQL Injection
                cmd.Parameters.AddWithValue("@cuenta", cuenta);
                cmd.Parameters.AddWithValue("@cargo", cargo);
                cmd.Parameters.AddWithValue("@usuario", usuario);
                cmd.Parameters.AddWithValue("@pass", pass);

                try
                {
                    // 3. Abrír conexión y ejecutar comando
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Usuario actualizado correctamente.");
                    dgvDetalle[1, posicion].Value = cbxCargo.Text;
                    dgvDetalle[2, posicion].Value = txtUsuario.Text;
                    dgvDetalle[3, posicion].Value = txtContraseña.Text;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al actualizar Cliente: {ex.Message}");
                }
            }
            limpiar();
        }

        void eliminarbase()
        {
            posicion = dgvDetalle.CurrentRow.Index;
            var cuenta = dgvDetalle[0, posicion].Value.ToString();
            // Conexion
            string queryEliminar = @"DELETE FROM Usuarios WHERE Cuenta = @cuenta";

            // 1. Conexion y comando
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(queryEliminar, conn))
                {
                    // 2. Parámetros para evitar SQL Injection
                    cmd.Parameters.AddWithValue("@cuenta", cuenta);

                    try
                    {
                        // 3. Abrír conexión y ejecutar comando

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Usuario eliminado correctamente.");
                        dgvDetalle.Rows.RemoveAt(posicion);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar Cliente: {ex.Message}");
                    }

                }

            }
        }

        void agregarbase()
        {
            cargo = cbxCargo.Text;
            usuario = txtUsuario.Text;
            pass = txtContraseña.Text;
            if (cargo == "" || usuario == "" || pass == "")
            {
                MessageBox.Show("No debe haber campos vacíos");
                return;
            }
            int lastID = 0;
            if (dgvDetalle.Rows.Count > 0)
            {
                lastID = dgvDetalle.Rows.Cast<DataGridViewRow>()
                                    .Max(row => Convert.ToInt32(row.Cells[0].Value));
            }
            int newID = lastID + 1;
            string query = "INSERT INTO Usuarios (Cargo, Usuario, Contraseña) " +
                   "VALUES (@cargo, @usuario, @pass)";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@cargo", cargo);
                cmd.Parameters.AddWithValue("@usuario", usuario);
                cmd.Parameters.AddWithValue("@pass", pass);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Usuario agregado.");
                    dgvDetalle.Rows.Add(newID.ToString(), cargo, usuario, pass);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al agregar cliente {ex.Message}");
                }

            }
            limpiar();
        }

        private void FormAdministrarUsuarios_Load(object sender, EventArgs e)
        {

        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            agregarbase();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Gerente gerente = new Gerente();
            gerente.Show();
        }

        private void cbxCargo_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void cbxCargo_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void txtUsuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                txtContraseña.Focus(); // Pasa el foco al siguiente TextBox
                txtContraseña.Select();
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtContraseña_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                btnCrear.PerformClick(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void cbxCargo_TextUpdate(object sender, EventArgs e)
        {            
                                       
        }

        private void btnVaciar_Click(object sender, EventArgs e)
        {
            limpiar();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Estas seguro que deseas eliminar este usuario?",
                                                                "Confirmacion", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                eliminarbase();
                limpiar();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Estas seguro de modificar este cliente?", "Confirmación", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                modificarbase();
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
                    txtUsuario.Text = dgvDetalle[2, posicion].Value.ToString();
                    txtContraseña.Text = dgvDetalle[3, posicion].Value.ToString();
                    btnCrear.Enabled = false;
                    btnEditar.Enabled = true;
                    btnEliminar.Enabled = true;
                    cbxCargo.Focus();
                    cbxCargo.Select();
                }
                else
                {
                    MessageBox.Show("La fila seleccionada contiene celdas vacías.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}
