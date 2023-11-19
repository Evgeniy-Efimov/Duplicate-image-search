using EngineProject.Managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineProject.Entities
{
    public class ImageFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        private Bitmap Bitmap { get; set; }
        private Bitmap OriginalBitmap { get; set; }
        public void DisposeOriginalBitmap()
        {
            OriginalBitmap.Dispose();
            GC.SuppressFinalize(OriginalBitmap);
            OriginalBitmap = null;
        }
        public Bitmap GetOriginalBitmap()
        {
            if (OriginalBitmap != null /*&& OriginalBitmap.*/) return OriginalBitmap;
            var ms = new MemoryStream();
            ms.Write(OriginalBitmapBytes, 0, Convert.ToInt32(OriginalBitmapBytes.Length));
            var bitmap = new Bitmap(ms, false);
            ms.Dispose();
            OriginalBitmap = bitmap;
            return bitmap;
        }
        public byte[] OriginalBitmapBytes { get; set; }
        public bool Mooved { get; set; }
        public bool Removed { get; set; }

        public PixelInfo[][] PixelsData { get; set; }
        public PixelSector[] PixelSectors { get; set; }

        public ImageFile(FileInfo fileInfo, int sectorsCount)
        {
            FileName = fileInfo.Name;
            FilePath = fileInfo.FullName;

            var fileBytes = File.ReadAllBytes(fileInfo.FullName);
            OriginalBitmapBytes = fileBytes;
            var ms = new MemoryStream(fileBytes);
            var originalBitmap = (Bitmap)Image.FromStream(ms);
            Bitmap = originalBitmap.Clone(new Rectangle(0, 0, originalBitmap.Width, originalBitmap.Height),
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            originalBitmap.Dispose();
            ms.Dispose();

            int width = Bitmap.Width;
            int height = Bitmap.Height;

            var widthOverflow = (double)width / SettingsManager.maxWidth;
            var heightOverflow = (double)height / SettingsManager.maxHeight;
            if (widthOverflow > 1 || height > 1)
            {
                var maxOverflow = widthOverflow > heightOverflow ? widthOverflow : heightOverflow;
                int newWidth = (int)(width / maxOverflow);
                int newHeight = (int)(height / maxOverflow);
                var newBitmap = new Bitmap(newWidth, newHeight);
                using (Graphics graphics = Graphics.FromImage((Image)newBitmap))
                {
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                    graphics.DrawImage(Bitmap, 0, 0, newWidth, newHeight);
                }
                Bitmap.Dispose();
                Bitmap = newBitmap;
                width = newWidth;
                height = newHeight;
            }

            PixelSectors = new PixelSector[sectorsCount];
            PixelsData = new PixelInfo[Bitmap.Width][];

            if (sectorsCount > width + 1) throw new Exception($"File {FilePath} has small resolution ({width}x{height}) for sectors count {sectorsCount}");
            if (sectorsCount > height + 1) throw new Exception($"File {FilePath} has small resolution ({width}x{height}) for sectors count {sectorsCount}");

            int sectorsInRowCount = (int)Math.Sqrt(sectorsCount);
            int sectorWidth = width / sectorsInRowCount;
            int sectorHeight = height / sectorsInRowCount;
            for (int x = 0; x < width; x++)
            {
                PixelInfo[] PixelsColumn = new PixelInfo[Bitmap.Height];
                for (int y = 0; y < height; y++)
                {
                    var color = Bitmap.GetPixel(x, y);
                    var pixel = new PixelInfo()
                    {
                        R = color.R,
                        G = color.G,
                        B = color.B,
                        Brightness = color.GetBrightness(),
                        X = x,
                        Y = y
                    };
                    PixelsColumn[y] = pixel;
                    var sectorNumber = GetSectorNumber(x, y, sectorWidth, sectorHeight, sectorsInRowCount);
                    if (PixelSectors[sectorNumber - 1] == null) PixelSectors[sectorNumber - 1] = new PixelSector();
                    PixelSectors[sectorNumber - 1].PixelsData.Add(pixel);
                }
                PixelsData[x] = PixelsColumn;
            }
            Bitmap.Dispose();
            for (int i = 0; i < PixelSectors.Count(); i++)
            {
                PixelSectors[i].GetPixelsStats();
            }
            PixelsData = null;
            
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private int GetSectorNumber(int x, int y, int sectorWidth, int sectorHeight, int sectorsInRowCount)
        {
            int xIndex = (int)Math.Ceiling((double)x / sectorWidth);
            int yIndex = (int)Math.Ceiling((double)y / sectorHeight);

            Func<int, int> normalize = (int a) => { return a == 0 ? 1 : a > sectorsInRowCount ? sectorsInRowCount : a; };
            xIndex = normalize(xIndex);
            yIndex = normalize(yIndex);

            return (yIndex - 1) * sectorsInRowCount + xIndex;
        }
    }
}
