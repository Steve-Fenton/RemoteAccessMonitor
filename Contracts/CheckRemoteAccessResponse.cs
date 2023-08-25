using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RemoteAccessScanner
{
    public class CheckRemoteAccessResponse
    {
        public Collection<RemoteAccessEvent> RemoteAccessEvents { get; set; }
        public Collection<ConnectedUser> ConnectedUsers { get; set; }
    }
}
