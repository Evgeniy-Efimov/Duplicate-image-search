using EngineProject.Entities;
using EngineProject.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EngineProject.Managers
{
    public static class FileManager
    {
        public static ImageConverter imageConverter = new ImageConverter();
        public static List<ImageFile> GetAllImagesOnPath(string path, List<string> ext, int sectorsCount, Form form, ProgressBar progressBar)
        {
            var listOfImages = new List<ImageFile>();
            var filesList = GetAllFilesInDirectory(path, ext, sectorsCount);
            var filesCount = filesList.Count;

            int i = 0;
            foreach (var file in filesList)
            {
                listOfImages.Add(new ImageFile(file, sectorsCount));

                i++;
                var percentProgress = (int)((double)i / filesCount * 100);
                ThreadHelperUI.SetProgressBarValue(form, progressBar, percentProgress);
            }
            return listOfImages;
        }

        public static void MoveFileToDupFolder(ImageFile image)
        {
            var newFolderPath = Path.Combine(SettingsManager.path, SettingsManager.dupFolderName);
            if (!Directory.Exists(newFolderPath))
            {
                Directory.CreateDirectory(newFolderPath);
            }
            var newFileName = $"moved_{DateTime.Now.ToString("dd_MM_yyyy HH_mm_ss")}_" + image.FileName;
            var newFilePath = Path.Combine(newFolderPath, newFileName);
            File.Move(image.FilePath, newFilePath);
            image.FileName = newFileName;
            image.FilePath = newFilePath;
            image.Mooved = true;
        }

        public static void RemoveFile(ImageFile image)
        {
            File.Delete(image.FilePath);
            image.Mooved = true;
            image.Removed = true;
        }

        private static List<FileInfo> GetAllFilesInDirectory(string path, List<string> ext, int sectorsCount)
        {
            var directoryInfo = new DirectoryInfo(path);
            var files = new List<FileInfo>();
            files.AddRange(directoryInfo.GetFiles()
                .Where(w => ext.Select(s => s.ToLower()).Contains(w.Extension.ToLower()))
                .ToList());
            foreach (var directory in directoryInfo.GetDirectories()
                .Where(w => w.Name != SettingsManager.dupFolderName))
            {
                files.AddRange(GetAllFilesInDirectory(directory.FullName, ext, sectorsCount));
            }
            return files;
        }
    }
}
