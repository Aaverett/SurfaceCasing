using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcGISRESTClient
{
    public class Field
    {
        protected string _name;
        protected string _type;

        public Field(Newtonsoft.Json.Linq.JContainer fieldMetadata)
        {
            object oName = fieldMetadata["name"];

            if (oName is Newtonsoft.Json.Linq.JValue)
            {
                _name = (string) ((Newtonsoft.Json.Linq.JValue)oName).Value;
            }

            object oType = fieldMetadata["type"];

            if (oType is Newtonsoft.Json.Linq.JValue)
            {
                _type = (string)((Newtonsoft.Json.Linq.JValue)oType).Value;
            }
        }

        public string FieldName
        {
            get
            {
                return _name;
            }
        }

        public string DataTypeName
        {
            get
            {
                return _type;
            }
        }

        public Type DataType
        {
            get
            {
                Type t = typeof(string);

                switch (DataTypeName)
                {
                    case "esriFieldTypeInteger": t = typeof(int);
                        break;
                    case "esriFieldTypeDouble": t = typeof(Double);
                        break;
                    case "esriFieldTypeSmallInteger": t = typeof(int);
                        break;
                    case "esriFieldTypeString": t = typeof(string);
                        break;
                    case "esriFieldTypeGeometry": t = typeof(object);
                        break;
                    default: t = typeof(string);
                        break;
                }

                return t;
            }
        }
    }
}
