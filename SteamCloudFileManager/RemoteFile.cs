using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Steamworks;

namespace SteamCloudFileManager
{
    class RemoteFile : IRemoteFile
    {
        static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        RemoteStorage parent;

        public string Name { get; private set; }
        public bool Exists
        {
            get
            {
                checkParentDisposed();
                return SteamRemoteStorage.FileExists(Name);
            }
        }
        public bool IsPersisted
        {
            get
            {
                checkParentDisposed();
                return SteamRemoteStorage.FilePersisted(Name);
            }
        }
        public int Size
        {
            get
            {
                checkParentDisposed();
                return SteamRemoteStorage.GetFileSize(Name);
            }
        }
        public DateTime Timestamp
        {
            get
            {
                checkParentDisposed();
                long timestamp = SteamRemoteStorage.GetFileTimestamp(Name);
                return UnixEpoch.AddSeconds(timestamp).ToLocalTime();
            }
        }
        public ERemoteStoragePlatform SyncPlatforms
        {
            get
            {
                checkParentDisposed();
                return SteamRemoteStorage.GetSyncPlatforms(Name);
            }
            set
            {
                checkParentDisposed();
                SteamRemoteStorage.SetSyncPlatforms(Name, value);
            }
        }

        internal RemoteFile(RemoteStorage parent, string name)
        {
            this.parent = parent;
            Name = name;
        }

        void checkParentDisposed()
        {
            if (parent.IsDisposed) throw new InvalidOperationException("Instance is no longer valid.");
        }

        public int Read(byte[] buffer, int count)
        {
            checkParentDisposed();
            return SteamRemoteStorage.FileRead(Name, buffer, count);
        }

        public byte[] ReadAllBytes()
        {
            byte[] buffer = new byte[Size];
            int read = Read(buffer, buffer.Length);
            if (read != buffer.Length) throw new IOException("Could not read entire file.");
            return buffer;
        }

        public bool Write(byte[] buffer, int count)
        {
            checkParentDisposed();
            return SteamRemoteStorage.FileWrite(Name, buffer, count);
        }

        public bool WriteAllBytes(byte[] buffer)
        {
            return Write(buffer, buffer.Length);
        }

        // Todo: implement async write

        public bool Forget()
        {
            checkParentDisposed();
            return SteamRemoteStorage.FileForget(Name);
        }

        public bool Delete()
        {
            checkParentDisposed();
            return SteamRemoteStorage.FileDelete(Name);
        }
    }
}
