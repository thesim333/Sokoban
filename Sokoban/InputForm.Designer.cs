namespace Sokoban
{
    partial class InputForm
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
            this.txtBoxName = new System.Windows.Forms.TextBox();
            this.lblWhat = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(236, 134);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // txtBoxName
            // 
            this.txtBoxName.Location = new System.Drawing.Point(48, 65);
            this.txtBoxName.Name = "txtBoxName";
            this.txtBoxName.Size = new System.Drawing.Size(207, 20);
            this.txtBoxName.TabIndex = 1;
            this.txtBoxName.TextChanged += new System.EventHandler(this.txtBoxName_TextChanged);
            // 
            // lblWhat
            // 
            this.lblWhat.AutoSize = true;
            this.lblWhat.Location = new System.Drawing.Point(45, 37);
            this.lblWhat.Name = "lblWhat";
            this.lblWhat.Size = new System.Drawing.Size(0, 13);
            this.lblWhat.TabIndex = 2;
            // 
            // InputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(323, 169);
            this.ControlBox = false;
            this.Controls.Add(this.lblWhat);
            this.Controls.Add(this.txtBoxName);
            this.Controls.Add(this.btnOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputForm";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtBoxName;
        private System.Windows.Forms.Label lblWhat;
    }
}