using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SteamCloudFileManager
{
    public partial class MainForm : Form
    {
        IRemoteStorage storage;

        public MainForm()
        {
            InitializeComponent();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            try
            {
                uint appId;
                if (string.IsNullOrWhiteSpace(appIdTextBox.Text))
                {
                    MessageBox.Show(this, "Please enter an App ID.", "Failed to connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!uint.TryParse(appIdTextBox.Text.Trim(), out appId))
                {
                    MessageBox.Show(this, "Please make sure the App ID you entered is valid.", "Failed to connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                storage = RemoteStorage.CreateInstance(uint.Parse(appIdTextBox.Text));
                //storage = new RemoteStorageLocal("remote", uint.Parse(appIdTextBox.Text));
                refreshButton.Enabled = true;
                refreshButton_Click(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Failed to connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            if (storage == null)
            {
                MessageBox.Show(this, "Not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            try
            {
                List<IRemoteFile> files = storage.GetFiles();
                remoteListView.Items.Clear();
                foreach (IRemoteFile file in files)
                {
                    ListViewItem itm = new ListViewItem(new string[] { file.Name, file.Timestamp.ToString(), file.Size.ToString(), file.IsPersisted.ToString(), file.Exists.ToString() }) { Tag = file };
                    remoteListView.Items.Add(itm);
                }
                updateQuota();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Can't refresh." + Environment.NewLine + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        void updateQuota()
        {
            if (storage == null) throw new InvalidOperationException("Not connected");
            int totalBytes, availBytes;
            storage.GetQuota(out totalBytes, out availBytes);
            quotaLabel.Text = string.Format("{0}/{1} bytes used", totalBytes - availBytes, totalBytes);
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            if (storage == null)
            {
                MessageBox.Show(this, "Not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (remoteListView.SelectedIndices.Count != 1)
            {
                MessageBox.Show(this, "Please select only one file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            IRemoteFile file = remoteListView.SelectedItems[0].Tag as IRemoteFile;
            saveFileDialog1.FileName = Path.GetFileName(file.Name);
            if (saveFileDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    File.WriteAllBytes(saveFileDialog1.FileName, file.ReadAllBytes());
                    MessageBox.Show(this, "File downloaded.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "File download failed." + Environment.NewLine + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (storage == null)
            {
                MessageBox.Show(this, "Not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (remoteListView.SelectedIndices.Count == 0)
            {
                MessageBox.Show(this, "Please select files to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show(this, "Are you sure you want to delete the selected files?", "Confirm deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.No) return;

            bool allSuccess = true;

            foreach (ListViewItem item in remoteListView.SelectedItems)
            {
                IRemoteFile file = item.Tag as IRemoteFile;
                try
                {
                    bool success = file.Delete();
                    if (!success)
                    {
                        allSuccess = false;
                        MessageBox.Show(this, file.Name + " failed to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        item.Remove();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, file.Name + " failed to delete." + Environment.NewLine + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            updateQuota();
            if (allSuccess) MessageBox.Show(this, "Files deleted.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void remoteListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            downloadButton.Enabled = deleteButton.Enabled = (storage != null && remoteListView.SelectedIndices.Count > 0);
        }
    }
}
