using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusixSync.Networking.Interfaces;

namespace MusixSync.Networking.Models
{
    public class SimpleFileProvider : IFileProvider
    {
        public string SourceDirectory;

        public SimpleFileProvider(string sourceDirectory) => SourceDirectory = sourceDirectory;

        public List<string> GetLocalFiles()
        {
            return Directory.GetFiles(SourceDirectory, "*.mp3").Select(x => new FileInfo(x).Name).ToList();
        }

        public bool FileExists(string file)
        {
            return File.Exists(Path.Combine(SourceDirectory, file));
        }

        public FileStream OpenFile(string file)
        {
            return File.OpenRead(Path.Combine(SourceDirectory, file));
        }
    }
}
