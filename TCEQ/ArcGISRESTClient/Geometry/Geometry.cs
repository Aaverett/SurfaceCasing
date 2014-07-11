using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcGISRESTClient.Geometry
{
    public abstract class Geometry
    {
        public abstract Newtonsoft.Json.Linq.JValue GetJValue();
    }
}
