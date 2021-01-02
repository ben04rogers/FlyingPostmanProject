////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// FileName: Plane.cs
// Author: Benjamin Rogers 
// Last modified: 25/05/2019
// Description: Contains properties for Plane objects. 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingPostmanBenjaminRogers
{
    class Plane
    {
        private double _range;
        private double _speed;
        private double _takeOffTime;
        private double _landTime;
        private double _refuelTime;

        // Initialise a new instance of plane
        public Plane(double range, double speed, double takeOffTime, double landTime, double refuelTime)
        {
            _range = range;
            _speed = speed;
            _takeOffTime = takeOffTime;
            _landTime = landTime;
            _refuelTime = refuelTime;

        }

        public double Range
        {
            get
            {
                return _range;
            }
            set
            {
                _range = value;
            }
        }

        public double Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                _speed = value;
            }
        }

        public double TakeOffTime
        {
            get
            {
                return _takeOffTime;
            }
            set
            {
                _takeOffTime = value;
            }
        }

        public double LandTime
        {
            get
            {
                return _landTime;
            }
            set
            {
                _landTime = value;
            }
        }

        public double RefuelTime
        {
            get
            {
                return _refuelTime;
            }
            set
            {
                _refuelTime = value;
            }
        }
    }
}
