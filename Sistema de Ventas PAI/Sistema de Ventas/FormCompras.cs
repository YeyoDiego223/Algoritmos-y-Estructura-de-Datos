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
    public partial class FormCompras : Form
    {
        int posicion, idCompra = 0, idProducto = 0, stockProducto, idProveedor = 0, lastID = 0, newID;
        string proveedor, fecha, producto, numeroPattern = @"^[1-9]\d*$", connectionString = ConfigurationManager.ConnectionStrings["SQLiteDB"].ConnectionString;
        double precio, cantidad, total;
        public FormCompras()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            InitializeComponent();
            dtpFecha.Focus();
            limpiar();
            llenarDataGridView();
            cbxProveedor.DropDownStyle = ComboBoxStyle.DropDownList;
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
            }
            else if (Form1.cargo == "Cajero")
            {
                pctbxClientes.Enabled = false;
                pctbxClientes.Image = ConvertToGrayscale(Image.FromFile("Resources\\pcbxxClientes.png"));
                pctbxDetalleVenta.Enabled = false;
                pctbxDetalleVenta.Image = ConvertToGrayscale(Image.FromFile("Resources\\pcbxxDetalleVenta.png"));               
                pctbxProveedores.Enabled = false;
                pctbxProveedores.Image = ConvertToGrayscale(Image.FromFile("Resources\\pcbxxProveedores.png"));
                pctbxProductos.Enabled = false;
                pctbxProductos.Image = ConvertToGrayscale(Image.FromFile("Resources\\pcbxxProductos.png"));
            }
        }

        void eliminarbase()
        {
            posicion = dgvDetalle.CurrentRow.Index;
            var id = dgvDetalle[0, posicion].Value.ToString();
            // Conexión
            string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
            string queryEliminarCompras = "DELETE FROM Compras WHERE ID_Compra = @idCompra";
            string queryEliminarDetalle = "DELETE FROM Detalles_Compra WHERE ID_Compra = @idCompra";

            try
            {
                // 1. Conexión y comando
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();
                    using (SqliteCommand cmd = new SqliteCommand(queryEliminarDetalle, conn))
                    {
                        // 2. Parámetros para evitar SQL Injection
                        cmd.Parameters.AddWithValue("@idCompra", id);
                        cmd.ExecuteNonQuery();
                    }
                    // Luego eliminar el registro en Compras
                    using (SqliteCommand cmdCompra = new SqliteCommand(queryEliminarCompras, conn))
                    {
                        cmdCompra.Parameters.AddWithValue("@idCompra", idCompra);
                        cmdCompra.ExecuteNonQuery();
                    }

                    MessageBox.Show("Compra eliminada correctamente");
                    dgvDetalle.Rows.RemoveAt(posicion);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar la compra: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void modificarbase()
        {
            posicion = dgvDetalle.CurrentRow.Index;
            idCompra = Convert.ToInt32(dgvDetalle[0, posicion].Value.ToString());
            fecha = dtpFecha.Value.ToString("yyyy-MM-dd");
            proveedor = cbxProveedor.Text;
            producto = cbxProducto.Text;
            if (string.IsNullOrWhiteSpace(txtCantidad.Text))
            {
                MessageBox.Show("Cantidad no debe estar vacío", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCantidad.Text = "";
                txtCantidad.Focus();
                return;
            }
            cantidad = Convert.ToDouble(txtCantidad.Text);
            precio = Convert.ToDouble(txtPrecio.Text);
            total = Convert.ToDouble(txtTotal.Text);

            // Rstriccion campos vacíos
            if (fecha == "" || proveedor == "" || producto == "" || string.IsNullOrWhiteSpace(txtCantidad.Text) || string.IsNullOrWhiteSpace(txtPrecio.Text) || string.IsNullOrWhiteSpace(txtTotal.Text))
            {
                MessageBox.Show("No debe haber campos vacios", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            

            string queryObtenerIDProveedor = "SELECT ID_Proveedor FROM Proveedores WHERE Nombre_Proveedor = @nombreProveedor";
            string queryObtenerIDProducto = "SELECT ID_Producto, Stock FROM Productos WHERE Nombre_Producto = @nombreProducto";
            string queryObtenerCantidadAnterior = "SELECT Cantidad FROM Detalles_Compra WHERE ID_Compra = @idCompra AND ID_Producto = @idProducto";
            string queryActualizarCompra = "UPDATE Compras SET ID_Proveedor = @idProveedor, Fecha_Compra = @fechacompra, Total_Compra = @totalcompra WHERE ID_Compra = @idCompra";
            string queryActualizarDetalleCompra = "UPDATE Detalles_Compra SET ID_Compra = @idCompra, ID_Producto = @idProducto, Cantidad = @cantidad, Precio_Compra = @precio WHERE ID_Compra = @idCompra";
            string queryActualizarStockProducto = "UPDATE Productos SET Stock = @nuevoStock WHERE ID_Producto = @idProducto";

            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Obtener el ID_Proveedor
                    using (SqliteCommand cmdObtenerIDProveedor = new SqliteCommand(queryObtenerIDProveedor, conn))
                    {
                        cmdObtenerIDProveedor.Parameters.AddWithValue("@nombreProveedor", proveedor);
                        object result = cmdObtenerIDProveedor.ExecuteScalar(); // Ejecutar y obtener el resultado

                        if (result != null)
                        {
                            idProveedor = Convert.ToInt32(result);
                        }
                        else
                        {
                            MessageBox.Show("El proveedor seleccionado no existe en la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    // Obtener la cantidad anterior agregada al producto (si existe)
                    double cantidadAnterior = 0;
                    using (SqliteCommand cmdObtenerCantidadAnterior = new SqliteCommand(queryObtenerCantidadAnterior, conn))
                    {
                        cmdObtenerCantidadAnterior.Parameters.AddWithValue("@idCompra", idCompra);
                        cmdObtenerCantidadAnterior.Parameters.AddWithValue("@idProducto", idProducto);
                        object result = cmdObtenerCantidadAnterior.ExecuteScalar();

                        if (result != null)
                        {
                            cantidadAnterior = Convert.ToDouble(result);
                        }
                    }

                    // Calcular la diferencia entre la cantidad actual y la anterior
                    double diferencia = cantidad - cantidadAnterior;
                    double nuevoStock = stockProducto + diferencia;

                    // Comprobar si hay sufiente stock
                    if (nuevoStock < 0)
                    {
                        MessageBox.Show("No hay sufiente stock para realizar esta operación", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Actualizar el stock del producto en la tabla Productos
                    using (SqliteCommand cmdActualizarStock = new SqliteCommand(queryActualizarStockProducto, conn))
                    {
                        cmdActualizarStock.Parameters.AddWithValue("@nuevoStock", nuevoStock);
                        cmdActualizarStock.Parameters.AddWithValue("@idProducto", idProducto);
                        cmdActualizarStock.ExecuteNonQuery();
                    }

                    // Actualizar tabla Compra
                    using (SqliteCommand cmdActualizarCompra = new SqliteCommand(queryActualizarCompra, conn))
                    {
                        cmdActualizarCompra.Parameters.AddWithValue("@idProveedor", idProveedor);
                        cmdActualizarCompra.Parameters.AddWithValue("@fechacompra", fecha);
                        cmdActualizarCompra.Parameters.AddWithValue("@totalcompra", total);
                    }

                    // Actualizar tabla DetalleCompra
                    using (SqliteCommand cmdActualizarDetalleCompra = new SqliteCommand(queryActualizarDetalleCompra, conn))
                    {
                        cmdActualizarDetalleCompra.Parameters.AddWithValue("@idCompra", idCompra);
                        cmdActualizarDetalleCompra.Parameters.AddWithValue("@idProducto", idProducto);
                        cmdActualizarDetalleCompra.Parameters.AddWithValue("@cantidad", cantidad);
                        cmdActualizarDetalleCompra.Parameters.AddWithValue("@precio", precio);

                        // Ejecutar la consulta de actualización
                        int rowsAffected = cmdActualizarDetalleCompra.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Compra actualizada correctamente");
                            dgvDetalle[1, posicion].Value = dtpFecha.Text;
                            dgvDetalle[2, posicion].Value = cbxProveedor.Text;
                            dgvDetalle[3, posicion].Value = cbxProducto.Text;
                            dgvDetalle[4, posicion].Value = txtCantidad.Text;
                            dgvDetalle[5, posicion].Value = txtPrecio.Text;
                            dgvDetalle[6, posicion].Value = txtTotal.Text;
                            var colCodigo = dgvDetalle[0, posicion].Value.ToString();
                        }
                        else
                        {
                            MessageBox.Show("No se encontró el registro de compra a modificar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            cbxProveedor.Focus();
        }

        void agregarbase()
        {
            fecha = dtpFecha.Value.ToString("yyyy-MM-dd");
            proveedor = cbxProveedor.Text;
            producto = cbxProducto.Text;
            total = Convert.ToDouble(txtTotal.Text);
            double.TryParse(txtPrecio.Text, out precio);
            // Restricción de cantidad vacio
            if (string.IsNullOrWhiteSpace(txtCantidad.Text))
            {
                MessageBox.Show("Cantidad no debe estar vacío", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCantidad.Text = "";
                return;
            }
            double.TryParse(txtCantidad.Text, out cantidad);
            double.TryParse(txtTotal.Text, out total);

            // Rstriccion campos vacíos
            if (fecha == "" || proveedor == "" || producto == "" || string.IsNullOrWhiteSpace(txtCantidad.Text) || string.IsNullOrWhiteSpace(txtPrecio.Text) || string.IsNullOrWhiteSpace(txtTotal.Text))
            {
                MessageBox.Show("No debe haber campos vacios", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verificar y convertir el contenido a txtPrecio
            if (!double.TryParse(txtPrecio.Text, out precio))
            {
                MessageBox.Show("El precio debe ser un número valido.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTotal.Text = "";
                return;
            }


            // Verificar y convertir el contenido en txtCantidad
            if (double.TryParse(txtCantidad.Text, out cantidad))
            {

            }
            else
            {
                MessageBox.Show("La cantidad tiene que ser un número valido.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTotal.Text = "";
                return;
            }

            string queryObtenerIDProducto = "SELECT ID_Producto, Stock FROM Productos WHERE Nombre_Producto = @nombreProducto";
            string queryObtenerIDCompra = "SELECT ID_Compra FROM Compras WHERE Fecha_Compra = @fecha AND ID_Proveedor = @idProveedor AND Total_Compra = @total";
            string queryObtenerIDProveedor = "SELECT ID_Proveedor FROM Proveedores WHERE Nombre_Proveedor = @nombreProveedor";
            string queryActualizarStockProducto = "UPDATE Productos SET Stock = Stock + @cantidad WHERE ID_Producto = @idProducto";
            string queryAgregarCompra = "INSERT INTO Compras (ID_Proveedor, Fecha_Compra, Total_Compra) VALUES (@idProveedor, @fecha, @total)";
            string queryAgregarDetalleCompra = "INSERT INTO Detalles_Compra (ID_Compra, ID_Producto, Cantidad, Precio_Compra) VALUES (@idCompra, @idProducto, @cantidad, @precio)";

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
                            MessageBox.Show("El producto seleccionado no existe en la base de datos", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        reader.Close(); // Cerrar el DataReader
                    }

                    // Obtener ID del Proveedor
                    using (SqliteCommand cmdObtenerIDProveedor = new SqliteCommand(queryObtenerIDProveedor, conn))
                    {
                        cmdObtenerIDProveedor.Parameters.AddWithValue("@nombreProveedor", proveedor);
                        Console.WriteLine("El Proveedor: " + proveedor);
                        object ResultConsultaProveedor = cmdObtenerIDProveedor.ExecuteScalar();

                        if (ResultConsultaProveedor != null)
                        {
                            idProveedor = Convert.ToInt32(ResultConsultaProveedor);
                            Console.WriteLine("El ID Proveedor consultado: " + idProveedor);
                        }
                        else
                        {
                            MessageBox.Show("El Proveedor no existe en la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Agregar compra
                    using (SqliteCommand cmdAgregarCompra = new SqliteCommand(queryAgregarCompra, conn))
                    {
                        cmdAgregarCompra.Parameters.AddWithValue("@fecha", fecha);
                        cmdAgregarCompra.Parameters.AddWithValue("@idProveedor", idProveedor);
                        cmdAgregarCompra.Parameters.AddWithValue("@total", total);
                        cmdAgregarCompra.ExecuteNonQuery();
                    }

                    // Obtener ID de la compra
                    using (SqliteCommand cmdObtenerIDCompra = new SqliteCommand(queryObtenerIDCompra, conn))
                    {
                        cmdObtenerIDCompra.Parameters.AddWithValue("@fecha", fecha);
                        cmdObtenerIDCompra.Parameters.AddWithValue("@idProveedor", idProveedor);
                        cmdObtenerIDCompra.Parameters.AddWithValue("@total", total);
                        object ConsultaCompra = cmdObtenerIDCompra.ExecuteScalar();

                        if (ConsultaCompra != null)
                        {
                            idCompra = Convert.ToInt32(ConsultaCompra);
                        }
                        else
                        {
                            MessageBox.Show("La compra seleccionada no existe en la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Agregar detalle de compra
                    using (SqliteCommand cmdAgregarDetalleCompra = new SqliteCommand(queryAgregarDetalleCompra, conn))
                    {
                        cmdAgregarDetalleCompra.Parameters.AddWithValue("@idCompra", idCompra);
                        cmdAgregarDetalleCompra.Parameters.AddWithValue("@idProducto", idProducto);
                        cmdAgregarDetalleCompra.Parameters.AddWithValue("@cantidad", cantidad);
                        cmdAgregarDetalleCompra.Parameters.AddWithValue("@precio", precio);
                        cmdAgregarDetalleCompra.ExecuteNonQuery();  // Ejecutar inserción en Detalle Compra
                    }

                    // Actualizar el stock del producto
                    using (SqliteCommand cmdActualizarStock = new SqliteCommand(queryActualizarStockProducto, conn))
                    {
                        cmdActualizarStock.Parameters.AddWithValue("@cantidad", cantidad);
                        cmdActualizarStock.Parameters.AddWithValue("@idProducto", idProducto);
                        cmdActualizarStock.ExecuteNonQuery();  // Ejecutar la actualización del stock
                    }                                      

                    MessageBox.Show("Compra Agregada y Stock Actualizado");

                    if (dgvDetalle.Rows.Count > 0)
                    {
                        lastID = dgvDetalle.Rows.Cast<DataGridViewRow>()
                                            .Max(row => Convert.ToInt32(row.Cells[0].Value));
                    }
                    newID = lastID + 1;
                    // Actualizar DataGridView
                    dgvDetalle.Rows.Add(newID.ToString(), fecha, producto, producto, cantidad, precio, total);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al agregar comprar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conn.Close();
                }
            }
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

        void llenarComboBoxProveedores()
        {
            string queryproveedores = "SELECT Nombre_Proveedor FROM Proveedores";
            try
            {   
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    using (SqliteCommand cmd = new SqliteCommand(queryproveedores, conn))
                    {
                        conn.Open();
                        SqliteDataReader reader = cmd.ExecuteReader();
                        cbxProveedor.Items.Clear();

                        while (reader.Read())
                        {
                            cbxProveedor.Items.Add(reader["Nombre_Proveedor"].ToString());
                        }
                        if (cbxProveedor.Items.Count > 0)
                        {
                            cbxProveedor.SelectedIndex = 0;
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
            // Consulta SQL corregida
            string query = @"
        SELECT 
            c.ID_Compra AS Codigo,
            c.Fecha_Compra AS Fecha,
            proveedor.Nombre_Proveedor AS Proveedor,
            producto.Nombre_Producto AS Producto,
            d.Cantidad AS Cantidad,
            producto.Precio AS Precio,
            (producto.Precio * d.Cantidad) AS Total
        FROM Compras c
        INNER JOIN Proveedores proveedor ON c.ID_Proveedor = proveedor.ID_Proveedor
        INNER JOIN Detalles_Compra d ON c.ID_Compra = d.ID_Compra
        INNER JOIN Productos producto ON d.ID_Producto = producto.ID_Producto";

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
                                reader["Proveedor"],      // Columna: Proveedor
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
            dtpFecha.Text = "";
            cbxProveedor.Text = "";
            cbxProveedor.Items.Clear();
            cbxProducto.Text = "";
            cbxProducto.Items.Clear();
            txtPrecio.Text = "";
            txtCantidad.Text = "";
            txtTotal.Text = "";
            lblExistencia.Text = "";
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormProveedores proveedores = new FormProveedores();
            proveedores.Show();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
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

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormClientes clientes = new FormClientes();
            clientes.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            agregarbase();
            limpiar();
            cbxProveedor.Focus();                            
        }

        private void cbxProducto_Click(object sender, EventArgs e)
        {
            llenarComboBoxProductosPrecio();
        }

        private void cbxProveedor_Click_1(object sender, EventArgs e)
        {
            llenarComboBoxProveedores();
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

        private void txtPrecio_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCantidad.Text)) // Verificar si el campo no está vacío
            {
                llenartotal();
            }
        }

        private void btnModificar_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Estás seguro que deseas modificar este proveedor", "Confirmacion", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                modificarbase();               
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormPrincipal principal = new FormPrincipal();
            principal.Show();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            limpiar();
            btnAgregar.Enabled = true;
            cbxProveedor.Focus();
        }

        private void dgvDetalle_CellClick(object sender, DataGridViewCellEventArgs e)
        {            
            if (e.RowIndex >= 0) // Verifica que no sea el encabezado
            {
                DataGridViewRow filaSeleccionada = dgvDetalle.Rows[e.RowIndex];
                if (filaSeleccionada.Cells[0].Value != null && filaSeleccionada.Cells[1].Value != null)
                {
                    posicion = dgvDetalle.CurrentRow.Index;
                    dtpFecha.Text = dgvDetalle[1, posicion].Value.ToString();
                    llenarComboBoxProveedores();
                    cbxProveedor.Text = dgvDetalle[2, posicion].Value.ToString();
                    llenarComboBoxProductosPrecio();
                    cbxProducto.Text = dgvDetalle[3, posicion].Value.ToString();
                    txtCantidad.Text = dgvDetalle[4, posicion].Value.ToString();
                    txtPrecio.Text = dgvDetalle[5, posicion].Value.ToString();
                    txtTotal.Text = dgvDetalle[6, posicion].Value.ToString();
                    btnAgregar.Enabled = false;
                    btnModificar.Enabled = true;
                    btnEliminar.Enabled = true;
                    dtpFecha.Focus();
                }
                else
                {
                    MessageBox.Show("La fila seleccionada contiene celdas vacías.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
        }

        private void cbxProveedor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                dtpFecha.Focus(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void dtpFecha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                txtTotal.Focus(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void txtTotal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Verifica si se presionó la tecla Enter
            {
                btnAgregar.PerformClick(); // Pasa el foco al siguiente TextBox
                e.Handled = true; // Marca el evento como manejado
                e.SuppressKeyPress = true; // Evita el sonido de "beep"
            }
        }

        private void dgvDetalle_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvDetalle_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dgvDetalle.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetalle.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetalle.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetalle.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetalle.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetalle.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            if (e.ColumnIndex == 5 && e.Value != null || e.ColumnIndex == 6 && e.Value != null)
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

        private void cbxProveedor_Click(object sender, EventArgs e)
        {
            llenarComboBoxProveedores();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Estas seguro que deseas eliminar esta compra?", "Confirmación", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                cbxProveedor.Focus();
                eliminarbase();
                limpiar();  
            }            
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Estás seguro que quieres modificar esta compra?", "Confirmación", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {                
                modificarbase();                
                limpiar();
                cbxProveedor.Focus();
            }            
        }
    }
}
