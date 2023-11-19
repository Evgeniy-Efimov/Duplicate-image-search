using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineProject.Entities;
using System.Windows.Forms;
using EngineProject.Helpers;

namespace EngineProject.Managers
{
    public static class DupManager
    {
        public static ImageFileDupGroup GetDupGroupForImage(ImageFileComparing targetImage, List<ImageFileComparing> dupImagesList)
        {
            if (!targetImage.IsMarked) 
            {
                var group = new ImageFileDupGroup();
                foreach (var dupImage in dupImagesList.Where(w => !w.IsMarked
                    && targetImage.ImageFile.FilePath != w.ImageFile.FilePath))
                {
                    var dupCoef = dupImage.GetDupCoef(targetImage.ImageFile);
                    if (dupCoef <= SettingsManager.maxDupDifferenceInProcent)
                    {
                        dupImage.IsMarked = true;
                        group.ImageFiles.Add(dupImage.ImageFile);
                    }
                }
                if (group.ImageFiles.Any())
                {
                    targetImage.IsMarked = true;
                    group.ImageFiles.Add(targetImage.ImageFile);
                    return group;
                } 
            }
            return null;
        }

        public static List<ImageFileDupGroup> GetDups(List<ImageFile> images, ProgressBar progressBar, Form form)
        {
            var groupsList = new List<ImageFileDupGroup>();
            var imageComparingList = images.Select(s => new ImageFileComparing(s)).ToList();
            var imageListCount = imageComparingList.Count();

            int i = 0;
            foreach (var imageComparing in imageComparingList)
            { 
                var groupForImage = GetDupGroupForImage(imageComparing, imageComparingList);
                if (groupForImage != null) groupsList.Add(groupForImage);

                i++;
                var percentProgress = (int)((double)i / imageListCount * 100);
                ThreadHelperUI.SetProgressBarValue(form, progressBar, percentProgress);
            }
            return groupsList;
        }
    }
}
