using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusixSync.Mobile
{
    public interface IAdapterFileWriter
    {
        bool _FileExists(string file);
        FileStream _CreateFile(string file);
    }
}
