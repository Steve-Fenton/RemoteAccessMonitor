using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace RemoteAccessScanner
{
    public class CheckingProcess : RemoteAccessScanner.ICheckingProcess
    {

        private ISystemProcessProvider _processSupplier;

        public CheckingProcess(ISystemProcessProvider processSupplier)
        {
            _processSupplier = processSupplier;
        }

        /// <summary>
        /// Checks the process list against memory and compiles a number of remote access events
        /// </summary>
        public CheckRemoteAccessResponse CheckRemoteAccess(Collection<ConnectedUser> connectedUsers)
        {
            CheckRemoteAccessResponse response = new CheckRemoteAccessResponse();
            response.ConnectedUsers = new Collection<ConnectedUser>();
            foreach (ConnectedUser user in connectedUsers)
            {
                response.ConnectedUsers.Add(user);
            }
            response.RemoteAccessEvents = new Collection<RemoteAccessEvent>();

            Collection<ProcessItem> processes;
            try
            {
                processes = _processSupplier.GetProcessList();
                List<ConnectedUser> activeUsers = new List<ConnectedUser>();

                foreach (ProcessItem process in processes)
                {
                    if (process.SessionId > 0)
                    {
                        ConnectedUser connectedUser = new ConnectedUser() 
                        {
                            UserName = _processSupplier.GetUserForProcess(process.Id)
                        };

                        // Add the users to "still active" list
                        if (activeUsers.Count(a => a.UserName.Equals(connectedUser.UserName)) == 0)
                        {
                            activeUsers.Add(connectedUser);
                        }

                        // If we don't currently hold and alert for this user, add one
                        if (response.ConnectedUsers.Count(c => c.UserName.Equals(connectedUser.UserName)) == 0) {
                            response.ConnectedUsers.Add(connectedUser);
                            RemoteAccessEvent remoteAccessEvent = new RemoteAccessEvent()
                            {
                                EventTime = DateTime.Now,
                                EventType = Enumerations.EventType.Detected,
                                UserName = connectedUser.UserName
                            };
                            response.RemoteAccessEvents.Add(remoteAccessEvent);
                        }
                    }
                }

                // Check to see if any know connections have disconnected.
                List<ConnectedUser> removeUsers = new List<ConnectedUser>();
                foreach (ConnectedUser user in response.ConnectedUsers)
                {
                    if (activeUsers.Count(a => a.UserName.Equals(user.UserName)) == 0)
                    {
                        removeUsers.Add(user);
                        RemoteAccessEvent remoteAccessEvent = new RemoteAccessEvent()
                        {
                            EventTime = DateTime.Now,
                            EventType = Enumerations.EventType.Ended,
                            UserName = user.UserName
                        };
                        response.RemoteAccessEvents.Add(remoteAccessEvent);
                    }
                }

                // Remove disconnected users from conntected user list
                foreach (ConnectedUser user in removeUsers)
                {
                    response.ConnectedUsers.Remove(user);
                }
            }
            finally
            {
                processes = null;
            }
            return response;

        }
    }
}
