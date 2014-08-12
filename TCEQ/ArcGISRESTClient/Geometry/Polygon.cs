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

                    rings.Add(r);
                }
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
            throw new NotImplementedException();
        }
    }
}
