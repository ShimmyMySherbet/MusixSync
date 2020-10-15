using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MusixSync.Networking.Interfaces;
using MusixSync.Networking.Models;

namespace MusixSync.Mobile
{
    public class MInterop
    {
        public static IFileWriter GetWriter()
        {
            foreach (Assembly A in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type T in A.GetTypes())
                {
                    if (typeof(IAdapterFileWriter).IsAssignableFrom(T) && !T.IsInterface && T != typeof(BasicFileWriter))
                    {//FileWriterAdapterBase
                        return new FileWriterAdapterBase((IAdapterFileWriter)Activator.CreateInstance(T));
                    }
                }
            }
            return null;
        }
    }
}
