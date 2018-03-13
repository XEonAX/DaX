namespace DaX
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.sessionTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.sessionDS = new DaX.SessionDS();
            this.dcUrlDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dcSizeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDL = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sessionTableBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sessionDS)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(963, 249);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 27);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToResizeRows = false;
            this.dgv.AutoGenerateColumns = false;
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dcUrlDataGridViewTextBoxColumn,
            this.dcSizeDataGridViewTextBoxColumn,
            this.colDL});
            this.dgv.DataSource = this.sessionTableBindingSource;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.Location = new System.Drawing.Point(0, 0);
            this.dgv.Name = "dgv";
            this.dgv.RowTemplate.Height = 24;
            this.dgv.Size = new System.Drawing.Size(1038, 276);
            this.dgv.TabIndex = 1;
            this.dgv.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellContentClick);
            // 
            // sessionTableBindingSource
            // 
            this.sessionTableBindingSource.DataMember = "SessionTable";
            this.sessionTableBindingSource.DataSource = this.sessionDS;
            // 
            // sessionDS
            // 
            this.sessionDS.DataSetName = "SessionDS";
            this.sessionDS.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // dcUrlDataGridViewTextBoxColumn
            // 
            this.dcUrlDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dcUrlDataGridViewTextBoxColumn.DataPropertyName = "dcUrl";
            this.dcUrlDataGridViewTextBoxColumn.HeaderText = "dcUrl";
            this.dcUrlDataGridViewTextBoxColumn.Name = "dcUrlDataGridViewTextBoxColumn";
            // 
            // dcSizeDataGridViewTextBoxColumn
            // 
            this.dcSizeDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dcSizeDataGridViewTextBoxColumn.DataPropertyName = "dcSize";
            this.dcSizeDataGridViewTextBoxColumn.FillWeight = 20F;
            this.dcSizeDataGridViewTextBoxColumn.HeaderText = "dcSize";
            this.dcSizeDataGridViewTextBoxColumn.Name = "dcSizeDataGridViewTextBoxColumn";
            // 
            // colDL
            // 
            this.colDL.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDL.FillWeight = 10F;
            this.colDL.HeaderText = "DL";
            this.colDL.Name = "colDL";
            this.colDL.Text = "Download";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1038, 276);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dgv);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sessionTableBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sessionDS)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.BindingSource sessionTableBindingSource;
        private SessionDS sessionDS;
        private System.Windows.Forms.DataGridViewTextBoxColumn dcUrlDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dcSizeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewButtonColumn colDL;
    }
}

