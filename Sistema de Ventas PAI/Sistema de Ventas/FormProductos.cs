using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using System.Drawing.Imaging;
using System.Configuration;

namespace Sistema_de_Ventas
{
    public partial class FormProductos : Form
    {
        int posicion, id, stock, precio;
        string producto, descripcion, categoria, connectionString = ConfigurationManager.ConnectionStrings["SQLiteDB"].ConnectionString;
        public FormProductos()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            InitializeComponent();
            txtProducto.Focus();
            llenarDataGridView();
            txtProducto.Focus();
            cbxCategoria.SelectedIndex = 0;
            iniciosesion();
            // Ajustar automáticamente el tamaño de las columnas para que se ajusten al contenido
            dgvDetalle.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            // Ajustar automáticamente el tamaño de la última columna rellenando el espacio restante
            dgvDetalle.Columns[dgvDetalle.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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
                pctbxProveedores.Enabled = false;
                pctbxProveedores.Image = ConvertToGrayscale(Image.FromFile("Resources\\pcbxxProveedores.png"));                
            }
        }

        private void agregarbase()
        {
            producto = txtProducto.Text;
            descripcion = txtDescripcion.Text;
            precio = Convert.ToInt32(txtPrecio.Text);
            stock = Convert.ToInt32(txtStock.Text);
            categoria = cbxCategoria.Text;
            ;
            if (producto == "" || descripcion == "" || string.IsNullOrWhiteSpace(txtPrecio.Text) || string.IsNullOrWhiteSpace(txtStock.Text) || categoria == "")
            {
                MessageBox.Show("No debe haber campos vacios", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }            

            // Obtener el último valor de ID en el DataGridView
            int lastID = 0;
            if (dgvDetalle.Rows.Count > 0)
            {
                // Obtén el valor máximo de la columna "ID" (columna 0 en este caso)
                lastID = dgvDetalle.Rows.Cast<DataGridViewRow>()
                                        .Max(row => Convert.ToInt32(row.Cells[0].Value));
            }

            // Nuevo ID es el último ID + 1
            int newID = lastID + 1;

            // Agregar el nuevo producto al DataGridView con el nuevo ID


            string query = "INSERT INTO Productos (Nombre_Producto, Descripcion, Precio, Stock, Categoria) " +
                           "VALUES (@nombre, @descripcion, @precio, @stock, @categoria)";

            using (SqliteConnection conn = new SqliteConnection(connectionString))
            using (SqliteCommand cmd = new SqliteCommand(query, conn))
            {
                

                if (precio > 0 || stock > 0)
                {                    
                    cmd.Parameters.AddWithValue("@stock", stock);
                    cmd.Parameters.AddWithValue("@categoria", categoria);
                    dgvDetalle.Rows.Add(newID.ToString(), producto, descripcion, precio, stock, categoria);
                }
                else
                {
                    MessageBox.Show("Precio o Stock no tienen un formato valido", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Detener la ejecución
                }

                // Asignar valores a los parámetros
                cmd.Parameters.AddWithValue("@nombre", producto);
                cmd.Parameters.AddWithValue("@descripcion", descripcion);
                cmd.Parameters.AddWithValue("@precio", precio);

                // Ejecutar la consulta
                try
                {
                    // Abrir la conexión y ejecutar el comando
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Producto agregado correctamente.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al agregar producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            limpiar();
            txtProducto.Focus();
        }

        private void modificarbase()
        {
            if (dgvDetalle.CurrentRow == null)
            {
                MessageBox.Show("Por favor, selecciona una fila antes de continuar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int posicion = dgvDetalle.CurrentRow.Index;
            id = Convert.ToInt32(dgvDetalle[0, posicion].Value); // ID como entero
            producto = txtProducto.Text;
            descripcion = txtDescripcion.Text;
            decimal precio = Convert.ToDecimal(txtPrecio.Text); // Precio como decimal
            if (!int.TryParse(txtStock.Text, out stock)) // Stock como entero
            {
                MessageBox.Show("El valor ingresado debe de ser entero.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            categoria = cbxCategoria.Text;


            if (precio < 0 || stock < 0)
            {
                MessageBox.Show("Utiliza formato valido", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = @"
            UPDATE Productos 
            SET Nombre_Producto = @nombreProducto, 
                Descripcion = @descripcion, 
                Precio = @precio, 
                Stock = @stock, 
                Categoria = @categoria
            WHERE ID_Producto = @idProducto";

            using (SqliteConnection conn = new SqliteConnection(connectionString))
            using (SqliteCommand cmd = new SqliteCommand(query, conn))
            {
                // Asignar parámetros con tipos correctos
                cmd.Parameters.AddWithValue("@idProducto", id);
                cmd.Parameters.AddWithValue("@nombreProducto", producto);
                cmd.Parameters.AddWithValue("@descripcion", descripcion);
                cmd.Parameters.AddWithValue("@precio", precio);
                cmd.Parameters.AddWithValue("@stock", stock);
                cmd.Parameters.AddWithValue("@categoria", categoria);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Producto actualizado correctamente.");
                    // Actualizar valores en el `DataGridView`
                    dgvDetalle[1, posicion].Value = txtProducto.Text;
                    dgvDetalle[2, posicion].Value = txtDescripcion.Text;
                    dgvDetalle[3, posicion].Value = txtPrecio.Text;
                    dgvDetalle[4, posicion].Value = txtStock.Text;
                    dgvDetalle[5, posicion].Value = cbxCategoria.Text;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al actualizar producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            limpiar();
            txtProducto.Focus();
        }

        void eliminarbase()
        {
            posicion = dgvDetalle.CurrentRow.Index;
            var id = dgvDetalle[0, posicion].Value.ToString();
                // Conexion
                string query = @"DELETE FROM Productos WHERE ID_Producto = @idProducto";

                // 1. Conexion y comando
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();
                    using (SqliteCommand cmd = new SqliteCommand(query, conn))
                    {
                        // 2. Parámetros para evitar SQL Injection
                        cmd.Parameters.AddWithValue("@idProducto", id);

                        try
                        {
                            // 3. Abrír conexión y ejecutar comando

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Producto eliminado correctamente.");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error al eliminar producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    
                }
                
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

        private void llenarDataGridView()
        {
            // Cadena de conexion

            // Consulta SQL
            string query = "SELECT ID_Producto, Nombre_Producto, Descripcion, Precio, Stock, Categoria FROM Productos";

            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqliteCommand cmd = new SqliteCommand(query, conn);

                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        // Obtener el último ID en el DataGridView para asegurar la secuencia
                        int lastID = 0;
                        if (dgvDetalle.Rows.Count > 0)
                        {
                            // Recorrer las filas del DataGridView para obtener el valor máximo de ID
                            lastID = dgvDetalle.Rows.Cast<DataGridViewRow>()
                                                    .Max(row => Convert.ToInt32(row.Cells["colCodigo"].Value));
                        }

                        // Recorrer los datos del reader y agregar las filas al DataGridView
                        while (reader.Read())
                        {
                            // Nuevo ID es el último ID + 1
                            lastID++;

                            // Crear una nueva fila y asignar los valores manualmente
                            int rowIndex = dgvDetalle.Rows.Add();
                            dgvDetalle.Rows[rowIndex].Cells["colCodigo"].Value = lastID;  // Usamos lastID
                            dgvDetalle.Rows[rowIndex].Cells["colNombre"].Value = reader["Nombre_Producto"];
                            dgvDetalle.Rows[rowIndex].Cells["colDescripcion"].Value = reader["Descripcion"];
                            dgvDetalle.Rows[rowIndex].Cells["colPrecio"].Value = reader["Precio"];
                            dgvDetalle.Rows[rowIndex].Cells["colStock"].Value = reader["Stock"];
                            dgvDetalle.Rows[rowIndex].Cells["colCategoria"].Value = reader["Categoria"];
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
            txtProducto.Text = "";
            txtDescripcion.Text = "";
            txtPrecio.Text = "";
            txtStock.Text = "";
            cbxCategoria.Text = "";
        }

        private void label1_Click(object sender, EventArgs e)
        {

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
            FormProveedores proveedores = new FormProveedores();
            proveedores.Show();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormVentas ventas = new FormVentas();
            ventas.Show();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmEstadisticas detalles = new frmEstadisticas();
            detalles.Show();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormPrincipal principal = new FormPrincipal();
            principal.Show();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            agregarbase();            
        }

        private void cbxCategoria_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                btnAgregar.PerformClick(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                txtDescripcion.Focus(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtDescripcion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                txtPrecio.Focus(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtPrecio_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                txtStock.Focus(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtStock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                cbxCategoria.Focus(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtProducto_TextChanged(object sender, EventArgs e)
        {

        }

        private void cbxCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dgvDetalle_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dgvDetalle.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetalle.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            if (e.ColumnIndex == 3 && e.Value !=null)
            {
                // Formatear la celda para mostrar el signo del dolar
                e.Value = "$" + e.Value.ToString();
                e.FormattingApplied = true;
            }
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormCompras compras = new FormCompras();
            compras.Show();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Estás seguro que deseas modificar este producto?", "Confirmación", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                modificarbase();                
            }            
        } 

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Estas seguro que deseas eliminar este producto?",
                                                "Confirmación", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                eliminarbase();
                dgvDetalle.Rows.RemoveAt(posicion);
                limpiar();
                txtProducto.Focus();
            }  
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            limpiar();
            btnAgregar.Enabled = true;
            txtProducto.Focus();
        }

        private void dgvDetalle_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Verifica que no sea el encabezado
            {
                DataGridViewRow filaSeleccionada = dgvDetalle.Rows[e.RowIndex];
                if (filaSeleccionada.Cells[0].Value != null && filaSeleccionada.Cells[1].Value != null)
                {
                    posicion = dgvDetalle.CurrentRow.Index;
                    txtProducto.Text = dgvDetalle[1, posicion].Value.ToString();
                    txtDescripcion.Text = dgvDetalle[2, posicion].Value.ToString();
                    txtPrecio.Text = dgvDetalle[3, posicion].Value.ToString();
                    txtStock.Text = dgvDetalle[4, posicion].Value.ToString();
                    cbxCategoria.Text = dgvDetalle[5, posicion].Value.ToString();
                    btnAgregar.Enabled = false;
                    btnModificar.Enabled = true;
                    btnEliminar.Enabled = true;
                    txtProducto.Focus();
                    
                }
                else
                {
                    MessageBox.Show("La fila seleccionada contiene celdas vacias.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}
