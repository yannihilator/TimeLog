using System;

namespace TimeLog.Models
{
    public class LogEntry
    {
        public int Id {get;set;}
        public DateTime StartTime {get;set;}
        public DateTime EndTime {get;set;}
        public string Description {get;set;}
        public string ChargeNumber {get;set;}
    }
}