using System;
using System.Collections.Generic;
using System.Text;

namespace BEGWebAppLib.AJAXGeometry
{
    /// <summary>
    /// Easily JSONable line geometry object
    /// </summary>
    public class LineGeometry
    {
        public System.Collections.Generic.List<BEGWebAppLib.AJAXGeometry.PointGeometry> points;

        protected void Initialise()
        {
            points = new System.Collections.Generic.List<BEGWebAppLib.AJAXGeometry.PointGeometry>();
        }

        public LineGeometry(ESRI.ArcGIS.ADF.ArcGISServer.PolylineN pn)
        {
            Initialise();

            for (int i = 0; i < pn.PathArray.Length; i++)
            {
                for (int j = 0; j < pn.PathArray[i].PointArray.Length; j++)
                {
                    try
                    {
                        ESRI.ArcGIS.ADF.ArcGISServer.PointN pointN = (ESRI.ArcGIS.ADF.ArcGISServer.PointN)pn.PathArray[i].PointArray[j];
                    
                        AddPoint(pointN.X, pointN.Y, pointN.Z);
                    }
                    catch
                    {

                    }
                }
            }

        }

        public LineGeometry(ESRI.ArcGIS.ADF.Web.Geometry.Polyline pn)
        {
            Initialise();

            for (int i = 0; i < pn.Paths.Count; i++)
            {
                for (int j = 0; j < pn.Paths[i].Points.Count; i++)
                {
                    AddPoint(pn.Paths[i].Points[j].X, pn.Paths[i].Points[j].Y, pn.Paths[i].Points[j].Z);
                }
            }
        }

        public LineGeometry()
        {
            Initialise();
        }

        public void AddPoint(double x, double y, double z)
        {
            BEGWebAppLib.AJAXGeometry.PointGeometry pg = new PointGeometry(x, y, z);

            points.Add(pg);
        }

        public void AddPoint(BEGWebAppLib.AJAXGeometry.PointGeometry pg)
        {
            points.Add(pg);
        }
    }
}
