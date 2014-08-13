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
            //Fetch the real metadata from the service.
            Newtonsoft.Json.Linq.JContainer jc =  _parentService.GetJsonData(this.BaseURL, null);

            //Get the attributes unique to this table from the metadata.
            object oGeomType = jc["geometryType"];

            if (oGeomType is Newtonsoft.Json.Linq.JValue)
            {
                _geomType = (string)((Newtonsoft.Json.Linq.JValue)oGeomType).Value;
            }
        }

        public virtual System.Data.DataTable Query(string whereClause, JToken geometry)
        {
            return Query(whereClause, geometry, "esriGeometryPoint");
        }

        public System.Data.DataTable Query(string whereClause, JToken geometry, string geometryType)
        {
            JObject parameters = new JObject(
                new JProperty("where", whereClause));

            if (geometry != null)
            {
                parameters["geometry"] = geometry;
                parameters["geometryType"] = geometryType;
            }

            parameters["outFields"] = "*";
            parameters["outSR"] = 4326;

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
                new JProperty("layers", "all:" + this._id.ToString()),
                new JProperty("tolerance", 1),
                new JProperty("mapExtent", _parentService.FullExtent),
                new JProperty("imageDisplay", "1000,1000,96")

                );

            string parentServiceBaseURL = _parentService.BaseURL;


            JContainer jc = _parentService.GetJsonData(parentServiceBaseURL + "/identify", parameters);

            JContainer jc_results = (JContainer) jc["results"];

            if (jc_results.Count > 0)
            {

                JContainer jc_result0 = (JContainer)jc_results[0];

                JContainer jc_attributes = (JContainer)jc_result0["attributes"];

                JValue jv = (JValue)jc_attributes["Pixel Value"];

                object o = jv.Value;

                if (o is double)
                {
                    ret = (double)o;
                }
                else if (o is string)
                {
                    Double.TryParse((string)o, out ret);
                }
            }
            
            

            return ret;
        }

        public override void FillDataTableWithJContainer(System.Data.DataTable dt, JContainer jc)
        {
            JArray jaFeatures = (JArray)jc["features"];

            for (int i = 0; i < jaFeatures.Count; i++)
            {
                System.Data.DataRow dr = dt.NewRow();

                JObject feature = (JObject)jaFeatures[i];

                JContainer jcattrs = (JContainer)feature["attributes"];
                JObject jcgeomobj = (JObject)feature["geometry"];

                //Handle the geometry.
                Geometry.Geometry g = ConvertGeometry(jcgeomobj);

                dr["SHAPE"] = g;

                for (int j = 0; j < _fields.Count; j++)
                {
                    string fieldname = _fields[j].FieldName;

                    object val = jcattrs[fieldname];

                    if (fieldname.ToLower() == "SHAPE".ToLower())
                    {
                        continue;
                    }
                    
                    if (val == null)
                    {
                        dr[fieldname] = System.DBNull.Value;
                    }
                    else
                    {
                        if (val is JValue)
                        {
                            object oval = (object)((JValue)val).Value;

                            if (oval == null)
                            {
                                dr[fieldname] = System.DBNull.Value;
                            }
                            else
                            {
                                dr[fieldname] = oval;
                            }
                        }
                        else
                        {
                            dr[fieldname] = val;
                        }
                    }
                    
                }

                dt.Rows.Add(dr);
            }
        }

        public Geometry.Geometry ConvertGeometry(Newtonsoft.Json.Linq.JToken jdata)
        {
            Geometry.Geometry ret = null;

            try
            {
                switch (_geomType)
                {
                    case "esriGeometryPoint": ret = new Geometry.Point(jdata);
                        break;
                    case "esriGeometryPolyline": ret = new Geometry.Polyline((Newtonsoft.Json.Linq.JArray)jdata);
                        break;
                    case "esriGeometryPolygon": ret = new Geometry.Polygon((Newtonsoft.Json.Linq.JObject)jdata);
                        break;
                }
            }
            catch
            {

            }

            return ret;
        }
    }
}
