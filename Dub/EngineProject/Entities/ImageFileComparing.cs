using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineProject.Entities
{
    public class ImageFileComparing
    {
        public ImageFile ImageFile { get; set; }
        public bool IsMarked { get; set; }

        public double GetDupCoef(ImageFile dupImage)
        {
            double diff = 0;
            int pixelSectorsCount = dupImage.PixelSectors.Count();
            for (int i = 0; i < pixelSectorsCount; i++)
            {
                var stats = ImageFile.PixelSectors[i].GetPixelsStats();
                var dupStats = dupImage.PixelSectors[i].GetPixelsStats();
                diff += PixelsStats.GetSectorDifferenceInProcent(stats, dupStats);
            }
            return diff / pixelSectorsCount;
        }

        public ImageFileComparing(ImageFile imageFile)
        {
            ImageFile = imageFile;
            IsMarked = false;
        }
    }
}
