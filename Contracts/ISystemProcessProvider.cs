using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace RemoteAccessScanner
{
    public interface ISystemProcessProvider
    {
        Collection<ProcessItem> GetProcessList();
        ProcessItem GetCurrentProcess();
        string GetUserForProcess(int processId);
    }
}
