using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Sistema_de_Ventas
{

    public partial class FormVentas : Form
    {
        string fecha, cliente, total;
        int posicion;
        int i = 1;
        public FormVentas()
        {
            InitializeComponent();
            llenarDataGridView();
            dtpfecha.Focus();
            limpiar();
        }

        void eliminarbase()
        {
            fecha = dtpfecha.Value.ToString("yyyy-MM-dd");
            // Conexión
            string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
            string query = @"DELETE FROM Ventas WHERE Fecha_Venta = @fechaventa";

            // 1. Conexión y comando
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // 2. Parámetros para evitar SQL Injection
                    cmd.Parameters.AddWithValue("@fechaventa", fecha);

                    try
                    {
                        // 3. Abrír conexión y ejecutar comando
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Venta eliminada correctamente");
                        dgvDetalle.Rows.RemoveAt(posicion);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar venta: {ex.Message}");
                    }
                }
            }
        }

        void modificarbase()
        {
            posicion = dgvDetalle.CurrentRow.Index;
            var idVenta = dgvDetalle[0, posicion].Value.ToString();
            fecha = dtpfecha.Value.ToString("yyyy-MM-dd");
            cliente = dgvDetalle[2, posicion].Value.ToString();
            total = dgvDetalle[3, posicion].Value.ToString();
            int idCliente = 0;

            string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
            string queryObtenerID = "SELECT ID_Cliente FROM Clientes WHERE Nombre = @nombreCliente";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    // Obtener el ID_Cliente
                    using (SqlCommand cmdObtenerID = new SqlCommand(queryObtenerID, conn))
                    {
                        cmdObtenerID.Parameters.AddWithValue("@nombreCliente", cliente);
                        conn.Open();
                        object result = cmdObtenerID.ExecuteScalar(); // Ejecutar y obtener el resultado

                        if (result != null)
                        {
                            idCliente = Convert.ToInt32(result);
                        }
                        else
                        {
                            MessageBox.Show("El cliente seleccionado no existe en la base de datos");
                            return;
                        }
                    }

                    // Consulta para actualizar la venta

                    string queryActualizarCompra = @"
                        UPDATE Ventas
                        SET Fecha_Venta = @fechaventa,
                            ID_Cliente = @idCliente,
                            Total_Venta = @totalventa
                        WHERE ID_Venta = @idVenta";

                    using (SqlCommand cmdActualizarVenta = new SqlCommand(queryActualizarCompra, conn))
                    {
                        cmdActualizarVenta.Parameters.AddWithValue("@idVenta", idVenta);
                        cmdActualizarVenta.Parameters.AddWithValue("@fechaventa", fecha);
                        cmdActualizarVenta.Parameters.AddWithValue("@idCliente", idCliente);
                        cmdActualizarVenta.Parameters.AddWithValue("@totalventa", total);

                        // Ejecutar la consulta de actualización
                        int rowsAffected = cmdActualizarVenta.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Venta actualizada correctamente");
                            dgvDetalle[1, posicion].Value = dtpfecha.Text;
                            dgvDetalle[2, posicion].Value = cbxCliente.Text;
                            dgvDetalle[3, posicion].Value = txtTotal.Text;
                            var colCodigo = dgvDetalle[0, posicion].Value.ToString();
                        }
                        else
                        {
                            MessageBox.Show("No se encontró el registro de venta a modificar");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al actualizar compra: {ex.Message}");
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        void agregarbase()
        {
            fecha = dtpfecha.Value.ToString("yyyy-MM-dd");
            cliente = cbxCliente.Text;
            total = txtTotal.Text;
            int idCliente = 0;

            if (fecha == "" || cliente == "" || total == "")
            {
                MessageBox.Show("No hay datos");
            }
            else
            {
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
                // Agregar el nuevo producto al DataGridViwe con el nuevo ID

                string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
                string queryObtenerID = "SELECT ID_Cliente FROM Clientes WHERE Nombre = @nombrecliente";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        // Obtener el ID_Cliente
                        using (SqlCommand cmdObtenerID = new SqlCommand(queryObtenerID, conn))
                        {
                            cmdObtenerID.Parameters.AddWithValue("@nombrecliente", cliente);
                            conn.Open();
                            object result = cmdObtenerID.ExecuteScalar(); // Ejecutar y obtener el resultado

                            if (result != null)
                            {
                                idCliente = Convert.ToInt32(result);
                            }
                            else
                            {
                                MessageBox.Show("El cliente seleccionado no existe en la base de datos");
                                return;
                            }
                        }
                        string query = "INSERT INTO Ventas (Fecha_Venta, ID_Cliente, Total_Venta)" +
                            "VALUES (@fecha, @idCliente, @total)";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@fecha", fecha);
                            cmd.Parameters.AddWithValue("@idCliente", idCliente);
                            cmd.Parameters.AddWithValue("@total", total);
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                        MessageBox.Show("Venta agregada");
                        dgvDetalle.Rows.Add(newID.ToString(), fecha, cliente, total);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al agregar venta {ex.Message}");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }

            }
        }

        void llenarComboBoxCliente()
        {
            string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
            string query = "SELECT Nombre FROM Clientes";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
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
                MessageBox.Show($"Error al llenar ComboBox: {ex.Message}");
            }
        }

        void llenarDataGridView()
        {
            // Cadena de conexion
            string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";

            // Consulta SQL
            string query = @"
            SELECT 
                v.ID_Venta,
                v.Fecha_Venta,
                c.Nombre,
                v.Total_Venta
            FROM Ventas v
            INNER JOIN Clientes c
            ON v.ID_Cliente = c.ID_Cliente";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);

                    using (SqlDataReader reader = cmd.ExecuteReader())
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
                            dgvDetalle.Rows[rowIndex].Cells["colCodigo"].Value = reader["ID_Venta"];
                            dgvDetalle.Rows[rowIndex].Cells["colFecha"].Value = reader["Fecha_Venta"];
                            dgvDetalle.Rows[rowIndex].Cells["colCliente"].Value = reader["Nombre"];
                            dgvDetalle.Rows[rowIndex].Cells["colTotal"].Value = reader["Total_Venta"];
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
            dtpfecha.Text = "";
            cbxCliente.Text = "";
            txtTotal.Text = "";
            cbxCliente.Items.Clear();
        }

        private void dgvDetalle_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            posicion = dgvDetalle.CurrentRow.Index;
            dtpfecha.Text = dgvDetalle[1, posicion].Value.ToString();
            cbxCliente.Text = dgvDetalle[2, posicion].Value.ToString();
            txtTotal.Text = dgvDetalle[3, posicion].Value.ToString();
            btnAgregar.Enabled = false;
            btnModificar.Enabled = true;
            btnEliminar.Enabled = true;
            dtpfecha.Focus();
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
            FormDetalles detalles = new FormDetalles();
            detalles.Show();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            agregarbase();
            limpiar();
            dtpfecha.Focus();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Estás seguro que deseas modificar este producto", "Confirmación", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                fecha = dtpfecha.Text;
                cliente = cbxCliente.Text;
                total = txtTotal.Text;
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

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            posicion = dgvDetalle.CurrentRow.Index;
            dtpfecha.Text = dgvDetalle[1, posicion].Value.ToString();
            cbxCliente.Text = dgvDetalle[2, posicion].Value.ToString();
            txtTotal.Text = dgvDetalle[3, posicion].Value.ToString();
            btnAgregar.Enabled = false;
            btnModificar.Enabled = true;
            btnEliminar.Enabled = true;
            dtpfecha.Focus();
            llenarComboBoxCliente();
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
            dgvDetalle.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetalle.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetalle.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            if (e.ColumnIndex == 3 && e.Value !=null)
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
                    txtTotal.Text = dgvDetalle[3, posicion].Value.ToString();
                    btnAgregar.Enabled = false;
                    btnModificar.Enabled = true;
                    btnEliminar.Enabled = true;
                    dtpfecha.Focus();
                }
                else
                {
                    MessageBox.Show("La fila seleccionada contiene celdas vacías.");
                }

            }   
        }
    }
}
