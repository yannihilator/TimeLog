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

        static void UserInterface()
        {   
            Console.Clear();
            //Populates Today's entries part of the UI       
            var entries = Controller.GetTodaysLogEntries();
            Console.WriteLine($"\nEntries for today, {DateTime.Now.ToShortDateString()}");
            Console.WriteLine("*******************************************\n");
            if (entries?.Count > 0)
            {
                int counter = 1;
                foreach (LogEntry entry in entries)
                {
                    Console.WriteLine($"{entry.Id} - {entry.StartTime.ToShortTimeString()} to {entry.EndTime.ToShortTimeString()}: {entry.ChargeNumber}");
                    counter++;
                }
            }
            else Console.WriteLine("No entries for today\n");
            Console.WriteLine("\n*******************************************");

            //action part of the UI begins here
            Console.WriteLine("\nAvailable Actions: \n");
            Console.WriteLine("-###       |   Entry ID to take an action for it.");
            Console.WriteLine("-[Enter]   |   Start the timer.");
            Console.WriteLine("-total     |   Show daily totals.");
            Console.WriteLine("-stop      |   Exit Application.");
            var output = Console.ReadLine();

            if (output != string.Empty && output.All(x => char.IsNumber(x)))
            {
                Console.WriteLine("what action would you like to take");
            }
            else if(output == string.Empty)
            {
                Console.WriteLine("Press the enter key to stop the timer.\n");
                StartTimer();
            }
            else if (output.ToLower() == "total")
            {

            }
            else if (output.ToLower() == "stop")
            {
                
            }
            else
            {
                Console.WriteLine("Invalid input. Please try again.");
                UserInterface();
            }
        }

        static void TimerTick()
        {
            Console.Write($"\r{(DateTime.Now - currentEntry.StartTime).ToString("hh\\:mm\\:ss")}");
        }

        static void StartTimer()
        {
            if (currentEntry == null) currentEntry = new LogEntry();
            currentEntry.StartTime = DateTime.Now;
            Console.ForegroundColor = ConsoleColor.Green;
            timer.Start();

            var output = Console.ReadLine();
            if (output == string.Empty) StopTimer();
        }

        static void StopTimer()
        {
            timer.Stop();
            currentEntry.EndTime = DateTime.Now;
            Console.ResetColor();
            Console.WriteLine($"\nWhat is the charge number?\n");
            
            var chargeNumbers = Controller.ChargeNumbers();
            int counter = 0;
            foreach (string chargeNumber in chargeNumbers) 
            {
                Console.WriteLine($"{counter} - {chargeNumber}");
                counter++;
            }

            var output = Console.ReadLine();
            if (output.All(x => char.IsNumber(x)))
            {
                currentEntry.ChargeNumber = chargeNumbers[Convert.ToInt32(output)];
            }
            else if (output != string.Empty)
            {
                currentEntry.ChargeNumber = output;
            }
            else
            {
                currentEntry.ChargeNumber = "null";
            }

            Controller.AddEntry(currentEntry);
            currentEntry = null;
            Console.Clear();
            UserInterface();
        }
    }
}
