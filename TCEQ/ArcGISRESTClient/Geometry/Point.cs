using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcGISRESTClient.Geometry
{
    public class Point : Geometry
    {
        protected double _x;
        protected double _y;

        public Point(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public double X
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value;
            }
        }

        public double Y
        {
            get
            {
                return _y;
            }

            set
            {
                _y = value;
            }
        }

        public override Newtonsoft.Json.Linq.JValue GetJValue()
        {
            string value = X.ToString() + "," + Y.ToString();

            return new Newtonsoft.Json.Linq.JValue(value);
        }
    }
}
