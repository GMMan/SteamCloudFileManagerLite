using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SteamCloudFileManager
{
    /// <summary>
    /// Dummy local implementation of RemoteStorage interface for offline testing
    /// </summary>
    class RemoteStorageLocal : IRemoteStorage
    {
        string basePath;
        uint appId;
        
        public RemoteStorageLocal(string basePath, uint appId)
        {
            this.basePath = basePath;
            this.appId = appId;
            Directory.CreateDirectory(Path.Combine(basePath, appId.ToString()));
        }

        public IRemoteFile GetFile(string name)
        {
            string lowerName = name.ToLowerInvariant();
            return new RemoteFileLocal(lowerName, new FileInfo(Path.Combine(basePath, appId.ToString(), lowerName)));
        }

        public List<IRemoteFile> GetFiles()
        {
            string searchPath = Path.Combine(basePath, appId.ToString());
            List<IRemoteFile> files = new List<IRemoteFile>();
            foreach (string file in Directory.GetFiles(searchPath, "*", SearchOption.AllDirectories))
            {
                string curPath = file.Replace(searchPath, "").TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToLowerInvariant();
                files.Add(new RemoteFileLocal(curPath, new FileInfo(file)));
            }
            return files;
        }

        public bool GetQuota(out int totalBytes, out int availableBytes)
        {
            DriveInfo di = new DriveInfo(Path.GetPathRoot(Path.GetFullPath(basePath)));
            totalBytes = di.TotalSize > int.MaxValue ? int.MaxValue : (int)di.TotalSize;
            availableBytes = di.AvailableFreeSpace > int.MaxValue ? int.MaxValue : (int)di.AvailableFreeSpace;
            return true;
        }

        public bool IsCloudEnabledForAccount
        {
            get { return true; }
        }

        public bool IsCloudEnabledForApp
        {
            get
            {
                return true;
            }
            set
            {
                throw new NotSupportedException();
            }
        }
    }
}
