using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcGISRESTClient.Geometry
{
    public class Polyline : Geometry
    {
        protected List<Point> _points = null;

        public Polyline()
        {
            _points = new List<Point>();
        }

        public Polyline(Newtonsoft.Json.Linq.JArray jdata)
        {
            InitWithJArray(jdata);
        }

        public Polyline(List<Point> points)
        {
            InitWithPoints(points);
        }

        protected void InitWithJArray(Newtonsoft.Json.Linq.JArray jdata)
        {
            _points = new List<Point>();

            for(int i=0; i < jdata.Count; i++)
            {
                Newtonsoft.Json.Linq.JArray pointdata = (Newtonsoft.Json.Linq.JArray) jdata[i];

                Point p = new Point(pointdata);

                _points.Add(p);
            }
        }

        protected void InitWithPoints(List<Point> points)
        {
            _points = new List<Point>();

            for (int i = 0; i < points.Count; i++)
            {
                _points.Add(points[i]);
            }
        }

        public override Newtonsoft.Json.Linq.JToken GetJToken()
        {
            Newtonsoft.Json.Linq.JArray ret = new Newtonsoft.Json.Linq.JArray();

            for (int i = 0; i < _points.Count; i++ )
            {
                ret.Add(_points[i].GetJToken());
            }

            return ret;
        }

        public override Envelope GetBounds()
        {
            double maxx = 0, minx = 0, maxy = 0, miny = 0;

            for (int i = 0; i < _points.Count; i++)
            {
                Point p = _points[i];
                if (i == 0)
                {
                    maxx = p.X;
                    maxy = p.Y;
                    minx = p.X;
                    miny = p.Y;
                }
                else
                {
                    if (p.X < minx)
                    {
                        minx = p.X;
                    }

                    if (p.X > maxx)
                    {
                        maxx = p.X;
                    }

                    if (p.Y < miny)
                    {
                        miny = p.Y;
                    }

                    if (p.Y > maxy)
                    {
                        maxy = p.Y;
                    }
                }
            }

            Envelope ret = new Envelope(minx, maxx, miny, maxy);

            return ret;
        }

        public Newtonsoft.Json.Linq.JArray GetPathJToken()
        {
            Newtonsoft.Json.Linq.JArray jaPoints = new Newtonsoft.Json.Linq.JArray();

            for (int i = 0; i < _points.Count; i++)
            {
                jaPoints.Add(_points[i].GetPathJToken());
            }

            return jaPoints;
        }

        public override string GeometryTypeName
        {
            get { return "esriGeometryPolyline"; }
        }
    }
}
