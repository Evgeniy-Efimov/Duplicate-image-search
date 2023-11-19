using EngineProject.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EngineProject.Helpers
{
    public static class ThreadHelperUI
    {
        delegate void PopListViewCallback(Form form, ListView listView, Button button, List<ImageFileDupGroup> dupGroups);

        public static void PopListView(Form form, ListView listView, Button button, List<ImageFileDupGroup> dupGroups)
        {
            if (listView.InvokeRequired)
            {
                PopListViewCallback callBackDelegate = new PopListViewCallback(PopListView);
                form.Invoke(callBackDelegate, new object[] { form, listView, button, dupGroups });
            }
            else
            {
                ImageList leftImageList = new ImageList();
                leftImageList.ImageSize = new Size(100, 100);
                listView.LargeImageList = leftImageList;

                int imageIndex = 0;
                foreach (var dupGroup in dupGroups)
                {
                    var firstGroupImage = dupGroup.ImageFiles.First();
                    using (var stream = new FileStream(firstGroupImage.FilePath, FileMode.Open, FileAccess.Read))
                    {
                        leftImageList.Images.Add(Image.FromStream(stream));
                        var fileNames = string.Join(", ", dupGroup.ImageFiles.Select(s => s.FileName).ToArray());
                        if (fileNames.Length > 30) fileNames = fileNames.Substring(0, 30) + "...";
                        var filePaths = string.Join(", ", dupGroup.ImageFiles.Select(s => s.FilePath).ToArray());
                        if (filePaths.Length > 90) filePaths = filePaths.Substring(0, 90) + "...";
                        listView.Items.Add(new ListViewItem
                        {
                            ImageIndex = imageIndex,
                            Text = fileNames,
                            Tag = filePaths
                        });
                    }
                    imageIndex++;
                }

                button.Text = "Search";
            }
        }


        delegate void ClearListViewCallback(Form form, ListView listView);

        public static void ClearListView(Form form, ListView listView)
        {
            if (listView.InvokeRequired)
            {
                ClearListViewCallback callBackDelegate = new ClearListViewCallback(ClearListView);
                form.Invoke(callBackDelegate, new object[] { form, listView });
            }
            else
            {
                listView.Clear();
            }
        }

        delegate void DisableButtonCallback(Form form, Button button);

        public static void DisableButton(Form form, Button button)
        {
            if (button.InvokeRequired)
            {
                DisableButtonCallback callBackDelegate = new DisableButtonCallback(DisableButton);
                form.Invoke(callBackDelegate, new object[] { form, button });
            }
            else
            {
                button.Enabled = false;
            }
        }

        delegate void ClearPictureBoxCallBack(Form form, PictureBox pictureBox);

        public static void ClearPictureBox(Form form, PictureBox pictureBox)
        {
            if (pictureBox.InvokeRequired)
            {
                ClearPictureBoxCallBack callBackDelegate = new ClearPictureBoxCallBack(ClearPictureBox);
                form.Invoke(callBackDelegate, new object[] { form, pictureBox });
            }
            else
            {
                pictureBox.Image = null;
            }
        }

        delegate void SetProgressBarCallBack(Form form, ProgressBar progressBar);

        public static void SetProgressBar(Form form, ProgressBar progressBar)
        {
            if (progressBar.InvokeRequired)
            {
                SetProgressBarCallBack callBackDelegate = new SetProgressBarCallBack(SetProgressBar);
                form.Invoke(callBackDelegate, new object[] { form, progressBar });
            }
            else
            {
                progressBar.Maximum = 100;
                progressBar.Step = 1;
                progressBar.Value = 0;
            }
        }

        delegate void SetProgressBarValueCallBack(Form form, ProgressBar progressBar, int percent);

        public static void SetProgressBarValue(Form form, ProgressBar progressBar, int percent)
        {
            if (progressBar.InvokeRequired)
            {
                SetProgressBarValueCallBack callBackDelegate = new SetProgressBarValueCallBack(SetProgressBarValue);
                form.Invoke(callBackDelegate, new object[] { form, progressBar, percent });
            }
            else
            {
                progressBar.Value = percent;
            }
        }

        delegate void SetButtonTextCallBack(Form form, Button button, string text);

        public static void SetButtonText(Form form, Button button, string text)
        {
            if (button.InvokeRequired)
            {
                SetButtonTextCallBack callBackDelegate = new SetButtonTextCallBack(SetButtonText);
                form.Invoke(callBackDelegate, new object[] { form, button, text });
            }
            else
            {
                button.Text = text;
            }
        }
    }
}
