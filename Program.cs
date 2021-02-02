using System;
using System.Linq;
using TimeLog.Models;

namespace TimeLog
{
    class Program
    {
        static void Main(string[] args)
        {
            var entries = Controller.GetTodaysLogEntries();
            Console.WriteLine($"Entries for today, {DateTime.Now.ToShortDateString()}");
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
            Console.WriteLine("*******************************************");

            UserInterface();
        }

        static void UserInterface()
        {          
            Console.WriteLine("\nType in an entry's ID and press Enter to take an action, or press the enter key to start the timer.");
            var output = Console.ReadLine();
            if(output == string.Empty)
            {
                Console.WriteLine("timer starting");
            }
            else if (output.All(x => char.IsNumber(x)))
            {
                Console.WriteLine("what action would you like to take");
            }
            else
            {
                Console.WriteLine("Invalid input. Please try again.");
                UserInterface();
            }
        }

        static void StartTimer()
        {

        }

        static void StopTimer()
        {

        }
    }
}
