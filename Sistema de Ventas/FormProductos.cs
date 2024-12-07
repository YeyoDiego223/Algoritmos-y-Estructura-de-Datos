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
    public partial class FormProductos : Form
    {
        int i = 1;
        int posicion;
        string producto, descripcion, precio, stock, categoria;
        public FormProductos()
        {
            InitializeComponent();
            txtProducto.Focus();
            llenarDataGridView();
            txtProducto.Focus();
            cbxCategoria.SelectedIndex = 0;
        }



        void eliminarbase()
        {
            posicion = dgvDetalle.CurrentRow.Index;
            var id = dgvDetalle[0, posicion].Value.ToString();
                // Conexion
                string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
                string query = @"DELETE FROM Productos WHERE ID_Producto = @idProducto";

                // 1. Conexion y comando
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
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
                            MessageBox.Show($"Error al eliminar producto: {ex.Message}");
                        }
                    
                }
                
            }        
         }

        void modificarbase()
        {
            posicion = dgvDetalle.CurrentRow.Index;
            var id = dgvDetalle[0, posicion].Value.ToString();
            var producto = dgvDetalle[1, posicion].Value.ToString();
            var descripcion = dgvDetalle[2, posicion].Value.ToString();
            var precio = dgvDetalle[3, posicion].Value.ToString();
            var stock = dgvDetalle[4, posicion].Value.ToString();
            var categoria = dgvDetalle[5, posicion].Value.ToString();
            // Conexión
            string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
            string query = @"
        UPDATE Productos 
        SET Nombre_Producto = @nombreProducto, 
            Descripcion = @descripcion, 
            Precio = @precio, 
            Stock = @stock, 
            Categoria = @categoria
        WHERE ID_Producto = @idProducto";

            // 1. Conexión y comando
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                // 2. Parámetros para evitar SQL Injection
                cmd.Parameters.AddWithValue("@idProducto", id);
                cmd.Parameters.AddWithValue("@nombreProducto", producto);
                cmd.Parameters.AddWithValue("@descripcion", descripcion);
                cmd.Parameters.AddWithValue("@precio", precio);
                cmd.Parameters.AddWithValue("@stock", stock);
                cmd.Parameters.AddWithValue("@categoria", categoria);

                try
                {
                    // 3. Abrír conexión y ejecutar comando
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Producto actualizado correctamente.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al actualizar producto: {ex.Message}");
                }
            }
        }


        private void llenarDataGridView()
        {
        // Cadena de conexion
        string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";

        // Consulta SQL
        string query = "Select ID_Producto, Nombre_Producto, Descripcion, Precio, Stock, Categoria FROM Productos";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Crear una nueva fila y asignar los valores manualmente
                            int rowIndex = dgvDetalle.Rows.Add();
                            dgvDetalle.Rows[rowIndex].Cells["colCodigo"].Value = reader["ID_Producto"];
                            dgvDetalle.Rows[rowIndex].Cells["colNombre"].Value = reader["Nombre_Producto"];
                            dgvDetalle.Rows[rowIndex].Cells["colDescripcion"].Value = reader["Descripcion"];
                            dgvDetalle.Rows[rowIndex].Cells["colPrecio"].Value = reader["Precio"];
                            dgvDetalle.Rows[rowIndex].Cells["colStock"].Value = reader["Stock"];
                            dgvDetalle.Rows[rowIndex].Cells["colCategoria"].Value = reader["Categoria"];
                            i = i + 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al llenar el DataGridView: {ex.Message}");
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
            FormVentas ventas = new FormVentas();
            ventas.Show();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormDetalles detalles = new FormDetalles();
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
            producto = txtProducto.Text;
            descripcion = txtDescripcion.Text;
            precio = txtPrecio.Text;
            stock = txtStock.Text;
            categoria = cbxCategoria.Text;
            if (producto == "" || descripcion == "" || precio == "" || stock == "" || categoria == "")
            {
                MessageBox.Show("No hay datos");
            }
            else
            {
                dgvDetalle.Rows.Add(i + "", producto, descripcion, precio, stock, categoria);
                i = i + 1;
                string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
                string query = "INSERT INTO Productos (Nombre_Producto, Descripcion, Precio, Stock, Categoria) " +
                       "VALUES (@nombre, @descripcion, @precio, @stock, @categoria)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    decimal precioDecimal;
                    int stockInt;

                    if (decimal.TryParse(precio, out precioDecimal) && int.TryParse(stock, out stockInt))
                    {
                        cmd.Parameters.AddWithValue("@stock", stock);
                        cmd.Parameters.AddWithValue("@categoria", categoria);
                    }
                    else
                    {
                        MessageBox.Show("Precio o Stock no tienen un formato valido");
                        return; //Detener la ejecucion
                    }
                    // Asignar valores a los parametros
                    cmd.Parameters.AddWithValue("@nombre", producto);
                    cmd.Parameters.AddWithValue("@descripcion", descripcion);
                    cmd.Parameters.AddWithValue("@precio", precio);
                    

                    // Ejecutar la consulta
                    try
                    {
                        // 3. Abrír conexión y ejecutar comando
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Producto agregado correctamente.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al agregar producto: {ex.Message}");
                    }
                }
                limpiar();
                txtProducto.Focus();
            }
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

        private void btnModificar_Click(object sender, EventArgs e)
        {
            producto = txtProducto.Text;
            descripcion = txtDescripcion.Text;
            precio = txtPrecio.Text;
            stock = txtStock.Text;
            categoria = cbxCategoria.Text;
            
            dgvDetalle[1, posicion].Value = txtProducto.Text;
            dgvDetalle[2, posicion].Value = txtDescripcion.Text;
            dgvDetalle[3, posicion].Value = txtPrecio.Text;
            dgvDetalle[4, posicion].Value = txtStock.Text;
            dgvDetalle[5, posicion].Value = cbxCategoria.Text;   
            modificarbase();
            limpiar();
            txtProducto.Focus();
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
                    MessageBox.Show("La fila seleccionada contiene celdas vacias.");
                }
            }
        }
    }
}
