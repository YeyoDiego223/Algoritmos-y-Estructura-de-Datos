using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Sistema_de_Ventas
{

    public partial class FormVentas : Form
    {
        string numeroPattern = @"^[1-9]\d*$", connectionString = ConfigurationManager.ConnectionStrings["SQLiteDB"].ConnectionString;
        string fecha, cliente, producto;
        double precio,  cantidad, total, cantidadAnterior, diferencia, nuevoStock;
        int posicion, idVenta = 0, idProducto = 0, stockProducto, idCliente = 0, lastID = 0, newID;
        public FormVentas()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            InitializeComponent();
            dtpfecha.Focus();
            limpiar();
            cbxCliente.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxProducto.DropDownStyle = ComboBoxStyle.DropDownList;
            txtTotal.ReadOnly = true;
            txtPrecio.ReadOnly = true;
            lblExistencia.Text = "";
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
                pctbxProductos.Enabled = false;
                pctbxProductos.Image = ConvertToGrayscale(Image.FromFile("Resources\\pcbxxProductos.png"));
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

        void eliminarbase()
        {
            idVenta = Convert.ToInt32(dgvDetalle[0, posicion].Value);           
            // Conexión
            string queryEliminarVenta = "DELETE FROM Ventas WHERE ID_Venta = @idVenta";
            string queryEliminarDetalle = "DELETE FROM Detalles_Venta WHERE ID_Venta = @idVenta";

            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();
                    // Primero eliminar los registro relacionados en Detalles_Venta
                    using (SqliteCommand cmdDetalle = new SqliteCommand(queryEliminarDetalle, conn))
                    {
                        // 2. Parámetros para evitar SQL Injection
                        cmdDetalle.Parameters.AddWithValue("@idVenta", idVenta);
                        cmdDetalle.ExecuteNonQuery();                       
                    }
                    // Luego eliminar el registro en Ventas
                    using (SqliteCommand cmdVenta = new SqliteCommand(queryEliminarVenta, conn))
                    {
                        cmdVenta.Parameters.AddWithValue("@idVenta", idVenta);
                        cmdVenta.ExecuteNonQuery();
                    }

                    MessageBox.Show("Venta eliminada correctamente.");
                    dgvDetalle.Rows.RemoveAt(posicion);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar la venta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }   
        }

        void modificarbase()
        {
            posicion = dgvDetalle.CurrentRow.Index;
            idVenta = Convert.ToInt32(dgvDetalle[0, posicion].Value.ToString());
            fecha = dtpfecha.Value.ToString("yyyy-MM-dd");
            cliente = cbxCliente.Text;
            producto = cbxProducto.Text;
            // Restricción de campo vacío de cantidad
            if (string.IsNullOrWhiteSpace(txtCantidad.Text))
            {
                MessageBox.Show("Cantidad no debe estar vacío", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCantidad.Text = "";
                return;
            }
            cantidad = Convert.ToDouble(txtCantidad.Text);
            precio = Convert.ToDouble(txtPrecio.Text);
            total = Convert.ToDouble(txtTotal.Text);
            // Rstriccion campos vacíos
            if (fecha == "" || cliente == "" || producto == "" || string.IsNullOrWhiteSpace(txtCantidad.Text) || string.IsNullOrWhiteSpace(txtPrecio.Text) || string.IsNullOrWhiteSpace(txtTotal.Text))
            {
                MessageBox.Show("No debe haber campos vacios", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string queryObtenerIDCliente = "SELECT ID_Cliente FROM Clientes WHERE Nombre = @nombreCliente";
            string queryObtenerIDProducto = "SELECT ID_Producto, Stock FROM Productos WHERE Nombre_Producto = @nombreProducto";
            string queryActualizarVenta = "UPDATE Ventas SET Fecha_Venta = @fechaventa, ID_Cliente = @idCliente, Total_Venta = @totalventa WHERE ID_Venta = @idVenta";
            string queryActualizarDetalleVenta = "UPDATE Detalles_Venta SET ID_Venta = @idVenta, ID_Producto = @idProducto, Cantidad = @cantidad, Precio = @precio WHERE ID_Venta = @idVenta";
            string queryActualizarStockProducto = "UPDATE Productos SET Stock = @nuevoStock WHERE ID_Producto = @idProducto";

            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Obtener el ID_Cliente
                    using (SqliteCommand cmdObtenerIDCliente = new SqliteCommand(queryObtenerIDCliente, conn))
                    {
                        cmdObtenerIDCliente.Parameters.AddWithValue("@nombreCliente", cliente);
                        object result = cmdObtenerIDCliente.ExecuteScalar(); // Ejecutar y obtener el resultado

                        if (result != null)
                        {
                            idCliente = Convert.ToInt32(result);
                        }
                        else
                        {
                            MessageBox.Show("El cliente seleccionado no existe en la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Obtener ID Producto

                    using (SqliteCommand cmdObtenerIDProducto = new SqliteCommand(queryObtenerIDProducto, conn))
                    {
                        cmdObtenerIDProducto.Parameters.AddWithValue("@nombreProducto", producto);
                        SqliteDataReader reader = cmdObtenerIDProducto.ExecuteReader();

                        if (reader.Read())
                        {
                            idProducto = Convert.ToInt32(reader["ID_Producto"]);
                            stockProducto = Convert.ToInt32(reader["Stock"]);
                        }
                        else
                        {
                            MessageBox.Show("El producto seleccionado no existe en la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        reader.Close(); // Cerrar el DataReader
                    }

                    // Obtener la cantidad vendida anterior agregada al producto (si existe)
                    cantidadAnterior = Convert.ToDouble(dgvDetalle[4, posicion].Value.ToString());
                    // Calcular la diferencia entre la cantidad actual y la anterior
                    diferencia = stockProducto + cantidadAnterior;
                    nuevoStock = diferencia - cantidad;
                    // Actualizar el stock del producto en la tabla Productos
                    using (SqliteCommand cmdActualizarStock = new SqliteCommand(queryActualizarStockProducto, conn))
                    {
                        cmdActualizarStock.Parameters.AddWithValue("@nuevoStock", nuevoStock);
                        cmdActualizarStock.Parameters.AddWithValue("@idProducto", idProducto);
                        cmdActualizarStock.ExecuteNonQuery();
                    }

                    // Actualizar tabla Venta

                    using (SqliteCommand cmdActualizarVenta = new SqliteCommand(queryActualizarVenta, conn))
                    {
                        cmdActualizarVenta.Parameters.AddWithValue("@idVenta", idVenta);
                        cmdActualizarVenta.Parameters.AddWithValue("@fechaventa", fecha);
                        cmdActualizarVenta.Parameters.AddWithValue("@idCliente", idCliente);
                        cmdActualizarVenta.Parameters.AddWithValue("@totalventa", total);
                    }

                    // Actualizar tabla DetalleVenta
                    using (SqliteCommand cmdActualizarDetalleVenta = new SqliteCommand(queryActualizarDetalleVenta, conn))
                    {
                        cmdActualizarDetalleVenta.Parameters.AddWithValue("@idVenta", idVenta);
                        cmdActualizarDetalleVenta.Parameters.AddWithValue("@idProducto", idProducto);
                        cmdActualizarDetalleVenta.Parameters.AddWithValue("@cantidad", cantidad);
                        cmdActualizarDetalleVenta.Parameters.AddWithValue("@precio", precio);

                        // Ejecutar la consulta de actualización
                        int rowsAffected = cmdActualizarDetalleVenta.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Venta actualizada correctamente");
                            dgvDetalle[1, posicion].Value = dtpfecha.Text;
                            dgvDetalle[2, posicion].Value = cbxCliente.Text;
                            dgvDetalle[3, posicion].Value = cbxProducto.Text;
                            dgvDetalle[4, posicion].Value = txtCantidad.Text;
                            dgvDetalle[5, posicion].Value = txtPrecio.Text;
                            dgvDetalle[6, posicion].Value = txtTotal.Text;
                            var colCodigo = dgvDetalle[0, posicion].Value.ToString();
                        }
                        else
                        {
                            MessageBox.Show("No se encontró el registro de venta a modificar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show($"Error al actualizar compra: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conn.Close();
                }
            }
            limpiar();
            cbxCliente.Focus();
        }

        void agregarbase()
        {
            fecha = dtpfecha.Value.ToString("yyyy-MM-dd");
            cliente = cbxCliente.Text;
            producto = cbxProducto.Text;
            total = Convert.ToDouble(txtTotal.Text);
            double.TryParse(txtPrecio.Text, out precio);
            double.TryParse(txtCantidad.Text, out cantidad);
            

            if (fecha == "" || cliente == "" || producto == "")
            {
                MessageBox.Show("No debe dejar campos vacíos", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCantidad.Text))
            {
                MessageBox.Show("Cantidad no debe estar vacío", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCantidad.Text = "";
                txtCantidad.Focus();
                return;
            }

            // Verificar y convertir el contenido a txtPrecio
            if (!double.TryParse(txtPrecio.Text, out precio))
            {
                MessageBox.Show("El precio debe ser un número valido.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }           

            string queryObtenerIDProducto = "SELECT ID_Producto, Stock FROM Productos WHERE Nombre_Producto = @nombreProducto";
            string queryObtenerIDVenta = "SELECT ID_Venta FROM Ventas WHERE Fecha_Venta = @fecha AND ID_Cliente = @idCliente AND Total_Venta = @total";
            string queryAgregarDetalleVenta = "INSERT INTO Detalles_Venta (ID_Venta, ID_Producto, Cantidad, Precio) VALUES (@idVenta, @idProducto, @cantidad, @precio)";
            string queryActualizarStockProducto = "UPDATE Productos SET Stock = Stock - @cantidad WHERE ID_Producto = @idProducto AND Stock >= @cantidad;";
            string queryObtenerIDCliente = "SELECT ID_Cliente FROM Clientes WHERE Nombre = @nombrecliente";
            string queryAgregarVenta = "INSERT INTO Ventas (Fecha_Venta, ID_Cliente, Total_Venta)" + "VALUES (@fecha, @idCliente, @total)";

            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                try
                {
                    conn.Open();                    
                    // Obtener ID del producto y su stock
                    using (SqliteCommand cmdObtenerIDProducto = new SqliteCommand(queryObtenerIDProducto, conn))
                    {
                        cmdObtenerIDProducto.Parameters.AddWithValue("@nombreProducto", producto);
                        SqliteDataReader reader = cmdObtenerIDProducto.ExecuteReader();

                        if (reader.Read())
                        {
                            idProducto = Convert.ToInt32(reader["ID_Producto"]);
                            stockProducto = Convert.ToInt32(reader["Stock"]);
                        }
                        else
                        {
                            MessageBox.Show("El producto seleccionado no existe en la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        reader.Close(); // Cerrar el DataReader
                    }

                    // Comprobar si hay suficiente stock
                    if (stockProducto < cantidad)
                    {
                        MessageBox.Show("No hay suficiente stock para la venta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Consultar ID Cliente
                    using (SqliteCommand cmdObtenerIDCliente = new SqliteCommand(queryObtenerIDCliente, conn))
                    {
                        cmdObtenerIDCliente.Parameters.AddWithValue("@nombrecliente", cliente);
                        object result = cmdObtenerIDCliente.ExecuteScalar();

                        if (result != null)
                        {
                            idCliente = Convert.ToInt32(result);
                        }
                        else
                        {
                            MessageBox.Show("El cliente seleccionado no existe en la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Agregar Venta
                    using (SqliteCommand cmdAgregarVenta = new SqliteCommand(queryAgregarVenta, conn))
                    {
                        cmdAgregarVenta.Parameters.AddWithValue("@fecha", fecha);
                        cmdAgregarVenta.Parameters.AddWithValue("@idCliente", idCliente);
                        cmdAgregarVenta.Parameters.AddWithValue("@total", total);
                        cmdAgregarVenta.ExecuteNonQuery();
                    }

                    // Obtener ID de la venta
                    using (SqliteCommand cmdObtenerIDVenta = new SqliteCommand(queryObtenerIDVenta, conn))
                    {
                        cmdObtenerIDVenta.Parameters.AddWithValue("@fecha", fecha);
                        cmdObtenerIDVenta.Parameters.AddWithValue("@idCliente", idCliente);
                        cmdObtenerIDVenta.Parameters.AddWithValue("@total", total);
                        object resultVenta = cmdObtenerIDVenta.ExecuteScalar();

                        if (resultVenta != null)
                        {
                            idVenta = Convert.ToInt32(resultVenta);
                        }
                        else
                        {
                            MessageBox.Show("La venta seleccionada no existe en la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Agregar detalle de venta
                    using (SqliteCommand cmdAgregarDetalleVenta = new SqliteCommand(queryAgregarDetalleVenta, conn))
                    {
                        cmdAgregarDetalleVenta.Parameters.AddWithValue("@idVenta", idVenta);
                        cmdAgregarDetalleVenta.Parameters.AddWithValue("@idProducto", idProducto);
                        cmdAgregarDetalleVenta.Parameters.AddWithValue("@cantidad", cantidad);
                        cmdAgregarDetalleVenta.Parameters.AddWithValue("@precio", precio);
                        cmdAgregarDetalleVenta.ExecuteNonQuery();  // Ejecutar inserción en Detalles_Venta
                    }

                    // Actualizar el stock del producto
                    using (SqliteCommand cmdActualizarStock = new SqliteCommand(queryActualizarStockProducto, conn))
                    {
                        cmdActualizarStock.Parameters.AddWithValue("@cantidad", cantidad);
                        cmdActualizarStock.Parameters.AddWithValue("@idProducto", idProducto);
                        cmdActualizarStock.ExecuteNonQuery();  // Ejecutar la actualización del stock
                    }                                    

                    MessageBox.Show("Venta Agregada y Stock Actualizado");

                    if (dgvDetalle.Rows.Count > 0)
                    {
                        lastID = dgvDetalle.Rows.Cast<DataGridViewRow>()
                                            .Max(row => Convert.ToInt32(row.Cells[0].Value));
                    }
                    newID = lastID + 1;
                    // Actualizar DataGridView
                    dgvDetalle.Rows.Add(newID.ToString(), fecha, cliente, producto, cantidad, precio, total);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al agregar venta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conn.Close();
                }
            }
            limpiar();
            dtpfecha.Focus();
        }       

        void llenartotal()
        {
            double.TryParse(txtPrecio.Text, out precio);
            double.TryParse(txtCantidad.Text, out cantidad);
            double.TryParse(txtTotal.Text, out total);
            if (!Regex.IsMatch(txtCantidad.Text, numeroPattern))
            {
                MessageBox.Show("La cantidad debe ser un número válido.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCantidad.Focus();
                txtCantidad.Text = "";
                return;
            }
            // Calcular el total
            total = precio * cantidad;
            txtTotal.Text = total.ToString();
        }

        void llenarExistencia()
        {
            string query = "SELECT Stock FROM Productos WHERE Nombre_Producto = @nombreProducto";

            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    // Obtener el producto seleccionado del Combobox
                    producto = cbxProducto.Text;
                    cmd.Parameters.AddWithValue("@nombreProducto", producto);

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        lblExistencia.Text = $"En existencia: {result.ToString()}";
                    }
                    else
                    {
                        lblExistencia.Text = $"En existencia: {result.ToString()}";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener existencia: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void llenarPrecio()
        {
            string precio = @"SELECT Precio FROM Productos WHERE Nombre_Producto = @nombreProducto";
            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    using (SqliteCommand cmd = new SqliteCommand(precio, conn))
                    {
                        producto = cbxProducto.Text;
                        cmd.Parameters.AddWithValue("@nombreProducto", producto);
                        conn.Open();
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            txtPrecio.Clear();
                            txtPrecio.Text = result.ToString();
                        }
                        else
                        {
                            MessageBox.Show("El Producto seleccionado no existe en la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtPrecio.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener el Precio {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void llenarComboBoxProductosPrecio()
        {
            string productos = "SELECT Nombre_Producto FROM Productos";
            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    using (SqliteCommand cmd = new SqliteCommand(productos, conn))
                    {
                        conn.Open();
                        SqliteDataReader reader = cmd.ExecuteReader();
                        cbxProducto.Items.Clear();

                        while (reader.Read())
                        {
                            cbxProducto.Items.Add(reader["Nombre_Producto"].ToString());
                        }
                        if (cbxProducto.Items.Count > 0)
                        {
                            cbxProducto.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al llenar el ComboBox: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void llenarComboBoxCliente()
        {
            string cliente = "SELECT Nombre FROM Clientes";
            try
            {   
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    using (SqliteCommand cmd = new SqliteCommand(cliente, conn))
                    {
                        conn.Open();
                        SqliteDataReader reader = cmd.ExecuteReader();
                        cbxCliente.Items.Clear();

                        while (reader.Read())
                        {
                            cbxCliente.Items.Add(reader["Nombre"].ToString());
                        }
                        if (cbxCliente.Items.Count > 0)
                        {
                            cbxCliente.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al llenar ComboBox: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void llenarDataGridView()
        {
            // Consulta SQL corregida
            string query = @"
        SELECT 
            v.ID_Venta AS Codigo,
            v.Fecha_Venta AS Fecha,
            c.Nombre AS Cliente,
            p.Nombre_Producto AS Producto,
            d.Cantidad AS Cantidad,
            p.Precio AS Precio,
            (p.Precio * d.Cantidad) AS Total
        FROM Ventas v
        INNER JOIN Clientes c ON v.ID_Cliente = c.ID_Cliente
        INNER JOIN Detalles_Venta d ON v.ID_Venta = d.ID_Venta
        INNER JOIN Productos p ON d.ID_Producto = p.ID_Producto";

            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();
                    SqliteCommand cmd = new SqliteCommand(query, conn);

                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        dgvDetalle.Rows.Clear();

                        while (reader.Read())
                        {
                            dgvDetalle.Rows.Add(
                                reader["Codigo"],       // Columna: Código
                                reader["Fecha"],        // Columna: Fecha
                                reader["Cliente"],      // Columna: Cliente
                                reader["Producto"],     // Columna: Producto
                                reader["Cantidad"],     // Columna: Cantidad
                                reader["Precio"],       // Columna: Precio
                                reader["Total"]
                                );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al llenar el DataGridView: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void limpiar()
        {
            btnAgregar.Enabled = true;
            btnModificar.Enabled = false;
            btnEliminar.Enabled = false;
            dtpfecha.Text = "";
            cbxCliente.Text = "";
            cbxCliente.Items.Clear();
            cbxProducto.Text = "";
            cbxProducto.Items.Clear();
            txtPrecio.Text = "";
            txtCantidad.Text = "";
            txtTotal.Text = "";
            lblExistencia.Text = "";
        }

        private void dgvDetalle_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormCompras compras = new FormCompras();
            compras.Show();
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
            frmEstadisticas detalles = new frmEstadisticas();
            detalles.Show();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            agregarbase();            
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Estás seguro que deseas modificar este producto", "Confirmación", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                fecha = dtpfecha.Text;
                cliente = cbxCliente.Text;
                total = Convert.ToDouble(txtTotal.Text);
                modificarbase();
                limpiar();
                dtpfecha.Focus();
            }            
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Estás seguro que deseas eliminar este producto?", "Confirmación", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                dtpfecha.Focus();
                eliminarbase();
            }
            
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            limpiar();
            btnAgregar.Enabled = true;
            dtpfecha.Focus();
        }

        private void dgvDetalle_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void txtProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                cbxCliente.Focus(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtPrecio_TextChanged(object sender, EventArgs e)
        {
            
            if (!string.IsNullOrEmpty(txtCantidad.Text)) // Verificar si el campo no está vacío
            {
                llenartotal();
            }
        }

        private void txtPrecio_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                txtTotal.Focus(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtCantidad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                btnAgregar.PerformClick(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void cbxProducto_Click(object sender, EventArgs e)
        {
            llenarComboBoxProductosPrecio();
        }

        private void btnModificar_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Estás seguro que deseas modificar este proveedor", "Confirmacion", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                modificarbase();                
            }
        }

        private void cbxProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            llenarPrecio();
            llenarExistencia();
        }

        private void txtCantidad_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCantidad.Text)) // Verificar si el campo no está vacío
            {
                llenartotal();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormProductos configuracion = new FormProductos();
            configuracion.Show();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormPrincipal principal = new FormPrincipal();
            principal.Show();
        }

        private void dgvDetalle_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dgvDetalle.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetalle.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetalle.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetalle.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetalle.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetalle.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            if (e.ColumnIndex == 5 && e.Value !=null || e.ColumnIndex == 6 && e.Value !=null)
            {
                // Formatear la celda para mostrar el signo del colar
                e.Value = "$" + e.Value.ToString();
                e.FormattingApplied = true;
            }
            if (dgvDetalle.Columns[e.ColumnIndex].Name == "colFecha")
            {
                if (e.Value != null && e.Value != DBNull.Value)
                {
                    try
                    {
                        // Convierte el valor a DateTime y formatea
                        DateTime fecha = Convert.ToDateTime(e.Value);
                        e.Value = fecha.ToString("yyyy-MM--dd");
                        e.FormattingApplied = true; // Indica que el formato fue aplicado
                    }
                    catch
                    {
                        e.FormattingApplied = false; // Si hay un error, no se aplica el formato
                    }
                }
            }
        }

        private void cbxCliente_Click(object sender, EventArgs e)
        {
            llenarComboBoxCliente();
        }

        private void dgvDetalle_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormClientes clientes = new FormClientes();
            clientes.Show();
        }

        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Verifica que no sea el encabezado
            {
                DataGridViewRow filaSeleccionada = dgvDetalle.Rows[e.RowIndex];
                if (filaSeleccionada.Cells[0].Value != null && filaSeleccionada.Cells[1].Value != null)
                {
                    posicion = dgvDetalle.CurrentRow.Index;
                    dtpfecha.Text = dgvDetalle[1, posicion].Value.ToString();
                    llenarComboBoxCliente();
                    cbxCliente.Text = dgvDetalle[2, posicion].Value.ToString();
                    llenarComboBoxProductosPrecio();
                    cbxProducto.Text = dgvDetalle[3, posicion].Value.ToString();
                    txtCantidad.Text = dgvDetalle[4, posicion].Value.ToString();
                    txtPrecio.Text = dgvDetalle[5, posicion].Value.ToString();
                    txtTotal.Text = dgvDetalle[6, posicion].Value.ToString();
                    btnAgregar.Enabled = false;
                    btnModificar.Enabled = true;
                    btnEliminar.Enabled = true;
                    dtpfecha.Focus();
                }
                else
                {
                    MessageBox.Show("La fila seleccionada contiene celdas vacías.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }   
        }
    }
}
