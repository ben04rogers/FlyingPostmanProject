////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// FileName: Station.cs
// Author: Benjamin Rogers 
// Last modified: 23/05/2019
// Description: Contains methods regarding Station objects and calculating distance between stations
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingPostmanBenjaminRogers
{
    class Station
    {
        private string _stationName;
        private int _xVal;
        private int _yVal;

        // Initialise a new instance of station
        public Station(string stationName, int xVal, int yVal)
        {
            _stationName = stationName;
            _xVal = xVal;
            _yVal = yVal;
        }

        public string StationName
        {
            get
            {
                return _stationName;
            }
            set
            {
                _stationName = value;
            }
        }

        public int XValue
        {
            get
            {
                return _xVal;
            }
            set
            {
                _xVal = value;
            }
        }

        public int YValue
        {
            get
            {
                return _yVal;
            }
            set
            {
                _yVal = value;
            }
        }

        // Fill Station array with strings from file 
        public static Station[] FormatStationDetails(int inputFileLinesLength, string[] splitWords)
        {
            Station[] stationDetails = new Station[inputFileLinesLength];
            int stationCounter = 0;
            while (stationCounter < inputFileLinesLength)
            {
                for (int i = 0; i < splitWords.Length; i += 3)
                {
                    stationDetails[stationCounter] = new Station(splitWords[i], Convert.ToInt32(splitWords[i + 1]), Convert.ToInt32(splitWords[i + 2]));
                    stationCounter++;
                }
            }
            return stationDetails;
        }

        // Calculate distance between two stations using the co-ordinates
        public static double Distance(int x1, int x2, int y1, int y2)
        {
            var temp1 = Math.Pow((x2 - x1), 2);
            var temp2 = Math.Pow((y2 - y1), 2);
            var result = Math.Sqrt(temp1 + temp2);

            return result;
        }

        // Calculate total distance of given tour
        public static double totalRoute(List<Station> route)
        {
            double[] distanceTotal = new double[route.Count];

            for (int i = 0; i < route.Count - 1; i++)
            {
                distanceTotal[i] = Math.Round(Station.Distance((route[i].XValue),
                (route[i + 1].XValue),
                (route[i].YValue),
                (route[i + 1].YValue)), 4);
            }
            int lastStation = distanceTotal.GetUpperBound(0);

            // Store last distance total
            distanceTotal[lastStation] = Math.Round(Station.Distance((route[lastStation].XValue),
            (route[0].XValue),
            (route[lastStation].YValue),
            (route[0].YValue)), 4);

            return distanceTotal.Sum();
        }
    }
}
