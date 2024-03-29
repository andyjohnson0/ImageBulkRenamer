﻿namespace uk.andyjohnson.ImageBulkRenamer
{
    partial class RenameItemEditDialog
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.specifyRadioButton = new System.Windows.Forms.RadioButton();
            this.modificationRadioButton = new System.Windows.Forms.RadioButton();
            this.creationRadioButton = new System.Windows.Forms.RadioButton();
            this.specifyFilenameTextBox = new System.Windows.Forms.TextBox();
            this.specifyTimestampTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.modificationFilenameTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.modificationTimestampTextBox = new System.Windows.Forms.TextBox();
            this.creationFilenameTextBox = new System.Windows.Forms.TextBox();
            this.creationTimestampTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.specifySidecarTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(56, 371);
            this.okButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(88, 27);
            this.okButton.TabIndex = 11;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(162, 371);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(88, 27);
            this.cancelButton.TabIndex = 12;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // specifyRadioButton
            // 
            this.specifyRadioButton.AutoSize = true;
            this.specifyRadioButton.Location = new System.Drawing.Point(14, 14);
            this.specifyRadioButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.specifyRadioButton.Name = "specifyRadioButton";
            this.specifyRadioButton.Size = new System.Drawing.Size(199, 19);
            this.specifyRadioButton.TabIndex = 1;
            this.specifyRadioButton.TabStop = true;
            this.specifyRadioButton.Text = "Specify Filename and Timestamp";
            this.specifyRadioButton.UseVisualStyleBackColor = true;
            // 
            // modificationRadioButton
            // 
            this.modificationRadioButton.AutoSize = true;
            this.modificationRadioButton.Location = new System.Drawing.Point(14, 151);
            this.modificationRadioButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.modificationRadioButton.Name = "modificationRadioButton";
            this.modificationRadioButton.Size = new System.Drawing.Size(198, 19);
            this.modificationRadioButton.TabIndex = 5;
            this.modificationRadioButton.TabStop = true;
            this.modificationRadioButton.Text = "Use File Modification Timestamp";
            this.modificationRadioButton.UseVisualStyleBackColor = true;
            // 
            // creationRadioButton
            // 
            this.creationRadioButton.AutoSize = true;
            this.creationRadioButton.Location = new System.Drawing.Point(14, 261);
            this.creationRadioButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.creationRadioButton.Name = "creationRadioButton";
            this.creationRadioButton.Size = new System.Drawing.Size(175, 19);
            this.creationRadioButton.TabIndex = 8;
            this.creationRadioButton.TabStop = true;
            this.creationRadioButton.Text = "Use File Creation Timestamp";
            this.creationRadioButton.UseVisualStyleBackColor = true;
            // 
            // specifyFilenameTextBox
            // 
            this.specifyFilenameTextBox.Location = new System.Drawing.Point(104, 72);
            this.specifyFilenameTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.specifyFilenameTextBox.Name = "specifyFilenameTextBox";
            this.specifyFilenameTextBox.Size = new System.Drawing.Size(185, 23);
            this.specifyFilenameTextBox.TabIndex = 3;
            // 
            // specifyTimestampTimePicker
            // 
            this.specifyTimestampTimePicker.CustomFormat = "yyyy/MM/dd hh:mm:ss";
            this.specifyTimestampTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.specifyTimestampTimePicker.Location = new System.Drawing.Point(104, 42);
            this.specifyTimestampTimePicker.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.specifyTimestampTimePicker.Name = "specifyTimestampTimePicker";
            this.specifyTimestampTimePicker.Size = new System.Drawing.Size(185, 23);
            this.specifyTimestampTimePicker.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 75);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Filename";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 48);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Timestamp";
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(240, -263);
            this.dateTimePicker2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(233, 23);
            this.dateTimePicker2.TabIndex = 3;
            // 
            // modificationFilenameTextBox
            // 
            this.modificationFilenameTextBox.Enabled = false;
            this.modificationFilenameTextBox.Location = new System.Drawing.Point(104, 207);
            this.modificationFilenameTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.modificationFilenameTextBox.Name = "modificationFilenameTextBox";
            this.modificationFilenameTextBox.Size = new System.Drawing.Size(185, 23);
            this.modificationFilenameTextBox.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(47, 211);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Filename";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(38, 181);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "Timestamp";
            // 
            // modificationTimestampTextBox
            // 
            this.modificationTimestampTextBox.Enabled = false;
            this.modificationTimestampTextBox.Location = new System.Drawing.Point(104, 177);
            this.modificationTimestampTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.modificationTimestampTextBox.Name = "modificationTimestampTextBox";
            this.modificationTimestampTextBox.Size = new System.Drawing.Size(185, 23);
            this.modificationTimestampTextBox.TabIndex = 6;
            // 
            // creationFilenameTextBox
            // 
            this.creationFilenameTextBox.Enabled = false;
            this.creationFilenameTextBox.Location = new System.Drawing.Point(104, 321);
            this.creationFilenameTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.creationFilenameTextBox.Name = "creationFilenameTextBox";
            this.creationFilenameTextBox.Size = new System.Drawing.Size(185, 23);
            this.creationFilenameTextBox.TabIndex = 10;
            // 
            // creationTimestampTextBox
            // 
            this.creationTimestampTextBox.Enabled = false;
            this.creationTimestampTextBox.Location = new System.Drawing.Point(104, 288);
            this.creationTimestampTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.creationTimestampTextBox.Name = "creationTimestampTextBox";
            this.creationTimestampTextBox.Size = new System.Drawing.Size(185, 23);
            this.creationTimestampTextBox.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(46, 325);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "Filename";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(38, 291);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 15);
            this.label6.TabIndex = 4;
            this.label6.Text = "Timestamp";
            // 
            // specifySidecarTextBox
            // 
            this.specifySidecarTextBox.Location = new System.Drawing.Point(104, 102);
            this.specifySidecarTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.specifySidecarTextBox.Name = "specifySidecarTextBox";
            this.specifySidecarTextBox.Size = new System.Drawing.Size(185, 23);
            this.specifySidecarTextBox.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(56, 106);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 15);
            this.label7.TabIndex = 4;
            this.label7.Text = "Sidecar";
            // 
            // RenameItemEditDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(306, 409);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.specifyTimestampTimePicker);
            this.Controls.Add(this.creationTimestampTextBox);
            this.Controls.Add(this.creationFilenameTextBox);
            this.Controls.Add(this.modificationTimestampTextBox);
            this.Controls.Add(this.modificationFilenameTextBox);
            this.Controls.Add(this.specifySidecarTextBox);
            this.Controls.Add(this.specifyFilenameTextBox);
            this.Controls.Add(this.creationRadioButton);
            this.Controls.Add(this.modificationRadioButton);
            this.Controls.Add(this.specifyRadioButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RenameItemEditDialog";
            this.Text = "Edit Item";
            this.Load += new System.EventHandler(this.ItemEditDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.RadioButton specifyRadioButton;
        private System.Windows.Forms.RadioButton modificationRadioButton;
        private System.Windows.Forms.RadioButton creationRadioButton;
        private System.Windows.Forms.TextBox specifyFilenameTextBox;
        private System.Windows.Forms.DateTimePicker specifyTimestampTimePicker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.TextBox modificationFilenameTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox modificationTimestampTextBox;
        private System.Windows.Forms.TextBox creationFilenameTextBox;
        private System.Windows.Forms.TextBox creationTimestampTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox specifySidecarTextBox;
        private System.Windows.Forms.Label label7;
    }
}