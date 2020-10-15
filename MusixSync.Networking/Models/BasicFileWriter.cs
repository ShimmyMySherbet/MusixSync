using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusixSync.Networking.Interfaces;

namespace MusixSync.Networking.Models
{
    public class BasicFileWriter : IFileWriter
    {
        public string OutputDirectory;

        public BasicFileWriter(string outfileDirectory) => OutputDirectory = outfileDirectory;

        public bool FileExists(string File)
        {
            return System.IO.File.Exists(Path.Combine(OutputDirectory, File));
        }

        public FileStream OpenWrite(string File)
        {
            return System.IO.File.OpenWrite(Path.Combine(OutputDirectory, File));
        }
    }
}
