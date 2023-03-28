using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Reflection;


namespace uk.andyjohnson.ImageBulkRenamer
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private DirectoryInfo imageDir;
        private PreviewWkrResult lastPreviewResult;


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
            var versionStr = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            MessageBox.Show(this,
                            $"Image File Bulk Renamer v{versionStr}\n\n" +
                            "by Andrew Johnson.| https://andyjohnson.uk\n\n" +
                            "See https://github.com/andyjohnson0/ImageBulkRenamer for info",
                            "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new FolderBrowserDialog();
            dlg.Description = "Select Image Folder";
            if (imageDir != null)
                dlg.SelectedPath = imageDir.FullName;
            else
                dlg.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (dlg.ShowDialog(this) == DialogResult.Cancel)
                return;
            DoPreview(new DirectoryInfo(dlg.SelectedPath));
        }


        private void listView_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo hitInfo = listView.HitTest(e.Location);
            if ((hitInfo == null) || (hitInfo.Item == null))
                return;

            if (e.Button == MouseButtons.Right)
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
            if (e.Button == MouseButtons.Left)
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
            if (lastPreviewResult == null)
                return;

            var itemIdx = (int)((ToolStripMenuItem)sender).Tag;  // Index into items array.
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = Path.Combine(imageDir.FullName, lastPreviewResult.Items[itemIdx].InputImageFileName);
            proc.Start();
        }


        private void ShowItemEditDialog(int itemIdx)
        {
            if (lastPreviewResult == null)
                return;

            var dlg = new RenameItemEditDialog(lastPreviewResult.Items[itemIdx]);
            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            lastPreviewResult.Items[itemIdx] = dlg.Item;
            var lvi = CreateListViewItem(lastPreviewResult.Items[itemIdx]);
            listView.Items[itemIdx] = lvi;
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }


        private void startButton_Click(object sender, EventArgs e)
        {
            if (lastPreviewResult == null)
                return;

            DoRename(this.imageDir);
        }


        private static ListViewItem CreateListViewItem(RenameItem item)
        {
            var lvi = new ListViewItem();
            lvi.Text = item.InputImageFileName;
            if (item.InputImageExifTimestamp != DateTime.MinValue)
                lvi.SubItems.Add(item.InputImageExifTimestamp.ToString("yyyy-MM-dd HH:mm:ss"));
            else
                lvi.SubItems.Add("");
            if (item.OutputImageFileName != null)
                lvi.SubItems.Add(item.OutputImageFileName);
            else
                lvi.SubItems.Add("");
            if (item.OutputImageFileTimestamp != DateTime.MinValue)
                lvi.SubItems.Add(item.OutputImageFileTimestamp.ToString("yyyy-MM-dd HH:mm:ss"));
            else
                lvi.SubItems.Add("");
            if (item.RenameStatus != null)
                lvi.SubItems.Add(item.RenameStatus);
            else
                lvi.SubItems.Add("");
            return lvi;
        }



        #region Preview

        private void DoPreview(DirectoryInfo di)
        {
            this.imageDir = di;

            this.UseWaitCursor = true;
            this.Enabled = false;
            var wkr = new BackgroundWorker();
            wkr.DoWork += new DoWorkEventHandler(PreviewWkr_DoWork);
            wkr.RunWorkerCompleted += new RunWorkerCompletedEventHandler(PreviewWkr_RunWorkerCompleted);
            wkr.WorkerReportsProgress = true;
            wkr.ProgressChanged += new ProgressChangedEventHandler(PreviewWkr_ProgressChanged);
            var workerArgs = new PreviewWkrArgs
            {
                FallbackToFileCreationTimestamp = fallbackToFileCreationCb.Checked,
                RenameSidecarFiles = renameXmpSidecarFilesCb.Checked
            };
            wkr.RunWorkerAsync(workerArgs);
        }


        private class PreviewWkrArgs
        {
            public bool FallbackToFileCreationTimestamp;
            public bool RenameSidecarFiles;
        }

        private class PreviewWkrResult
        {
            public RenameItem[] Items;
            public int NoExifTimestampCount = 0;
            public int FallbackTimestampCount = 0;
        }


        void PreviewWkr_DoWork(object sender, DoWorkEventArgs e)
        {
            var wkr = sender as BackgroundWorker;
            var args = e.Argument as PreviewWkrArgs;
            var result = new PreviewWkrResult();

            result.NoExifTimestampCount = 0;

            FileInfo[] files = this.imageDir.GetFiles("*.jp*g");
            result.Items = new RenameItem[files.Length];

            for(int i = 0; i < files.Length; i++)
            {
                var imageFi = files[i];

                DateTime picTimestampLocal;
                try
                {
                    using (var bmp = new Bitmap(imageFi.FullName))
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
                    if (args.FallbackToFileCreationTimestamp)
                    {
                        picTimestampLocal = imageFi.CreationTime;
                        result.NoExifTimestampCount++;
                        result.FallbackTimestampCount++;
                    }
                    else
                    {
                        picTimestampLocal = DateTime.MinValue;
                        result.NoExifTimestampCount++;
                    }
                }

                // Image file
                var item = new RenameItem()
                {
                    InputImageFileName = imageFi.Name,
                    InputImageExifTimestamp = picTimestampLocal,
                    InputImageFileCreatonTimestamp = imageFi.CreationTimeUtc,
                    InputImageFileModificationTimestamp = imageFi.LastWriteTimeUtc
                };
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
                            if (result.Items[j].OutputImageFileName == outputFileName)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            item.OutputImageFileName = outputFileName;
                            break;
                        }
                    }
                }
                item.OutputImageFileTimestamp = picTimestampLocal;

                // Optional sidecar file
                var sidecarFi = new FileInfo(imageFi.FullName + ".xmp");
                if (sidecarFi.Exists)
                {
                    item.InputSidecarFileName = sidecarFi.Name;
                    item.OutputSidecarFileName = item.OutputImageFileName + ".xmp";
                }

                //
                result.Items[i] = item;

                int progressPct = (int)(((double)(i + 1) / (double)files.Length) * 100D);
                wkr.ReportProgress(progressPct, (i, result.Items[i]));
            }

            e.Result = result;
        }


        void PreviewWkr_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var state = ((int itemIdx, RenameItem item))e.UserState;

            if (state.itemIdx == 0)
                toolStripProgressBar.Visible = true;
            toolStripProgressBar.Value = e.ProgressPercentage;
            toolStripStatusLabel.Text = string.Format("Building preview ({0}%)", e.ProgressPercentage);

            listView.BeginUpdate();
            if (state.itemIdx == 0)
                listView.Items.Clear();
            var lvi = CreateListViewItem(state.item);
            listView.Items.Add(lvi);
            lvi.EnsureVisible();
            listView.EndUpdate();
        }


        void PreviewWkr_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = e.Result as PreviewWkrResult;
            this.lastPreviewResult = result;

            listView.EnsureVisible(0);
            toolStripProgressBar.Visible = false;
            toolStripStatusLabel.Text = "Ready";
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            this.UseWaitCursor = false;
            this.Enabled = true;
            startButton.Enabled = (listView.Items.Count > 0);

            if (result.NoExifTimestampCount > 0)
            {
                string msg = string.Format("{0} {1} had no EXIF timestamp. {2} file creation {3} were used.",
                                           result.NoExifTimestampCount,
                                           (result.NoExifTimestampCount == 1) ? "image" : "images",
                                           result.FallbackTimestampCount,
                                           (result.FallbackTimestampCount == 1) ? "timestamp" : "timestamps");
                MessageBox.Show(this, msg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion Preview



        #region Image Renaming


        private void DoRename(
            DirectoryInfo di)
        {
            this.UseWaitCursor = true;
            this.Enabled = false;

            var wkr = new BackgroundWorker();
            wkr.DoWork += new DoWorkEventHandler(RenameWkr_DoWork);
            wkr.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RenameWkr_RunWorkerCompleted);
            wkr.WorkerReportsProgress = true;
            wkr.ProgressChanged += new ProgressChangedEventHandler(RenameWkr_ProgressChanged);
            var workerArgs = new RenameWkrArgs()
            {
                Items = lastPreviewResult.Items
            };
            wkr.RunWorkerAsync(workerArgs);
        }

        private class RenameWkrArgs
        {
            public RenameItem[] Items;
        }

        private class RenameWkrResult
        {
            public int OkCount = 0;
            public int NoChangeCount = 0;
            public int SkippedCount = 0;
            public int NameConflictCount = 0;
            public int ErrorCount = 0;
        }

        private static class RenameStatus
        {
            public const string OkLabel = "OK";
            public const string NoChangeLabel = "No change";
            public const string SkippedLabel = "Skipped";
            public const string NameConflictLabel = "Filename already exists";
            public const string ErrorLabel = "Error";
        }


        void RenameWkr_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker wkr = sender as BackgroundWorker;
            var args = e.Argument as RenameWkrArgs;
            var result = new RenameWkrResult();

            for (int i = 0; i < args.Items.Length; i++)
            {
                if (args.Items[i].OutputImageFileName != null)
                {
                    var sourceImageFileName = Path.Combine(imageDir.FullName, args.Items[i].InputImageFileName);
                    var destImageFileName = Path.Combine(imageDir.FullName, args.Items[i].OutputImageFileName);

                    if (string.Equals(args.Items[i].InputImageFileName, args.Items[i].OutputImageFileName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        args.Items[i].RenameStatus = RenameStatus.NoChangeLabel;
                        result.NoChangeCount++;
                    }
                    else if (File.Exists(destImageFileName))
                    {
                        args.Items[i].RenameStatus = RenameStatus.NameConflictLabel;
                        result.NameConflictCount++;
                    }
                    else
                    {
                        bool succeeded = true;

                        // Rename image
                        try
                        {
                            File.Move(sourceImageFileName, destImageFileName);
                        }
                        catch (Exception)
                        {
                            succeeded = false;
                        }

                        // Redate
                        try
                        {
                            if (args.Items[i].OutputImageFileTimestamp != DateTime.MinValue)
                            {
                                File.SetCreationTime(destImageFileName, args.Items[i].OutputImageFileTimestamp);
                                File.SetLastWriteTime(destImageFileName, args.Items[i].OutputImageFileTimestamp);
                            }
                        }
                        catch (Exception)
                        {
                            succeeded = false;
                        }

                        if (succeeded && !string.IsNullOrEmpty(args.Items[i].InputSidecarFileName) && !string.IsNullOrEmpty(args.Items[i].OutputSidecarFileName))
                        {
                            var sourceSidecarFileName = Path.Combine(imageDir.FullName, args.Items[i].InputSidecarFileName);
                            var destSidecarFileName = Path.Combine(imageDir.FullName, args.Items[i].OutputSidecarFileName);
                            try
                            {
                                File.Move(sourceSidecarFileName, destSidecarFileName);
                            }
                            catch (Exception)
                            {
                                succeeded = false;
                            }
                        }

                        if (succeeded)
                        {
                            args.Items[i].RenameStatus = RenameStatus.OkLabel;
                            result.OkCount++;
                        }
                        else
                        {
                            args.Items[i].RenameStatus = RenameStatus.ErrorLabel;
                            result.ErrorCount++;
                        }
                    }
                }
                else
                {
                    args.Items[i].RenameStatus = RenameStatus.SkippedLabel;
                    result.SkippedCount++;
                }

                int progressPct = (int)(((double)(i + 1) / (double)args.Items.Length) * 100D);
                wkr.ReportProgress(progressPct, (i, args.Items[i]));
            }

            e.Result = result;
        }


        void RenameWkr_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var state = ((int itemIdx, RenameItem item))e.UserState;

            if (state.itemIdx == 0)
                toolStripProgressBar.Visible = true;
            toolStripProgressBar.Value = e.ProgressPercentage;
            toolStripStatusLabel.Text = string.Format("Renaming files ({0}%)", e.ProgressPercentage);

            listView.BeginUpdate();
            ListViewItem lvi = listView.Items[state.itemIdx];
            lvi.SubItems[4].Text = state.item.RenameStatus;
            switch(state.item.RenameStatus)
            {
                case RenameStatus.OkLabel:
                    break;
                case RenameStatus.NoChangeLabel:
                    break;
                case RenameStatus.SkippedLabel:
                    lvi.BackColor = Color.Orange;
                    break;
                case RenameStatus.NameConflictLabel:
                    lvi.BackColor = Color.Orange;
                    break;
                case RenameStatus.ErrorLabel:
                    lvi.BackColor = Color.Red;
                    break;
            }
            lvi.EnsureVisible();
            listView.EndUpdate();
        }


        void RenameWkr_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = e.Result as RenameWkrResult;

            toolStripProgressBar.Visible = false;
            toolStripStatusLabel.Text = "Ready";
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            this.UseWaitCursor = false;
            this.Enabled = true;

            var sb = new StringBuilder();
            sb.Append(string.Format("{0} image {1} renamed.", result.OkCount, (result.OkCount == 1) ? "file was" : "files were"));
            var icon = MessageBoxIcon.Information;
            if (result.NoChangeCount > 0)
            {
                sb.Append(string.Format(" {0} image {1} not renamed.", result.NoChangeCount, (result.NoChangeCount == 1) ? "file was" : "files were"));
            }
            if (result.SkippedCount > 0)
            {
                sb.Append(string.Format(" {0} image {1} skipped.", result.SkippedCount, (result.SkippedCount == 1) ? "file was" : "files were"));
                icon = MessageBoxIcon.Warning;
            }
            if (result.NameConflictCount > 0)
            {
                sb.Append(string.Format(" {0} image {1} had name conflicts.", result.NameConflictCount, (result.NameConflictCount == 1) ? "file" : "files"));
                icon = MessageBoxIcon.Warning;
            }
            if (result.ErrorCount > 0)
            {
                sb.Append(string.Format(" {0} {1} detected.", result.ErrorCount, (result.ErrorCount == 1) ? "error was" : "errors were"));
                icon = MessageBoxIcon.Warning;
            }
            MessageBox.Show(this, sb.ToString(), "Rename Completed", MessageBoxButtons.OK, icon);
        }

        #endregion Image Renaming
    }
}
