using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.ADF.Web.UI.WebControls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;

namespace BEGWebAppLib
{
    /// <summary>
    /// This class serves as an abstraction layer on the GIS.  There's a lot of code that's required for the purpose
    /// </summary>
    public class GISHandler
    {
        private ESRI.ArcGIS.ADF.Web.UI.WebControls.MapResourceManager _m_mrm;
        private ESRI.ArcGIS.ADF.Web.UI.WebControls.Map _mapctl;
        private ESRI.ArcGIS.ADF.Web.DataSources.IMapFunctionality _mf;
        private ESRI.ArcGIS.ADF.Web.DataSources.IGISResource _gr;
        private ESRI.ArcGIS.Server.IServerContext _server_context;
        //This is the workspace that the layers exist in.
        //private ESRI.ArcGIS.Geodatabase.IWorkspace ws;

        public ESRI.ArcGIS.ADF.Web.DataSources.IGISDataSource _gds;


        //These are the proxy objects for the objects on the GIS server.
        private ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.MapFunctionality _mf_local;
        private ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.MapResourceLocal _mr_local;

        private ESRI.ArcGIS.ADF.Web.DataSources.IQueryFunctionality _query_functionality;

        //Set the default map units.
        //private ESRI.ArcGIS.ADF.ArcGISServer.esriUnits _e_units = esriUnits.esriMeters;

        //Some primitives.
        private bool _init = false;   //This is used to keep track of whether or not this GISHandler is ready.
        private int _mrm_index;       //This keeps track of which resource item within the mapresourcemanager we want to use.

        public string SDE_Servername = "igor";
        public string SDE_Instance = "5151";
        public string SDE_Database = "sde";
        public string SDE_User = "sde";
        public string SDE_Password = "BT&74z";
        public string SDE_Version = "SDE.DEFAULT";

        int maxrecords = 15000;

        private ESRI.ArcGIS.ADF.Web.DataSources.Graphics.MapResource __mr_g;

        /// <summary>
        /// Constructor used when you have a map resource manager to pass to the GISHandler.
        /// </summary>
        /// <param name="mrm_1">Map resource manager you want the GISHandler to use.</param>
        /// <param name="resourceindex">The resource within that mrm you want it to use.</param>
        public GISHandler(ESRI.ArcGIS.ADF.Web.UI.WebControls.MapResourceManager mrm, int resourceindex)
        {
            //Call the init procedure.
            Initialize(mrm, resourceindex);
        }

        /// <summary>
        /// Constructor used when you don't have a map resource manager to pass to the GISHandler. 
        /// </summary>
        public GISHandler(string servicename, string servername, string framename)
        {
            Initialize_cold(servicename,servername,framename);
        }

        /// <summary>
        /// Initializes the GISHandler.  This was  moved out of the constructor so that other methods could access it without duplication.
        /// </summary>
        /// <param name="mrm_1"></param>
        /// <param name="resourceindex">This is the index of the resource within the mrm.</param>
        private void Initialize(MapResourceManager mrm, int resourceindex)
        {
            //If the whole object is null, or first resource is null...
            if (mrm == null || mrm.GetResource(resourceindex) == null)
            {
                ESRI.ArcGIS.ADF.Web.DataSources.IMapResource mr = mrm.GetResource(resourceindex);

                //End processing; there's not much to be done at this point.
                if (mr == null) return;
                
                
            }

            //
            _mrm_index = resourceindex;

            //Assign the resourcemanager we were passed to our local var.
            _m_mrm = mrm;

            foreach (MapResourceItem mri in _m_mrm.ResourceItems)
            {
                if (mri.Resource is ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.MapResourceLocal)
                {
                    _gr = (ESRI.ArcGIS.ADF.Web.DataSources.IGISResource) mri.Resource;
                    break;
                }
            }

            //Get the resource index, or flag false 
            //if ((_gr = _m_mrm.GetResource(0)) == null) return;
            
            //Set up the query functionality.
            _mf = (ESRI.ArcGIS.ADF.Web.DataSources.IMapFunctionality)_gr.CreateFunctionality(typeof(ESRI.ArcGIS.ADF.Web.DataSources.IMapFunctionality), null);
            if (_mf == null) return;

            //Set up the local map functionality.
            _mf_local = (ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.MapFunctionality)_mf;

            //Get the local map resource.
            _mr_local = (ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.MapResourceLocal)_gr;

            if (_mf_local == null || _mr_local == null) return;

            _gds = (ESRI.ArcGIS.ADF.Web.DataSources.IGISDataSource)_gr.DataSource;

            //Finally, we'll make sure all of our resources are initialized.
            if (mrm.Initialized == false) mrm.Initialize();

            ESRI.ArcGIS.ADF.Identity ident = new ESRI.ArcGIS.ADF.Identity();

            /*ident.UserName = "ArcGISWebServices";
            ident.Password = "aggies";
            ident.Domain = "igor";*/

            /*ident.UserName = "averetta";
            ident.Password = "scorp1onsarecool";
            ident.Domain = "BEGNTS";

            _msp.Identity = ident;*/

            //We got this far, and nothing has crashed, so we'll set init to true.
            _init = true;
        }

        /// <summary>
        /// This attempts the initialize_cold routine using the default values.
        /// </summary>
        private void Initialize_cold()
        {

            Initialize_cold("service", "localhost", "(default)");
        }

        /// <summary>
        /// This method is used to reinitialize the GISHandler when used on a page that doesn't have a map resource manager.
        /// </summary>
        private void Initialize_cold(string servicename, string servername, string framename)
        {
            //We obviously didn't have an MRM passed to us, so we'll need to create one.
            MapResourceManager mrm1 = new MapResourceManager();
            mrm1.ID = "programmaticallycreatedMRM";

            MapResourceItem mri = new MapResourceItem();
            GISResourceItemDefinition def = new GISResourceItemDefinition();

            //Give our map resource item a name.
            mri.Name = "crc";

            //This is actually the name of the server on which the GIS server is running.
            def.DataSourceDefinition = servername;

            //This is the type of server this is.
            def.DataSourceType = "ArcGIS Server Local";

            //This the name of the map service and data frame this resource is supposed to use.
            def.ResourceDefinition = framename + "@" + servicename;

            def.DataSourceShared = true;
            mri.Parent = mrm1;
            mri.Definition = def;
            ESRI.ArcGIS.ADF.Web.DisplaySettings ds = new ESRI.ArcGIS.ADF.Web.DisplaySettings();
            ds.DisplayInTableOfContents = true;
            ds.Visible = true;
            mri.DisplaySettings = ds;
            mrm1.ResourceItems.Insert(0, mri);
            mrm1.CreateResource(mri);

            Initialize(mrm1, 0);
        }

        private ESRI.ArcGIS.ADF.Web.DataSources.Graphics.MapResource _mr_g
        {
            get
            {
                if (__mr_g == null)
                {
                    ESRI.ArcGIS.ADF.Web.DataSources.Graphics.MapResource gResource = null;
                    System.Collections.IEnumerable gfc = _mapctl.GetFunctionalities();
                    foreach (ESRI.ArcGIS.ADF.Web.DataSources.IGISFunctionality gfunc in gfc)
                    {
                        if (gfunc.Resource.Name == "AGResource")
                        {
                            gResource = (ESRI.ArcGIS.ADF.Web.DataSources.Graphics.MapResource)gfunc.Resource;
                            break;
                        }
                        else
                        {
                            MapResourceItem mapResourceItem = new MapResourceItem();

                            GISResourceItemDefinition definition = new GISResourceItemDefinition();

                            mapResourceItem.Name = "AGResource";
                            definition.ResourceDefinition = "GraphicsResource";
                            definition.DataSourceDefinition = "In Memory";
                            definition.DataSourceType = "GraphicsLayer";

                            definition.DataSourceShared = true;
                            mapResourceItem.Parent = _m_mrm;
                            mapResourceItem.Definition = definition;
                            ESRI.ArcGIS.ADF.Web.DisplaySettings displaysettings = new ESRI.ArcGIS.ADF.Web.DisplaySettings();
                            displaysettings.Transparency = 0.0F;
                            displaysettings.Visible = true;
                            mapResourceItem.DisplaySettings = displaysettings;

                            _m_mrm.ResourceItems.Insert(0, mapResourceItem);
                            gResource =
                                (ESRI.ArcGIS.ADF.Web.DataSources.Graphics.MapResource)
                                _m_mrm.CreateResource(mapResourceItem);
                            break;
                        }
                    }

                    __mr_g = gResource;
                    __mr_g.Initialize();
                }

                return __mr_g;
            }

            set
            {
                __mr_g = value;
            }
        }

        /// <summary>
        /// Gets or sets the map control on which to operate.
        /// </summary>
        public ESRI.ArcGIS.ADF.Web.UI.WebControls.Map mapctl
        {
            set
            {
                if (value.MapResourceManagerInstance == _m_mrm)
                {
                    _mapctl = value;
                }
            }

            get
            {
                return _mapctl;
            }
        }

