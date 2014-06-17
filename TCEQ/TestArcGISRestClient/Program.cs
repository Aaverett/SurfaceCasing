using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestArcGISRestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string restServceURL = "http://coastal.beg.utexas.edu:6080/arcgis/rest/services/surfacecasing/surfacecasing_backend_rasters/MapServer";

            ArcGISRESTClient.ArcGISRESTClient arc = new ArcGISRESTClient.ArcGISRESTClient(restServceURL);

            //Newtonsoft.Json.Linq.JValue geom = new Newtonsoft.Json.Linq.JValue("-97.097168,29.773914");
            
            double d = arc.LayersAsListed[0].QueryRasterLayer(-97.097168, 29.773914);
        }
    }
}
