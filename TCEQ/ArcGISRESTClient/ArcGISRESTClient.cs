using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;


namespace ArcGISRESTClient
{
    public class ArcGISRESTClient
    {
        protected string _serviceURL = null;
        protected string _reqMode = "json";

        public string _lastResponse = null;

        protected List<Layer> _layers = null;
        protected List<Table> _tables = null;

        protected Dictionary<int, Layer> _layersAsListed = null;
        protected Dictionary<int, Table> _tablesAsListed = null;

        protected Newtonsoft.Json.Linq.JContainer _fullExtent = null;

        public ArcGISRESTClient(string serviceURL)
        {
            _serviceURL = serviceURL;

            _InitializeFromRESTService();
        }

        protected void _InitializeFromRESTService()
        {
            string baseServiceResponse = DoWebRequestSynchronous("", null);

            Newtonsoft.Json.Linq.JContainer serviceMetadata = ParseJsonResponse(baseServiceResponse);

            try
            {
                _fullExtent = (Newtonsoft.Json.Linq.JContainer) serviceMetadata["fullExtent"];
            }
            catch
            {
                _fullExtent = null;
            }

            Newtonsoft.Json.Linq.JContainer tables = serviceMetadata["tables"] as Newtonsoft.Json.Linq.JContainer;
            Newtonsoft.Json.Linq.JContainer layers = serviceMetadata["layers"] as Newtonsoft.Json.Linq.JContainer;

            ConstructTables(tables);
            ConstructLayers(layers);

           
        }

        public Newtonsoft.Json.Linq.JContainer GetJsonData(string path, Newtonsoft.Json.Linq.JContainer parameters)
        {
            string responseJson = DoWebRequestSynchronous(path, parameters);

            Newtonsoft.Json.Linq.JContainer parsedData = ParseJsonResponse(responseJson);

            return parsedData;
        }


        /// <summary>
        /// Sends a request to the service synchronously, captures the response, and returns the raw data as a string.
        /// </summary>
        /// <returns></returns>
        protected string DoWebRequestSynchronous(string path, Newtonsoft.Json.Linq.JContainer parameters)
        {
            string retString = string.Empty;

            StringBuilder args = new StringBuilder();

            //Set the request format to whatever format the user wants. (usually json)
            args.AppendFormat("?f={0}", this._reqMode);

            if (parameters != null)
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    string key = "";
                    string value = "";

                    Newtonsoft.Json.Linq.JProperty jp = (Newtonsoft.Json.Linq.JProperty)parameters.ElementAt(i);

                    key = jp.Name;
                    value = jp.Value.ToString();

                    args.AppendFormat("&{0}={1}", key, value);
                }
            }

            string finalURL;

            if (path.StartsWith(_serviceURL))
            {
                finalURL = path + args;
            }
            else
            {
                finalURL = _serviceURL + path + args;
            }

            HttpWebRequest hwr = WebRequest.Create(finalURL) as HttpWebRequest;
            hwr.Method = "POST";
            hwr.ContentType = "application/x-www-form-urlencoded";

            //Write the request body to the request.
            byte[] bytearray = System.Text.Encoding.UTF8.GetBytes(args.ToString());
            hwr.ContentLength = bytearray.Length;

            Stream dataStream = hwr.GetRequestStream();
            dataStream.Write(bytearray, 0, bytearray.Length);

            
            using (HttpWebResponse response = hwr.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());

                string responseData = reader.ReadToEnd();

                retString = responseData;
                _lastResponse = retString;
            }

            return retString;
        }

        protected string DoWebRequestAsynchronous()
        {
            //TODO:  Implement the asynchronous version.
            return string.Empty;
        }

        protected Newtonsoft.Json.Linq.JContainer ParseJsonResponse(string json)
        {
            return JsonConvert.DeserializeObject(json) as Newtonsoft.Json.Linq.JContainer;
        }

        protected void ConstructTables(Newtonsoft.Json.Linq.JContainer tableMetadata)
        {
            _tables = new List<Table>();
            _tablesAsListed = new Dictionary<int, Table>();

            for (int i = 0; i < tableMetadata.Count; i++)
            {
                Table t = new Table((Newtonsoft.Json.Linq.JContainer)tableMetadata[i], this);

                _tables.Add(t);
            }

            for (int i = 0; i < _tables.Count; i++)
            {
                _tablesAsListed[_tables[i].ID] = _tables[i];
            }
        }

        protected void ConstructLayers(Newtonsoft.Json.Linq.JContainer layerMetadata)
        {
            _layers = new List<Layer>();
            _layersAsListed = new Dictionary<int, Layer>();
            for (int i = 0; i < layerMetadata.Count; i++)
            {
                Layer l = new Layer((Newtonsoft.Json.Linq.JContainer)layerMetadata[i], this);

                _layers.Add(l);
            }

            //Populate the "as-listed" dictionaries.  This allows picking a layer as client.Layers[id]
            for (int i = 0; i < _layers.Count; i++)
            {
                _layersAsListed[_layers[i].ID] = _layers[i];
            }
        }

        public string BaseURL
        {
            get
            {
                return _serviceURL;
            }
        }

        public Dictionary<int, Layer> LayersAsListed
        {
            get
            {
                return _layersAsListed;
            }
        }

        public Dictionary<int, Table> TablesAsListed
        {
            get
            {
                return _tablesAsListed;
            }
        }

        public Newtonsoft.Json.Linq.JContainer FullExtent
        {
            get
            {
                return _fullExtent;
            }
        }

        public Layer GetLayerByName(string layerName)
        {
            Layer ret = null;

            for (int i = 0; i < _layers.Count; i++)
            {
                if (_layers[i].Name == layerName)
                {
                    ret = _layers[i];
                }
            }

            return ret;
        }

        public Table GetTableByName(string tableName)
        {
            Table ret = null;

            for (int i = 0; i < _tables.Count; i++)
            {
                if (_tables[i].Name == tableName)
                {
                    ret = _tables[i];
                }
            }

            return ret;

        }
    }
}
