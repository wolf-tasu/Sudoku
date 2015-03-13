namespace Sudoku_V2
{
    partial class SettingsForm
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
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbStatus = new System.Windows.Forms.CheckBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblHelpTip = new System.Windows.Forms.Label();
            this.cbHelpTip = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(116, 52);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(197, 52);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cbStatus
            // 
            this.cbStatus.AutoSize = true;
            this.cbStatus.Location = new System.Drawing.Point(138, 12);
            this.cbStatus.Name = "cbStatus";
            this.cbStatus.Size = new System.Drawing.Size(15, 14);
            this.cbStatus.TabIndex = 2;
            this.cbStatus.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(13, 13);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(119, 13);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Show help bar on form?";
            // 
            // lblHelpTip
            // 
            this.lblHelpTip.AutoSize = true;
            this.lblHelpTip.Location = new System.Drawing.Point(13, 32);
            this.lblHelpTip.Name = "lblHelpTip";
            this.lblHelpTip.Size = new System.Drawing.Size(97, 13);
            this.lblHelpTip.TabIndex = 4;
            this.lblHelpTip.Text = "Show help tool tip?";
            // 
            // cbHelpTip
            // 
            this.cbHelpTip.AutoSize = true;
            this.cbHelpTip.Location = new System.Drawing.Point(138, 32);
            this.cbHelpTip.Name = "cbHelpTip";
            this.cbHelpTip.Size = new System.Drawing.Size(15, 14);
            this.cbHelpTip.TabIndex = 5;
            this.cbHelpTip.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(284, 81);
            this.Controls.Add(this.cbHelpTip);
            this.Controls.Add(this.lblHelpTip);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.cbStatus);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox cbStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblHelpTip;
        private System.Windows.Forms.CheckBox cbHelpTip;
    }
}