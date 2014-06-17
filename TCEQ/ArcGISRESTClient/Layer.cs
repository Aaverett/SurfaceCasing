using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ArcGISRESTClient
{
    public class Layer : Table
    {
        protected string _geomType;

        public Layer(Newtonsoft.Json.Linq.JContainer layerMetadata, ArcGISRESTClient parentService)
        {
            InitializeWithTableMetadata(layerMetadata, parentService);

            InitializeWithLayerMetadata(layerMetadata);
        }

        protected void InitializeWithLayerMetadata(Newtonsoft.Json.Linq.JContainer layerMetadata)
        {
            //Get the attributes unique to this table from the metadata.
            object oGeomType = layerMetadata["geometryType"];

            if (oGeomType is Newtonsoft.Json.Linq.JValue)
            {
                _geomType = (string)((Newtonsoft.Json.Linq.JValue)oGeomType).Value;
            }
        }

        public virtual System.Data.DataTable Query(string whereClause, JValue geometry)
        {
            JObject parameters = new JObject(
                new JProperty("where", whereClause));

            if (geometry != null)
            {
                parameters["geometry"] = geometry;
            }
            

            JContainer jc = _parentService.GetJsonData(BaseURL + "/query", parameters);

            System.Data.DataTable dt = CreateEmptyDataTable();

            FillDataTableWithJContainer(dt, jc);

            return dt;
        }

        public virtual double QueryRasterLayer(double x, double y)
        {
            double ret = 0;

            JValue geometry = new JValue(x + "," + y);

            JObject parameters = new JObject(
                new JProperty("geometry", geometry),
                new JProperty("layers", this._id.ToString()),
                new JProperty("tolerance", 1),
                new JProperty("mapExtent", _parentService.FullExtent),
                new JProperty("imageDisplay", "1000,1000,96")

                );

            string parentServiceBaseURL = _parentService.BaseURL;


            JContainer jc = _parentService.GetJsonData(parentServiceBaseURL + "/identify", parameters);

            JContainer jc_results = (JContainer) jc["results"];

            JContainer jc_result0 = (JContainer) jc_results[0];

            JContainer jc_attributes = (JContainer)jc_result0["attributes"];

            JValue jv = (JValue) jc_attributes["Pixel Value"];

            object o = jv.Value;

            if (o is double)
            {
                ret = (double) o;
            }
            else if (o is string)
            {
                Double.TryParse((string) o, out ret);
            }

            return ret;
        }
    }
}
