using System;
using System.Configuration;
using Microsoft.Data.Sqlite;
using System.Windows.Forms;

namespace Sistema_de_Ventas
{
    public partial class Cambiar_Contraseñas : Form
    {
        string newpass, newuser, newcargo, connectionString = ConfigurationManager.ConnectionStrings["SQLiteDB"].ConnectionString;
        public Cambiar_Contraseñas()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            InitializeComponent();
            cbxCargo.DropDownStyle = ComboBoxStyle.DropDownList;

            iniciosesion();
        }

        void iniciosesion()
        {
            if (Form1.cargo == "Gerente")
            {
                cbxCargo.Items.Add(Form1.cargo);
                cbxCargo.Text = Form1.cargo;
                cbxUsuario.Text = Form1.usuario;
                cbxCargo.SelectedIndex = 0;
            }
            else if (Form1.cargo == "Administrador")
            {
                cbxCargo.Items.Add(Form1.cargo);
                cbxCargo.Text = Form1.cargo;
                cbxUsuario.Text = Form1.usuario;
            }
        }

        void consultarcargo()
        {
            Form1.usuario = cbxUsuario.Text;
            string queryObtenerCuenta = "SELECT Cuenta FROM Usuarios WHERE Usuario = @usuario";

            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();
                    using (SqliteCommand cmdObtenerCargo = new SqliteCommand(queryObtenerCuenta, conn))
                    {
                        cmdObtenerCargo.Parameters.AddWithValue("@usuario", Form1.usuario);
                        object resultCargo = cmdObtenerCargo.ExecuteScalar();

                        if (resultCargo != null)
                        {
                            Form1.usuariocuenta = resultCargo.ToString();
                        }
                        else
                        {
                            MessageBox.Show("El cargo seleccionada no existe en la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al identificar Usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void llenarcomboboxcargo()
        {
            string queryCargo = "SELECT Cargo FROM Usuarios";
            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {

                    using (SqliteCommand cmd = new SqliteCommand(queryCargo, conn))
                    {
                        conn.Open();
                        SqliteDataReader reader = cmd.ExecuteReader();
                        cbxCargo.Items.Clear();

                        while (reader.Read())
                        {
                            cbxCargo.Items.Add(reader["Cargo"].ToString());
                        }
                        if (cbxCargo.Items.Count > 0)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al llenar ComboBox: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void llenarusuario()
        {
            string queryllenarcbxUsuario = "SELECT Usuario FROM Usuarios";
            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    using (SqliteCommand cmdQueryLlenarCbxUsuario = new SqliteCommand(queryllenarcbxUsuario, conn))
                    {
                        conn.Open();
                        SqliteDataReader reader = cmdQueryLlenarCbxUsuario.ExecuteReader();
                        cbxUsuario.Items.Clear();

                        while (reader.Read())
                        {
                            cbxUsuario.Items.Add(reader["Usuario"].ToString());
                        }
                        if (cbxUsuario.Items.Count > 0)
                        {
                            cbxUsuario.SelectedIndex = 0;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al llenar el ComboBox: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void actualizarpass()
        {
            newuser = cbxUsuario.Text;
            newpass = txtContraseña.Text;
            newcargo = cbxCargo.Text;
            string queryCambiarUserPassCargo = "UPDATE Usuarios SET Usuario = @usuario, Contrasena = @pass WHERE Cuenta = @cuenta";

            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    using (SqliteCommand cmdCambiarUserPassCargo = new SqliteCommand(queryCambiarUserPassCargo, conn))
                    {
                        cmdCambiarUserPassCargo.Parameters.AddWithValue("@cuenta", Form1.usuariocuenta);
                        cmdCambiarUserPassCargo.Parameters.AddWithValue("@usuario", newuser);
                        cmdCambiarUserPassCargo.Parameters.AddWithValue("@pass", newpass);
                        cmdCambiarUserPassCargo.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al identificar Usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void cambiarcontra()
        {
            if (Form1.cargo == "Gerente")
            {
                if (newuser == "" || newpass == "")
                {
                    MessageBox.Show("Los campos estan vacio");
                    return;
                }
                actualizarpass();
                MessageBox.Show("Contraseña de gerente actualizada");
                txtContraseña.Text = "";
                cbxUsuario.Text = "";
            }
            else if (Form1.cargo == "Administrador")
            {
                if (newuser == "" || newpass == "")
                {
                    MessageBox.Show("Los campos estan vacio");
                    return;
                }
                actualizarpass();
                MessageBox.Show("Contraseña de Administrador actualizada");
                txtContraseña.Text = "";
                cbxUsuario.Text = "";
            }
            else if (Form1.cargo == "Cajero")
            {
                if (newuser == "" || newpass == "")
                {
                    MessageBox.Show("El campo esta vacio");
                    return;
                }

                MessageBox.Show("Contraseña de Cajero actualizada");
                txtContraseña.Text = "";
            }
        }

        private void txtContraseña_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                btnCambiar.PerformClick();
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void Cambiar_Contraseñas_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (Form1.cargo == "Gerente")
            {
                this.Hide();
                Gerente gerente = new Gerente();
                gerente.Show();
            }
            else if (Form1.cargo == "Administrador")
            {
                this.Hide();
                Administrador administrador = new Administrador();
                administrador.Show();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbxCuenta_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnCambiar_Click(object sender, EventArgs e)
        {
            if (cbxUsuario.Text == "" || txtContraseña.Text == "" || cbxCargo.Text == "")
            {
                MessageBox.Show("No debe haber campos vacios", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            cambiarcontra();
        }
    }
}
