using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Steamworks;

namespace SteamCloudFileManager
{
    class RemoteStorage : IRemoteStorage, IDisposable
    {
        static RemoteStorage instance;
        static object sync = new object();

        internal bool IsDisposed { get; private set; }

        public bool IsCloudEnabledForAccount
        {
            get
            {
                //checkDisposed();
                // Not static because we need to ensure Steamworks API is initted
                return SteamRemoteStorage.IsCloudEnabledForAccount();
            }
        }
        public bool IsCloudEnabledForApp
        {
            get
            {
                checkDisposed();
                return SteamRemoteStorage.IsCloudEnabledForApp();
            }
            set
            {
                checkDisposed();
                SteamRemoteStorage.SetCloudEnabledForApp(value);
            }
        }

        internal RemoteStorage(uint appID)
        {
            Environment.SetEnvironmentVariable("SteamAppID", appID.ToString());
            bool init = SteamAPI.Init();
            if (!init)
            {
                // Setting environment variable didn't work, so use steam_appid.txt instead
                try
                {
                    File.WriteAllText("steam_appid.txt", appID.ToString());
                    init = SteamAPI.Init();
                    File.Delete("steam_appid.txt");
                }
                catch
                { }
            }

            if (!init) throw new Exception("Cannot initialize Steamworks API.");
        }

        public List<IRemoteFile> GetFiles()
        {
            checkDisposed();
            List<IRemoteFile> files = new List<IRemoteFile>();

            int fileCount = SteamRemoteStorage.GetFileCount();
            for (int i = 0; i < fileCount; ++i)
            {
                int length;
                string name = SteamRemoteStorage.GetFileNameAndSize(i, out length);
                RemoteFile file = new RemoteFile(this, name);
                files.Add(file);
            }

            return files;
        }

        public IRemoteFile GetFile(string name)
        {
            checkDisposed();
            return new RemoteFile(this, name.ToLowerInvariant());
        }

        public bool GetQuota(out int totalBytes, out int availableBytes)
        {
            checkDisposed();
            return SteamRemoteStorage.GetQuota(out totalBytes, out availableBytes);
        }

        void checkDisposed()
        {
            if (IsDisposed) throw new InvalidOperationException("Instance is no longer valid.");
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                SteamAPI.Shutdown();
                IsDisposed = true;
            }
        }

        public static RemoteStorage CreateInstance(uint appID)
        {
            lock (sync)
            {
                if (instance != null)
                {
                    instance.Dispose();
                    instance = null;
                }

                RemoteStorage rs = new RemoteStorage(appID);
                instance = rs;
                return rs;
            }
        }
    }
}
