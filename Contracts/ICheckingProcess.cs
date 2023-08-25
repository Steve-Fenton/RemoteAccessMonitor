using System.Collections.ObjectModel;
namespace RemoteAccessScanner
{
    public interface ICheckingProcess
    {
        RemoteAccessScanner.CheckRemoteAccessResponse CheckRemoteAccess(Collection<ConnectedUser> connectedUsers);
    }
}
