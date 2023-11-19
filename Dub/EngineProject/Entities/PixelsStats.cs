using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineProject.Entities
{
    public class PixelsStats
    {
        /*
        public double AverageSectorR { get; set; }
        public double AverageSectorG { get; set; }
        public double AverageSectorB { get; set; }
        public double AverageSectorBrightness { get; set; }

        public double MinSectorR { get; set; }
        public double MinSectorG { get; set; }
        public double MinSectorB { get; set; }
        public double MinSectorBrightness { get; set; }

        public double MaxSectorR { get; set; }
        public double MaxSectorG { get; set; }
        public double MaxSectorB { get; set; }
        public double MaxSectorBrightness { get; set; }*/

        public double absoluteQDiffR { get; set; }
        public double absoluteQDiffG { get; set; }
        public double absoluteQDiffB { get; set; }
        public double absoluteQDiffBr { get; set; }

        private const short ARGBSize = 255;
        private const short BrightnessSize = 1;

        public static double GetSectorDifferenceInProcent(PixelsStats target, PixelsStats dup)
        {
            var statsToAverage = new List<double>();
            /*
            statsToAverage.Add(Math.Abs((target.AverageSectorR - dup.AverageSectorR) / ARGBSize));
            statsToAverage.Add(Math.Abs((target.AverageSectorG - dup.AverageSectorG) / ARGBSize));
            statsToAverage.Add(Math.Abs((target.AverageSectorB - dup.AverageSectorB) / ARGBSize));
            statsToAverage.Add(Math.Abs((target.AverageSectorBrightness - dup.AverageSectorBrightness) / BrightnessSize));

            statsToAverage.Add(Math.Abs((target.MinSectorR - dup.MinSectorR) / ARGBSize));
            statsToAverage.Add(Math.Abs((target.MinSectorG - dup.MinSectorG) / ARGBSize));
            statsToAverage.Add(Math.Abs((target.MinSectorB - dup.MinSectorB) / ARGBSize));
            statsToAverage.Add(Math.Abs((target.MinSectorBrightness - dup.MinSectorBrightness) / BrightnessSize));

            statsToAverage.Add(Math.Abs((target.MaxSectorR - dup.MaxSectorR) / ARGBSize));
            statsToAverage.Add(Math.Abs((target.MaxSectorG - dup.MaxSectorG) / ARGBSize));
            statsToAverage.Add(Math.Abs((target.MaxSectorB - dup.MaxSectorB) / ARGBSize));
            statsToAverage.Add(Math.Abs((target.MaxSectorBrightness - dup.MaxSectorBrightness) / BrightnessSize));
            */
            statsToAverage.Add(target.absoluteQDiffR > dup.absoluteQDiffR
                ? dup.absoluteQDiffR / target.absoluteQDiffR
                : target.absoluteQDiffR / dup.absoluteQDiffR);

            statsToAverage.Add(target.absoluteQDiffG > dup.absoluteQDiffG
                ? dup.absoluteQDiffG / target.absoluteQDiffG
                : target.absoluteQDiffG / dup.absoluteQDiffG);

            statsToAverage.Add(target.absoluteQDiffB > dup.absoluteQDiffB
                ? dup.absoluteQDiffB / target.absoluteQDiffB
                : target.absoluteQDiffB / dup.absoluteQDiffB);

            statsToAverage.Add(target.absoluteQDiffBr > dup.absoluteQDiffBr
                ? dup.absoluteQDiffBr / target.absoluteQDiffBr
                : target.absoluteQDiffBr / dup.absoluteQDiffBr);

            return Math.Abs((statsToAverage.Sum(s => s) / statsToAverage.Count()) - 1);
        }

        /*
        public static double GetMaxDifferenceInProcent(PixelsStats target, PixelsStats dup)
        {
            var diff = new List<double>();
            diff.Add(Math.Abs((target.AverageSectorR - dup.AverageSectorR) / ARGBSize));
            diff.Add(Math.Abs((target.AverageSectorG - dup.AverageSectorG) / ARGBSize));
            diff.Add(Math.Abs((target.AverageSectorB - dup.AverageSectorB) / ARGBSize));
            diff.Add(Math.Abs((target.AverageSectorBrightness - dup.AverageSectorBrightness) / BrightnessSize));
            return diff.Max();
        }
        */
    }
}
