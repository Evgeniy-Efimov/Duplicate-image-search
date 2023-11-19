using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EngineProject.Managers;
using EngineProject.Entities;
using System.Threading;
using EngineProject.Helpers;

namespace Dub
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            try
            {
                InitializeComponent();
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                numericUpDown1.Value = (decimal)Math.Sqrt(SettingsManager.sectorsCount);
                textBox1.Text = getSectorsCount((int)numericUpDown1.Value).ToString();
                numericUpDown2.Value = (int)(SettingsManager.maxDupDifferenceInProcent * 100);
                textBox3.Text = SettingsManager.path;
            }
            catch(Exception ex)
            {
                LogManager.LogException(ex, "Initializing error");
            }
        }

        private List<ImageFileDupGroup> dupGroups = new List<ImageFileDupGroup>();
        private int? SelectedGroupIndex = null;
        private int? SelectedImageIndex = null;
        private ImageFile selectedImage = null;
        private Thread searchingThread = null;
        private string previousPath = null;
        private List<ImageFile> imageFiles = null;

        private int getSectorsCount(int newValue)
        {
            var min = 4; var max = 81;
            var sectorsCount = newValue * newValue;
            if (sectorsCount < min) sectorsCount = min;
            if (sectorsCount > max) sectorsCount = max;
            return sectorsCount;
        }

        private void numericUpDown1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void numericUpDown1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            var sectorsCount = getSectorsCount((int)numericUpDown1.Value);
            SettingsManager.sectorsCount = sectorsCount;
            textBox1.Text = sectorsCount.ToString();
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            SettingsManager.maxDupDifferenceInProcent = (double)numericUpDown2.Value / 100;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                var openFolderDialog = new FolderBrowserDialog();
                var dialog = openFolderDialog.ShowDialog();
                if (dialog == System.Windows.Forms.DialogResult.OK)
                {
                    string folderName = openFolderDialog.SelectedPath;
                    textBox3.Text = folderName;
                    SettingsManager.path = folderName;
                }
            }
            catch(Exception ex)
            {
                LogManager.LogException(ex, "Select folder error");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SettingsManager.path)) throw new Exception("Path to folder is empty");
                if (searchingThread == null || !searchingThread.IsAlive)
                {
                    button1.Text = "Stop searching";

                    searchingThread = new Thread(new ThreadStart(
                        () =>
                        {
                            try
                            {
                                ThreadHelperUI.ClearListView(this, listView1);
                                ThreadHelperUI.ClearListView(this, listView2);
                                ThreadHelperUI.ClearPictureBox(this, pictureBox1);
                                selectedImage = null;
                                SelectedGroupIndex = null;
                                SelectedImageIndex = null;

                                ThreadHelperUI.DisableButton(this, button2);
                                ThreadHelperUI.DisableButton(this, button3);
                                ThreadHelperUI.DisableButton(this, button4);

                                ThreadHelperUI.SetProgressBar(this, progressBar1);
                                ThreadHelperUI.SetProgressBar(this, progressBar2);

                                var path = SettingsManager.path;
                                var ext = SettingsManager.extentions;
                                var sectorsCount = SettingsManager.sectorsCount;

                                List<ImageFile> images = new List<ImageFile>();
                                if (previousPath == path && imageFiles != null)
                                {
                                    images = imageFiles;
                                    ThreadHelperUI.SetProgressBarValue(this, progressBar1, 100);
                                }
                                else
                                {
                                    images = FileManager.GetAllImagesOnPath(path, ext, sectorsCount, this, progressBar1);
                                }

                                if (dupGroups != null)
                                {
                                    dupGroups = null;
                                    GC.Collect();
                                    GC.WaitForPendingFinalizers();
                                }
                                dupGroups = DupManager.GetDups(images, progressBar2, this);
                                previousPath = path;
                                imageFiles = images;

                                ThreadHelperUI.PopListView(this, listView1, button1, dupGroups);
                            }
                            catch(Exception threadEx)
                            {
                                LogManager.LogException(threadEx, "Searching error");
                                ThreadHelperUI.SetButtonText(this, button1, "Search");
                                ThreadHelperUI.SetProgressBar(this, progressBar1);
                                ThreadHelperUI.SetProgressBar(this, progressBar2);
                            }
                        }
                        ));

                    searchingThread.Start();
                }
                else
                {
                    if (searchingThread != null && searchingThread.IsAlive)
                        searchingThread.Abort();
                    button1.Text = "Search";
                    ThreadHelperUI.SetProgressBar(this, progressBar1);
                    ThreadHelperUI.SetProgressBar(this, progressBar2);
                }
            }
            catch(Exception ex)
            {
                LogManager.LogException(ex, "Searching error");
                if (searchingThread != null && searchingThread.IsAlive)
                    searchingThread.Abort();
                button1.Text = "Search";
                ThreadHelperUI.SetProgressBar(this, progressBar1);
                ThreadHelperUI.SetProgressBar(this, progressBar2);
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    SelectedGroupIndex = listView1.SelectedItems[0].ImageIndex;

                    listView2.Clear();

                    ImageList rightImageList = new ImageList();
                    rightImageList.ImageSize = new Size(70, 70);
                    listView2.LargeImageList = rightImageList;

                    int imageIndex = 0;
                    foreach (var dupImage in dupGroups[(int)SelectedGroupIndex].ImageFiles)
                    {
                        using (var stream = new FileStream(dupImage.FilePath, FileMode.Open, FileAccess.Read))
                        {
                            rightImageList.Images.Add(Image.FromStream(stream));
                            var fileName = dupImage.FileName;
                            if (fileName.Length > 30) fileName = fileName.Substring(0, 30) + "...";
                            var filePath = dupImage.FilePath;
                            if (filePath.Length > 90) filePath = filePath.Substring(0, 90) + "...";
                            listView2.Items.Add(new ListViewItem
                            {
                                ImageIndex = imageIndex,
                                Text = fileName,
                                Tag = filePath
                            });
                        }
                        imageIndex++;
                    }
                }
            }
            catch(Exception ex)
            {
                LogManager.LogException(ex, "List view 1 error");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void listView2_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (SelectedGroupIndex != null
                    && listView2.SelectedItems.Count > 0)
                {
                    if (selectedImage != null)
                    { 
                        selectedImage.DisposeOriginalBitmap();
                    }
                    SelectedImageIndex = listView2.SelectedItems[0].ImageIndex;
                    selectedImage = dupGroups[(int)SelectedGroupIndex].ImageFiles[(int)SelectedImageIndex];
                    pictureBox1.Image = selectedImage.GetOriginalBitmap();
                    pictureBox1.Refresh();
                    button2.Enabled = !selectedImage.Removed;
                    button3.Enabled = !selectedImage.Mooved;
                    button4.Enabled = !selectedImage.Mooved;
                }
            }
            catch(Exception ex)
            {
                LogManager.LogException(ex, "List view 2 error");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (SelectedGroupIndex != null
                    && SelectedImageIndex != null)
                {
                    var selectedImage = dupGroups[(int)SelectedGroupIndex].ImageFiles[(int)SelectedImageIndex];
                    System.Diagnostics.Process.Start(Path.GetDirectoryName(selectedImage.FilePath));
                }
            }
            catch(Exception ex)
            {
                LogManager.LogException(ex, "Open file location error");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (SelectedGroupIndex != null
                    && SelectedImageIndex != null
                    && selectedImage != null)
                {
                    FileManager.MoveFileToDupFolder(selectedImage);
                    imageFiles.Remove(selectedImage);
                    button3.Enabled = !selectedImage.Mooved;
                    button4.Enabled = !selectedImage.Mooved;
                }
            }
            catch(Exception ex)
            {
                LogManager.LogException(ex, "Moving file error");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (SelectedGroupIndex != null
                    && SelectedImageIndex != null
                    && selectedImage != null)
                {
                    FileManager.RemoveFile(selectedImage);
                    button2.Enabled = !selectedImage.Removed;
                    button3.Enabled = !selectedImage.Mooved;
                    button4.Enabled = !selectedImage.Mooved;
                    listView2.Items[(int)SelectedImageIndex].Remove();
                    imageFiles.Remove(selectedImage);
                    dupGroups[(int)SelectedGroupIndex].ImageFiles.Remove(selectedImage);
                    ThreadHelperUI.ClearPictureBox(this, pictureBox1);
                    SelectedImageIndex = null;
                    selectedImage = null;
                }
            }
            catch(Exception ex)
            {
                LogManager.LogException(ex, "Removal file error");
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            SettingsManager.path = textBox3.Text;
        }
    }
}
