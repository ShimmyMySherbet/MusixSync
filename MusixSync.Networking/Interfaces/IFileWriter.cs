using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusixSync.Networking.Interfaces
{
    public interface IFileWriter
    {
        bool FileExists(string File);

        FileStream OpenWrite(string File);
    }

}
