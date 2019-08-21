
namespace ConversationLogger.Common
{
    using System;
    using System.IO;

    public static class Constants
    {
        public static string LogFolder { get; } = EnsureLogFolderExists();

        private static string EnsureLogFolderExists()
        {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LyncLog");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return folder;
        }
    }
}
