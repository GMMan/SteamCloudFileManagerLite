using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Steamworks;

namespace SteamCloudFileManager
{
    class RemoteFileLocal : IRemoteFile
    {
        string path;
        FileInfo fi;

        internal RemoteFileLocal(string path, FileInfo fi)
        {
            this.path = path;
            this.fi = fi;
        }

        public bool Delete()
        {
            try
            {
                fi.Delete();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Exists
        {
            get { return fi.Exists; }
        }

        public bool Forget()
        {
            throw new NotSupportedException();
        }

        public bool IsPersisted
        {
            get { return Exists; }
        }

        public string Name
        {
            get { return path; }
        }

        public int Read(byte[] buffer, int count)
        {
            if (count > buffer.Length) throw new ArgumentOutOfRangeException("count", "Number of bytes to read must be less than equal to buffer length.");
            using (FileStream fs = fi.OpenRead())
            {
                return fs.Read(buffer, 0, count);
            }
        }

        public byte[] ReadAllBytes()
        {
            byte[] buffer = new byte[Size];
            Read(buffer, buffer.Length);
            return buffer;
        }

        public int Size
        {
            get { return (int)fi.Length; }
        }

        public ERemoteStoragePlatform SyncPlatforms
        {
            get
            {
                return ERemoteStoragePlatform.k_ERemoteStoragePlatformWindows;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public DateTime Timestamp
        {
            get { return fi.LastWriteTime; }
        }

        public bool Write(byte[] buffer, int count)
        {
            try
            {
                using (FileStream fs = fi.Create())
                {
                    fs.Write(buffer, 0, count);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool WriteAllBytes(byte[] buffer)
        {
            return Write(buffer, buffer.Length);
        }
    }
}
