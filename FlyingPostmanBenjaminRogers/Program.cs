////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// FileName: Program.cs
// Author: Benjamin Rogers 
// Last modified: 24/05/2019
// Description: This is the main file
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace FlyingPostmanBenjaminRogers
{
    class Program
    {
        static void Main(string[] args)
        {
            string mailFile = null;
            string planeFile = null;
            string inputTime = null;

            //================== Exception that checks number of arguments =================
            try
            {
                mailFile = args[0];
                planeFile = args[1];
                inputTime = args[2];
            }
            catch (IndexOutOfRangeException) // Catch if less than 3 arguments are entered
            {
                Console.WriteLine("Incorrect number of arguments.\n\n" +
                "Please enter arguments in the format: \n" +
                "\"mail.txt plane.txt 20:00 -o output.txt\" (last two arguments are optional) ");
                return;
            }
            

            //================== Check for output flag -o =================
            bool checkForOutput = Array.Exists(args, element => element == "-o");

            //================== Exception that checks mail & plane files exist ================
            List<string> mailFileLines = null; // Holds each line of mail file
            List<string> planeFileLines = null; // Holds each line of plane file

            try
            {
                mailFileLines = FormatFile(mailFile);
                planeFileLines = FormatFile(planeFile);
            }
            catch (FileNotFoundException) 
            {
                Console.WriteLine("Mail file or Plane file does not exist.");
                return;
            }

            //================== Exception that checks format of plane & mail files =================
            string planeElements = null;
            string[] planeSpecsString = null; 
            double[] planeSpecsDouble = null;

            string[] stationElements = null; // Each individual element in text file (not grouped)
            Station[] stationsOrdered = null; // This will be used to reference individual station

            // Try parse plane file
            try
            {
                // Convert single string into string seperated by spaces
                planeElements = planeFileLines.Aggregate((i, j) => i + " " + j).ToString(); // Split single string into to strings seperated by space
                planeSpecsString = planeElements.Split();
                planeSpecsDouble = new double[planeSpecsString.Length]; // Convert to int for easy multiplication

                // Fill in array
                for (int n = 0; n < planeSpecsString.Length; n++)
                    planeSpecsDouble[n] = double.Parse(planeSpecsString[n]);

                // Try parse mail file
                stationElements = FormatSplitWords(mailFileLines);
                stationsOrdered = Station.FormatStationDetails(mailFileLines.Count, stationElements);
            }
            catch (Exception)
            {
                Console.WriteLine("Mail file or plane file is in incorrect format");
                Console.ReadLine();
                return;
            }

            //================== Run code block if -o is provided =================
            if (checkForOutput == true)
            {
                //================== Store plane details ==================
                Plane plane1 = new Plane(planeSpecsDouble[0],
                    planeSpecsDouble[1], planeSpecsDouble[2],
                    planeSpecsDouble[3], planeSpecsDouble[4]);

                //================== Algorithm - Level 1 ==================
                // Empty tour with post office initially added
                List<Station> originalOrder = stationsOrdered.ToList();

                Stopwatch timer = new Stopwatch(); // Track time it takes to sort using algorithm
                timer.Start();
                List<Station> algorithmOrder = new List<Station>(Tour.Level1Algorithm(originalOrder));

                //================== Algorithm - Level 2 ==================
                algorithmOrder = Tour.Level2Algorithm(algorithmOrder);

                algorithmOrder.Reverse(); 
                timer.Stop();

                // Format timer
                TimeSpan timerSpan = timer.Elapsed;
                string timerFormatted = timerSpan.ToString("s\\.fff");

                //================== Store distance totals between distance[i] & distance[i + 1] ==================
                double[] distToStation = new double[mailFileLines.Count];
            
                for (int i = 0; i < stationsOrdered.Length - 1; i++)
                {
                    distToStation[i] = Math.Round(Station.Distance((algorithmOrder[i].XValue),
                    (algorithmOrder[i + 1].XValue),
                    (algorithmOrder[i].YValue),
                    (algorithmOrder[i + 1].YValue)), 4);
                }
                int lastStation = algorithmOrder.Count - 2;

                // Store last distance total
                distToStation[lastStation] = Math.Round(Station.Distance((algorithmOrder[lastStation].XValue),
                (algorithmOrder[0].XValue),
                (algorithmOrder[lastStation].YValue),
                (algorithmOrder[0].YValue)), 4);

                //================== Setup list for storing times ==================
                DateTime initialTime = DateTime.Parse(inputTime); // Set intial time to user input
                string initialTimeFormatted = initialTime.ToString(@"HH\:mm"); // Format the intial time

                List<DateTime> timesBetweenStations = new List<DateTime>(); // List of times unformatted
                timesBetweenStations.Add(initialTime);
                timesBetweenStations.Add(initialTime.AddMinutes((Tour.CalTime(distToStation[0], plane1))));

                List<string> timesFormatted = new List<string>(); // List of formatted times in format "HH:mm"
                timesFormatted.Add(initialTimeFormatted);
                timesFormatted.Add(initialTime.AddMinutes((Tour.CalTime(distToStation[0], plane1))).ToString(@"HH\:mm"));

                // Set first index in list to compare times
                var previousTime = timesBetweenStations[0];

                for (int i = 1; i < distToStation.Length; i++)
                {
                    var nextTime = timesBetweenStations.Last().AddMinutes((Tour.CalTime(distToStation[i], plane1)));
                    var nextTotalTime = nextTime - previousTime; // Used to compare to range

                    // If the next time of trip is less than plane range, then just add the next time
                    if (nextTotalTime.TotalHours < plane1.Range)
                    {
                        var time = timesBetweenStations.Last().AddMinutes((Tour.CalTime(distToStation[i], plane1)));
                        timesBetweenStations.Add(time);
                        timesFormatted.Add(time.ToString(@"HH\:mm"));
                    }

                    // If the next time of trip exceeds plane range, add refuel to itinerary
                    else if (nextTotalTime.TotalHours >= plane1.Range)
                    {
                        var refuelTime = timesBetweenStations.Last().AddMinutes(plane1.RefuelTime);
                        timesBetweenStations.Add(refuelTime);
                        previousTime = timesBetweenStations.Last(); // Set previous time to current time

                        // Time after refuel to next station
                        var afterRefuel = timesBetweenStations.Last().AddMinutes((Tour.CalTime(distToStation[i], plane1)));
                        timesBetweenStations.Add(afterRefuel);

                        string refuelString = "*** refuel " + plane1.RefuelTime + " minutes ***";
                        timesFormatted.Add(refuelString);

                        timesFormatted.Add(refuelTime.ToString(@"HH\:mm"));
                        timesFormatted.Add(afterRefuel.ToString(@"HH\:mm"));
                    }
                }

                // Total duration of tour
                TimeSpan totalTime = timesBetweenStations[timesBetweenStations.Count - 1] - timesBetweenStations[0];

                //================== Output to Console Window & file ==================
                // Setup output file
                string outputFile = args[4];
                FileStream outFile = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                StreamWriter writer = new StreamWriter(outFile);

                // Print results to console
                Console.WriteLine("Reading input from {0}", mailFile);
                Console.WriteLine("Optimising tour length: Level 2...");
                Console.WriteLine("Elapsed time: {0} seconds", timerFormatted);

                // Write to output file
                writer.WriteLine("Optimising tour length: Level 2..."); 
                writer.WriteLine("Elapsed time: {0} seconds", timerFormatted);

                // Determine if tour time is less than 1 day
                if (totalTime.Days >= 1)
                {
                    Console.WriteLine("Tour time: {0} days {1} hours {2} minutes", 
                        totalTime.Days, totalTime.Hours, totalTime.Minutes);
                    writer.WriteLine("Tour time: {0} days {1} hours {2} minutes", 
                        totalTime.Days, totalTime.Hours, totalTime.Minutes);
                }
                else
                {
                    Console.WriteLine("Tour time: {0} hours {1} minutes", totalTime.Hours, totalTime.Minutes);
                    writer.WriteLine("Tour time: {0} hours {1} minutes", totalTime.Hours, totalTime.Minutes);
                }

                Console.WriteLine("Tour length: {0:0.0000}", Station.totalRoute(algorithmOrder));
                writer.WriteLine("Tour length: {0}", Station.totalRoute(algorithmOrder));

                // Print rest of results to console and output file
                int count = 0;
                for (int i = 0; i < timesFormatted.Count - 1; i++)
                {
                    // If the next next string DOES NOT contains refuel text
                    if (timesFormatted[i].Contains("refuel"))
                    {
                        Console.WriteLine("*** refuel {0} minutes ***", plane1.RefuelTime);
                        writer.WriteLine("*** refuel {0} minutes ***", plane1.RefuelTime);
                    }

                    // If the next next string contains refuel text
                    else if (!timesFormatted[i].Contains("refuel") && !timesFormatted[i + 1].Contains("refuel"))
                    {
                        Console.WriteLine("{0}\t->\t{1}\t{2}\t{3}", algorithmOrder[count].StationName,
                            algorithmOrder[count + 1].StationName,
                            timesFormatted[i], timesFormatted[i + 1]);

                        writer.WriteLine("{0}\t->\t{1}\t{2}\t{3}", algorithmOrder[count].StationName,
                            algorithmOrder[count + 1].StationName,
                            timesFormatted[i], timesFormatted[i + 1]);

                        count++;
                    }
                }
                // Close output file
                writer.Close();
                outFile.Close();
            }
            //================== Run code block if no output -o is provided ================
            else if (checkForOutput == false)
            {
                //================== Store plane details ==================
                Plane plane1 = new Plane(planeSpecsDouble[0],
                    planeSpecsDouble[1], planeSpecsDouble[2],
                    planeSpecsDouble[3], planeSpecsDouble[4]);

                //================== Algorithm - Level 1 ==================
                // Empty tour with post office initially added
                List<Station> originalOrder = stationsOrdered.ToList();

                Stopwatch timer = new Stopwatch(); // Track time it takes to sort using algorithm
                timer.Start();
                List<Station> algorithmOrder = new List<Station>(Tour.Level1Algorithm(originalOrder));

                //================== Algorithm - Level 2 ==================
                algorithmOrder = Tour.Level2Algorithm(algorithmOrder);

                algorithmOrder.Reverse();
                timer.Stop();

                // Format timer
                TimeSpan timerSpan = timer.Elapsed;
                string timerFormatted = timerSpan.ToString("s\\.fff");

                //================== Store distance totals between distance[i] & distance[i + 1] ==================
                double[] distToStation = new double[mailFileLines.Count];

                for (int i = 0; i < stationsOrdered.Length - 1; i++)
                {
                    distToStation[i] = Math.Round(Station.Distance((algorithmOrder[i].XValue),
                    (algorithmOrder[i + 1].XValue),
                    (algorithmOrder[i].YValue),
                    (algorithmOrder[i + 1].YValue)), 4);
                }
                int lastStation = algorithmOrder.Count - 2;

                // Store last distance total
                distToStation[lastStation] = Math.Round(Station.Distance((algorithmOrder[lastStation].XValue),
                (algorithmOrder[0].XValue),
                (algorithmOrder[lastStation].YValue),
                (algorithmOrder[0].YValue)), 4);

                //================== Setup list for storing times ==================
                DateTime initialTime = DateTime.Parse(inputTime); // Set intial time to user input
                string initialTimeFormatted = initialTime.ToString(@"HH\:mm"); // Format the intial time

                List<DateTime> timesBetweenStations = new List<DateTime>(); // List of times unformatted
                timesBetweenStations.Add(initialTime);
                timesBetweenStations.Add(initialTime.AddMinutes((Tour.CalTime(distToStation[0], plane1))));

                List<string> timesFormatted = new List<string>(); // List of formatted times in format "HH:mm"
                timesFormatted.Add(initialTimeFormatted);
                timesFormatted.Add(initialTime.AddMinutes((Tour.CalTime(distToStation[0], plane1))).ToString(@"HH\:mm"));

                // Set first index in list to compare times
                var previousTime = timesBetweenStations[0];

                for (int i = 1; i < distToStation.Length; i++)
                {
                    var nextTime = timesBetweenStations.Last().AddMinutes((Tour.CalTime(distToStation[i], plane1)));
                    var nextTotalTime = nextTime - previousTime; // Used to compare to range

                    // If the next time of trip is less than plane range, then just add the next time
                    if (nextTotalTime.TotalHours < plane1.Range)
                    {
                        var time = timesBetweenStations.Last().AddMinutes((Tour.CalTime(distToStation[i], plane1)));
                        timesBetweenStations.Add(time);
                        timesFormatted.Add(time.ToString(@"HH\:mm"));
                    }

                    // If the next time of trip exceeds plane range, add refuel to itinerary
                    else if (nextTotalTime.TotalHours >= plane1.Range)
                    {
                        var refuelTime = timesBetweenStations.Last().AddMinutes(plane1.RefuelTime);
                        timesBetweenStations.Add(refuelTime);
                        previousTime = timesBetweenStations.Last(); // Set previous time to current time

                        // Time after refuel to next station
                        var afterRefuel = timesBetweenStations.Last().AddMinutes((Tour.CalTime(distToStation[i], plane1)));
                        timesBetweenStations.Add(afterRefuel);

                        string refuelString = "*** refuel " + plane1.RefuelTime + " minutes ***";
                        timesFormatted.Add(refuelString);

                        timesFormatted.Add(refuelTime.ToString(@"HH\:mm"));
                        timesFormatted.Add(afterRefuel.ToString(@"HH\:mm"));
                    }
                }

                // Total duration of tour
                TimeSpan totalTime = timesBetweenStations[timesBetweenStations.Count - 1] - timesBetweenStations[0];

                //================== Output to Console ONLY ==================

                Console.WriteLine("Reading input from {0}", mailFile);
                Console.WriteLine("Optimising tour length: Level 2...");
                Console.WriteLine("Elapsed time: {0} seconds", timerFormatted);

                // Determine if tour time is less than 1 day
                if (totalTime.Days >= 1)
                {
                    Console.WriteLine("Tour time: {0} days {1} hours {2} minutes",
                        totalTime.Days, totalTime.Hours, totalTime.Minutes);
                }
                else
                {
                    Console.WriteLine("Tour time: {0} hours {1} minutes", totalTime.Hours, totalTime.Minutes);
                }

                Console.WriteLine("Tour length: {0:0.0000}", Station.totalRoute(algorithmOrder));

                // Print rest of results to console and output file
                int count = 0;
                for (int i = 0; i < timesFormatted.Count - 1; i++)
                {
                    // If the next next string DOES NOT contains refuel text
                    if (timesFormatted[i].Contains("refuel"))
                    {
                        Console.WriteLine("*** refuel {0} minutes ***", plane1.RefuelTime);
                    }

                    // If the next next string contains refuel text
                    else if (!timesFormatted[i].Contains("refuel") && !timesFormatted[i + 1].Contains("refuel"))
                    {
                        Console.WriteLine("{0}\t->\t{1}\t{2}\t{3}", algorithmOrder[count].StationName,
                            algorithmOrder[count + 1].StationName,
                            timesFormatted[i], timesFormatted[i + 1]);

                        count++;
                    }
                }

            }
        
        }

        // Read text and put into list
        static List<string> FormatFile(string inputFile)
        {
            List<string> lines = File.ReadAllLines(inputFile).ToList(); 
            return lines;
        }

        // Split text file into individual elements
        static string[] FormatSplitWords(List<string> inputFileLines)
        {
            string elements = inputFileLines.Aggregate((i, j) => i + " " + j).ToString(); 
            return elements.Split(); 
        }
    }
}
