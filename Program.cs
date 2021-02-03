using System;
using System.Linq;
using TimeLog.Models;
using System.Timers;

namespace TimeLog
{
    class Program
    {
        private static Timer timer = new Timer() {Interval = 1000};
        private static LogEntry currentEntry;

        static void Main(string[] args)
        {
            //subscribes timer to tick event
            timer.Elapsed += delegate{TimerTick();};

            //Populates entries in memory and calls user interface to be populated
            Controller.PopulateEntries();
            UserInterface();
        }

        private static void UserInterface()
        {   
            Console.Clear();
            //Populates Today's entries part of the UI       
            var entries = Controller.entries.Where(x => x.StartTime.Date == DateTime.Now.Date).ToList();
            Console.Write($"\nEntries for today, ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"{DateTime.Now.ToShortDateString()}\n");
            Console.ResetColor();
            Console.WriteLine("*******************************************\n");
            Console.ForegroundColor = ConsoleColor.Magenta;
            if (entries?.Count > 0)
            {
                int counter = 1;
                foreach (LogEntry entry in entries)
                {
                    Console.WriteLine($"{entry.Id} - {entry.StartTime.ToShortTimeString()} to {entry.EndTime.ToShortTimeString()}: {entry.ChargeNumber}");
                    counter++;
                }
            }
            else Console.WriteLine("No entries for today");
            Console.ResetColor();
            Console.WriteLine("\n*******************************************");

            GeneralActions();
        }

        private static void GeneralActions()
        {
            //action part of the UI begins here
            Console.WriteLine("\nAvailable Actions: \n");
            Console.WriteLine("-###       |   Entry ID to take an action for it.");
            Console.WriteLine("-[Enter]   |   Start the timer.");
            Console.WriteLine("-totals    |   Show daily totals.");
            Console.WriteLine("-exit      |   Exit Application.");
            var output = Console.ReadLine();

            if (output != string.Empty && output.All(x => char.IsNumber(x)))
            {
                ItemActions(Convert.ToInt32(output));
            }
            else if(output == string.Empty)
            {
                Console.WriteLine("Press the enter key to stop the timer.\n");
                StartTimer();
            }
            else if (output.ToLower() == "totals")
            {
                //outputs daily totals by charge numbers
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine();
                foreach (var group in Controller.TodaysEntriesByChargeNumber())
                {
                    Console.WriteLine($"{group.Key} - {group.Select(x => x.EndTime - x.StartTime).Aggregate(TimeSpan.Zero, (t1, t2) => t1 + t2).ToString("hh\\:mm\\:ss")}");
                }
                Console.ResetColor();
                GeneralActions();
            }
            else if (output.ToLower() == "exit")
            {
                Environment.Exit(0);
            }
            else
            {
                InvalidInput();
                GeneralActions();
            }
        }

        private static void ItemActions(int itemId)
        {
            Console.WriteLine($"\nAvailable Actions for entry {itemId}: \n");
            Console.WriteLine("-delete    |   Deletes Entry.");
            Console.WriteLine("-edit      |   Start the timer.");
            var output = Console.ReadLine();

            switch (output.ToLower())
            {
                case "delete":
                {
                    Controller.DeleteEntry(itemId);
                    UserInterface();
                    break;
                }
                case "edit":
                {                  
                    LogEntry oldEntry = Controller.entries.Where(x => x.Id == itemId).FirstOrDefault();
                    LogEntry updatedEntry = new LogEntry()
                    {
                        Id = oldEntry.Id,
                        StartTime = oldEntry.StartTime,
                        EndTime = oldEntry.EndTime,
                        ChargeNumber = oldEntry.ChargeNumber,
                        Description = oldEntry.Description
                    };

                    Console.WriteLine($"\nWhat would you like to edit?\n");
                    Console.WriteLine("-start       |   Start Time");
                    Console.WriteLine("-end         |   End Time");
                    Console.WriteLine("-charge      |   Charge Number");
                    Console.WriteLine("-description |   Item Description");
                    var attribute = Console.ReadLine();

                    switch (attribute.ToLower())
                    {
                        case "start":
                        {
                            Console.WriteLine("Type in a new start time in military time (hh:mm:ss) or press the enter key to keep the old value.");
                            var newStart = ConvertTime(Console.ReadLine(), oldEntry.StartTime.Date);
                            if (newStart != null) updatedEntry.StartTime = newStart.Value;
                            else updatedEntry.StartTime = oldEntry.StartTime;
                            break;
                        }
                        case "end":
                        {
                            Console.WriteLine("Type in a new end time in military time (hh:mm:ss) or press the enter key to keep the old value.");
                            var newEnd = ConvertTime(Console.ReadLine(), oldEntry.EndTime.Date);
                            if (newEnd != null) updatedEntry.EndTime = newEnd.Value;
                            else updatedEntry.EndTime = oldEntry.EndTime;
                            break;
                        }
                        case "charge":
                        {
                            updatedEntry.ChargeNumber = ChooseChargeNumber();
                            break;
                        }
                        case "description":
                        {
                            Console.WriteLine("Please enter a new description.");
                            updatedEntry.Description = Console.ReadLine();
                            break;
                        }
                    }
                    Controller.UpdateEntry(updatedEntry);
                    break;
                }
                default:
                {
                    InvalidInput();
                    ItemActions(itemId);
                    break;
                }              
            }
        }

        private static void EditItemDialogue(int itemId)
        {
            LogEntry updatedEntry = new LogEntry();
            LogEntry oldEntry = Controller.entries.Where(x => x.Id == itemId).FirstOrDefault();
            Console.WriteLine($"Type in a new start time in military time (hh:mm:ss) or press the enter key to keep the old value.");
            var newStart = Console.ReadLine();
            if (!string.IsNullOrEmpty(newStart))
            {
                var newStartTime = ConvertTime(newStart, oldEntry.StartTime);
                if (newStartTime != null) 
                {
                    updatedEntry.StartTime = newStartTime.Value;
                }
                else
                {
                    InvalidInput();
                    EditItemDialogue(itemId);
                }
            }
        }

        private static DateTime? ConvertTime(string time, DateTime date)
        {
            bool converts = TimeSpan.TryParse(time, out var newTime);
            bool withinLimits = newTime.Hours < 24 && newTime.Minutes < 60 && newTime.Seconds < 60;
            if (converts && withinLimits)
            {
                return new DateTime(date.Year,
                    date.Month,
                    date.Day,
                    newTime.Hours,
                    newTime.Minutes,
                    newTime.Seconds);               
            }
            else return null;
        }

        private static string ChooseChargeNumber()
        {
            var chargeNumbers = Controller.ChargeNumbers();
            int counter = 0;
            foreach (string chargeNumber in chargeNumbers) 
            {
                Console.WriteLine($"{counter} - {chargeNumber}");
                counter++;
            }

            var output = Console.ReadLine();
            if (output.All(x => char.IsNumber(x))) return chargeNumbers[Convert.ToInt32(output)];
            else if (output != string.Empty) return output;
            else return null;
        }

        private static void InvalidInput()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid input. Please try again.");
            Console.ResetColor();
        }

        private static void TimerTick()
        {
            Console.Write($"\r{(DateTime.Now - currentEntry.StartTime).ToString("hh\\:mm\\:ss")}");
        }

        private static void StartTimer()
        {
            if (currentEntry == null) currentEntry = new LogEntry();
            currentEntry.StartTime = DateTime.Now;
            Console.ForegroundColor = ConsoleColor.Green;
            timer.Start();

            var output = Console.ReadLine();
            if (output == string.Empty) StopTimer();
        }

        private static void StopTimer()
        {
            timer.Stop();
            currentEntry.EndTime = DateTime.Now;
            Console.ResetColor();
            Console.WriteLine($"\nWhat is the charge number?\n");
            
            currentEntry.ChargeNumber = ChooseChargeNumber();

            Controller.AddEntry(currentEntry);
            currentEntry = null;
            Console.Clear();
            UserInterface();
        }
    }
}
