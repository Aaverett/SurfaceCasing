using System;
using System.Collections.Generic;
using System.Text;

namespace BEGWebAppLib.AJAXGeometry
{
    /// <summary>
    /// Easily JSONable point feature
    /// </summary>
    public class PointGeometry
    {
        public double X;
        public double Y;
        public double Z;

        public PointGeometry(ESRI.ArcGIS.ADF.ArcGISServer.PointN p)
        {
            X = p.X;
            Y = p.Y;
            Z = p.Z;
        }

        public PointGeometry(ESRI.ArcGIS.ADF.Web.Geometry.Point p)
        {
            X = p.X;
            Y = p.Y;
            Z = p.Z;
        }

        public PointGeometry(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public PointGeometry()
        {

        }
    }
}
