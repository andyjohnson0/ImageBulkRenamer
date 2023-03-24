using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Reflection;


namespace uk.andyjohnson.ImageBulkRenamer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private DirectoryInfo imageDir;


        private void MainForm_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel.Text = "Ready";
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var msg = string.Format("Image File Bulk Renamer v{0} by Andrew Johnson.\r\n\r\nSee https://github.com/andyjohnson0/ImageBulkRenamer for info.",
                                    Assembly.GetExecutingAssembly().GetName().Version.ToString());
            MessageBox.Show(this, msg, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new FolderBrowserDialog();
            dlg.Description = "Select Image Folder";
            if (imageDir != null)
                dlg.SelectedPath = imageDir.FullName;
            else
                dlg.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.Cancel)
                return;
            imageDir = new DirectoryInfo(dlg.SelectedPath);

            this.UseWaitCursor = true;
            this.Enabled = false;
            preview_fallbackToFileCreationTimestamp = fallbackToFileCreationCb.Checked;
            BackgroundWorker wkr = new BackgroundWorker();
            wkr.DoWork += new DoWorkEventHandler(GetTimestampsWkr_DoWork);
            wkr.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GetTimestampsWkr_RunWorkerCompleted);
            wkr.WorkerReportsProgress = true;
            wkr.ProgressChanged += new ProgressChangedEventHandler(GetTimestampsWkr_ProgressChanged);
            wkr.RunWorkerAsync();
        }


        private void startButton_Click(object sender, EventArgs e)
        {
            this.UseWaitCursor = true;
            this.Enabled = false;

            var wkr = new BackgroundWorker();
            wkr.DoWork += new DoWorkEventHandler(RenameWkr_DoWork);
            wkr.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RenameWkr_RunWorkerCompleted);
            wkr.WorkerReportsProgress = true;
            wkr.ProgressChanged += new ProgressChangedEventHandler(RenameWkr_ProgressChanged);
            wkr.RunWorkerAsync();
        }


        private void listView_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo hitInfo = listView.HitTest(e.Location);
            if ((hitInfo == null) || (hitInfo.Item == null))
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                var menu = new ContextMenuStrip();
                var item = menu.Items.Add("&Edit");
                item.Click += new EventHandler(ItemEdit_Click);
                item.Tag = hitInfo.Item.Index;
                item = menu.Items.Add("&Preview");
                item.Click += new EventHandler(ItemPreview_Click);
                item.Tag = hitInfo.Item.Index;
                menu.Show(listView, e.Location);
            }
        }


        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo hitInfo = listView.HitTest(e.Location);
            if ((hitInfo == null) || (hitInfo.Item == null))
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ShowItemEditDialog(hitInfo.Item.Index);
            }
        }


        void ItemEdit_Click(object sender, EventArgs e)
        {
            var itemIdx = (int) ((ToolStripMenuItem)sender).Tag;  // Index into items array.
            ShowItemEditDialog(itemIdx);
        }


        void ItemPreview_Click(object sender, EventArgs e)
        {
            var itemIdx = (int)((ToolStripMenuItem)sender).Tag;  // Index into items array.
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = Path.Combine(imageDir.FullName, items[itemIdx].InputFileName);
            proc.Start();
        }


        private void ShowItemEditDialog(int itemIdx)
        {
            var dlg = new ItemEditDialog(items[itemIdx]);
            if (dlg.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
                return;

            items[itemIdx] = dlg.Item;
            var lvi = CreateListViewItem(items[itemIdx]);
            listView.Items[itemIdx] = lvi;
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }



        #region Preview

        private bool preview_fallbackToFileCreationTimestamp;
        private RenameItem[] items;
        private int noExifTimestampCount;
        private int fallbackTimestampCount;


        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        void GetTimestampsWkr_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker wkr = sender as BackgroundWorker;

            noExifTimestampCount = 0;

            FileInfo[] files = imageDir.GetFiles("*.jp*g");
            items = new RenameItem[files.Length];

            for(int i = 0; i < files.Length; i++)
            {
                FileInfo fi = files[i];

                DateTime picTimestampLocal;
                try
                {
                    using (var bmp = new Bitmap(fi.FullName))
                    {
                        PropertyItem bmpDateTime = bmp.GetPropertyItem(0x9003);
                        if (bmpDateTime == null)
                            bmpDateTime = bmp.GetPropertyItem(0x0132);
                        string picTimestampStr = Encoding.ASCII.GetString(bmpDateTime.Value, 0, bmpDateTime.Len - 1);
                        DateTime.TryParseExact(picTimestampStr, "yyyy:MM:dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture,
                                               System.Globalization.DateTimeStyles.AssumeLocal, out picTimestampLocal);
                    }
                }
                catch (ArgumentException)
                {
                    if (preview_fallbackToFileCreationTimestamp)
                    {
                        picTimestampLocal = fi.CreationTime;
                        noExifTimestampCount++;
                        fallbackTimestampCount++;
                    }
                    else
                    {
                        picTimestampLocal = DateTime.MinValue;
                        noExifTimestampCount++;
                    }
                }

                var item = new RenameItem();
                item.InputFileName = fi.Name;
                item.InputExifTimestamp = picTimestampLocal;
                item.InputFileCreatonTimestamp = fi.CreationTimeUtc;
                item.InputFileModificationTimestamp = fi.LastWriteTimeUtc;
                if (picTimestampLocal != DateTime.MinValue)
                {
                    for(int iSuffix = 0; iSuffix <= 99; iSuffix++)
                    {
                        string outputFileName = RenameItem.BuildFileName(picTimestampLocal);
                        if (iSuffix > 0)
                            outputFileName += "_" + iSuffix.ToString("00");
                        outputFileName += ".jpg";

                        bool found = false;
                        for (int j = 0; j < i; j++)
                        {
                            if (items[j].OutputFileName == outputFileName)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            item.OutputFileName = outputFileName;
                            break;
                        }
                    }
                }
                item.OutputFileTimestamp = picTimestampLocal;
                items[i] = item;

                int progress = (int)(((double)(i + 1) / (double)files.Length) * 100D);
                wkr.ReportProgress(progress, i);
            }
        }


        void GetTimestampsWkr_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int itemIdx = (int)e.UserState;

            if (itemIdx == 0)
                toolStripProgressBar.Visible = true;
            toolStripProgressBar.Value = e.ProgressPercentage;
            toolStripStatusLabel.Text = string.Format("Building preview ({0}%)", e.ProgressPercentage);

            listView.BeginUpdate();

            if (itemIdx == 0)
                listView.Items.Clear();
            var lvi = CreateListViewItem(items[itemIdx]);
            listView.Items.Add(lvi);
            lvi.EnsureVisible();

            listView.EndUpdate();
        }


        void GetTimestampsWkr_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripProgressBar.Visible = false;
            toolStripStatusLabel.Text = "Ready";
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            this.UseWaitCursor = false;
            this.Enabled = true;
            startButton.Enabled = (listView.Items.Count > 0);

            if (noExifTimestampCount > 0)
            {
                string msg = string.Format("{0} {1} had no EXIF timestamp. {2} file creation {3} were used.",
                                           noExifTimestampCount,
                                           (noExifTimestampCount == 1) ? "image" : "images",
                                           fallbackTimestampCount,
                                           (fallbackTimestampCount == 1) ? "timestamp" : "timestamps");
                MessageBox.Show(this, msg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }




        private static ListViewItem CreateListViewItem(RenameItem item)
        {
            var lvi = new ListViewItem();
            lvi.Text = item.InputFileName;
            if (item.InputExifTimestamp != DateTime.MinValue)
                lvi.SubItems.Add(item.InputExifTimestamp.ToString("yyyy-MM-dd HH:mm:ss"));
            else
                lvi.SubItems.Add("");
            if (item.OutputFileName != null)
                lvi.SubItems.Add(item.OutputFileName);
            else
                lvi.SubItems.Add("");
            if (item.OutputFileTimestamp != DateTime.MinValue)
                lvi.SubItems.Add(item.OutputFileTimestamp.ToString("yyyy-MM-dd HH:mm:ss"));
            else
                lvi.SubItems.Add("");
            if (item.RenameStatus != null)
                lvi.SubItems.Add(item.RenameStatus);
            else
                lvi.SubItems.Add("");
            return lvi;
        }


        #endregion Preview



        #region Image Renaming

        private int okCount;
        private int noChangeCount;
        private int skippedCount;
        private int nameConflictCount;
        private int errorCount;
        private const string okLabel = "OK";
        private const string noChangeLabel = "No change";
        private const string skippedLabel = "Skipped";
        private const string nameConflictLabel = "Filename already exists";
        private const string errorLabel = "Error";

        void RenameWkr_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker wkr = sender as BackgroundWorker;

            okCount = noChangeCount = skippedCount = nameConflictCount = errorCount = 0;

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].OutputFileName != null)
                {
                    var sourceFileName = Path.Combine(imageDir.FullName, items[i].InputFileName);
                    var destFileName = Path.Combine(imageDir.FullName, items[i].OutputFileName);

                    if (string.Equals(items[i].InputFileName, items[i].OutputFileName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        items[i].RenameStatus = noChangeLabel;
                        noChangeCount++;
                    }
                    else if (File.Exists(destFileName))
                    {
                        items[i].RenameStatus = nameConflictLabel;
                        nameConflictCount++;
                    }
                    else
                    {
                        bool succeeded = true;

                        // Rename
                        try
                        {
                            File.Move(sourceFileName, destFileName);
                        }
                        catch (Exception)
                        {
                            succeeded = false;
                        }

                        // Redate
                        try
                        {
                            if (items[i].OutputFileTimestamp != DateTime.MinValue)
                            {
                                File.SetCreationTime(destFileName, items[i].OutputFileTimestamp);
                                File.SetLastWriteTime(destFileName, items[i].OutputFileTimestamp);
                            }
                        }
                        catch (Exception)
                        {
                            succeeded = false;
                        }

                        if (succeeded)
                        {
                            items[i].RenameStatus = okLabel;
                            okCount++;
                        }
                        else
                        {
                            items[i].RenameStatus = errorLabel;
                            errorCount++;
                        }
                    }
                }
                else
                {
                    items[i].RenameStatus = skippedLabel;
                    skippedCount++;
                }

                int progress = (int)(((double)(i + 1) / (double)items.Length) * 100D);
                wkr.ReportProgress(progress, i);
            }
        }


        void RenameWkr_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int itemIdx = (int)e.UserState;

            if (itemIdx == 0)
                toolStripProgressBar.Visible = true;
            toolStripProgressBar.Value = e.ProgressPercentage;
            toolStripStatusLabel.Text = string.Format("Renaming files ({0}%)", e.ProgressPercentage);

            listView.BeginUpdate();
            ListViewItem lvi = listView.Items[itemIdx];
            lvi.SubItems[4].Text = items[itemIdx].RenameStatus;

            switch(items[itemIdx].RenameStatus)
            {
                case okLabel:
                    break;
                case noChangeLabel:
                    break;
                case skippedLabel:
                    lvi.BackColor = Color.Orange;
                    break;
                case nameConflictLabel:
                    lvi.BackColor = Color.Orange;
                    break;
                case errorLabel:
                    lvi.BackColor = Color.Red;
                    break;
            }

            lvi.EnsureVisible();
            listView.EndUpdate();
        }


        void RenameWkr_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripProgressBar.Visible = false;
            toolStripStatusLabel.Text = "Ready";
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            this.UseWaitCursor = false;
            this.Enabled = true;

            var sb = new StringBuilder();
            sb.Append(string.Format("{0} image {1} renamed.", okCount, (okCount == 1) ? "file was" : "files were"));
            var icon = MessageBoxIcon.Information;
            if (noChangeCount > 0)
            {
                sb.Append(string.Format(" {0} image {1} not renamed.", noChangeCount, (noChangeCount == 1) ? "file was" : "files were"));
            }
            if (skippedCount > 0)
            {
                sb.Append(string.Format(" {0} image {1} skipped.", skippedCount, (skippedCount == 1) ? "file was" : "files were"));
                icon = MessageBoxIcon.Warning;
            }
            if (nameConflictCount > 0)
            {
                sb.Append(string.Format(" {0} image {1} had name conflicts.", nameConflictCount, (nameConflictCount == 1) ? "file" : "files"));
                icon = MessageBoxIcon.Warning;
            }
            if (errorCount > 0)
            {
                sb.Append(string.Format(" {0} {1} detected.", errorCount, (errorCount == 1) ? "error was" : "errors were"));
                icon = MessageBoxIcon.Warning;
            }
            MessageBox.Show(this, sb.ToString(), "Rename Completed", MessageBoxButtons.OK, icon);

        }

        #endregion Image Renaming
    }
}
