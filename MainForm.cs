using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;


namespace ImageBulkRenamer
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
            BackgroundWorker wkr = new BackgroundWorker();
            wkr.DoWork += new DoWorkEventHandler(PreviewWkr_DoWork);
            wkr.RunWorkerCompleted += new RunWorkerCompletedEventHandler(PreviewWkr_RunWorkerCompleted);
            wkr.WorkerReportsProgress = true;
            wkr.ProgressChanged += new ProgressChangedEventHandler(PreviewWkr_ProgressChanged);
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
                ContextMenu menu = new ContextMenu();
                MenuItem item = menu.MenuItems.Add("&Edit");
                item.Click += new EventHandler(EditItem_Click);
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


        void EditItem_Click(object sender, EventArgs e)
        {
            var itemIdx = (int) ((MenuItem)sender).Tag;  // Index into items array.
            ShowItemEditDialog(itemIdx);
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

        private RenameItem[] items;


        void PreviewWkr_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker wkr = sender as BackgroundWorker;

            FileInfo[] files = imageDir.GetFiles("*.jp*g");
            items = new RenameItem[files.Length];

            for(int i = 0; i < files.Length; i++)
            {
                FileInfo fi = files[i];

                DateTime picTimestamp;
                try
                {
                    var bmp = new Bitmap(fi.FullName);
                    PropertyItem bmpDateTime = bmp.GetPropertyItem(0x0132);
                    string picTimestampStr = Encoding.ASCII.GetString(bmpDateTime.Value, 0, bmpDateTime.Len - 1);
                    DateTime.TryParseExact(picTimestampStr, "yyyy:MM:dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture,
                                           System.Globalization.DateTimeStyles.AssumeLocal, out picTimestamp);
                }
                catch (ArgumentException)
                {
                    picTimestamp = DateTime.MinValue;
                }

                var item = new RenameItem();
                item.InputFileName = fi.Name;
                item.InputExifTimestamp = picTimestamp;
                item.InputFileCreatonTimestamp = fi.CreationTimeUtc;
                item.InputFileModificationTimestamp = fi.LastWriteTimeUtc;
                if (picTimestamp != DateTime.MinValue)
                    item.OutputFileName = RenameItem.BuildFileName(picTimestamp, fi.Extension);
                item.OutputFileTimestamp = picTimestamp;
                items[i] = item;

                int progress = (int)(((double)(i + 1) / (double)files.Length) * 100D);
                wkr.ReportProgress(progress, i);
            }
        }


        void PreviewWkr_ProgressChanged(object sender, ProgressChangedEventArgs e)
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



        void PreviewWkr_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripProgressBar.Visible = false;
            toolStripStatusLabel.Text = "Ready";
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            this.UseWaitCursor = false;
            this.Enabled = true;
            startButton.Enabled = (listView.Items.Count > 0);
        }


        #endregion Preview



        #region Image Renaming

        private int okCount;
        private int skippedCount;
        private int errorCount;
        private const string okLabel = "OK";
        private const string skippedLabel = "Skipped";
        private const string errorLabel = "Error";

        void RenameWkr_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker wkr = sender as BackgroundWorker;

            okCount = skippedCount = errorCount = 0;

            for (int i = 0; i < items.Length; i++)
            {
                if ((items[i].OutputFileName != null) &&
                    (items[i].InputFileName != items[i].OutputFileName) &&
                    (items[i].OutputFileTimestamp != DateTime.MinValue))
                {
                    var sourceFileName = Path.Combine(imageDir.FullName, items[i].InputFileName);
                    var destFileName = Path.Combine(imageDir.FullName, items[i].OutputFileName);
                    try
                    {
                        File.Move(sourceFileName, destFileName);
                        File.SetCreationTimeUtc(destFileName, items[i].OutputFileTimestamp);
                        File.SetLastAccessTimeUtc(destFileName, items[i].OutputFileTimestamp);
                        items[i].RenameStatus = okLabel;
                        okCount++;
                    }
                    catch (Exception)
                    {
                        items[i].RenameStatus = errorLabel;
                        errorCount++;
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
            if (items[itemIdx].RenameStatus == skippedLabel)
            {
                lvi.BackColor = Color.Orange;
            }
            else if (items[itemIdx].RenameStatus == errorLabel)
            {
                lvi.BackColor = Color.Red;
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
            if (skippedCount > 0)
            {
                sb.Append(string.Format(" {0} image {1} skipped.", skippedCount, (skippedCount == 1) ? "file was" : "files were"));
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
