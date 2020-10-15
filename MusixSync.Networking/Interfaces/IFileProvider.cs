using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusixSync.Networking.Interfaces
{
    public interface IFileProvider
    {
        List<string> GetLocalFiles();

        bool FileExists(string file);

        FileStream OpenFile(string file);
    }
}
