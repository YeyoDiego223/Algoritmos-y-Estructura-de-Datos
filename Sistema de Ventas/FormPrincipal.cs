using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Sistema_de_Ventas
{
    public partial class FormPrincipal : Form
    {
        public FormPrincipal()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            InitializeComponent();
            condicionales();
        }
         
        void condicionales()
        {
            if (Form1.cargo == "Administrador")
            {
                pctbxVentas.Enabled = false;
                pctbxVentas.Image = ConvertToGrayscale(Image.FromFile("C:\\Users\\flore\\OneDrive\\Documentos\\Sistema de ventas\\pcbxVentas.png"));
                pctbxDetalleVenta.Enabled = false;
                pctbxDetalleVenta.Image = ConvertToGrayscale(Image.FromFile("C:\\Users\\flore\\OneDrive\\Documentos\\Sistema de ventas\\pcbxDetalleVenta.png"));
                pctbxCompras.Enabled = false;
                pctbxCompras.Image = ConvertToGrayscale(Image.FromFile("C:\\Users\\flore\\OneDrive\\Documentos\\Sistema de ventas\\pcbxCompras.png"));
            }
            else if (Form1.cargo == "Cajero")
            {                
                pctbxClientes.Enabled = false;
                pctbxClientes.Image = ConvertToGrayscale(Image.FromFile("C:\\Users\\flore\\OneDrive\\Documentos\\Sistema de ventas\\pcbxClientes.png"));
                pctbxDetalleVenta.Enabled = false;
                pctbxDetalleVenta.Image = ConvertToGrayscale(Image.FromFile("C:\\Users\\flore\\OneDrive\\Documentos\\Sistema de ventas\\pcbxDetalleVenta.png"));
                pctbxCompras.Enabled = false;
                pctbxCompras.Image = ConvertToGrayscale(Image.FromFile("C:\\Users\\flore\\OneDrive\\Documentos\\Sistema de ventas\\pcbxCompras.png"));
                pctbxProveedores.Enabled = false;
                pctbxProveedores.Image = ConvertToGrayscale(Image.FromFile("C:\\Users\\flore\\OneDrive\\Documentos\\Sistema de ventas\\pcbxProveedores.png"));
                pctbxProductos.Enabled = false;
                pctbxProductos.Image = ConvertToGrayscale(Image.FromFile("C:\\Users\\flore\\OneDrive\\Documentos\\Sistema de ventas\\pcbxProductos.png"));
            }
        }

        private Image ConvertToGrayscale(Image originalImage)
        {
            // Crear un nuevo Bitmap con las mismas dimensiones que la imagen original
            Bitmap grayscaleImage = new Bitmap(originalImage.Width, originalImage.Height);

            // Crear gráficos desde el Bitmap
            using (Graphics g = Graphics.FromImage(grayscaleImage))
            {
                // Crear un conjunto de atributos de imagen
                ImageAttributes attributes = new ImageAttributes();

                // Crear una matriz de escala de grises
                float[][] colorMatrixElements = {
            new float[] { 0.3f, 0.3f, 0.3f, 0, 0 },
            new float[] { 0.59f, 0.59f, 0.59f, 0, 0 },
            new float[] { 0.11f, 0.11f, 0.11f, 0, 0 },
            new float[] { 0, 0, 0, 1, 0 },
            new float[] { 0, 0, 0, 0, 1 }
        };

                // Crear la matriz de color
                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

                // Establecer la matriz de color en los atributos
                attributes.SetColorMatrix(colorMatrix);

                // Dibujar la imagen original en escala de grises
                g.DrawImage(originalImage,
                    new Rectangle(0, 0, originalImage.Width, originalImage.Height),
                    0, 0, originalImage.Width, originalImage.Height,
                    GraphicsUnit.Pixel, attributes);
            }

            return grayscaleImage;
        }


        public void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormProveedores proveedores = new FormProveedores();
            proveedores.Show();
        }

        public void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormClientes clientes = new FormClientes();
            clientes.Show();
        }

        public void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormVentas ventas = new FormVentas();
            ventas.Show();
        }

        public void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmEstadisticas detalles = new frmEstadisticas();
            detalles.Show();
        }

        public void pictureBox5_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormCompras compras = new FormCompras();
            compras.Show();
        }

        public void pictureBox6_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormProductos configuracion = new FormProductos();
            configuracion.Show();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 IniciarSesion = new Form1();
            IniciarSesion.Show();
        }

        private void FormPrincipal_Load(object sender, EventArgs e)
        {

        }
    }
}
