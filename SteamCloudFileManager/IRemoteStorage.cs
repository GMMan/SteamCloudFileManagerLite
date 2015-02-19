using System;
using System.Collections.Generic;

namespace SteamCloudFileManager
{
    interface IRemoteStorage
    {
        IRemoteFile GetFile(string name);
        List<IRemoteFile> GetFiles();
        bool GetQuota(out int totalBytes, out int availableBytes);
        bool IsCloudEnabledForAccount { get; }
        bool IsCloudEnabledForApp { get; set; }
    }
}
