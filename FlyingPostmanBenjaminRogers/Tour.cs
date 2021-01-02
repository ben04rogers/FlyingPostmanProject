////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// FileName: Tour.cs
// Author: Benjamin Rogers 
// Last modified: 24/05/2019
// Description: Holds all methods pertaining to the tour - calculating time and sorting
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingPostmanBenjaminRogers
{
    class Tour
    {
        // Calculate time between two stations
        public static double CalTime(double distBetween, Plane plane)
        {
            double result = ((distBetween / plane.Speed) * 60) + (plane.LandTime) + (plane.TakeOffTime);
            return Math.Round(result);
        }

        // Algorithm - Level 1
        public static List<Station> Level1Algorithm(List<Station> enteredStations)
        {
            List<Station> stationsOrdered = new List<Station>();

            // Add Post Office twice (start & finish) and then next station
            stationsOrdered.Add(enteredStations[0]);
            stationsOrdered.Add(enteredStations[1]);
            stationsOrdered.Add(enteredStations[0]);

            for (int i = 2; i < enteredStations.Count; i++)
            {
                // Set large initial distance to compare first value to
                double distance = 200000;
                int counter = 1;

                for (int j = 1; j < stationsOrdered.Count; j++)
                {
                    List<Station> route = stationsOrdered.ToList();
                    route.Insert(j, enteredStations[i]);
                    double checkTotalDistance = Station.totalRoute(route);

                    if (checkTotalDistance < distance)
                    {
                        distance = checkTotalDistance;
                        counter = j;
                    }
                }
                stationsOrdered.Insert(counter, enteredStations[i]);
            }
            return stationsOrdered;
        }

        public static List<Station> Level2Algorithm(List<Station> currentTour)
        {
            double shortestDistance = Station.totalRoute(currentTour);

            int counter = 0;

            List<Station> shortestTour = new List<Station>(currentTour);

            for (int i = 1; i < shortestTour.Count - 1; i++)
            {
                Station element = shortestTour[i];

                for (int j = 1; j < shortestTour.Count - 1; j++)
                {
                    double testDistance = 0;
                    List<Station> testTour = new List<Station>(shortestTour);
                    testTour.Remove(element);

                    testTour.Insert(j, element);
                    testDistance = Station.totalRoute(testTour);

                    if (testDistance < shortestDistance)
                    {
                        shortestDistance = testDistance;
                        shortestTour = testTour;
                        counter = counter + 1;
                        j = 1;
                        i = 1;
                    }
                }
            }
            return shortestTour;
        }
    }
}
