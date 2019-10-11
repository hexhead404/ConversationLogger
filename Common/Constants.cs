// <copyright file="Constants.cs" company="Hexhead404">
// Copyright (c) Hexhead404. All rights reserved.
// </copyright>

namespace ConversationLogger.Common
{
    using System;
    using System.IO;

    /// <summary>
    /// A class to hold common runtime constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Gets the log folder path
        /// </summary>
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
