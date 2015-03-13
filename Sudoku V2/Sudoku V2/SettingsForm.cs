using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sudoku_V2
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }
        Properties.Settings settings = new Properties.Settings();

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Changes to the user settings will not be visible until a restart is performed, \r\n Would you like to restart the application now?", "Restart Application?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                if (result == DialogResult.Yes)
                {
                    SaveSettings();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else if (result == DialogResult.No)
                {
                    SaveSettings();
                    this.DialogResult = DialogResult.No;
                    this.Close();
                }
                else
                {
                    if (settings.ShowStatus)
                    {
                        cbStatus.Checked = true;
                    }
                    else
                    {
                        cbStatus.Checked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }

        private void SaveSettings()
        {
            if (cbStatus.Checked)
            {
                settings.ShowStatus = true;
            }
            else
            {
                settings.ShowStatus = false;
            }
            if (cbHelpTip.Checked)
            {
                settings.ShowToolTip = true;
            }
            else
            {
                settings.ShowToolTip = false;
            }
            settings.Save();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            cbHelpTip.Enabled = true;
            if (settings.ShowStatus)
            {
                cbStatus.Checked = true;
            }
            else
            {
                cbStatus.Checked = false;
            }
            if (settings.ShowToolTip)
            {
                cbHelpTip.Checked = true;
            }
            else
            {
                cbHelpTip.Checked = false;
            }
        }
    }
}
