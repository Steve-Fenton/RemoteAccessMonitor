using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace RemoteAccessScanner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ISystemProcessProvider supplier = new SystemProcessProvider();
            ProcessItem thisProcess = supplier.GetCurrentProcess();

            Collection<ProcessItem> processes = supplier.GetProcessList();

            bool alreadyRunning = false;

            foreach (ProcessItem item in processes)
            {
                if (item.ProcessName == thisProcess.ProcessName && item.Id != thisProcess.Id) {
                    alreadyRunning = true;
                    break;
                }
            }
            
            if (alreadyRunning)
            {
                MessageBox.Show("Remote Access Monitor is already running.");
                Application.Current.Shutdown();
            }

            base.OnStartup(e);
        }
    }
}