        /// <summary>
        /// This method converts the coordinates from a point on a map (m) to that map's coordinate system.
        /// </summary>
        /// <param name="sp">The screenpoint coord object representing a spot on the map.</param>
        /// <param name="m">The map that was clicked on.</param>
        /// <returns>The coords in the map's coord system for the point represented by </returns>
        public ESRI.ArcGIS.ADF.Web.Geometry.Point translateScreenCoords(System.Drawing.Point sp, ESRI.ArcGIS.ADF.Web.UI.WebControls.Map m)
        {
            //Get the extent of the map.
            ESRI.ArcGIS.ADF.Web.Geometry.Envelope visibleExtent = m.Extent;

            ESRI.ArcGIS.ADF.Web.Geometry.Point mypoint = ESRI.ArcGIS.ADF.Web.Geometry.Point.ToMapPoint(sp, visibleExtent, _mf.DisplaySettings.ImageDescriptor.Width, _mf.DisplaySettings.ImageDescriptor.Height);

            return mypoint;
        }

        /// <summary>
        /// Converts the hardcore COM object kind of geometry into the friendlier adf.web kind.
        /// </summary>
        /// <param name="g">Input geometry com object</param>
        /// <returns></returns>
        public ESRI.ArcGIS.ADF.Web.Geometry.Geometry convertGeometryTypeWeb(ESRI.ArcGIS.Geometry.IGeometry g)
        {
            if (g is ESRI.ArcGIS.Geometry.IPoint)
            {
                ESRI.ArcGIS.ADF.ArcGISServer.PointN pn = (ESRI.ArcGIS.ADF.ArcGISServer.PointN)ESRI.ArcGIS.ADF.ArcGISServer.Converter.ComObjectToValueObject(g, _sc, typeof(ESRI.ArcGIS.ADF.ArcGISServer.PointN));

                ESRI.ArcGIS.ADF.Web.Geometry.Point p = ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ToAdfPoint(pn);

                return (ESRI.ArcGIS.ADF.Web.Geometry.Geometry)p;
            }
            else if (g is ESRI.ArcGIS.Geometry.IPolyline)
            {
                ESRI.ArcGIS.ADF.ArcGISServer.PolylineN pn = (ESRI.ArcGIS.ADF.ArcGISServer.PolylineN)ESRI.ArcGIS.ADF.ArcGISServer.Converter.ComObjectToValueObject(g, _sc, typeof(ESRI.ArcGIS.ADF.ArcGISServer.PolylineN));

                ESRI.ArcGIS.ADF.Web.Geometry.Polyline p = ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ToAdfPolyline(pn);

                return (ESRI.ArcGIS.ADF.Web.Geometry.Geometry)p;
            }
            else if (g is ESRI.ArcGIS.Geometry.IPolygon)
            {
                ESRI.ArcGIS.ADF.ArcGISServer.PolygonN pn = (ESRI.ArcGIS.ADF.ArcGISServer.PolygonN)ESRI.ArcGIS.ADF.ArcGISServer.Converter.ComObjectToValueObject(g, _sc, typeof(ESRI.ArcGIS.ADF.ArcGISServer.PolygonN));

                ESRI.ArcGIS.ADF.Web.Geometry.Geometry p = ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ToAdfPolygon(pn);


                return (ESRI.ArcGIS.ADF.Web.Geometry.Geometry)p;
            }

            return null;
        }

        /// <summary>
        /// This attempts to convert the IGeometry passed to it to the corresponding type from the ArcGISServer namespace.
        /// </summary>
        /// <param name="ig">The source geometry</param>
        public ESRI.ArcGIS.ADF.ArcGISServer.Geometry translateAOToADF(ESRI.ArcGIS.Geometry.IGeometry ig)
        {
            ESRI.ArcGIS.ADF.ArcGISServer.Geometry g = (ESRI.ArcGIS.ADF.ArcGISServer.Geometry) ESRI.ArcGIS.ADF.ArcGISServer.Converter.ComObjectToValueObject(ig, _sc, typeof(ESRI.ArcGIS.Geometry.IPolygon));

            return g;
        }

        public ESRI.ArcGIS.ADF.Web.Geometry.Geometry translateAOToWeb(ESRI.ArcGIS.Geometry.IGeometry ig)
        {
            ESRI.ArcGIS.ADF.ArcGISServer.Geometry _g = translateAOToADF(ig);

            ESRI.ArcGIS.ADF.Web.Geometry.Geometry gg = ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ToAdfGeometry(_g);

            return gg;
        }

        public ESRI.ArcGIS.Geometry.IGeometry translateADFToAO(ESRI.ArcGIS.ADF.ArcGISServer.Geometry g)
        {
            ESRI.ArcGIS.Geometry.IGeometry ig = (ESRI.ArcGIS.Geometry.IGeometry)ESRI.ArcGIS.ADF.ArcGISServer.Converter.ValueObjectToComObject(g, _sc);

            return ig;
        }

        public ESRI.ArcGIS.ADF.ArcGISServer.Geometry translateWebToADF(ESRI.ArcGIS.ADF.Web.Geometry.Geometry g)
        {
            ESRI.ArcGIS.ADF.ArcGISServer.Geometry gg;

            gg = ESRI.ArcGIS.ADF.ArcGISServer.Converter.FromAdfGeometry(g);

            return gg;
        }
        
        /// <summary>
        /// This method converts the coordinates from a point on the map (m) to that map's coordinate system.
        /// </summary>
        /// <param name="sp">The coordinates of the point in question in screen coords (pixels)</param>
        /// <param name="m">The map control.</param>
        /// <returns>ArcObjects point object (remember that it's a COM proxy)</returns>
        public ESRI.ArcGIS.Geometry.IPoint translateScreenCoordsAO(System.Drawing.Point sp, ESRI.ArcGIS.ADF.Web.UI.WebControls.Map m)
        {
            //Get an ADF point
            ESRI.ArcGIS.ADF.Web.Geometry.Point p = translateScreenCoords(sp, m);

            //Now we need to conver that into an arobjects COM proxy.

            //The first step in that procedure is to convert to a value object.
            ESRI.ArcGIS.ADF.ArcGISServer.PointN pn = ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.FromAdfPoint(p);

            ESRI.ArcGIS.Geometry.IPoint ip = (ESRI.ArcGIS.Geometry.IPoint) ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ValueObjectToComObject(pn, _sc);

            return ip;
        }

        /// <summary>
        /// Generates a square polygon analogous to a buffer around the point provided in the context of the map.  Killer app for this is to make a mouse click cover a small area, instead of just a point.
        /// </summary>
        /// <param name="sp">The point in screen coords within the map image.</param>
        /// <param name="m">The map control that provides the context</param>
        /// <param name="bufferPx">The number of pixels to measure to each side of the square</param>
        /// <param name="inputCoordSysID">The ID of the coordinate system of the map.</param>
        /// <param name="outputCoordSysID">The ID of the coordinate system you want the polygon in when it's returned.</param>
        /// <returns></returns>
        public ESRI.ArcGIS.ADF.Web.Geometry.Polygon pointFudge(System.Drawing.Point sp, ESRI.ArcGIS.ADF.Web.UI.WebControls.Map m, int bufferPx, int inputCoordSysID, int outputCoordSysID)
        {
            ESRI.ArcGIS.Geometry.IPolygon p = (ESRI.ArcGIS.Geometry.IPolygon)pointFudgeAO(sp, m, bufferPx, inputCoordSysID, outputCoordSysID);

            ESRI.ArcGIS.Geometry.IGeometry bg = (ESRI.ArcGIS.Geometry.IGeometry)p;

            ESRI.ArcGIS.ADF.ArcGISServer.PolygonN pgn = (ESRI.ArcGIS.ADF.ArcGISServer.PolygonN) ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ComObjectToValueObject(bg,_sc,typeof(ESRI.ArcGIS.ADF.ArcGISServer.PolygonN));

            ESRI.ArcGIS.ADF.Web.Geometry.Polygon pn = (ESRI.ArcGIS.ADF.Web.Geometry.Polygon) ESRI.ArcGIS.ADF.ArcGISServer.Converter.ToAdfGeometry(pgn);

            //Return our polygon.
            return pn;
        }

