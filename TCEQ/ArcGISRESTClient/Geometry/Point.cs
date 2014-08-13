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

        public Point(Newtonsoft.Json.Linq.JToken jdata)
        {
            if (jdata is Newtonsoft.Json.Linq.JArray)
            {
                Newtonsoft.Json.Linq.JArray ja = (Newtonsoft.Json.Linq.JArray)jdata;
                if (ja.Count == 2)
                {
                    double x = System.Convert.ToDouble(ja[0]);
                    double y = System.Convert.ToDouble(ja[1]);

                    X = x;
                    Y = y;
                }
            }
            else if (jdata is Newtonsoft.Json.Linq.JObject)
            {
                Newtonsoft.Json.Linq.JObject jo = (Newtonsoft.Json.Linq.JObject)jdata;

                double x = System.Convert.ToDouble(jo["x"]);
                double y = System.Convert.ToDouble(jo["y"]);

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
            Newtonsoft.Json.Linq.JObject jo = new Newtonsoft.Json.Linq.JObject();

            jo["x"] = _x;
            jo["y"] = _y;

            return jo;
        }

        public Newtonsoft.Json.Linq.JToken GetPathJToken()
        {
            Newtonsoft.Json.Linq.JArray jaCords = new Newtonsoft.Json.Linq.JArray();

            jaCords.Add(_x);
            jaCords.Add(_y);

            return jaCords;
        }

        public override Envelope GetBounds()
        {
            return new Envelope(_x, _x, _y, _y);
        }

        public Polygon GetBufferedPolygon(double bufferDistance)
        {
            List<Point> points = new List<Point>();

            for (double operTheta = 0; operTheta < (Math.PI * 2); operTheta += (Math.PI * 2) / 8)
            {
                double x = _x + bufferDistance * Math.Cos(operTheta);
                double y = _y + bufferDistance * Math.Sin(operTheta);

                Point p = new Point(x, y);

                points.Add(p);
            }

            Ring r = new Ring(points);

            Polygon ret = new Polygon(r);

            return ret;
        }

        public override string GeometryTypeName
        {
            get { return "esriGeometryPoint"; }
        }
    }
}
