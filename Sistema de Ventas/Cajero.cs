﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sistema_de_Ventas
{
    public partial class Cajero : Form
    {
        public Cajero()
        {
            InitializeComponent();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
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

        private void Cajero_Load(object sender, EventArgs e)
        {

        }
    }
}
