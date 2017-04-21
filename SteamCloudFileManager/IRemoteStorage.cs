using System;
using System.Collections.Generic;

namespace SteamCloudFileManager
{
    interface IRemoteStorage
    {
        IRemoteFile GetFile(string name);
        List<IRemoteFile> GetFiles();
        bool GetQuota(out ulong totalBytes, out ulong availableBytes);
        bool IsCloudEnabledForAccount { get; }
        bool IsCloudEnabledForApp { get; set; }
    }
}
