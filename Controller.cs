using System;
using System.Collections.Generic;
using System.Linq;
using TimeLog.Models;

namespace TimeLog
{
    public static class Controller
    {
        private static List<LogEntry> entries;
        public static List<LogEntry> GetAllLogEntries()
        {
            return entries;
        }
        public static List<LogEntry> GetTodaysLogEntries()
        {
            return entries?.Where(x => x.StartTime.Date == DateTime.Now.Date).ToList();
        }
    }
}