namespace Sokoban
{
    partial class SizeDialog
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
            this.btnOK = new System.Windows.Forms.Button();
            this.lblRows = new System.Windows.Forms.Label();
            this.lblCols = new System.Windows.Forms.Label();
            this.numUpDownCols = new System.Windows.Forms.NumericUpDown();
            this.numUpDownRows = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownCols)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownRows)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(101, 154);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // lblRows
            // 
            this.lblRows.AutoSize = true;
            this.lblRows.Location = new System.Drawing.Point(87, 90);
            this.lblRows.Name = "lblRows";
            this.lblRows.Size = new System.Drawing.Size(34, 13);
            this.lblRows.TabIndex = 2;
            this.lblRows.Text = "Rows";
            // 
            // lblCols
            // 
            this.lblCols.AutoSize = true;
            this.lblCols.Location = new System.Drawing.Point(87, 50);
            this.lblCols.Name = "lblCols";
            this.lblCols.Size = new System.Drawing.Size(47, 13);
            this.lblCols.TabIndex = 3;
            this.lblCols.Text = "Columns";
            // 
            // numUpDownCols
            // 
            this.numUpDownCols.Location = new System.Drawing.Point(144, 48);
            this.numUpDownCols.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numUpDownCols.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numUpDownCols.Name = "numUpDownCols";
            this.numUpDownCols.Size = new System.Drawing.Size(46, 20);
            this.numUpDownCols.TabIndex = 4;
            this.numUpDownCols.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // numUpDownRows
            // 
            this.numUpDownRows.Location = new System.Drawing.Point(144, 88);
            this.numUpDownRows.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numUpDownRows.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numUpDownRows.Name = "numUpDownRows";
            this.numUpDownRows.Size = new System.Drawing.Size(46, 20);
            this.numUpDownRows.TabIndex = 5;
            this.numUpDownRows.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // SizeDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 189);
            this.ControlBox = false;
            this.Controls.Add(this.numUpDownRows);
            this.Controls.Add(this.numUpDownCols);
            this.Controls.Add(this.lblCols);
            this.Controls.Add(this.lblRows);
            this.Controls.Add(this.btnOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SizeDialog";
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownCols)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownRows)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblRows;
        private System.Windows.Forms.Label lblCols;
        private System.Windows.Forms.NumericUpDown numUpDownCols;
        private System.Windows.Forms.NumericUpDown numUpDownRows;
    }
}