        /// <summary>
        /// This 
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="m"></param>
        /// <param name="bufferPx"></param>
        /// <param name="inputCoordSysID"></param>
        /// <param name="outputCoordSysID"></param>
        /// <returns></returns>
        public ESRI.ArcGIS.Geometry.IPolygon pointFudgeAO(System.Drawing.Point sp, ESRI.ArcGIS.ADF.Web.UI.WebControls.Map m, int bufferPx, int inputCoordSys, int outputCoordSys)
        {
            System.Drawing.Rectangle r = new System.Drawing.Rectangle(sp.X - bufferPx, sp.Y - bufferPx, bufferPx * 2, bufferPx * 2);

            ESRI.ArcGIS.ADF.Web.Geometry.PointCollection pc = new ESRI.ArcGIS.ADF.Web.Geometry.PointCollection();

            int mapwidth = _mf.DisplaySettings.ImageDescriptor.Width;
            int mapheight = _mf.DisplaySettings.ImageDescriptor.Height;

            ESRI.ArcGIS.ADF.Web.Geometry.Envelope visibleExtent = m.Extent;

            //
            pc.Add(ESRI.ArcGIS.ADF.Web.Geometry.Point.ToMapPoint(r.Left, r.Bottom, visibleExtent, mapwidth, mapheight));
            pc.Add(ESRI.ArcGIS.ADF.Web.Geometry.Point.ToMapPoint(r.Right, r.Bottom, visibleExtent, mapwidth, mapheight));
            pc.Add(ESRI.ArcGIS.ADF.Web.Geometry.Point.ToMapPoint(r.Right, r.Top, visibleExtent, mapwidth, mapheight));
            pc.Add(ESRI.ArcGIS.ADF.Web.Geometry.Point.ToMapPoint(r.Left, r.Top, visibleExtent, mapwidth, mapheight));

            ESRI.ArcGIS.ADF.Web.Geometry.Ring ring = new ESRI.ArcGIS.ADF.Web.Geometry.Ring();
            ring.Points = pc;
            //Make a ringcollection out of the ring.
            ESRI.ArcGIS.ADF.Web.Geometry.RingCollection rings = new ESRI.ArcGIS.ADF.Web.Geometry.RingCollection();
            rings.Add(ring);
            ESRI.ArcGIS.ADF.Web.Geometry.Polygon poly = new ESRI.ArcGIS.ADF.Web.Geometry.Polygon();
            poly.Rings = rings;

            //<Insert profanity here> Ok, this is getting pretty f*&^ing obnoxious.  We're going to try buffering the polygon with a zero magnitude, and see 
            //if we can get the server to generate a properly closed polygon for us.
            ESRI.ArcGIS.Geometry.IGeometry ig = ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Local.Converter.ToIGeometry(poly, _sc);

            ESRI.ArcGIS.Geometry.ITopologicalOperator topop = (ESRI.ArcGIS.Geometry.ITopologicalOperator)ig;

            ESRI.ArcGIS.Geometry.IGeometry bg = topop.Buffer(0.000);

            return (ESRI.ArcGIS.Geometry.IPolygon)bg;
        }

        public ESRI.ArcGIS.Geometry.IPolygon bufferPoint(ESRI.ArcGIS.Geometry.IPoint pointToBuffer, double distance)
        {
            ESRI.ArcGIS.Geometry.ITopologicalOperator ito = (ESRI.ArcGIS.Geometry.ITopologicalOperator)pointToBuffer;

            ESRI.ArcGIS.Geometry.IGeometry ig = ito.Buffer(distance);

            return (ESRI.ArcGIS.Geometry.IPolygon)ig;

        }

        /// <summary>
        /// Queries the specified layer using the parameters specified and returns the result as a recordset.
        /// </summary>
        /// <param name="whereclause"></param>
        /// <param name="geofilter"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public ESRI.ArcGIS.ADF.ArcGISServer.RecordSet performQuery(string whereclause, ESRI.ArcGIS.ADF.ArcGISServer.Geometry geofilter, string layerName, ESRI.ArcGIS.ADF.ArcGISServer.esriSpatialRelEnum rel)
        {
            int layerIndex = translateLayerName(layerName);

            return performQuery(whereclause, geofilter, layerIndex, rel);
        }

        public ESRI.ArcGIS.ADF.ArcGISServer.RecordSet performQuery(string whereclause, ESRI.ArcGIS.ADF.ArcGISServer.Geometry geofilter, int layerIndex, ESRI.ArcGIS.ADF.ArcGISServer.esriSpatialRelEnum rel)
        {
            //This is what we'll be returning.
            ESRI.ArcGIS.ADF.ArcGISServer.RecordSet rs = null;

            ESRI.ArcGIS.ADF.ArcGISServer.QueryFilter qf;

            

            if(geofilter == null)
            {
                qf = new ESRI.ArcGIS.ADF.ArcGISServer.QueryFilter();
                qf.WhereClause = whereclause;
                rs = _msp.QueryFeatureData(_mapDescription.Name, layerIndex, qf);
            }
            else
            {
                //Create a spatial filter.
                ESRI.ArcGIS.ADF.ArcGISServer.SpatialFilter sf = new ESRI.ArcGIS.ADF.ArcGISServer.SpatialFilter();

                sf.FilterGeometry = geofilter;
                
                if (whereclause != null)
                {
                    sf.WhereClause = whereclause;
                }
                else sf.WhereClause = "";

                //Set the spatial relationship.
                sf.SpatialRel = rel;

                //Set the fields we want returned.
                sf.SubFields = "*";

                //There is actually a way to figure this out programmatically, but I forget what it is.  I guess it doesn't matter in this case.
                sf.GeometryFieldName = "SHAPE";

                try
                {
                    rs = _msp.QueryFeatureData(_mapDescription.Name, layerIndex, sf);
                }
                catch
                {
                    return null;
                }
            }
            
            return rs;
        }

        /// <summary>
        /// This is a more complex method that uses the DCOM ArcObjects interface to hit the server directly, instead of pussyfooting around with the ADF.
        /// This should be faster than the ADF version, as well as offer significantly greater functionality.
        /// </summary>
        /// <param name="whereclause">The where clause to be used to filter the data.</param>
        /// <param name="geofilter">The geometry used to filter the data</param>
        /// <param name="rel">The spatial relationship between </param>
        /// <param name="datasetName">The name of the dataset on the server that we want to query.</param>
        /// <returns>DataTable containing the results of our query.</returns>
        public System.Data.DataTable performBaseFeatureDataQuery(string whereclause, ESRI.ArcGIS.Geometry.IGeometry geofilter, ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum rel, string datasetName)
        {
            //Declare vars here
            IWorkspace ws;

            //Try to create the workspace.
            try
            {
                ws = getWorkspace();
            }
            catch
            {
                return null;
            }

            //Ok, now we have a workspace.
            
            //The next step is to try to find the dataset that the user specified.
            IDataset id = getFeatureClass(ws, datasetName);
            if (id == null) return null;
            IFeatureClass ifc;
            try
            {
                ifc = (IFeatureClass)id;
            }
            catch
            {
                return null;
            }


            //We now have a feature class that we can query, so we need to set up a query filter.
            IQueryFilter qf;

            try
            {
                if (geofilter == null)
                {
                    //Make sure we had an actual where clause, and abort if we didn't.  This prevents us from retrieving an entire dataset with no filtering at all.
                    //Attempting to retrieve the entire dataset could saveagely rape the DB server if the data size is large enough.
                    if (whereclause == string.Empty || whereclause == null) return null;
                    else
                    {
                        //Ok, lets give this a shot.
                        qf = (IQueryFilter)_sc.CreateObject("esriGeodatabase.QueryFilter");

                        qf.WhereClause = whereclause;
                    }
                }
                else
                {
                    //We had geometry passed to us.
                    ISpatialFilter isf = (ISpatialFilter)_sc.CreateObject("esriGeodatabase.SpatialFilter");
                    //Set up the query filter.
                    isf.Geometry = geofilter;
                    isf.GeometryField = ifc.ShapeFieldName;
                    isf.SpatialRel = rel;
                    qf = (IQueryFilter)isf;
                }
            }
            catch
            {
                return null;
            }

            try
            {
                System.Data.DataTable awesome = null;

                bool trySlowMethod = false;

                try
                {
                    ESRI.ArcGIS.Geodatabase.IRecordSetInit irsi = (ESRI.ArcGIS.Geodatabase.IRecordSetInit)_sc.CreateObject("esriGeodatabase.RecordSet");

                    //We have to limit the size of the return or the system can potentially crash and burn.
                    irsi.MaxRecordCount = maxrecords;

                    irsi.SetSourceTable((ITable) ifc, qf);

                    IRecordSet irs = (IRecordSet)irsi;

                    ESRI.ArcGIS.ADF.ArcGISServer.RecordSet rs = (ESRI.ArcGIS.ADF.ArcGISServer.RecordSet)ESRI.ArcGIS.ADF.ArcGISServer.Converter.ComObjectToValueObject(irs, _sc, typeof(ESRI.ArcGIS.ADF.ArcGISServer.RecordSet));

                    try
                    {
                        awesome = ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ToDataTable(rs);
                    }
                    catch
                    {
                        awesome = RecordSetToDataTable(rs);
                    }
                
                

                    if (awesome.Columns.Contains("SHAPE"))
                    {
                        int? geomFieldIndex = findGeometryFieldIndex(rs);
                        if (geomFieldIndex != null)
                        {
                            int gfi = System.Convert.ToInt32(geomFieldIndex);
                            for (int i = 0; i < awesome.Rows.Count; i++)
                            {
                                object o = rs.Records[i].Values[gfi];

                                if(o is System.Xml.XmlNode[])
                                {
                                    ESRI.ArcGIS.ADF.Web.Geometry.Geometry geom_un = UnserializeGeometryBroken((System.Xml.XmlNode[]) o);

                                    if (geom_un != null) awesome.Rows[i]["SHAPE"] = geom_un;
                                }
                            }
                        }
                    }
                }
                catch(System.Exception e)
                {
                    trySlowMethod = true;
                }
                #region Slow method - We try this if the standard technique throws an exception.
                if (trySlowMethod)
                {
                    awesome = SlowMethodBaseDataQuery((ITable) ifc, qf, maxrecords, true);
                }
                #endregion


                //Before we return our table, lets convert all of the NaN values to ordinary DBNulls, which ASP.NET is better able to handle.
                for (int i = 0; i < awesome.Rows.Count; i++)
                {
                    for (int j = 0; j < awesome.Columns.Count; j++)
                    {
                        if (awesome.Columns[j].DataType == typeof(Double))
                        {
                            //The double type.
                            if (awesome.Rows[i][j] is double)
                            {
                                double d = (double)awesome.Rows[i][j];

                                if (Double.IsNaN(d)) awesome.Rows[i][j] = System.DBNull.Value;
                            }
                        }
                    }
                }

                return awesome;
            }
            catch (System.Exception e)
            {
                //Might add a bit to log non-syntax exceptions later.
                return null;
            }
        }

