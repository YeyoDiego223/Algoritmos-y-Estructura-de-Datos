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
    public partial class FormCompras : Form
    {
        int i = 1;
        int posicion;
        string proveedor, fecha, total;
        public FormCompras()
        {
            InitializeComponent();
            cbxProveedor.DropDownStyle = ComboBoxStyle.DropDownList;
            llenarDataGridView();
            cbxProveedor.Focus();
            limpiar();

        }        
        
        void eliminarbase()
        {
            posicion = dgvDetalle.CurrentRow.Index;
            var id = dgvDetalle[0, posicion].Value.ToString();
            // Conexión
            string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
            string query = @"DELETE FROM Compras WHERE ID_Compra = @idCompra";

            // 1. Conexión y comando
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // 2. Parámetros para evitar SQL Injection
                    cmd.Parameters.AddWithValue("@idCompra", id);

                    try
                    {
                        // 3. Abrír conexión y ejecutar comando
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Compra eliminada correctamente");
                        dgvDetalle.Rows.RemoveAt(posicion);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar compra: {ex.Message}");
                    }
                }
            }
        }

        void modificarbase()
        {
            posicion = dgvDetalle.CurrentRow.Index;
            var idCompra = dgvDetalle[0, posicion].Value.ToString();
            var proveedor = dgvDetalle[1, posicion].Value.ToString();
            var fecha = dtpFecha.Value.ToString("yyyy-MM-dd");
            var total = dgvDetalle[3, posicion].Value.ToString();
            int idProveedor = 0; //Aquí se almacenara el ID_Proveedor obtenido

            string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
            string queryObtenerID = "SELECT ID_Proveedor FROM Proveedores WHERE Nombre_Proveedor = @nombreProveedor";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    // Obtener el ID_Proveedor
                    using (SqlCommand cmdObtenerID = new SqlCommand(queryObtenerID, conn))
                    {
                        cmdObtenerID.Parameters.AddWithValue("@nombreProveedor", proveedor);
                        conn.Open();
                        object result = cmdObtenerID.ExecuteScalar(); // Ejecutar y obtener el resultado

                        if (result != null)
                        {
                            idProveedor = Convert.ToInt32(result);
                        }
                        else
                        {
                            MessageBox.Show("El proveedor seleccionado no existe en la base de datos");
                            return;
                        }
                    }

                    // Consulta para actualizar la compra
                    string queryActualizarCompra = @"
                        UPDATE Compras
                        SET ID_Proveedor = @idProveedor,
                            Fecha_Compra = @fechaCompra,
                            Total_Compra = @totalCompra
                        WHERE ID_Compra = @idCompra";

                    using (SqlCommand cmdActualizarCompra = new SqlCommand(queryActualizarCompra, conn))
                    {
                        cmdActualizarCompra.Parameters.AddWithValue("@idProveedor", idProveedor);
                        cmdActualizarCompra.Parameters.AddWithValue("@fechaCompra", fecha);
                        cmdActualizarCompra.Parameters.AddWithValue("@totalCompra", total);
                        cmdActualizarCompra.Parameters.AddWithValue("@idCompra", idCompra);

                        // Ejecutar la consulta de actualización
                        int rowsAffected = cmdActualizarCompra.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Compra actualizada correctamente");
                            proveedor = cbxProveedor.Text;
                            fecha = dtpFecha.Text;
                            total = txtTotal.Text;
                            dgvDetalle[1, posicion].Value = cbxProveedor.Text;
                            dgvDetalle[2, posicion].Value = dtpFecha.Value.ToString("yyyy-MM-dd");
                            dgvDetalle[3, posicion].Value = txtTotal.Text;
                        }
                        else
                        {
                            MessageBox.Show("No se encontró el registro de compra a modificar");
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
            proveedor = cbxProveedor.Text;
            fecha = dtpFecha.Value.ToString("yyyy-MM-dd");
            total = txtTotal.Text;
            int idProveedor = 0; //Aquí se almacenara el ID_Proveedor obtenido
            if (proveedor == "" || fecha == "" || total == "")
            {
                MessageBox.Show("No hay datos");
            }
            else
            {
                int lastID = 0;
                if (dgvDetalle.Rows.Count > 0)
                {
                    // Obten el valor máximo de la columna "ID" (columna 0 en este caso)
                    lastID = dgvDetalle.Rows.Cast<DataGridViewRow>()
                                            .Max(row => Convert.ToInt32(row.Cells[0].Value));
                }
                // Nuevo ID es el último ID + 1
                int newID = lastID + 1;

                string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
                string queryObtenerID = "SELECT ID_Proveedor FROM Proveedores WHERE Nombre_Proveedor = @nombreProveedor";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        // Obtener el ID_Proveedor
                        using (SqlCommand cmdObtenerID = new SqlCommand(queryObtenerID, conn))
                        {
                            cmdObtenerID.Parameters.AddWithValue("@nombreProveedor", proveedor);
                            conn.Open();
                            object result = cmdObtenerID.ExecuteScalar(); // Ejecutar y obtener el resultado

                            if (result != null)
                            {
                                idProveedor = Convert.ToInt32(result);
                            }
                            else
                            {
                                MessageBox.Show("El proveedor seleccionado no existe en la base de datos");
                                return;
                            }
                        }
                        string query = "INSERT INTO Compras (ID_Proveedor, Fecha_Compra, Total_Compra) " +
                    "VALUES (@idProveedor, @fecha, @total)";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@idProveedor", idProveedor);
                            cmd.Parameters.AddWithValue("@fecha", fecha);
                            cmd.Parameters.AddWithValue("@total", total);
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                        MessageBox.Show("Compra agregada.");
                        dgvDetalle.Rows.Add(newID.ToString(), proveedor, fecha, total);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al agregar compra {ex.Message}");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        void llenarComboBox()
        {
            string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";
            string query = "SELECT Nombre_Proveedor FROM Proveedores";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
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
                MessageBox.Show($"Error al llenar el ComboBox: {ex.Message}");
            }
        }

        private void llenarDataGridView()
        {
            // Cadena de conexion
            string connectionString = "Server=MSI\\SQLEXPRESS;Database=BDTIENDA;Trusted_Connection=True;";

            // Consulta SQL
            string query = @"
            SELECT 
                c.ID_Compra, 
                p.Nombre_Proveedor,
                c.Fecha_Compra,
                c.Total_Compra
            FROM Compras c
            INNER JOIN Proveedores p 
            ON c.ID_Proveedor = p.ID_Proveedor";

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
                            dgvDetalle.Rows[rowIndex].Cells["colCodigo"].Value = reader["ID_Compra"];
                            dgvDetalle.Rows[rowIndex].Cells["colProveedor"].Value = reader["Nombre_Proveedor"];
                            dgvDetalle.Rows[rowIndex].Cells["colFecha"].Value = reader["Fecha_Compra"];
                            dgvDetalle.Rows[rowIndex].Cells["colTotal"].Value = reader["Total_Compra"];
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
            cbxProveedor.Text = "";
            dtpFecha.Text = "";
            txtTotal.Text = "";
            cbxProveedor.Items.Clear();
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
            FormDetalles detalles = new FormDetalles();
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
                    cbxProveedor.Text = dgvDetalle[1, posicion].Value.ToString();
                    dtpFecha.Text = dgvDetalle[2, posicion].Value.ToString();
                    txtTotal.Text = dgvDetalle[3, posicion].Value.ToString();
                    btnAgregar.Enabled = false;
                    btnModificar.Enabled = true;
                    btnEliminar.Enabled = true;
                    cbxProveedor.Focus();
                }
                else
                {
                    MessageBox.Show("La fila seleccionada contiene celdas vacías.");
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
            dgvDetalle.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDetalle.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            if (dgvDetalle.Columns[e.ColumnIndex].Name == "colFecha")
            {
                if (e.Value != null && e.Value != DBNull.Value)
                {
                    try
                    {
                        // Convierte el valor a DateTime y formatea
                        DateTime fecha = Convert.ToDateTime(e.Value);
                        e.Value = fecha.ToString("yyyy-MM-dd");
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
            llenarComboBox();
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
