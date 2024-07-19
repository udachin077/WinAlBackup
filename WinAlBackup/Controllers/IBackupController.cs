using System.Collections.Generic;
using WinAlBackup.Models;

namespace WinAlBackup.Controllers
{
    public interface IBackupController
    {
        IEnumerable<BackupFile> GetBackupFiles();
        BackupFile CreateBackup(bool copyEvents, bool copyVoices);
        void RestoreBackup(BackupFile file, bool copyEvents, bool copyVoices);
        void RemoveBackup(BackupFile file);
    }
}