        protected System.Data.DataTable SlowMethodBaseDataQuery(ITable ifc, IQueryFilter qf, int maxrecords, bool loadgeometry)
        {
            //Perform the search.
            ICursor featcur;

            System.Data.DataTable dt = null;

            try
            {
                featcur = (ICursor)ifc.Search(qf, true);
            }
            catch (Exception e)
            {
                return null;
            }

            try
            {
                dt = featCursorToDataTable((IFeatureCursor)featcur);
            }
            catch (Exception e)
            {
                return null;
            }

            return dt;
        }

        /// <summary>
        /// Converts from the ESRI recordset format to the .NET datatable format.  This gets us around some annoying limitations that ESRI imposes.
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public System.Data.DataTable RecordSetToDataTable(ESRI.ArcGIS.ADF.ArcGISServer.RecordSet rs)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            //The first thing we have to do is set up the datatable to contain our data.
            for (int i = 0; i < rs.Fields.FieldArray.Length; i++)
            {
                System.Data.DataColumn dc = new System.Data.DataColumn();

                dc.ColumnName = rs.Fields.FieldArray[i].Name;
                dc.DataType = ConvertEsriFieldType(rs.Fields.FieldArray[i].Type);
                dc.AllowDBNull = true;  //Make it able to contain a null so it won't crash.

                dt.Columns.Add(dc);
            }

