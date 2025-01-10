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
    public partial class Gerente : Form
    {
        public Gerente()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            InitializeComponent();

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 Form1 = new Form1();
            Form1.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormPrincipal principal = new FormPrincipal();
            principal.Show();
        }

        private void Gerente_Load(object sender, EventArgs e)
        {

        }

        private void btnCambiarContraseña_Click(object sender, EventArgs e)
        {
            this.Hide();
            Cambiar_Contraseñas cambiarcontra = new Cambiar_Contraseñas();
            cambiarcontra.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormAdministrarUsuarios administrar = new FormAdministrarUsuarios();
            administrar.Show();
        }
    }
}
