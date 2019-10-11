// <copyright file="Program.cs" company="Hexhead404">
// Copyright (c) Hexhead404. All rights reserved.
// </copyright>

namespace ConversationLogger.LyncListener
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    /// <summary>
    /// Main program class.
    /// </summary>
    internal static class Program
    {
        private static readonly Listener Listener = new Listener();

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">Optional arguments.</param>
        internal static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
            if (!Debugger.IsAttached && Process.GetProcessesByName("LyncLog").Length > 1)
            {
                return;
            }

            Listener.StartListening();
            Task.Delay(-1).Wait();
            Listener.StopListening();
        }

        private static void CurrentDomainOnProcessExit(object sender, EventArgs e)
        {
            Listener.Dispose();
            AppDomain.CurrentDomain.ProcessExit -= CurrentDomainOnProcessExit;
        }
    }
}
