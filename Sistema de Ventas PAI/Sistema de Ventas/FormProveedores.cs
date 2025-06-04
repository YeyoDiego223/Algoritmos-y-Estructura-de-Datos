using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Configuration;

namespace Sistema_de_Ventas
{
    
    public partial class FormProveedores : Form
    {
        
        int posicion, id;
        string nombre, contacto, email, telefono, telefonopattern = @"^-?\d*\.?\d+$", emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$", connectionString = ConfigurationManager.ConnectionStrings["SQLiteDB"].ConnectionString;
        public FormProveedores()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            InitializeComponent();
            iniciosesion();
            llenarDataGridView();
            cbxNombre.Focus();
            
            limpiar();
            // Ajustar automáticamente el tamaño de las columnas para que se ajusten al contenido
            dgvDetalle.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            // Ajustar automáticamente el tamaño de la última columna rellenando el espacio restante
            dgvDetalle.Columns[dgvDetalle.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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

        void iniciosesion()
        {
            if (Form1.cargo == "Administrador")
            {
                pctbxVentas.Enabled = false;
                pctbxVentas.Image = ConvertToGrayscale(Image.FromFile("Resources\\pcbxxVentas.png"));
                pctbxDetalleVenta.Enabled = false;
                pctbxDetalleVenta.Image = ConvertToGrayscale(Image.FromFile("Resources\\pcbxxDetalleVenta.png"));
                pctbxCompras.Enabled = false;
                pctbxCompras.Image = ConvertToGrayscale(Image.FromFile("Resources\\pcbxxCompras.png"));
            }
            else if (Form1.cargo == "Cajero")
            {
                pctbxClientes.Enabled = false;
                pctbxClientes.Image = ConvertToGrayscale(Image.FromFile("Resources\\pcbxxClientes.png"));
                pctbxDetalleVenta.Enabled = false;
                pctbxDetalleVenta.Image = ConvertToGrayscale(Image.FromFile("Resources\\pcbxxDetalleVenta.png"));
                pctbxCompras.Enabled = false;
                pctbxCompras.Image = ConvertToGrayscale(Image.FromFile("Resources\\pcbxxCompras.png"));                
                pctbxProductos.Enabled = false;
                pctbxProductos.Image = ConvertToGrayscale(Image.FromFile("Resources\\pcbxxProductos.png"));
            }
        }



        void eliminarbase()
        {
            posicion = dgvDetalle.CurrentRow.Index;
            var id = dgvDetalle[0, posicion].Value.ToString();

            string query = @"DELETE FROM Proveedores WHERE ID_Proveedor = @idProveedor";

            // 1. Conexion y comando
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
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
                        MessageBox.Show($"Error al eliminar Proveedor: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void modificarbase()
        {
            posicion = dgvDetalle.CurrentRow.Index;
            id = Convert.ToInt32(dgvDetalle[0, posicion].Value.ToString());
            nombre = cbxNombre.Text;
            contacto = txtContacto.Text;
            telefono = txtTelefono.Text;
            email = txtEmail.Text;

            // Rstriccion campos vacíos
            if (nombre == "" || contacto == "" || string.IsNullOrWhiteSpace(txtTelefono.Text) || email == "")
            {
                MessageBox.Show("No debe haber campos vacios", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Restricción de número
            if (!Regex.IsMatch(txtTelefono.Text, telefonopattern))
            {
                MessageBox.Show("El teléfono debe ser un número válido.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTelefono.Focus();
                txtTelefono.Text = "";
                return;
            }
            // Restricción de email
            if (!Regex.IsMatch(email, emailPattern))
            {
                MessageBox.Show("Por favor ingresa un correo electrónico valido", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Text = "";
                txtEmail.Focus();
                return;
            }            

            // Conexion
            string query = @"
        UPDATE Proveedores
        SET Nombre_Proveedor = @nombre,
            Contacto = @contacto,
            Telefono = @telefono,
            Email = @email
        WHERE ID_Proveedor = @idProveedor";
            // 1. Conexion y comando
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            using (SqliteCommand cmd = new SqliteCommand(query, conn))
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
                    MessageBox.Show($"Error al actualizar Proveedor: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            limpiar();
            cbxNombre.Focus();
        }

        void agregarbase()
        {
            nombre = cbxNombre.Text;
            contacto = txtContacto.Text;
            // Restricción campos vacios
            if (nombre == "" || contacto == "" || string.IsNullOrWhiteSpace(txtTelefono.Text) || email == "")
            {
                MessageBox.Show("No debe haber campos vacios", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Restricción números validos
            telefono = txtTelefono.Text;
            if (!Regex.IsMatch(txtTelefono.Text, telefonopattern))
            {
                MessageBox.Show("El teléfono debe de ser en número en entero.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTelefono.Focus();
                txtTelefono.Text = "";
                return;
            }
            // Restricción email valido
            email = txtEmail.Text;
            if (!Regex.IsMatch(email, emailPattern))
            {
                MessageBox.Show("Por favor ingresa un correo electrónico valido", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Text = "";
                txtEmail.Focus();
                return;
            }            
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

            string query = "INSERT INTO Proveedores (Nombre_Proveedor, Contacto, Telefono, Email)" +
                "VALUES (@nombre, @contacto, @telefono, @email)";
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            using (SqliteCommand cmd = new SqliteCommand(query, conn))
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
                    MessageBox.Show($"Error al agregar proveedor {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            limpiar();
            cbxNombre.Focus();
        }

        void llenarcombobox()
        {
            string query = "SELECT Nombre_Proveedor FROM Proveedores";

            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    using (SqliteCommand cmd = new SqliteCommand(query, conn))
                    {
                        conn.Open(); // abre la conexion
                        SqliteDataReader reader = cmd.ExecuteReader(); // Ejecuta la consulta

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
                MessageBox.Show($"Error al llenar el ComboBox: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void llenarDataGridView()
        {
            // Consulta SQL
            string query = "Select ID_Proveedor, Nombre_Proveedor, Contacto, Telefono, Email FROM Proveedores";

            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqliteCommand cmd = new SqliteCommand(query, conn);

                    using (SqliteDataReader reader = cmd.ExecuteReader())
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
                    MessageBox.Show($"Error al llenar el DataGridView: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            frmEstadisticas detalles = new frmEstadisticas();
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
                    MessageBox.Show("La fila seleccionada contiene celdas vacías.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void cbxNombre_Click(object sender, EventArgs e)
        {
            llenarcombobox();
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
            }            
        }
    }
}
