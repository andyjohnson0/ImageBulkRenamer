using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace uk.andyjohnson.ImageBulkRenamer
{
    public partial class RenameItemEditDialog : Form
    {
        public RenameItemEditDialog(RenameItem item)
        {
            InitializeComponent();

            this.Item = item;
        }


        public RenameItem Item { get; private set; }


        private void ItemEditDialog_Load(object sender, EventArgs e)
        {
            specifyTimestampTimePicker.Value = (Item.InputImageExifTimestamp != DateTime.MinValue) ? Item.InputImageExifTimestamp : Item.InputImageFileCreatonTimestamp;
            specifyFilenameTextBox.Text = RenameItem.BuildFileName(specifyTimestampTimePicker.Value) + ".jpg";
            if (!string.IsNullOrEmpty(Item.OutputSidecarFileName))
            {
                specifySidecarTextBox.Text = Item.OutputSidecarFileName;
            }
            else
            {
                specifySidecarTextBox.ReadOnly = true;
            }

            modificationFilenameTextBox.Text = RenameItem.BuildFileName(Item.InputImageFileModificationTimestamp) + ".jpg";
            modificationTimestampTextBox.Text = Item.InputImageFileModificationTimestamp.ToString("yyyy-MM-dd HH:mm:ss");

            creationFilenameTextBox.Text = RenameItem.BuildFileName(Item.InputImageFileCreatonTimestamp) + ".jpg";
            creationTimestampTextBox.Text = Item.InputImageFileCreatonTimestamp.ToString("yyyy-MM-dd HH:mm:ss");

            specifyRadioButton.Checked = true;
        }



        private void okButton_Click(object sender, EventArgs e)
        {
            if (specifyRadioButton.Checked)
            {
                this.Item.OutputImageFileName = specifyFilenameTextBox.Text;
                this.Item.OutputImageFileTimestamp = specifyTimestampTimePicker.Value;
                if (!specifySidecarTextBox.ReadOnly)
                {
                    this.Item.OutputSidecarFileName = specifySidecarTextBox.Text;
                }
            }
            else if (modificationRadioButton.Checked)
            {
                this.Item.OutputImageFileName = modificationFilenameTextBox.Text;
                this.Item.OutputImageFileTimestamp = Item.InputImageFileModificationTimestamp;
            }
            else if (creationRadioButton.Checked)
            {
                this.Item.OutputImageFileName = creationFilenameTextBox.Text;
                this.Item.OutputImageFileTimestamp = Item.InputImageFileCreatonTimestamp;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }


        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
