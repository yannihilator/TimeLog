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
        public static List<LogEntry> entries;
        private static string dataFilePath = $"{System.IO.Directory.GetCurrentDirectory()}/data.json";

        private static string SerializedEntries()
        {
            return JsonSerializer.Serialize(entries);
        }

        private static void SaveToDataFile()
        {
            File.WriteAllText(dataFilePath, SerializedEntries());
        }

        public static void PopulateEntries()
        {
            List<LogEntry> _entries = new List<LogEntry>();
            using (StreamReader r = new StreamReader(dataFilePath))
            {
                string json = r.ReadToEnd();
                if (json != null && json != string.Empty) entries = JsonSerializer.Deserialize<List<LogEntry>>(json);         
            }
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
            SaveToDataFile();
        }

        public static void DeleteEntry(int entryId)
        {
            var _entry = entries.Where(x => x.Id == entryId).FirstOrDefault();
            if (_entry != null) entries.Remove(_entry);
            SaveToDataFile();
        }

        public static void UpdateEntry(LogEntry entry)
        {
            var oldEntry = entries.Where(x => x.Id == entry.Id).FirstOrDefault();
            if (oldEntry != null)
            {
                entries.Remove(oldEntry);
                entries.Add(entry);
                entries = entries.OrderBy(x => x.StartTime).ToList();
                SaveToDataFile();
            }
        }

        public static List<IGrouping<string, LogEntry>> TodaysEntriesByChargeNumber()
        {
            return entries.Where(x => x.StartTime.Date == DateTime.Now.Date).GroupBy(x => x.ChargeNumber).ToList();
        }
    }
}