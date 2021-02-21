namespace WindowsFormsApp1
{
    partial class SQLForm
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.errorText = new System.Windows.Forms.RichTextBox();
            this.sqlExecuteButton = new System.Windows.Forms.Button();
            this.sqlQuery = new System.Windows.Forms.RichTextBox();
            this.sqlDatagridView = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sqlDatagridView)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.button1);
            this.splitContainer1.Panel1.Controls.Add(this.progressBar1);
            this.splitContainer1.Panel1.Controls.Add(this.errorText);
            this.splitContainer1.Panel1.Controls.Add(this.sqlExecuteButton);
            this.splitContainer1.Panel1.Controls.Add(this.sqlQuery);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.Red;
            this.splitContainer1.Panel2.Controls.Add(this.sqlDatagridView);
            this.splitContainer1.Size = new System.Drawing.Size(800, 480);
            this.splitContainer1.SplitterDistance = 153;
            this.splitContainer1.TabIndex = 0;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(595, 51);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(202, 23);
            this.progressBar1.TabIndex = 4;
            // 
            // errorText
            // 
            this.errorText.Location = new System.Drawing.Point(3, 101);
            this.errorText.Name = "errorText";
            this.errorText.Size = new System.Drawing.Size(586, 44);
            this.errorText.TabIndex = 3;
            this.errorText.Text = "";
            // 
            // sqlExecuteButton
            // 
            this.sqlExecuteButton.Location = new System.Drawing.Point(653, 22);
            this.sqlExecuteButton.Name = "sqlExecuteButton";
            this.sqlExecuteButton.Size = new System.Drawing.Size(75, 23);
            this.sqlExecuteButton.TabIndex = 1;
            this.sqlExecuteButton.Text = "Execute";
            this.sqlExecuteButton.UseVisualStyleBackColor = true;
            this.sqlExecuteButton.Click += new System.EventHandler(this.sqlExecuteButton_Click);
            // 
            // sqlQuery
            // 
            this.sqlQuery.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sqlQuery.ForeColor = System.Drawing.Color.Maroon;
            this.sqlQuery.Location = new System.Drawing.Point(3, 3);
            this.sqlQuery.Name = "sqlQuery";
            this.sqlQuery.Size = new System.Drawing.Size(586, 92);
            this.sqlQuery.TabIndex = 0;
            this.sqlQuery.Text = "SELECT COUNT(checksum) as c, name, checksum FROM Files GROUP BY checksum, name OR" +
    "DER BY c DESC";
            // 
            // sqlDatagridView
            // 
            this.sqlDatagridView.AllowUserToAddRows = false;
            this.sqlDatagridView.AllowUserToDeleteRows = false;
            this.sqlDatagridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sqlDatagridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqlDatagridView.Location = new System.Drawing.Point(0, 0);
            this.sqlDatagridView.Name = "sqlDatagridView";
            this.sqlDatagridView.ReadOnly = true;
            this.sqlDatagridView.Size = new System.Drawing.Size(800, 323);
            this.sqlDatagridView.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(653, 110);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Execute";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(592, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "label1";
            // 
            // SQLForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.splitContainer1);
            this.DoubleBuffered = true;
            this.Name = "SQLForm";
            this.Size = new System.Drawing.Size(800, 480);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sqlDatagridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button sqlExecuteButton;
        private System.Windows.Forms.RichTextBox sqlQuery;
        private System.Windows.Forms.DataGridView sqlDatagridView;
        private System.Windows.Forms.RichTextBox errorText;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
    }
}
