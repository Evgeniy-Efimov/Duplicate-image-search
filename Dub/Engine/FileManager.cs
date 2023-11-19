using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Engine.Entities;

namespace Engine
{
    public static class FileManager
    {
        public static List<ImageFile> GetAllImagesOnPath(string path, List<string> ext)
        {
            return GetAllFilesInDirectory(path, ext);        
        }

        private static List<ImageFile> GetAllFilesInDirectory(string path, List<string> ext)
        {
            var directoryInfo = new DirectoryInfo(path);
            var files = new List<ImageFile>();
            files.AddRange(directoryInfo.GetFiles().Where(w => ext.Contains(w.Extension))
                .Select(s => new ImageFile() { FileName = s.Name, FilePath = s.FullName }));
            foreach (var directory in directoryInfo.GetDirectories())
            {
                files.AddRange(GetAllFilesInDirectory(directory.FullName, ext));
            }
            return files;
        }
    }
}
