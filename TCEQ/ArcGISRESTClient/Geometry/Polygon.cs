using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcGISRESTClient.Geometry
{
    public class Polygon : Geometry
    {

        protected List<Ring> _rings = null;

        public List<Ring> Rings
        {
            get
            {
                if (_rings == null) _rings = new List<Ring>();

                return _rings;
            }
        }

        public Polygon(Newtonsoft.Json.Linq.JObject jdata)
        {
            Newtonsoft.Json.Linq.JArray rings = (Newtonsoft.Json.Linq.JArray) jdata["rings"];

            if(rings is Newtonsoft.Json.Linq.JContainer)
            {
                for(int i=0; i < rings.Count; i++)
                {
                    Ring r = new Ring((Newtonsoft.Json.Linq.JArray) rings[i]);

                    Rings.Add(r);
                }
            }
        }

        public Polygon(Ring ring)
        {
            Rings.Add(ring);
        }

        public Polygon(List<Ring> rings)
        {
            for (int i = 0; i < rings.Count; i++)
            {
                Rings.Add(rings[i]);
            }
        }

        public override Envelope GetBounds()
        {
            double maxx = 0, minx = 0, maxy = 0, miny=0;

            for(int i=0; i < Rings.Count; i++)
            {
                Envelope e = Rings[i].GetBounds();
                if(i == 0)
                {
                    maxx = e.MaxX;
                    maxy = e.MaxY;
                    minx = e.MinX;
                    miny = e.MinY;
                }
                else
                {
                    if(e.MinX < minx)
                    {
                        minx = e.MinX;
                    }

                    if(e.MaxX > maxx)
                    {
                        maxx = e.MaxX;
                    }

                    if(e.MinY < miny)
                    {
                        miny = e.MinY;
                    }

                    if(e.MaxY > maxy)
                    {
                        maxy = e.MaxY;
                    }
                }
            }

            Envelope ret = new Envelope(minx, maxx, miny, maxy);

            return ret;
        }

        public override Newtonsoft.Json.Linq.JToken GetJToken()
        {
            Newtonsoft.Json.Linq.JObject jo = new Newtonsoft.Json.Linq.JObject();

            Newtonsoft.Json.Linq.JArray jaRings = new Newtonsoft.Json.Linq.JArray();

            for (int i = 0; i < Rings.Count; i++)
            {
                jaRings.Add(Rings[i].GetPathJToken());
            }

            jo.Add("rings", jaRings);
            jo.Add("hasZ", false);
            jo.Add("hasM", false);

            return jo;
        }

        public override string GeometryTypeName
        {
            get { return "esriGeometryPolygon"; }
        }
    }
}
