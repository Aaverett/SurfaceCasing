using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;

namespace TCEQApp.LogicSets
{
    public class WilcoxAndCretaceousTop3K2012 : BaseLogicSet, ILogicSet
    {

        #region ILogicSet Members

        object ILogicSet.GetFactoryKey()
        {
            return "WilcoxAndCretaceousTop3K2012";
        }

        /// <summary>
        /// This retrieves the necessary information from the GDB.
        /// </summary>
        protected override void getData()
        {
            //Now we need to begin collecting the data necessary to generate our output div.
            _elevation = g.queryRaster(_coords, "sde.SDE.tceq_elev");
            _depth10k = g.queryRaster(_coords, "sde.SDE.tceq_10k");
            _depth3k = g.queryRaster(_coords, "sde.SDE.tceq_3k");
            _depth1kTop = g.queryRaster(_coords, "sde.SDE.tceq_tops");
            //_depth1kBottom = g.queryRaster(_coords, "sde.SDE.tceq_1k");
            _depth1kBottom = _depth3k;


            //There's one feature that we need to retrieve as well.
            DataTable aqs = g.performBaseFeatureDataQuery(string.Empty, (ESRI.ArcGIS.Geometry.IGeometry)_coords, ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelWithin, "sde.SDE.TCEQ_Aquifers");
            if (aqs != null && aqs.Rows.Count > 0)
            {
                _aquifername = (string)aqs.Rows[0]["Name"];
            }

            //To display the coordinates in lat/long, we need to reproject our feature to a GCS coord system.
            ESRI.ArcGIS.Geometry.IGeometry ig_projected = g.unprojectGeometry((ESRI.ArcGIS.Geometry.IGeometry)_coords, 102603, 4019);
            ESRI.ArcGIS.Geometry.IPoint ip_projected = (ESRI.ArcGIS.Geometry.IPoint)ig_projected;

            _latitude = ip_projected.Y;
            _longitude = ip_projected.X;

            //With the data values retrieved from the DB, we can calculate our output vals.
            //_topmsl = _depth1kTop;
            //_topgsfc = elevation - _depth1kTop;

            _topmsl = elevation;
            _topgsfc = 0;

            _botmsl = _depth1kBottom;
            _botgsfc = elevation - _depth1kBottom;

            //The "Protected base of usable quality water.
            _pbuqwmsl = _depth3k;
            _pbuqwgsfc = elevation - _depth3k;

            //The base of USDW
            _usdwmsl = _depth10k;
            _usdwgsfc = elevation - _depth10k;

            _doTopSentence = false;
            _botFWIZValueString = "See base of usable quality water";

        }

        #endregion
    }
}