            //Now, we copy the data into it.
            for (int i = 0; i < rs.Records.Length; i++)
            {
                //Create the row
                System.Data.DataRow dr = dt.NewRow();

                for (int j = 0; j < rs.Records[i].Values.Length; j++)
                {
                    if (rs.Records[i].Values[j] != null) dr[j] = rs.Records[i].Values[j];
                    else dr[j] = System.DBNull.Value;
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static Type ConvertEsriFieldType(ESRI.ArcGIS.ADF.ArcGISServer.esriFieldType fieldType)
        {
            Type t = Type.GetType("System.Object");

            switch (fieldType)
            {
                case ESRI.ArcGIS.ADF.ArcGISServer.esriFieldType.esriFieldTypeOID: t = Type.GetType("System.Int32");
                    break;
                case ESRI.ArcGIS.ADF.ArcGISServer.esriFieldType.esriFieldTypeInteger: t = Type.GetType("System.Int32");
                    break;
                case ESRI.ArcGIS.ADF.ArcGISServer.esriFieldType.esriFieldTypeGeometry: t = Type.GetType("System.Object");
                    break;
                case ESRI.ArcGIS.ADF.ArcGISServer.esriFieldType.esriFieldTypeDouble: t = Type.GetType("System.Double");
                    break;
                case ESRI.ArcGIS.ADF.ArcGISServer.esriFieldType.esriFieldTypeString: t = Type.GetType("System.String");
                    break;
            }

            return t;
        }

        private int? findGeometryFieldIndex(ESRI.ArcGIS.ADF.ArcGISServer.RecordSet rs)
        {
            for (int i = 0; i < rs.Fields.FieldArray.Length; i++)
            {
                if (rs.Fields.FieldArray[i].Type == ESRI.ArcGIS.ADF.ArcGISServer.esriFieldType.esriFieldTypeGeometry) return i;
            }

            return null;
        }

        /// <summary>
        /// Unserializes an apparently broken Geometry XML or something.  I wish I'd commented this originally.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        /*public ESRI.ArcGIS.ADF.Web.Geometry.Geometry UnserializeGeometryBroken(System.Xml.XmlNode[] x)
        {
            string outer1 = string.Empty;
            string outer2 = string.Empty;
            string middle = string.Empty;

            for (int i = 0; i < x.Length; i++)
            {
                string q = x[i].OuterXml;

                q = q.Replace("\"", "'");
                q = q.Replace("esri:", "typens:");
                middle += q;
            }



            if (x[0].Value.Contains("PolygonN"))
            {
                //outer1 = "<PolygonN xsi:type='typens:PolygonN' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:typens='http://www.esri.com/schemas/ArcGIS/9.2'>";

                //outer2 = "</PolygonN>";

                string k = outer1 + middle + outer2;

                ESRI.ArcGIS.ADF.ArcGISServer.PolygonN pn = (ESRI.ArcGIS.ADF.ArcGISServer.PolygonN)ESRI.ArcGIS.ADF.ArcGISServer.Converter.DeserializeValueObject((outer1 + middle + outer2), typeof(ESRI.ArcGIS.ADF.ArcGISServer.PolygonN));
                ESRI.ArcGIS.ADF.Web.Geometry.Polygon pg = (ESRI.ArcGIS.ADF.Web.Geometry.Polygon)ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ToAdfGeometry(pn);

                return (ESRI.ArcGIS.ADF.Web.Geometry.Geometry)pg;

            }
            else if (x[0].Value.Contains("PolylineN"))
            {
                //outer1 = "<PolylineN xsi:type='typens:PolylineN' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:typens='http://www.esri.com/schemas/ArcGIS/9.2'>";

                //outer2 = "</PolylineN>";

                ESRI.ArcGIS.ADF.ArcGISServer.PolylineN pn = (ESRI.ArcGIS.ADF.ArcGISServer.PolylineN)ESRI.ArcGIS.ADF.ArcGISServer.Converter.DeserializeValueObject((outer1 + middle + outer2), typeof(ESRI.ArcGIS.ADF.ArcGISServer.PolylineN));
                ESRI.ArcGIS.ADF.Web.Geometry.Polyline pg = (ESRI.ArcGIS.ADF.Web.Geometry.Polyline)ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ToAdfGeometry(pn);

                return (ESRI.ArcGIS.ADF.Web.Geometry.Geometry)pg;
            }
            else if (x[0].Value.Contains("PointN"))
            {
                //outer1 = "<PointN xsi:type='typens:PointN' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:typens='http://www.esri.com/schemas/ArcGIS/9.2'>";

                //outer2 = "</PointN>";

                ESRI.ArcGIS.ADF.ArcGISServer.PointN pn = (ESRI.ArcGIS.ADF.ArcGISServer.PointN)ESRI.ArcGIS.ADF.ArcGISServer.Converter.DeserializeValueObject((outer1 + middle + outer2), typeof(ESRI.ArcGIS.ADF.ArcGISServer.PointN));
                ESRI.ArcGIS.ADF.Web.Geometry.Point pg = (ESRI.ArcGIS.ADF.Web.Geometry.Point)ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ToAdfGeometry(pn);

                return (ESRI.ArcGIS.ADF.Web.Geometry.Geometry)pg;
            }



            return null;
        }*/

        public ESRI.ArcGIS.ADF.Web.Geometry.Geometry UnserializeGeometryBroken(System.Xml.XmlNode[] x)
        {
            string outer1 = string.Empty;
            string outer2 = string.Empty;
            string middle = string.Empty;

            for (int i = 1; i < x.Length; i++)
            {
                string q = x[i].OuterXml;

                //q = q.Replace("\"", "'");
                //q = q.Replace("esri:", "typens:");
                middle += q;
            }



            if (x[0].Value.Contains("PolygonN"))
            {
                outer1 = "<PolygonN xsi:type='typens:PolygonN' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:typens='http://www.esri.com/schemas/ArcGIS/9.2'>";

                outer2 = "</PolygonN>";

                middle.Replace("xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'", "");

                string k = outer1 + middle + outer2;

                ESRI.ArcGIS.ADF.ArcGISServer.PolygonN pn = (ESRI.ArcGIS.ADF.ArcGISServer.PolygonN)ESRI.ArcGIS.ADF.ArcGISServer.Converter.DeserializeValueObject((outer1 + middle + outer2), typeof(ESRI.ArcGIS.ADF.ArcGISServer.PolygonN));
                ESRI.ArcGIS.ADF.Web.Geometry.Polygon pg = (ESRI.ArcGIS.ADF.Web.Geometry.Polygon)ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ToAdfGeometry(pn);

                return (ESRI.ArcGIS.ADF.Web.Geometry.Geometry)pg;

            }
            else if (x[0].Value.Contains("PolylineN"))
            {
                outer1 = "<PolylineN xsi:type='typens:PolylineN' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:typens='http://www.esri.com/schemas/ArcGIS/9.2'>";

                outer2 = "</PolylineN>";

                ESRI.ArcGIS.ADF.ArcGISServer.PolylineN pn = (ESRI.ArcGIS.ADF.ArcGISServer.PolylineN)ESRI.ArcGIS.ADF.ArcGISServer.Converter.DeserializeValueObject((outer1 + middle + outer2), typeof(ESRI.ArcGIS.ADF.ArcGISServer.PolylineN));
                ESRI.ArcGIS.ADF.Web.Geometry.Polyline pg = (ESRI.ArcGIS.ADF.Web.Geometry.Polyline)ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ToAdfGeometry(pn);

                return (ESRI.ArcGIS.ADF.Web.Geometry.Geometry)pg;
            }
            else if (x[0].Value.Contains("PointN"))
            {
                outer1 = "<PointN xsi:type='typens:PointN' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:typens='http://www.esri.com/schemas/ArcGIS/9.2'>";

                outer2 = "</PointN>";

                //ESRI.ArcGIS.ADF.ArcGISServer.PointN pn = (ESRI.ArcGIS.ADF.ArcGISServer.PointN)ESRI.ArcGIS.ADF.ArcGISServer.Converter.DeserializeValueObject((outer1 + middle + outer2), typeof(ESRI.ArcGIS.ADF.ArcGISServer.PointN));
                ESRI.ArcGIS.ADF.ArcGISServer.PointN pn = (ESRI.ArcGIS.ADF.ArcGISServer.PointN)ESRI.ArcGIS.ADF.ArcGISServer.Converter.DeserializeValueObject((middle), typeof(ESRI.ArcGIS.ADF.ArcGISServer.PointN));
                ESRI.ArcGIS.ADF.Web.Geometry.Point pg = (ESRI.ArcGIS.ADF.Web.Geometry.Point)ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ToAdfGeometry(pn);

                return (ESRI.ArcGIS.ADF.Web.Geometry.Geometry)pg;
            }



            return null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ic"></param>
        /// <returns></returns>
        private System.Data.DataTable featCursorToDataTable(IFeatureCursor featcur)
        {
            if (featcur != null)
            {

                //We had a valid cursor returned.  That means that we're going to try to extract the data from it.

                System.Data.DataTable dt = new System.Data.DataTable();
                //Set up the fields.
                for (int i = 0; i < featcur.Fields.FieldCount; i++)
                {
                    IField f = featcur.Fields.get_Field(i);
                    dt.Columns.Add(new System.Data.DataColumn(f.Name, typeof(object)));
                }

                //Now that we have all of the cols created, we're ready to copy our data into the datatable.
                IFeature ifeat;
                object[] vals;
                while ((ifeat = featcur.NextFeature()) != null)
                {
                    vals = new object[featcur.Fields.FieldCount];
                    for (int i = 0; i < featcur.Fields.FieldCount; i++)
                    {
                        object o = ifeat.get_Value(i);

                        //Most of the time, it's just a matter of shoveling the value into the table, but the geometry needs special attention.
                        if (o is ESRI.ArcGIS.Geometry.IGeometry)
                        {
                            ESRI.ArcGIS.Geometry.IGeometry ig = (ESRI.ArcGIS.Geometry.IGeometry)o;
                            vals[i] = ig;
                        }
                        else vals[i] = o;

                        
                    }

                    dt.Rows.Add(vals);
                }

                return dt;
            }
            return null;
        }

        public System.Data.DataTable performMapServiceQuery(string whereclause, ESRI.ArcGIS.ADF.Web.Geometry.Geometry geofilter, string layername)
        {
            //Assuming we're initialized, we're now ready to get ready to perform the query.

            ESRI.ArcGIS.ADF.Web.QueryFilter qf = null;

            if (geofilter == null)
            {
                qf = new ESRI.ArcGIS.ADF.Web.QueryFilter();
            }
            else
            {
                ESRI.ArcGIS.ADF.Web.SpatialFilter sf = new ESRI.ArcGIS.ADF.Web.SpatialFilter();

                sf.Geometry = geofilter;
                sf.GeometryField = "SHAPE";

                qf = (ESRI.ArcGIS.ADF.Web.QueryFilter)sf;
            }

            qf.WhereClause = whereclause;
            
            System.Data.DataTable dt = _qf.Query(null, layername, qf);

            return dt;

        }

        /// <summary>
        /// This method gets an instance of the given dataset.
        /// </summary>
        /// <param name="ws"></param>
        /// <returns></returns>
        private IDataset getFeatureClass(IWorkspace ws, string datasetName)
        {
            //Make sure that we had a real workspace.
            if (ws == null) return null;
            
            //Get the dataset itself.
            IEnumDataset ied = ws.get_Datasets(esriDatasetType.esriDTFeatureClass);

            IDataset ids = null;
            while ((ids = ied.Next()) != null)
            {
                //Keep looking till we find the dataset we wanted.
                if (ids.Name.ToLower() == datasetName.ToLower()) break;
            }

            //Return this, whatever it contains.
            return ids;
        }

        IWorkspace _ws = null;

        /// <summary>
        /// This gets the workspace that our map's data exist in.
        /// </summary>
        /// <returns></returns>
        private IWorkspace getWorkspace()
        {
            if (_ws == null)
            {
                //Create the workspace factory
                IWorkspaceFactory factory = (IWorkspaceFactory)_sc.CreateObject("esriDataSourcesGDB.SdeWorkspaceFactory");

                ESRI.ArcGIS.esriSystem.IPropertySet ips = (ESRI.ArcGIS.esriSystem.IPropertySet)_sc.CreateObject("esriSystem.PropertySet");
                ips.SetProperty("SERVER", SDE_Servername);
                ips.SetProperty("INSTANCE", SDE_Instance);
                ips.SetProperty("DATABASE", SDE_Database);
                ips.SetProperty("USER", SDE_User);
                ips.SetProperty("PASSWORD", SDE_Password);
                ips.SetProperty("VERSION", SDE_Version);

                IWorkspace ws = factory.Open(ips, 0);

                _ws = ws;


            }

            return _ws;
        }

        /// <summary>
        /// Executes a query against the given raster dataset
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double queryRaster(ESRI.ArcGIS.Geometry.IPoint p, string datasetName)
        {
            IWorkspace ws = getWorkspace();

            IRasterDataset ird = getRasterDataset(ws, datasetName);
            IRaster ir = ird.CreateDefaultRaster();

            ESRI.ArcGIS.DataSourcesRaster.IRaster2 irs = (ESRI.ArcGIS.DataSourcesRaster.IRaster2)ir;

            int column, row;

            //Here, we map the geographic coords
            irs.MapToPixel(p.X, p.Y, out column, out row);

            object o = irs.GetPixelValue(0, column, row);

            //Now we need to convert whatever the value we get from that into a double.  It will have to be numeric in some form or another.
            if (o is int || o is long)
            {
                int o_i = (int)o;

                return (double)o_i;
            }
            else if (o is double)
            {
                double o_d;
                o_d = (double)o;

                return o_d;
            }
            else if (o is float)
            {
                float o_f;
                o_f = (float)o;
                return (double)o_f;
            }
            else if(o != null)
            {
                double o_x;
                string s = o.ToString();

                if (double.TryParse(s, out o_x)) return o_x;
                else return 0;
            }

            //Release the com objects used.
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(ir);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(ird);
            //System.Runtime.InteropServices.Marshal.FinalReleaseComObject(ws);

            return 0;

        }

        /// <summary>
        /// Gets a handle on the raster dataset 
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="datasetName"></param>
        /// <returns></returns>
        private IRasterDataset getRasterDataset(IWorkspace ws, string datasetName)
        {
            //Make sure that we had a real workspace.
            if (ws == null) return null;

            //Get the dataset itself.
            /*IEnumDataset ied = ws.get_Datasets(esriDatasetType.esriDTRasterDataset);

            IDataset ids = null;
            while ((ids = ied.Next()) != null)
            {
                //Keep looking till we find the dataset we wanted.
                if (ids.Name.ToLower() == datasetName.ToLower()) break;
            }

            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(ied);*/


            if (ws is IRasterWorkspaceEx)
            {
                IRasterWorkspaceEx rws = (IRasterWorkspaceEx)ws;

                IRasterDataset ird = rws.OpenRasterDataset(datasetName);

                return ird;
            }

            //Return this, whatever it contains.
            return null;
        }

        private IRasterLayer getRasterLayer(IWorkspace ws, string datasetName)
        {
            //Get the raster dataset.
            IRasterDataset ird = getRasterDataset(ws, datasetName);

            try
            {
                IRasterLayer irl = (IRasterLayer)ird;
            }
            catch
            {

            }

            return null;
        }
        
        #region utility methods

        /// <summary>
        /// This method translates a layer name into a layer index, which can be used to get the layer information.
        /// </summary>
        /// <param name="layername">The name of the layer you want.</param>
        /// <returns></returns>
        public int translateLayerName(string layerName)
        {
            //Make sure we have query functionality.
            if (!hasQueryFunctionality) throw new Exception("Resource does not have query functionality.");

            //get the queryable layers.
            string[] lids;
            string[] lnames;

            int layerindex = -1;

            //This gets the names and IDs of the queryable layers in our map resource.
            _qf.GetQueryableLayers(null, out lids, out lnames);

            for (int i = 0; i < lnames.Length; i++)
            {
                if (lnames[i] == layerName) //Is the layer name the same?
                {
                    if (lids[i] is string)
                    {
                        int lindex;
                        if (int.TryParse(lids[i], out lindex)) layerindex = lindex;
                    }
                    else
                    {
                        layerindex = i;
                    }
                }
            }

            return layerindex;
        }

        /// <summary>
        /// This method translates a layer index into the corresponding layer's name.
        /// </summary>
        /// <param name="layerIndex"></param>
        /// <returns></returns>
        public string translateLayerIndex(int layerIndex)
        {
            if (!hasQueryFunctionality) throw new Exception("Resource does not have query functionality");

            string[] lids;
            string[] lnames;

            _qf.GetQueryableLayers(null, out lids, out lnames);

            for (int i = 0; i < lnames.Length; i++)
            {
                int k;
                if (Int32.TryParse(lids[i], out k))
                {
                    if (k == layerIndex) return lnames[i];
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// This returns the set of queryable layer names as an array of strings.
        /// </summary>
        /// <returns></returns>
        public string[] getQueryableLayerNames()
        {
            string[] lnames;
            if (!hasQueryFunctionality)
            {
                lnames = new string[0];
                return lnames;
            }

            string[] lids;

            _qf.GetQueryableLayers(null, out lids, out lnames);

            return lnames;
        }

        /// <summary>
        /// This converts from a geographic coord system to a projected coord system.
        /// </summary>
        /// <param name="g">The geometry to be projected.</param>
        /// <param name="origCoordSys">The ID of the coordinate system that the original geometry is in.</param>
        /// <param name="outputCoordSys">The ID of the coordinate system desired.</param>
        /// <returns></returns>
        public ESRI.ArcGIS.Geometry.IGeometry project(ESRI.ArcGIS.Geometry.IGeometry g, int inputcoordsys, int outputcoordsys)
        {
            //If the coordinate systems are the same, we'll save ourselves a little processing.
            if (inputcoordsys == outputcoordsys) return g;

            ESRI.ArcGIS.Geometry.ISpatialReference srgeo;
            ESRI.ArcGIS.Geometry.SpatialReferenceEnvironment srenv = (ESRI.ArcGIS.Geometry.SpatialReferenceEnvironment)_sc.CreateObject("esriGeometry.SpatialReferenceEnvironment");
            ESRI.ArcGIS.Geometry.IGeographicCoordinateSystem geocoordsys = srenv.CreateGeographicCoordinateSystem(inputcoordsys);
            srgeo = (ESRI.ArcGIS.Geometry.ISpatialReference)geocoordsys;

            ESRI.ArcGIS.Geometry.ISpatialReference srproj;
            ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem projcoordsys = (ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem) srenv.CreateProjectedCoordinateSystem(outputcoordsys);
            srproj = (ESRI.ArcGIS.Geometry.ISpatialReference)projcoordsys;

            g.SpatialReference = srgeo;
            g.Project(srproj);

            return g;
        }
        
        /// <summary>
        /// Converts from a projected coord system to an unprojected coord system.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="outputcoordsys"></param>
        /// <param name="inputcoordsys"></param>
        /// <returns></returns>
        public ESRI.ArcGIS.Geometry.IGeometry unprojectGeometry(ESRI.ArcGIS.Geometry.IGeometry g, int inputcoordsys, int outputcoordsys)
        {

            ESRI.ArcGIS.Geometry.IGeometry gc;
            
            //Convert it to a value object
            ESRI.ArcGIS.ADF.ArcGISServer.PointN gv = (ESRI.ArcGIS.ADF.ArcGISServer.PointN)ESRI.ArcGIS.ADF.ArcGISServer.Converter.ComObjectToValueObject(g, _sc, typeof(ESRI.ArcGIS.ADF.ArcGISServer.PointN));
            //Then back to a COM proxy object.
            gc = (ESRI.ArcGIS.Geometry.IGeometry)ESRI.ArcGIS.ADF.ArcGISServer.Converter.ValueObjectToComObject(gv, _sc);
            
            //GC is now a copy of gv.

            //Set up our (actually) projected spatial reference.
            ESRI.ArcGIS.Geometry.ISpatialReference srproj;
            ESRI.ArcGIS.Geometry.SpatialReferenceEnvironment srenv = (ESRI.ArcGIS.Geometry.SpatialReferenceEnvironment)_sc.CreateObject("esriGeometry.SpatialReferenceEnvironment");
            ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem projcoordsys = srenv.CreateProjectedCoordinateSystem(inputcoordsys);
            srproj = (ESRI.ArcGIS.Geometry.ISpatialReference)projcoordsys;

            //Now the input spatial reference.
            ESRI.ArcGIS.Geometry.ISpatialReference srgeo;
            ESRI.ArcGIS.Geometry.IGeographicCoordinateSystem igcs = srenv.CreateGeographicCoordinateSystem(outputcoordsys);
            srgeo = (ESRI.ArcGIS.Geometry.ISpatialReference)igcs;

            gc.SpatialReference = srproj;

            gc.Project(srgeo);

            return gc;
        }

        public void zoomToCoords(ESRI.ArcGIS.Geometry.IPoint p, ESRI.ArcGIS.ADF.Web.UI.WebControls.Map m, double span, int inputcoordsys, int mapcoordsys)
        {
            //Create an envelope object.  This will be applied to the map as its extent.
            
            double centerX = p.X;
            double centerY = p.Y;
            double xMin, xMax, yMin, yMax;

            xMin = centerX - (Math.Abs(span) * 0.5);
            xMax = centerX + (Math.Abs(span) * 0.5);
            yMin = centerY - (Math.Abs(span) * 0.5);
            yMax = centerY + (Math.Abs(span) * 0.5);

            ESRI.ArcGIS.Geometry.IPoint upperRight, lowerLeft;

            upperRight = (ESRI.ArcGIS.Geometry.IPoint) _sc.CreateObject("esriGeometry.Point");
            lowerLeft = (ESRI.ArcGIS.Geometry.IPoint) _sc.CreateObject("esriGeometry.Point");

            upperRight.X = xMax;
            upperRight.Y = yMax;
            lowerLeft.X = xMin;
            lowerLeft.Y = yMin;

            ESRI.ArcGIS.Geometry.IGeometry upperRightProjected = project((ESRI.ArcGIS.Geometry.IGeometry) upperRight,inputcoordsys,mapcoordsys);
            ESRI.ArcGIS.Geometry.IGeometry lowerLeftProjected = project((ESRI.ArcGIS.Geometry.IGeometry) lowerLeft,inputcoordsys,mapcoordsys);

            //We now need to convert these back to the adf.web.geometry namespace.
            ESRI.ArcGIS.ADF.ArcGISServer.PointN upperRightPN = (ESRI.ArcGIS.ADF.ArcGISServer.PointN) ESRI.ArcGIS.ADF.ArcGISServer.Converter.ComObjectToValueObject(upperRightProjected,_sc,typeof(ESRI.ArcGIS.ADF.ArcGISServer.PointN));
            ESRI.ArcGIS.ADF.ArcGISServer.PointN lowerLeftPN = (ESRI.ArcGIS.ADF.ArcGISServer.PointN) ESRI.ArcGIS.ADF.ArcGISServer.Converter.ComObjectToValueObject(lowerLeftProjected,_sc,typeof(ESRI.ArcGIS.ADF.ArcGISServer.PointN));

            ESRI.ArcGIS.ADF.Web.Geometry.Point upperRightWeb = (ESRI.ArcGIS.ADF.Web.Geometry.Point) ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ToAdfPoint(upperRightPN);
            ESRI.ArcGIS.ADF.Web.Geometry.Point lowerLeftWeb = (ESRI.ArcGIS.ADF.Web.Geometry.Point) ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ToAdfPoint(lowerLeftPN);

            //Construct an extent based on the information we got.
            ESRI.ArcGIS.ADF.Web.Geometry.Envelope e = new ESRI.ArcGIS.ADF.Web.Geometry.Envelope(lowerLeftWeb, upperRightWeb);

            //Set the extent on the map.
            m.Extent = e;
        }
        
        //Retrieves the envelope for this object.
        public ESRI.ArcGIS.ADF.Web.Geometry.Envelope getEnvelope(ESRI.ArcGIS.ADF.Web.Geometry.Polygon poly)
        {
            //To get the envelope, we have to convert to a COM object handle.

            //First to a value object...
            ESRI.ArcGIS.ADF.ArcGISServer.PolygonN pn = (ESRI.ArcGIS.ADF.ArcGISServer.PolygonN)ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.FromAdfPolygon(poly);

            //Then to the com object
            ESRI.ArcGIS.Geometry.IPolygon ip = (ESRI.ArcGIS.Geometry.IPolygon) ESRI.ArcGIS.ADF.ArcGISServer.Converter.ValueObjectToComObject(pn, _sc);

            //Now we extract the envelope.
            ESRI.ArcGIS.Geometry.IEnvelope ie = ip.Envelope;

            //And convert back.
            ESRI.ArcGIS.ADF.ArcGISServer.EnvelopeN ae = (ESRI.ArcGIS.ADF.ArcGISServer.EnvelopeN)ESRI.ArcGIS.ADF.ArcGISServer.Converter.ComObjectToValueObject(ie, _sc, typeof(ESRI.ArcGIS.ADF.ArcGISServer.EnvelopeN));

            ESRI.ArcGIS.ADF.Web.Geometry.Envelope e = ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ToAdfEnvelope(ae);
            
            return e;
        }

        /// <summary>
        /// This constructs an IPoint from the coords given.
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public ESRI.ArcGIS.Geometry.IPoint makeIPoint(double X, double Y)
        {
            ESRI.ArcGIS.Geometry.IPoint ip = (ESRI.ArcGIS.Geometry.IPoint)_sc.CreateObject("esriGeometry.Point");

            ip.X = X;
            ip.Y = Y;

            return ip;
        }

        /// <summary>
        /// Assembles a polyline from a a series of points.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public ESRI.ArcGIS.Geometry.IPolyline makePolyLine(ESRI.ArcGIS.Geometry.IPoint[] points)
        {
            //If we only had one point passed to us (or zero), return null, since we can't create much of a polyline with zero or one point.
            if(points.Length < 2) return null;

            //Create the polyline object on the GIS server.
            ESRI.ArcGIS.Geometry.IPolyline ipolyline = (ESRI.ArcGIS.Geometry.IPolyline)_sc.CreateObject("esriGeometry.Polyline");

            //Cast to a point collection.
            ESRI.ArcGIS.Geometry.IPointCollection ipc = (ESRI.ArcGIS.Geometry.IPointCollection) ipolyline;

            //Now we loop through all the points 
            for (int i = 0; i < points.Length; i++)
            {
                ESRI.ArcGIS.Geometry.IPoint ip_before;
                ESRI.ArcGIS.Geometry.IPoint ip_after;

                object a, b;
                object missing = Type.Missing;

                //If this is the first point, there is no before.
                if (i == 0)
                {
                    ip_before = null;
                    ip_after = points[1];
                }
                else if (i == points.Length - 1)
                {
                    ip_after = null;
                    ip_before = points[i - 1];
                }
                else
                {
                    ip_after = points[i + 1];
                    ip_before = points[i - 1];
                }

                a = ip_after;
                b = ip_before;

                ipc.AddPoint(points[i], ref missing, ref missing);

            }

            return ipolyline;
        }

        public double convertUnits(double val, ESRI.ArcGIS.esriSystem.esriUnits origUnits, ESRI.ArcGIS.esriSystem.esriUnits finalUnits)
        {
            ESRI.ArcGIS.esriSystem.IUnitConverter iuc = (ESRI.ArcGIS.esriSystem.IUnitConverter) _sc.CreateObject("esriSystem.UnitConverter");

            double final = iuc.ConvertUnits(val, origUnits, finalUnits);

            return final;
        }

        #endregion

        #region drawing methods

        public void addPolygon(ESRI.ArcGIS.ADF.Web.Geometry.Polygon p, System.Drawing.Color symbolColor, System.Drawing.Color outlineColor, ESRI.ArcGIS.ADF.Web.Display.Symbol.PolygonFillType fillType, int borderWeight, double transparency, string labeltext)
        {
            ESRI.ArcGIS.ADF.Web.Display.Graphics.ElementGraphicsLayer eglayer;

            if (_mr_g.Graphics.Tables.Count == 0) eglayer = new ESRI.ArcGIS.ADF.Web.Display.Graphics.ElementGraphicsLayer();
            else eglayer = (ESRI.ArcGIS.ADF.Web.Display.Graphics.ElementGraphicsLayer) _mr_g.Graphics.Tables[0];

            ESRI.ArcGIS.ADF.Web.Display.Symbol.SimpleFillSymbol sfs = new ESRI.ArcGIS.ADF.Web.Display.Symbol.SimpleFillSymbol();
            sfs.BoundaryWidth = borderWeight;
            sfs.BoundaryColor = outlineColor;
            sfs.BoundaryTransparency = transparency;
            sfs.FillType = fillType;
            sfs.Color = symbolColor;
            sfs.Transparency = transparency;

            ESRI.ArcGIS.ADF.Web.Display.Graphics.GraphicElement ge = new ESRI.ArcGIS.ADF.Web.Display.Graphics.GraphicElement(p, sfs);

            eglayer.Add(ge);

            if(labeltext != string.Empty)
            {
                ESRI.ArcGIS.ADF.Web.Display.Symbol.TextMarkerSymbol tms = new ESRI.ArcGIS.ADF.Web.Display.Symbol.TextMarkerSymbol();
                tms.Font = new ESRI.ArcGIS.ADF.Web.FontInfo("Arial", 14, System.Drawing.Color.Black, ESRI.ArcGIS.ADF.Web.FontStyle.Regular);
                tms.Text = labeltext;
                tms.XOffset = 10;
                tms.YOffset = 10;
                eglayer.Add(new ESRI.ArcGIS.ADF.Web.Display.Graphics.GraphicElement(p, tms));
            }

            _mr_g.Graphics.Tables.Add(eglayer);

            _mapctl.RefreshResource(_mr_g.Name);
        }

        /// <summary>
        /// Adds this polygon to the map as part of the custom graphics layer.
        /// </summary>
        /// <param name="pn">The polygon to be drawn on the map.</param>
        public void addPolygon(ESRI.ArcGIS.Geometry.IPolygon pn)
        {
            ESRI.ArcGIS.ADF.ArcGISServer.PolygonN pn1 = (ESRI.ArcGIS.ADF.ArcGISServer.PolygonN)ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ComObjectToValueObject(pn, _sc, typeof(ESRI.ArcGIS.ADF.ArcGISServer.PolygonN));
            addPolygon(pn1);
        }

        /// <summary>
        /// Adds this polygon to the map as part of the custom graphics layer.
        /// </summary>
        /// <param name="pn">The polygon to be drawn on the map.</param>
        public void addPolygon(ESRI.ArcGIS.ADF.ArcGISServer.PolygonN pn)
        {
            ESRI.ArcGIS.ADF.ArcGISServer.RgbColor rgb = new ESRI.ArcGIS.ADF.ArcGISServer.RgbColor();
            rgb.Red = (byte)128;
            rgb.Blue = (byte) 20;
            rgb.Green = (byte) 20;
            rgb.AlphaValue = (byte)255;

            //Create our symbol.
            ESRI.ArcGIS.ADF.ArcGISServer.SimpleFillSymbol sfs = new ESRI.ArcGIS.ADF.ArcGISServer.SimpleFillSymbol();
            sfs.Style = ESRI.ArcGIS.ADF.ArcGISServer.esriSimpleFillStyle.esriSFSSolid;
            sfs.Color = rgb;

            ESRI.ArcGIS.ADF.ArcGISServer.PolygonElement pe = new ESRI.ArcGIS.ADF.ArcGISServer.PolygonElement();
            pe.Symbol = sfs;
            pe.Polygon = pn;

            //If the custom graphics layer isn't null...
            if (_mapDescription.CustomGraphics != null)
            {
                //We add a symbol to it.
                ESRI.ArcGIS.ADF.ArcGISServer.GraphicElement[] oldges = _mapDescription.CustomGraphics;
                int cnt = oldges.Length;
                ESRI.ArcGIS.ADF.ArcGISServer.GraphicElement[] newges = new ESRI.ArcGIS.ADF.ArcGISServer.GraphicElement[cnt + 1];
                //Copy the graphic elements into the new set.
                oldges.CopyTo(newges, 0);
                newges[cnt] = pe;
                _mapDescription.CustomGraphics = newges;
            }
            else
            {
                ESRI.ArcGIS.ADF.ArcGISServer.GraphicElement[] ges = new ESRI.ArcGIS.ADF.ArcGISServer.GraphicElement[1];
                ges[0] = pe;
                _mapDescription.CustomGraphics = ges;
            }
        }

        /// <summary>
        /// Adds the point feature to the custom graphics layer.
        /// </summary>
        /// <param name="ip">Point feature as a COM proxy on the GIS server</param>
        /// <param name="r">Red channel value</param>
        /// <param name="g">Red </param>
        /// <param name="b"></param>
        public void addPoint(ESRI.ArcGIS.Geometry.IPoint pn, byte r, byte g, byte b, ESRI.ArcGIS.ADF.ArcGISServer.esriSimpleMarkerStyle style)
        {
            ESRI.ArcGIS.ADF.ArcGISServer.PointN pn1 = (ESRI.ArcGIS.ADF.ArcGISServer.PointN)ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ComObjectToValueObject(pn, _sc, typeof(ESRI.ArcGIS.ADF.ArcGISServer.PointN));
            addPoint(pn1, r, g, b, style);
        }

        public void addPoint(ESRI.ArcGIS.ADF.Web.Geometry.Point p, System.Drawing.Color symbolColor, ESRI.ArcGIS.ADF.Web.Display.Symbol.MarkerSymbolType symbolType, int symbolSize, string labeltext)
        {
            ESRI.ArcGIS.ADF.Web.Display.Graphics.ElementGraphicsLayer eglayer;

            if (_mr_g.Graphics.Tables.Count == 0) eglayer = new ESRI.ArcGIS.ADF.Web.Display.Graphics.ElementGraphicsLayer();
            else eglayer = (ESRI.ArcGIS.ADF.Web.Display.Graphics.ElementGraphicsLayer) _mr_g.Graphics.Tables[0];

            //eglayer.RenderOnClient = true;

            ESRI.ArcGIS.ADF.Web.Display.Symbol.SimpleMarkerSymbol sms = new ESRI.ArcGIS.ADF.Web.Display.Symbol.SimpleMarkerSymbol();

            sms.Type = symbolType;
            sms.Width = symbolSize;
            sms.Color = symbolColor;

            ESRI.ArcGIS.ADF.Web.Display.Graphics.GraphicElement ge = new ESRI.ArcGIS.ADF.Web.Display.Graphics.GraphicElement(p, sms);

            eglayer.Add(ge);

            if(labeltext != string.Empty)
            {
                ESRI.ArcGIS.ADF.Web.Display.Symbol.TextMarkerSymbol tms = new ESRI.ArcGIS.ADF.Web.Display.Symbol.TextMarkerSymbol();
                tms.Font = new ESRI.ArcGIS.ADF.Web.FontInfo("Arial", 12, System.Drawing.Color.Black, ESRI.ArcGIS.ADF.Web.FontStyle.Regular);
                tms.Text = labeltext;
                tms.XOffset = 10;
                tms.YOffset = 10;
                eglayer.Add(new ESRI.ArcGIS.ADF.Web.Display.Graphics.GraphicElement(p, tms));
            }

            _mr_g.Graphics.Tables.Add(eglayer);

            

            _mapctl.RefreshResource(_mr_g.Name);
        }

        /// <summary>
        /// Adds the point feature to the custom graphics layer
        /// </summary>
        /// <param name="pn">Point feature in ADF format</param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="style"></param>
        public void addPoint(ESRI.ArcGIS.ADF.ArcGISServer.PointN pn, byte r, byte g, byte b, ESRI.ArcGIS.ADF.ArcGISServer.esriSimpleMarkerStyle style)
        {
            //Set up a color for our point symbol.
            ESRI.ArcGIS.ADF.ArcGISServer.RgbColor rgb = new ESRI.ArcGIS.ADF.ArcGISServer.RgbColor();
            rgb.Red = r;
            rgb.Green = g;
            rgb.Blue = b;
            rgb.AlphaValue = (byte)255;

            //Create our symbol.
            ESRI.ArcGIS.ADF.ArcGISServer.SimpleMarkerSymbol sms = new ESRI.ArcGIS.ADF.ArcGISServer.SimpleMarkerSymbol();
            sms.Style = style;
            sms.Size = 8.0;
            sms.Color = rgb;

            ESRI.ArcGIS.ADF.ArcGISServer.MarkerElement me = new ESRI.ArcGIS.ADF.ArcGISServer.MarkerElement();
            me.Point = pn;
            me.Symbol = sms;

            //If the custom graphics layer isn't null...
            if (_mapDescription.CustomGraphics != null)
            {
                //We add a symbol to it.
                ESRI.ArcGIS.ADF.ArcGISServer.GraphicElement[] oldges = _mapDescription.CustomGraphics;
                int cnt = oldges.Length;
                ESRI.ArcGIS.ADF.ArcGISServer.GraphicElement[] newges = new ESRI.ArcGIS.ADF.ArcGISServer.GraphicElement[cnt + 1];
                //Copy the graphic elements into the new set.
                oldges.CopyTo(newges, 0);
                newges[cnt] = me;
                _mapDescription.CustomGraphics = newges;
            }
            else
            {
                ESRI.ArcGIS.ADF.ArcGISServer.GraphicElement[] ges = new ESRI.ArcGIS.ADF.ArcGISServer.GraphicElement[1];
                ges[0] = me;
                _mapDescription.CustomGraphics = ges;
            }
        }

        public void addText(string text, ESRI.ArcGIS.ADF.ArcGISServer.Geometry ge, byte r, byte g, byte b, double size)
        {
            //Set up a color for our point symbol.
            ESRI.ArcGIS.ADF.ArcGISServer.RgbColor rgb = new ESRI.ArcGIS.ADF.ArcGISServer.RgbColor();
            rgb.Red = r;
            rgb.Green = g;
            rgb.Blue = b;
            rgb.AlphaValue = (byte)255;

            ESRI.ArcGIS.ADF.ArcGISServer.TextSymbol ts = new ESRI.ArcGIS.ADF.ArcGISServer.TextSymbol();
            ts.HorizontalAlignment = ESRI.ArcGIS.ADF.ArcGISServer.esriTextHorizontalAlignment.esriTHALeft;
            ts.VerticalAlignment = ESRI.ArcGIS.ADF.ArcGISServer.esriTextVerticalAlignment.esriTVATop;
            ts.Size = size;
            ts.FontName = "Arial";
            ts.FontItalic = true;

            ESRI.ArcGIS.ADF.ArcGISServer.TextElement te = new ESRI.ArcGIS.ADF.ArcGISServer.TextElement();
            te.Text = text;
            te.TextGeometry = ge;
            te.Symbol = ts;
            
            //If the custom graphics layer isn't null...
            if (_mapDescription.CustomGraphics != null)
            {
                //We add a symbol to it.
                ESRI.ArcGIS.ADF.ArcGISServer.GraphicElement[] oldges = _mapDescription.CustomGraphics;
                int cnt = oldges.Length;
                ESRI.ArcGIS.ADF.ArcGISServer.GraphicElement[] newges = new ESRI.ArcGIS.ADF.ArcGISServer.GraphicElement[cnt + 1];
                //Copy the graphic elements into the new set.
                oldges.CopyTo(newges, 0);
                newges[cnt] = te;
                _mapDescription.CustomGraphics = newges;
            }
            else
            {
                ESRI.ArcGIS.ADF.ArcGISServer.GraphicElement[] ges = new ESRI.ArcGIS.ADF.ArcGISServer.GraphicElement[1];
                ges[0] = te;
                _mapDescription.CustomGraphics = ges;
            }
        }

        /// <summary>
        /// Clears the custom graphics layer of any objects that have been added.
        /// </summary>
        public void clearGraphics()
        {
            //_mapDescription.CustomGraphics = null;

            //Empty out the custom graphics table.
            _mr_g.Graphics.Tables.Clear();

            
        }

        #endregion

        #region accessors
        /// <summary>
        /// This represents whether or not our resource has query functionality.
        /// </summary>
        public bool hasQueryFunctionality
        {
            get
            {
                bool s = _gr.SupportsFunctionality(typeof(ESRI.ArcGIS.ADF.Web.DataSources.IQueryFunctionality));

                return s;
            }
        }

        private ESRI.ArcGIS.ADF.Web.DataSources.IQueryFunctionality _qf
        {
            get
            {
                //First, we make sure that our resource has that functionality.
                if (!hasQueryFunctionality) throw new Exception("Resource does not have query functionality.");

                //Ok, do we have one created already?
                if (_query_functionality == null || !(_query_functionality is ESRI.ArcGIS.ADF.Web.DataSources.IQueryFunctionality))
                {
                    //No?  Let's create it.
                    _query_functionality = (ESRI.ArcGIS.ADF.Web.DataSources.IQueryFunctionality)_gr.CreateFunctionality(typeof(ESRI.ArcGIS.ADF.Web.DataSources.IQueryFunctionality), null);
                }

                //Now we check again.  If we don't have the query functionality, we throw an exception.
                if (_query_functionality == null || !(_query_functionality is ESRI.ArcGIS.ADF.Web.DataSources.IQueryFunctionality)) throw new Exception("Unable to access query functionality.");

                return _query_functionality;
            }
        }

        private ESRI.ArcGIS.ADF.ArcGISServer.MapServerProxy _msp
        {
            get
            {
                ESRI.ArcGIS.ADF.ArcGISServer.MapServerProxy m = _mr_local.MapServerProxy;
                if (m == null || !(m is ESRI.ArcGIS.ADF.ArcGISServer.MapServerProxy))// throw new Exception("Unable to access map server proxy");
                {
                    //Try to reinitialize with the defaults.
                    Initialize_cold();
                }
                return m;
            }
        }

        /// <summary>
        /// The map description.
        /// </summary>
        private ESRI.ArcGIS.ADF.ArcGISServer.MapDescription _mapDescription
        {
            get
            {
                if (_mf_local == null || !(_mf_local is ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.MapFunctionality))
                {
                    throw new Exception("Map functionality is not initialized.");
                }

                return _mf_local.MapDescription;

            }
        }

        public ESRI.ArcGIS.Server.IServerContext _sc
        {
            get
            {
                if (_server_context == null)
                {
                    //We need to set the server context before we can return it.
                    _server_context = _mr_local.ServerContextInfo.ServerContext;
                }

                return _server_context;
            }
        }



        #endregion 

        /// <summary>
        /// This retrieves the layers list from the map.
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable getLayersList()
        {
            //Create a datatable to store our results.
            System.Data.DataTable dt = new System.Data.DataTable();

            //Set up the datatable to hold
            dt.Columns.Add("layername");
            dt.Columns.Add("layerindex");

            //
            for (int i = 0; i < _mapDescription.LayerDescriptions.Length; i++)
            {
                //For each layer, we need to get its layer name and layer index.
                int li = _mapDescription.LayerDescriptions[i].LayerID;
                string ln = translateLayerIndex(li);

                if (ln != string.Empty && ln != null)
                {
                    System.Data.DataRow dr = dt.NewRow();
                    dr["layerindex"] = li;
                    dr["layername"] = ln;

                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }

        /// <summary>
        /// This method retrieves the name of the underlying dataset 
        /// </summary>
        /// <param name="layerindex"></param>
        /// <returns></returns>
        public string getLayerDatasetName(int layerindex)
        {
            
            return string.Empty;
        }
    }


}
