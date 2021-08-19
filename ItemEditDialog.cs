using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace uk.andyjohnson.ImageBulkRenamer
{
    public partial class ItemEditDialog : Form
    {
        public ItemEditDialog(RenameItem item)
        {
            InitializeComponent();

            this.Item = new RenameItem(item);
        }


        public RenameItem Item;


        private void ItemEditDialog_Load(object sender, EventArgs e)
        {
            specifyTimestampTimePicker.Value = (Item.InputExifTimestamp != DateTime.MinValue) ? Item.InputExifTimestamp : Item.InputFileCreatonTimestamp;
            specifyFilenameTextBox.Text = RenameItem.BuildFileName(specifyTimestampTimePicker.Value) + ".jpg";

            modificationFilenameTextBox.Text = RenameItem.BuildFileName(Item.InputFileModificationTimestamp) + ".jpg";
            modificationTimestampTextBox.Text = Item.InputFileModificationTimestamp.ToString("yyyy-MM-dd HH:mm:ss");

            creationFilenameTextBox.Text = RenameItem.BuildFileName(Item.InputFileCreatonTimestamp) + ".jpg";
            creationTimestampTextBox.Text = Item.InputFileCreatonTimestamp.ToString("yyyy-MM-dd HH:mm:ss");

            specifyRadioButton.Checked = true;
        }


        private void OnCheckedChange(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb == specifyRadioButton)
            {
                Item.OutputFileName = specifyFilenameTextBox.Text;
                Item.OutputFileTimestamp = specifyTimestampTimePicker.Value;
            }
            else if (rb == modificationRadioButton)
            {
                Item.OutputFileName = modificationFilenameTextBox.Text;
                Item.OutputFileTimestamp = Item.InputFileModificationTimestamp;
            }
            else if (rb == creationRadioButton)
            {
                Item.OutputFileName = creationFilenameTextBox.Text;
                Item.OutputFileTimestamp = Item.InputFileCreatonTimestamp;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Dispose();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Dispose();
        }


        
    }
}
