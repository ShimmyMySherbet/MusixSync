using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusixSync.Networking.Interfaces;

namespace MusixSync.Mobile
{
    public class FileWriterAdapterBase : IFileWriter
    {
        public IAdapterFileWriter U;
        public FileWriterAdapterBase(IAdapterFileWriter u) => U = u;

        public bool FileExists(string File)
        {
            return U._FileExists(File);
        }

        public FileStream OpenWrite(string File)
        {
            return U._CreateFile(File);
        }
    }
}
