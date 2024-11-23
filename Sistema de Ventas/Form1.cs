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
    public partial class Form1 : Form
    {
        

        public Form1()
        {
            InitializeComponent();
            cbxUsuario.DropDownStyle = ComboBoxStyle.DropDownList;//Desabilita el escribir en el combobox pero deja habilitado el seleccionar una opción
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

        private void pictureBox2_Click(object sender, EventArgs e)
        { 
            string usuario = cbxUsuario.Text;//Se declaran las variables tomándolas direcramente del elemento grafico
            string pass = txtContraseña.Text;

            if (usuario == "Gerente" && pass == "gerente")
            {
                this.Hide();//Oculta el formulario actual
                Gerente formgerente = new Gerente();//Declara un nuevo formulario
                formgerente.Show();//Muestra un formulario en este caso el de gerente
            }
            else if (usuario == "Administrador" && pass == "administrador")
            {
                this.Hide();
                Administrador formadmin = new Administrador();
                formadmin.Show();
            }
            else if (usuario == "Cajero" && pass == "cajero")
            {
                this.Hide();
                Cajero formcajero = new Cajero();
                formcajero.Show();
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
    }
}
