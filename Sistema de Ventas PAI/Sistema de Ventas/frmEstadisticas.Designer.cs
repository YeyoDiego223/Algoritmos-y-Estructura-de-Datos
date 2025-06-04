namespace Sistema_de_Ventas
{
    partial class frmEstadisticas
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEstadisticas));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.lblIngresoTotal = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSalir = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pctbxVentas = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pctbxProveedores = new System.Windows.Forms.PictureBox();
            this.pctbxProductos = new System.Windows.Forms.PictureBox();
            this.pctbxCompras = new System.Windows.Forms.PictureBox();
            this.pctbxClientes = new System.Windows.Forms.PictureBox();
            this.lblTotalVentas = new System.Windows.Forms.Label();
            this.chartVentas = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.dataGridViewDetalles = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSalir)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctbxVentas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctbxProveedores)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctbxProductos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctbxCompras)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctbxClientes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartVentas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDetalles)).BeginInit();
            this.SuspendLayout();
            // 
            // lblIngresoTotal
            // 
            this.lblIngresoTotal.AutoSize = true;
            this.lblIngresoTotal.Font = new System.Drawing.Font("ChunkFive", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIngresoTotal.Location = new System.Drawing.Point(666, 439);
            this.lblIngresoTotal.Name = "lblIngresoTotal";
            this.lblIngresoTotal.Size = new System.Drawing.Size(204, 35);
            this.lblIngresoTotal.TabIndex = 86;
            this.lblIngresoTotal.Text = "Ingreso Total";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panel1.Controls.Add(this.btnSalir);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.pctbxVentas);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.pctbxProveedores);
            this.panel1.Controls.Add(this.pctbxProductos);
            this.panel1.Controls.Add(this.pctbxCompras);
            this.panel1.Controls.Add(this.pctbxClientes);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(269, 784);
            this.panel1.TabIndex = 81;
            // 
            // btnSalir
            // 
            this.btnSalir.Image = ((System.Drawing.Image)(resources.GetObject("btnSalir.Image")));
            this.btnSalir.Location = new System.Drawing.Point(90, 690);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(84, 80);
            this.btnSalir.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnSalir.TabIndex = 9;
            this.btnSalir.TabStop = false;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(160, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(76, 69);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // pctbxVentas
            // 
            this.pctbxVentas.Image = ((System.Drawing.Image)(resources.GetObject("pctbxVentas.Image")));
            this.pctbxVentas.Location = new System.Drawing.Point(12, 322);
            this.pctbxVentas.Name = "pctbxVentas";
            this.pctbxVentas.Size = new System.Drawing.Size(242, 110);
            this.pctbxVentas.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pctbxVentas.TabIndex = 7;
            this.pctbxVentas.TabStop = false;
            this.pctbxVentas.Click += new System.EventHandler(this.pctbxVentas_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("League Spartan", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 40);
            this.label1.TabIndex = 2;
            this.label1.Text = "DETALLES";
            // 
            // pctbxProveedores
            // 
            this.pctbxProveedores.Image = ((System.Drawing.Image)(resources.GetObject("pctbxProveedores.Image")));
            this.pctbxProveedores.Location = new System.Drawing.Point(13, 87);
            this.pctbxProveedores.Name = "pctbxProveedores";
            this.pctbxProveedores.Size = new System.Drawing.Size(242, 110);
            this.pctbxProveedores.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pctbxProveedores.TabIndex = 3;
            this.pctbxProveedores.TabStop = false;
            this.pctbxProveedores.Click += new System.EventHandler(this.pctbxProveedores_Click);
            // 
            // pctbxProductos
            // 
            this.pctbxProductos.Image = ((System.Drawing.Image)(resources.GetObject("pctbxProductos.Image")));
            this.pctbxProductos.Location = new System.Drawing.Point(13, 563);
            this.pctbxProductos.Name = "pctbxProductos";
            this.pctbxProductos.Size = new System.Drawing.Size(242, 110);
            this.pctbxProductos.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pctbxProductos.TabIndex = 5;
            this.pctbxProductos.TabStop = false;
            this.pctbxProductos.Click += new System.EventHandler(this.pctbxProductos_Click);
            // 
            // pctbxCompras
            // 
            this.pctbxCompras.Image = ((System.Drawing.Image)(resources.GetObject("pctbxCompras.Image")));
            this.pctbxCompras.Location = new System.Drawing.Point(13, 447);
            this.pctbxCompras.Name = "pctbxCompras";
            this.pctbxCompras.Size = new System.Drawing.Size(242, 110);
            this.pctbxCompras.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pctbxCompras.TabIndex = 4;
            this.pctbxCompras.TabStop = false;
            this.pctbxCompras.Click += new System.EventHandler(this.pctbxCompras_Click);
            // 
            // pctbxClientes
            // 
            this.pctbxClientes.Image = ((System.Drawing.Image)(resources.GetObject("pctbxClientes.Image")));
            this.pctbxClientes.Location = new System.Drawing.Point(12, 206);
            this.pctbxClientes.Name = "pctbxClientes";
            this.pctbxClientes.Size = new System.Drawing.Size(242, 110);
            this.pctbxClientes.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pctbxClientes.TabIndex = 2;
            this.pctbxClientes.TabStop = false;
            this.pctbxClientes.Click += new System.EventHandler(this.pctbxClientes_Click);
            // 
            // lblTotalVentas
            // 
            this.lblTotalVentas.AutoSize = true;
            this.lblTotalVentas.Font = new System.Drawing.Font("ChunkFive", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalVentas.Location = new System.Drawing.Point(666, 494);
            this.lblTotalVentas.Name = "lblTotalVentas";
            this.lblTotalVentas.Size = new System.Drawing.Size(234, 35);
            this.lblTotalVentas.TabIndex = 87;
            this.lblTotalVentas.Text = "Total de Ventas";
            // 
            // chartVentas
            // 
            chartArea2.Name = "ChartArea1";
            this.chartVentas.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chartVentas.Legends.Add(legend2);
            this.chartVentas.Location = new System.Drawing.Point(312, 12);
            this.chartVentas.Name = "chartVentas";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chartVentas.Series.Add(series2);
            this.chartVentas.Size = new System.Drawing.Size(563, 372);
            this.chartVentas.TabIndex = 88;
            this.chartVentas.Text = "chart1";
            this.chartVentas.Click += new System.EventHandler(this.chartVentas_Click);
            // 
            // dataGridViewDetalles
            // 
            this.dataGridViewDetalles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDetalles.Location = new System.Drawing.Point(312, 433);
            this.dataGridViewDetalles.Name = "dataGridViewDetalles";
            this.dataGridViewDetalles.Size = new System.Drawing.Size(348, 334);
            this.dataGridViewDetalles.TabIndex = 89;
            // 
            // frmEstadisticas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1051, 779);
            this.Controls.Add(this.dataGridViewDetalles);
            this.Controls.Add(this.chartVentas);
            this.Controls.Add(this.lblTotalVentas);
            this.Controls.Add(this.lblIngresoTotal);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmEstadisticas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Detalles";
            this.Load += new System.EventHandler(this.frmEstadisticas_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSalir)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctbxVentas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctbxProveedores)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctbxProductos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctbxCompras)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctbxClientes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartVentas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDetalles)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblIngresoTotal;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox btnSalir;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pctbxVentas;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pctbxProveedores;
        private System.Windows.Forms.PictureBox pctbxProductos;
        private System.Windows.Forms.PictureBox pctbxCompras;
        private System.Windows.Forms.PictureBox pctbxClientes;
        private System.Windows.Forms.Label lblTotalVentas;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartVentas;
        private System.Windows.Forms.DataGridView dataGridViewDetalles;
    }
}