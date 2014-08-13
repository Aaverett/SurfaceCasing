using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcGISRESTClient.Geometry
{
    public class Envelope : Geometry
    {
        protected double _minX;
        protected double _maxX;
        protected double _minY;
        protected double _maxY;

        public Envelope(Newtonsoft.Json.Linq.JObject jdata)
        {

        }

        public Envelope(double minX, double maxX, double minY, double maxY)
        {
            _minX = minX;
            _maxX = maxX;
            _minY = minY;
            _maxY = maxY;
        }

        public double MaxX
        {
            get
            {
                return _maxX;
            }

            set
            {
                _maxX = value;
            }
        }

        public double MinX
        {
            get
            {
                return _minX;
            }

            set
            {
                _minX = value;
            }
        }

        public double MinY
        {
            get
            {
                return _minY;
            }

            set
            {
                _minY = value;
            }
        }

        public double MaxY
        {
            get
            {
                return _maxY;
            }

            set
            {
                _maxY = value;
            }
        }

        public override Envelope GetBounds()
        {
            return this;
        }

        public override Newtonsoft.Json.Linq.JToken GetJToken()
        {
            throw new NotImplementedException();
        }

        public override string  GeometryTypeName
        {
            get
            {
                return "esriGeometryEnvelope";
            }
        }
    }
}
