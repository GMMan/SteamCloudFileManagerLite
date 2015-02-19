using System;
using Steamworks;

namespace SteamCloudFileManager
{
    interface IRemoteFile
    {
        bool Delete();
        bool Exists { get; }
        bool Forget();
        bool IsPersisted { get; }
        string Name { get; }
        int Read(byte[] buffer, int count);
        byte[] ReadAllBytes();
        int Size { get; }
        ERemoteStoragePlatform SyncPlatforms { get; set; }
        DateTime Timestamp { get; }
        bool Write(byte[] buffer, int count);
        bool WriteAllBytes(byte[] buffer);
    }
}
