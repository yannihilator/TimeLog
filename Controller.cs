using System;
using System.Collections.Generic;
using System.Linq;
using TimeLog.Models;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TimeLog
{
    public static class Controller
    {
        private static List<LogEntry> entries;

        private static string SerializedEntries()
        {
            return JsonSerializer.Serialize(entries);
        }

        public static void PopulateEntries()
        {
            List<LogEntry> _entries = new List<LogEntry>();
            using (StreamReader r = new StreamReader("/Users/yanni/Documents/Repos/TimeLog/data.json"))
            {
                string json = r.ReadToEnd();
                if (json != null && json != string.Empty) entries = JsonSerializer.Deserialize<List<LogEntry>>(json);         
            }
        }

        public static List<LogEntry> GetAllLogEntries()
        {
            return entries;
        }
        
        public static List<LogEntry> GetTodaysLogEntries()
        {
            return entries?.Where(x => x.StartTime.Date == DateTime.Now.Date).ToList();
        }

        public static int GetNextId()
        {
            return entries.Count > 0 ? entries.Select(x => x.Id).Max() + 1 : 1;
        }
        public static List<string> ChargeNumbers()
        {
            return entries.Select(x => x.ChargeNumber).Distinct().ToList();
        }

        public static void AddEntry(LogEntry entry)
        {
            entry.Id = GetNextId();
            entries.Add(entry);
            File.WriteAllText("/Users/yanni/Documents/Repos/TimeLog/data.json", SerializedEntries());
        }
    }
}