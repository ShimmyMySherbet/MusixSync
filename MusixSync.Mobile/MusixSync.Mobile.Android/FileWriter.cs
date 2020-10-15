using System.IO;

namespace MusixSync.Mobile.Droid
{
    public class FileWriter : IAdapterFileWriter
    {
        public string Base;

        public FileWriter()
        {
            string Target = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "Music");

            if (Directory.Exists(Target))
            {
                Base = Target;
            }
            else
            {
                Base = Android.OS.Environment.DirectoryMusic;
            }
        }

        public FileStream _CreateFile(string file)
        {
            return File.OpenWrite(Path.Combine(Base, file));
        }

        public bool _FileExists(string file)
        {
            return File.Exists(Path.Combine(Base, file));
        }
    }
}