using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineProject.Entities
{
    public class PixelSector
    {
        public List<PixelInfo> PixelsData = new List<PixelInfo>();

        private PixelsStats pixelsStats = null;

        public PixelsStats GetPixelsStats()
        {
            if (pixelsStats != null) return pixelsStats;
            pixelsStats = new PixelsStats();

            var pixelsCount = PixelsData.Count;
            double avR = PixelsData.Sum(s => s.R) / pixelsCount;
            double avG = PixelsData.Sum(s => s.G) / pixelsCount;
            double avB = PixelsData.Sum(s => s.B) / pixelsCount;
            double avBr = PixelsData.Sum(s => s.Brightness) / pixelsCount;

            double dispSumR = PixelsData.Sum(s => Math.Pow((s.R - avR), 2));
            double dispSumG = PixelsData.Sum(s => Math.Pow((s.G - avG), 2));
            double dispSumB = PixelsData.Sum(s => Math.Pow((s.B - avB), 2));
            double dispSumBr = PixelsData.Sum(s => Math.Pow((s.Brightness - avBr), 2));

            /*
            foreach (var pixel in PixelsData)
            {
                pixelsStats.AverageSectorR += pixel.R;
                pixelsStats.AverageSectorG += pixel.G;
                pixelsStats.AverageSectorB += pixel.B;
                pixelsStats.AverageSectorBrightness += pixel.Brightness;

                pixelsStats.MaxSectorR = pixel.R > pixelsStats.MaxSectorR ? pixel.R : pixelsStats.MaxSectorR;
                pixelsStats.MaxSectorG = pixel.G > pixelsStats.MaxSectorG ? pixel.G : pixelsStats.MaxSectorG;
                pixelsStats.MaxSectorB = pixel.B > pixelsStats.MaxSectorB ? pixel.B : pixelsStats.MaxSectorB;
                pixelsStats.MaxSectorBrightness = pixel.Brightness > pixelsStats.MaxSectorBrightness ? pixel.Brightness : pixelsStats.MaxSectorBrightness;

                pixelsStats.MinSectorR = pixel.R < pixelsStats.MinSectorR ? pixel.R : pixelsStats.MinSectorR;
                pixelsStats.MinSectorG = pixel.G < pixelsStats.MinSectorG ? pixel.G : pixelsStats.MinSectorG;
                pixelsStats.MinSectorB = pixel.B < pixelsStats.MinSectorB ? pixel.B : pixelsStats.MinSectorB;
                pixelsStats.MinSectorBrightness = pixel.Brightness < pixelsStats.MinSectorBrightness ? pixel.Brightness : pixelsStats.MinSectorBrightness;
            }

            pixelsStats.AverageSectorR /= pixelsCount;
            pixelsStats.AverageSectorG /= pixelsCount;
            pixelsStats.AverageSectorB /= pixelsCount;
            pixelsStats.AverageSectorBrightness /= pixelsCount;
            */
            pixelsStats.absoluteQDiffR = Math.Sqrt(dispSumR);
            pixelsStats.absoluteQDiffG = Math.Sqrt(dispSumG);
            pixelsStats.absoluteQDiffB = Math.Sqrt(dispSumB);
            pixelsStats.absoluteQDiffBr = Math.Sqrt(dispSumBr);

            PixelsData = null;
            return pixelsStats;
        }
    }
}
