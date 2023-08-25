using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Collections.ObjectModel;

namespace RemoteAccessScanner
{
    public class SystemProcessProvider : ISystemProcessProvider
    {

        /// <summary>
        /// Returns a list of ProcessItems based on the current running prodcesses
        /// </summary>
        /// <returns>List of Process Items</returns>
        public Collection<ProcessItem> GetProcessList()
        {
            List<Process> processList = System.Diagnostics.Process.GetProcesses().ToList();
            Collection<ProcessItem> processItems = new Collection<ProcessItem>();
            foreach (Process process in processList)
            {
                processItems.Add(new ProcessItem() { Id = process.Id, SessionId = process.SessionId, ProcessName = process.ProcessName });
            }

            return processItems;
        }

        public ProcessItem GetCurrentProcess()
        {
            Process process = Process.GetCurrentProcess();
            return new ProcessItem() { Id = process.Id, SessionId = process.SessionId, ProcessName = process.ProcessName };
        }

        /// <summary>
        /// Retrieves a user account based on a process id
        /// </summary>
        /// <param name="processId"></param>
        /// <returns>A string representation of the user name and domian</returns>
        public string GetUserForProcess(int processId)
        {
            using (ManagementObject proc = new ManagementObject(string.Format("Win32_Process.Handle='{0}'", processId)))
            {
                proc.Get();
                string[] s = new String[2];
                //Invoke the method and populate the array with the user name and domain
                proc.InvokeMethod("GetOwner", (object[])s);
                return string.Format("{0}\\{1}", s[1], s[0]);
            }
        }

    }
}
