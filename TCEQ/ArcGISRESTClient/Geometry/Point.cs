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

        public Point(Newtonsoft.Json.Linq.JArray jdata)
        {
            if(jdata.Count == 2)
            {
                double x = System.Convert.ToDouble(jdata[0]);
                double y = System.Convert.ToDouble(jdata[1]);

                X = x;
                Y = y;
            }
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

        public override Newtonsoft.Json.Linq.JToken GetJToken()
        {
            string value = X.ToString() + "," + Y.ToString();

            return new Newtonsoft.Json.Linq.JValue(value);
        }

        public override Envelope GetBounds()
        {
            return new Envelope(_x, _x, _y, _y);
        }
    }
}
