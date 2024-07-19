using System;

namespace WinAlBackup.Models
{
    public class BackupFile
    {
        public BackupFile(string name, string fullName, long length, string extension, DateTime creationTime)
        {
            Name = name;
            FullName = fullName;
            Length = length;
            Extension = extension;
            CreationTime = creationTime;
        }

        public BackupFile(string name, string fullName, long length, string extension, DateTime creationTime, bool eventsDbExists, bool voicesDbExists) : this(name, fullName, length, extension, creationTime)
        {
            EventsDatabaseExists = eventsDbExists;
            VoicesDatabaseExists = voicesDbExists;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// 
        /// </summary>
        public long Length { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Extension { get; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreationTime { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool EventsDatabaseExists { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool VoicesDatabaseExists { get; }
    }
}
