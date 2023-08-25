using System;

namespace RemoteAccessScanner
{
    public class RemoteAccessEvent
    {
        public DateTime EventTime { get; set; }
        public string UserName { get; set; }
        public Enumerations.EventType EventType { get; set; }
    }
}
