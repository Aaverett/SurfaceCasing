using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcGISRESTClient.Geometry
{
    public class Ring : Polyline
    {
        public Ring(Newtonsoft.Json.Linq.JArray jdata)
        {
            InitWithJArray(jdata);
        }
    }
}
