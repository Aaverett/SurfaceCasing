using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ArcGISRESTClient
{
    public class Table
    {
        protected ArcGISRESTClient _parentService;
        protected System.Collections.Generic.List<Field> _fields;
        protected int _id;
        protected string _name;

        public Table()
        {
        }

        public Table(Newtonsoft.Json.Linq.JContainer tableMetadata, ArcGISRESTClient parentService)
        {
            InitializeWithTableMetadata(tableMetadata, parentService);
        }

        protected void InitializeWithTableMetadata(Newtonsoft.Json.Linq.JContainer tableMetadata, ArcGISRESTClient parentService)
        {
            _parentService = parentService;

            //Get the attributes unique to this table from the metadata.
            object oName = tableMetadata["name"];

            if (oName is Newtonsoft.Json.Linq.JValue)
            {
                _name = (string)((Newtonsoft.Json.Linq.JValue)oName).Value;
            }

            object oID = tableMetadata["id"];

            if (oID is Newtonsoft.Json.Linq.JValue)
            {
                Newtonsoft.Json.Linq.JValue jvID = (Newtonsoft.Json.Linq.JValue) oID;

                _id = System.Convert.ToInt32((long) jvID.Value);
            }

            //Now, handle the fields.
            _fields = new List<Field>();

            Newtonsoft.Json.Linq.JContainer data = parentService.GetJsonData(BaseURL, null);


            if (data["fields"] is Newtonsoft.Json.Linq.JValue)
            {

            }
            else
            {
                Newtonsoft.Json.Linq.JContainer fieldMetadata = (Newtonsoft.Json.Linq.JContainer)data["fields"];
                ConstructFields(fieldMetadata);
            }
        }

        protected void ConstructFields(Newtonsoft.Json.Linq.JContainer fieldsMetadata)
        {
            for (int i = 0; i < fieldsMetadata.Count; i++)
            {
                Field f = new Field((Newtonsoft.Json.Linq.JContainer)fieldsMetadata[i]);

                _fields.Add(f);
            }
        }

        public int ID
        {
            get
            {
                return _id;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string BaseURL
        {
            get
            {
                return "/" + ID.ToString();
            }
        }

        public virtual System.Data.DataTable Query (string whereClause)
        {
            
            JObject parameters = new JObject(
                new JProperty("where", whereClause));

            parameters["outFields"] = "*";

            JContainer jc = _parentService.GetJsonData(BaseURL + "/query", parameters);

            System.Data.DataTable dt = CreateEmptyDataTable();

            FillDataTableWithJContainer(dt, jc);

            return dt;
        }

        //Creates an empty data table with the appropriate set of fields.
        public System.Data.DataTable CreateEmptyDataTable()
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            for (int i = 0; i < _fields.Count; i++)
            {
                Type t = _fields[i].DataType;

                System.Data.DataColumn dc = new System.Data.DataColumn(_fields[i].FieldName, t);

                dt.Columns.Add(dc);

            }

            return dt;
        }

        public void FillDataTableWithJContainer(System.Data.DataTable dt, JContainer jc)
        {
            JArray jaFeatures = (JArray) jc["features"];

            for (int i = 0; i < jaFeatures.Count; i++)
            {
                System.Data.DataRow dr = dt.NewRow();

                JObject feature = (JObject)jaFeatures[i];

                for (int j = 0; j < _fields.Count; j++)
                {
                    string fieldname = _fields[j].FieldName;

                    JContainer jcattrs = (JContainer)feature["attributes"];

                    object val = jcattrs[fieldname];

                    if(val == null)
                    {
                        dr[fieldname] = System.DBNull.Value;
                    }
                    else
                    {
                        if (val is JValue)
                        {
                            object oval = (object) ((JValue) val).Value;

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
    }
}
