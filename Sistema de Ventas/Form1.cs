using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Sistema_de_Ventas
{
    public partial class Form1 : Form
    {
        string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
        public static string usuario, pass, usuariobase, passbase, cargo, usuariocuenta = "";
        public Form1()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            InitializeComponent();
            LlenarComboBoxUser();
            cbxUsuario.Focus();
            cbxUsuario.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxUsuario.SelectedIndex = 0;
            cbxCargo.DropDownStyle = ComboBoxStyle.DropDownList;//Desabilita el escribir en el combobox pero deja habilitado el seleccionar una opción
            cbxCargo.SelectedIndex = 0;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txtUsuario_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtContraseña_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) //Verifica si se presiono la tecla Enter
            {
                pictureBox1.PerformLayout(); // Ejecuta el evento
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; //Evita el sonido "beep"
            }
        }


        private void cbxUsuario_SelectedIndexChanged(object sender, EventArgs e)
        {
        }


        void llenarcomboboxcargo()
        {
            string queryCargo = "SELECT Cargo FROM Usuarios";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(queryCargo, conn))
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        cbxCargo.Items.Clear();

                        while (reader.Read())
                        {
                            cbxCargo.Items.Add(reader["Cargo"].ToString());
                        }
                        if (cbxCargo.Items.Count > 0)
                        {
                            cbxCargo.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al llenar ComboBox: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void LlenarComboBoxUser()
        {
            string queryseleccionarusuarios = "SELECT Usuario FROM Usuarios";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(queryseleccionarusuarios, conn))
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
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
                MessageBox.Show($"Error al llenar ComboBox: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void cuenta()
        {
            usuario = cbxUsuario.Text;
            pass = txtContraseña.Text;
                   
            string querySelectCuenta = "SELECT Cuenta FROM Usuarios WHERE Usuario = @user AND Contraseña = @pass";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmdObtenerCargo = new SqlCommand(querySelectCuenta, conn))
                    {
                        cmdObtenerCargo.Parameters.AddWithValue("@user", usuario);
                        cmdObtenerCargo.Parameters.AddWithValue("@pass", pass);
                        object resultCargo = cmdObtenerCargo.ExecuteScalar();

                        if (resultCargo != null)
                        {
                            usuariocuenta = resultCargo.ToString(); // Convertir el resultado a string
                            Console.WriteLine(usuariocuenta);
                        }
                        else
                        {
                            MessageBox.Show("El Usuario seleccionado no existe en la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    using (SqlCommand cmdObtenerPassUsuario = new SqlCommand(querySelectCuenta, conn))
                    {
                        cmdObtenerPassUsuario.Parameters.AddWithValue("@cuenta", usuariocuenta);
                        SqlDataReader reader = cmdObtenerPassUsuario.ExecuteReader();

                        if (reader.Read())
                        {
                            usuariobase = reader["Usuario"].ToString();
                            Console.WriteLine(usuariobase);
                            passbase = reader["Contraseña"].ToString();
                            Console.WriteLine(passbase);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al identificar Usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        public void pictureBox2_Click(object sender, EventArgs e)
        {            
            usuario = cbxUsuario.Text;
            pass = txtContraseña.Text;
            string queryObtenerCargo = "SELECT Cargo FROM Usuarios WHERE Usuario = @usuario";
            string queryUsuarioPass = "SELECT Usuario, Contraseña FROM Usuarios WHERE Cuenta = @cuenta";
            string querySelectCuenta = "SELECT Cuenta FROM Usuarios WHERE Usuario = @user AND Contraseña = @pass AND Cargo = @cargo";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmdObtenerCargo = new SqlCommand(queryObtenerCargo, conn))
                    {
                        cmdObtenerCargo.Parameters.AddWithValue("@usuario", usuario);
                        object resultCargo = cmdObtenerCargo.ExecuteScalar();

                        if (resultCargo != null)
                        {
                            cargo = resultCargo.ToString(); // Convertir el resultado a string
                        }
                        else
                        {
                            MessageBox.Show("Contraseña o Usuario incorrecto", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    using (SqlCommand cmdSelectCuenta = new SqlCommand(querySelectCuenta, conn))
                    {
                        cmdSelectCuenta.Parameters.AddWithValue("@user", usuario);
                        cmdSelectCuenta.Parameters.AddWithValue("@pass", pass);
                        cmdSelectCuenta.Parameters.AddWithValue("@cargo", cargo);
                        object resultCargo = cmdSelectCuenta.ExecuteScalar();

                        if (resultCargo != null)
                        {
                            usuariocuenta = resultCargo.ToString(); // Convertir el resultado a string
                            Console.WriteLine(usuariocuenta);
                        }
                        else
                        {
                            MessageBox.Show("Contraseña o Usuario incorrecto", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    using (SqlCommand cmdObtenerPassUsuario = new SqlCommand(queryUsuarioPass, conn))
                    {
                        cmdObtenerPassUsuario.Parameters.AddWithValue("@cuenta", usuariocuenta);
                        SqlDataReader reader = cmdObtenerPassUsuario.ExecuteReader();

                        if (reader.Read())
                        {
                            usuariobase = reader["Usuario"].ToString();
                            passbase = reader["Contraseña"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al identificar Usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (cbxCargo.Text != cargo)
            {
                // Mostrar un mensaje de error
                MessageBox.Show("El cargo seleccionado no coincide con el cargo del usuario", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (cbxCargo.Text == "" || cbxUsuario.Text == "" || txtContraseña.Text == "")
            {
                MessageBox.Show("Los campos están vacios", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (usuario == usuariobase && pass == passbase)
            {
                if (cargo == "Gerente")
                {
                    this.Hide();//Oculta el formulario actual
                    Gerente formgerente = new Gerente();//Declara un nuevo formulario
                    formgerente.Show();//Muestra un formulario en este caso el de gerente
                }
                else if (cargo == "Administrador")
                {
                    this.Hide();
                    Administrador formadministrador = new Administrador();
                    formadministrador.Show();
                }
                else if (cargo == "Cajero")
                {
                    this.Hide();
                    Cajero formcajero = new Cajero();
                    formcajero.Show();
                }
            }          
            else
            {
                MessageBox.Show("Contraseña o Usuario incorrecto");
            }
        }

        private void txtContraseña_Enter(object sender, EventArgs e)
        {
        }

        private void txtContraseña_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtContraseña_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                pictureBox2_Click(sender, e); // Ejecuta el evento Click del botón
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtUsuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                txtContraseña.Focus();
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtContraseña_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cbxUsuario.Focus();
        }
    }
}
