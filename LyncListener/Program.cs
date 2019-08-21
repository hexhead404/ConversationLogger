
namespace ConversationLogger.LyncListener
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    class Program
    {
        private static readonly Listener Listener = new Listener();

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit+= CurrentDomainOnProcessExit;
            if(Debugger.IsAttached || Process.GetProcessesByName("LyncLog").Length <= 1)
            {
                Listener.StartListening();
                Task.Delay(-1).Wait();
                Listener.StopListening();
            }
        }

        private static void CurrentDomainOnProcessExit(object sender, EventArgs e)
        {
            Listener.Dispose();
        }
    }
}
