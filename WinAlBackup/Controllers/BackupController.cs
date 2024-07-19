using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Reflection;
using WinAlBackup.Models;

namespace WinAlBackup.Controllers
{
    internal class BackupController : IBackupController
    {
        private readonly string RegionMapFileName = "region.map";
        private readonly string WinAlIniFileName = "WinAl.ini";
        private readonly string EventsDbFileName = "journal.db";
        private readonly string VoicesDbFileName = "sml32.db";
        private readonly string BaseFolder = "C:\\1Alarm";
        private readonly string RegionMapPath = "C:\\1Alarm\\region.map";
        private readonly string WinAlIniPath = "C:\\1Alarm\\WinAl.ini";
        private readonly string EventsDbPath = "C:\\1Alarm\\journal.db";
        private readonly string VoicesDbPath = "C:\\1Alarm\\sml32.db";
        private readonly string NastrFolder = "C:\\1Alarm\\Nastr";

        private readonly string BackupFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "winal-backups");

        public BackupController()
        {
            if (!Directory.Exists(BackupFolder))
                Directory.CreateDirectory(BackupFolder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="copyVoices"></param>
        /// <param name="copyEvents"></param>
        /// <returns></returns>
        public BackupFile CreateBackup(bool copyEvents, bool copyVoices)
        {
            string targetPath = Path.Combine(BackupFolder, GenerateBackupName());
            CopyBaseSettings(targetPath);
            CopyDatabase(targetPath, copyEvents, copyVoices);
            return ArchiveCreate(targetPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BackupFile> GetBackupFiles()
        {
            return new DirectoryInfo(BackupFolder).GetFiles().Select(x => BackupFileAdapter(x));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public void RemoveBackup(BackupFile file)
        {
            if (!File.Exists(file.FullName))
                throw new FileNotFoundException($"Файл не найден.\n{file.Name}");

            File.Delete(file.FullName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="copyVoices"></param>
        /// <param name="copyEvents"></param>
        public void RestoreBackup(BackupFile file, bool copyEvents, bool copyVoices)
        {
            using (ZipArchive archive = ZipFile.OpenRead(file.FullName))
            {
                ExtractToDirectory(archive, BaseFolder, copyEvents, copyVoices);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GenerateBackupName() => Guid.NewGuid().ToString();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private BackupFile BackupFileAdapter(FileInfo file)
        {
            using (ZipArchive archive = ZipFile.OpenRead(file.FullName))
                return new BackupFile(
                    file.Name.Replace(file.Extension, null),
                    file.FullName,
                    file.Length,
                    file.Extension,
                    file.CreationTime,
                    Exists(archive, EventsDbFileName),
                    Exists(archive, VoicesDbFileName)
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetPath"></param>
        private void CopyBaseSettings(string targetPath)
        {
            CopyFilesRecursively(NastrFolder, Path.Combine(targetPath, "Nastr"));
            File.Copy(RegionMapPath, Path.Combine(targetPath, RegionMapFileName), true);
            File.Copy(WinAlIniPath, Path.Combine(targetPath, WinAlIniFileName), true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="copyVoices"></param>
        /// <param name="copyEvents"></param>
        private void CopyDatabase(string targetPath, bool copyEvents, bool copyVoices)
        {
            string events_path = Path.Combine(BaseFolder, EventsDbFileName);
            string voices_path = Path.Combine(BaseFolder, VoicesDbFileName);

            if (copyEvents || copyVoices)
                Directory.CreateDirectory(targetPath);

            if (copyEvents && File.Exists(events_path))
                File.Copy(events_path, events_path.Replace(BaseFolder, targetPath), true);

            if (copyVoices && File.Exists(voices_path))
                File.Copy(voices_path, voices_path.Replace(BaseFolder, targetPath), true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        private void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            Directory.CreateDirectory(targetPath);

            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        private BackupFile ArchiveCreate(string sourcePath)
        {
            string targetPath = sourcePath + ".zip";
            ZipFile.CreateFromDirectory(sourcePath, targetPath);
            Directory.Delete(sourcePath, true);
            return BackupFileAdapter(new FileInfo(targetPath));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="archive"></param>
        /// <param name="destinationDirectoryName"></param>
        /// <param name="copyEvents"></param>
        /// <param name="copyVoices"></param>
        private void ExtractToDirectory(ZipArchive archive, string destinationDirectoryName, bool copyEvents, bool copyVoices)
        {
            foreach (ZipArchiveEntry file in archive.Entries)
            {
                string completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
                string directory = Path.GetDirectoryName(completeFileName);

                if (file.FullName.Contains(RegionMapFileName))
                {
                    file.ExtractToFile(RegionMapPath, true);
                    continue;
                }

                if (file.FullName.Contains(WinAlIniFileName))
                {
                    file.ExtractToFile(WinAlIniPath, true);
                    continue;
                }

                if (file.FullName.Contains("Nastr"))
                {
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    if (file.Name != "")
                        file.ExtractToFile(completeFileName, true);

                    continue;
                }

                if (copyEvents && file.FullName.Contains(EventsDbFileName))
                {
                    file.ExtractToFile(EventsDbPath, true);
                    continue;
                }

                if (copyVoices && file.FullName.Contains(VoicesDbFileName))
                {
                    file.ExtractToFile(VoicesDbPath, true);
                    continue;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="archive"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private bool Exists(ZipArchive archive, string target)
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (entry.FullName.Contains(target))
                    return true;
            }

            return false;
        }
    }
}