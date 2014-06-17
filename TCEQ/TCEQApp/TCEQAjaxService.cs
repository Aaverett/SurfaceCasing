using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for TCEQAjaxService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class TCEQAjaxService : System.Web.Services.WebService {

    public const string MRI_DEFINITION_STRING = "<Definition DataSourceDefinition=\"igor\" DataSourceType=\"ArcGIS Server Local\" Identity=\"To set, right-click project and 'Add ArcGIS Identity'\" ResourceDefinition=\"Layers@tceq\" DataSourceShared=\"True\" />";
    public const string LOGIC_LAYER_NAME = "sde.SDE.TCEQ_Logic";
    public const string QWELLS_LAYER_NAME = "sde.SDE.TCEQ_QWells";

    public TCEQAjaxService () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    protected GISHandler getGISHandler()
    {
        ESRI.ArcGIS.ADF.Web.UI.WebControls.MapResourceManager mrm = new ESRI.ArcGIS.ADF.Web.UI.WebControls.MapResourceManager();

        ESRI.ArcGIS.ADF.Web.UI.WebControls.MapResourceItem mri = new ESRI.ArcGIS.ADF.Web.UI.WebControls.MapResourceItem();
        mri.Definition = new ESRI.ArcGIS.ADF.Web.UI.WebControls.GISResourceItemDefinition(MRI_DEFINITION_STRING);
        mrm.ResourceItems.Add(mri);

        //Force initialization of the resource items.
        mrm.Initialize();

        //GISHandler g = new GISHandler("TCEQ", "igor", "TCEQ Surface Casing Estimator");

        GISHandler g = new GISHandler(mrm, 0);

        return g;
    }

    [WebMethod]
    public AJAXCasingRequestResponse casingQuery(double lon, double lat)
    {
        GISHandler g = getGISHandler();

        ESRI.ArcGIS.ADF.Web.Geometry.Point pt = new ESRI.ArcGIS.ADF.Web.Geometry.Point(lon, lat);

        ESRI.ArcGIS.ADF.ArcGISServer.Point padf = (ESRI.ArcGIS.ADF.ArcGISServer.Point) g.translateWebToADF(pt);

        ESRI.ArcGIS.Geometry.IPoint ip = (ESRI.ArcGIS.Geometry.IPoint) g.translateADFToAO(padf);

        //Now, we need to hit the GeoDB again, to find out what logic we need to use for that point.
        ESRI.ArcGIS.Geometry.IGeometry ig_projected_1 = g.project((ESRI.ArcGIS.Geometry.IGeometry)ip, constants.GCS_COORDINATE_SYSTEM_ID, 3081);
        ESRI.ArcGIS.Geometry.IPoint ip1 = (ESRI.ArcGIS.Geometry.IPoint)ig_projected_1;
        
        DataTable dt = g.performBaseFeatureDataQuery("", ig_projected_1, ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelWithin, LOGIC_LAYER_NAME);

        //Label the point the user clicked.
        ESRI.ArcGIS.ADF.ArcGISServer.PointN pn = (ESRI.ArcGIS.ADF.ArcGISServer.PointN)ESRI.ArcGIS.ADF.ArcGISServer.Local.Converter.ComObjectToValueObject(ip, g._sc, typeof(ESRI.ArcGIS.ADF.ArcGISServer.PointN));
        
        string label = string.Empty;

        //* * * * * * * * * * * * * * * * * * * * * * * * *
        //It should be noted that ip is actually NOT projected here.  This code is lifted from the original version of this application, where the map control on the 
        //page would send coords in TSMS, which had to be converted back to GCS for processing.  Now that we're using google maps, that's no longer the case,
        //and we need to convert the GCS values to TSMS for querying the business logic layer, and then just pass the GCS values into the rest of the logic.
        //* * * * * * * * * * * * * * * * * * * * * * * * *
        ESRI.ArcGIS.Geometry.IPoint ip_projected = ip;
        label = "Casing Query\r\nLat: " + Math.Round(ip_projected.Y, 6).ToString() + "\r\nLong: " + Math.Round(ip_projected.X, 6).ToString();

        //Create a control to house our results
        HtmlGenericControl contentsdiv = null;

        int rowscount = 0;
        if (dt.Rows.Count > 0)
        {
            //We had at least one row, so we'll go ahead and begin processing.
            //Get the class factory to create our handler objects.
            LogicSetFactory lsf = new LogicSetFactory();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //Make sure we didn't get a null value.
                if (dt.Rows[i]["ProcessorClass"] != null && !(dt.Rows[i]["ProcessorClass"] is System.DBNull))
                {
                    //Have the logicsetfactory create a logicset for us.
                    ILogicSet ils = lsf.CreateLogicSet(dt.Rows[i]["ProcessorClass"]);

                    if (ils != null)
                    {
                        //Pass the objecthandler to the logic set.
                        ils.g = g;

                        //Pass the coords to the logic set
                        ils.coords = ip;

                        contentsdiv = ils.GenerateOutputControl();


                        //Increment the row count.
                        rowscount++;
                    }
                }
            }
        }

        //If we didn't do any rows...
        if (rowscount == 0)
        {
            //Create a div to house our table.
            contentsdiv = new HtmlGenericControl("div");

            //No logic zone. :(
            Table t = new Table();
            t.CssClass = "resulttable";
            TableRow trh = new TableRow();
            TableRow trt = new TableRow();

            TableHeaderCell trhc = new TableHeaderCell();
            trhc.Text = "No Data Available";
            trh.Cells.Add(trhc);
            t.Rows.Add(trh);

            TableCell trtc = new TableCell();
            trtc.Text = "There are no data available for the location you have selected.  Please select a different location within one of the counties shown to contain data, or contact RRC for further information.";
            trt.Cells.Add(trtc);
            t.Rows.Add(trt);

            //Add the table to our div.
            contentsdiv.Controls.Add(t);

        }


        contentsdiv.Attributes.Add("class", "detailsdiv");
        contentsdiv.ID = "contentsdiv";

        //Now, we're ready to perform updating of the control.
        System.IO.StringWriter sw = new System.IO.StringWriter();

        System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);

        contentsdiv.RenderControl(htw);

        string html = sw.ToString();

        AJAXCasingRequestResponse acrr = new AJAXCasingRequestResponse();

        acrr.detailsHTML = html;
        acrr.latitude = lat;
        acrr.longitude = lon;
        return acrr;
    }

    [WebMethod]
    public AJAXCasingRequestResponse logQuery(double lon, double lat)
    {
        GISHandler g = getGISHandler();

        ESRI.ArcGIS.ADF.Web.Geometry.Point pt = new ESRI.ArcGIS.ADF.Web.Geometry.Point(lon, lat);
        ESRI.ArcGIS.ADF.ArcGISServer.Point padf = (ESRI.ArcGIS.ADF.ArcGISServer.Point)g.translateWebToADF(pt);

        ESRI.ArcGIS.Geometry.IPoint ip = (ESRI.ArcGIS.Geometry.IPoint)g.translateADFToAO(padf);

        //Now, we need to hit the GeoDB again, to find out what logic we need to use for that point.
        ESRI.ArcGIS.Geometry.IGeometry ig_projected_1 = g.project((ESRI.ArcGIS.Geometry.IGeometry)ip, constants.GCS_COORDINATE_SYSTEM_ID, 3081);
        ESRI.ArcGIS.Geometry.IPoint ip1 = (ESRI.ArcGIS.Geometry.IPoint)ig_projected_1;

        ESRI.ArcGIS.Geometry.IPolygon poly = g.bufferPoint(ip1, 400);
        //ESRI.ArcGIS.Geometry.IPolygon poly = g.pointFudgeAO(sp, m, constants.MAP_CLICK_FUDGE_DISTANCE, 3081, 3081);

        DataTable dt = g.performBaseFeatureDataQuery("", poly, ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelContains, QWELLS_LAYER_NAME);

        HtmlGenericControl contentsdiv = new HtmlGenericControl("div");

        double lat1 = 0;
        double lon1 = 0;

        ESRI.ArcGIS.Geometry.IPoint ip_marker = null;

        string html;

        if (dt != null) //Make sure that there was actual data.
        {
          

            contentsdiv.Visible = true;

            contentsdiv.Attributes.Add("class", "detailsdiv");

            //Assemble the heading row for the table.
            Table t = new Table();
            t.CssClass = "resulttable";
            TableRow tr_h = new TableRow();
            TableHeaderCell thc1 = new TableHeaderCell();
            thc1.Text = "Well Name";
            thc1.Width = 800;
            TableHeaderCell thc2 = new TableHeaderCell();
            thc2.Text = "View Log";
            thc2.Width = 195;
            tr_h.Controls.Add(thc1);
            tr_h.Controls.Add(thc2);
            t.Rows.Add(tr_h);

            //for(int i=0; i<dt.Rows.Count; i++)
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                object s = dt.Rows[i]["SHAPE"];

                if (s is ESRI.ArcGIS.Geometry.IPoint)
                {
                    ip_marker = (ESRI.ArcGIS.Geometry.IPoint)dt.Rows[i]["SHAPE"];
                }

                //Assemble a set of controls describing each record (supposed to be wells)
                TableRow tr = new TableRow();
                TableCell tc1 = new TableCell();
                tc1.Text = (string)dt.Rows[i]["QNUM"];

                TableCell tc2 = new TableCell();
                tc2.CssClass = "linkcell";

                bool kosher = true;
                
                if(!(dt.Rows[i]["IMAGE_NAME"] is string)) kosher = false;

                string filepath = string.Empty;
                if (kosher) filepath = TCEQFuncs.LocalLogImagePath + "\\" + ((string)dt.Rows[i]["IMAGE_NAME"]);
                else kosher = false;

                if(kosher && !System.IO.File.Exists(filepath)) kosher = false;


                if (kosher)
                {
                    HyperLink hl = new HyperLink();
                    hl.Text = "View Log";
                    hl.NavigateUrl = "javascript:openLogViewer('" + HttpUtility.UrlEncode((string)dt.Rows[i]["IMAGE_NAME"]) + "');";
                    tc2.Controls.Add(hl);
                }
                else
                {
                    Label l = new Label();
                    l.Text = "Log Image Unavailable";
                    tc2.Controls.Add(l);

                    if(!System.IO.File.Exists(TCEQFuncs.LocalLogImagePath + "/" + ((string)dt.Rows[i]["IMAGE_NAME"])))
                    {
                        //TCEQFuncs.NotifyAdminMissingLog(((string)dt.Rows[i]["IMAGE_NAME"]));
                    }
                }

                //Add the cells to the row.
                tr.Controls.Add(tc1);
                tr.Controls.Add(tc2);

                //Add the row to the table
                t.Rows.Add(tr);
            }

            contentsdiv.Controls.Add(t);

            contentsdiv.Attributes.Add("class", "detailsdiv");
        }

        //Now, we're ready to perform updating of the control.
        System.IO.StringWriter sw = new System.IO.StringWriter();

        System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);

        contentsdiv.RenderControl(htw);
        if (dt.Rows.Count > 0)
        {
            html = sw.ToString();
        }
        else
        {
            html = "";
        }

        AJAXCasingRequestResponse acrr = new AJAXCasingRequestResponse();

        if (ip_marker != null)
        {
            ESRI.ArcGIS.Geometry.IPoint ip_marker_gcs = (ESRI.ArcGIS.Geometry.IPoint) g.unprojectGeometry((ESRI.ArcGIS.Geometry.IGeometry)ip_marker, 3081, constants.GCS_COORDINATE_SYSTEM_ID);

            lat1 = ip_marker_gcs.Y;
            lon1 = ip_marker_gcs.X;
        }

        acrr.detailsHTML = html;
        acrr.latitude = lat1;
        acrr.longitude = lon1;
        return acrr;
    
    }

    [WebMethod]
    public AJAXCasingRequestResponse surveyInfoQuery(double lon, double lat)
    {
        GISHandler g = getGISHandler();

        ESRI.ArcGIS.ADF.Web.Geometry.Point pt = new ESRI.ArcGIS.ADF.Web.Geometry.Point(lon, lat);
        ESRI.ArcGIS.ADF.ArcGISServer.Point padf = (ESRI.ArcGIS.ADF.ArcGISServer.Point)g.translateWebToADF(pt);

        ESRI.ArcGIS.Geometry.IPoint ip1 = (ESRI.ArcGIS.Geometry.IPoint)g.translateADFToAO(padf);

        //Project to the TSMS coord system that the base data use.
        ESRI.ArcGIS.Geometry.IGeometry ig_projected_1 = g.project((ESRI.ArcGIS.Geometry.IGeometry)ip1, constants.GCS_COORDINATE_SYSTEM_ID, 3081);
        ESRI.ArcGIS.Geometry.IPoint point = (ESRI.ArcGIS.Geometry.IPoint)ig_projected_1;

        //This is the layer ID of the surveys layer.
        int layerID = 7;

        string datasetname = TCEQFuncs.getDataSetName(layerID);

        DataTable dt = g.performBaseFeatureDataQuery("", point, ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelIntersects, datasetname);

        //We need to set those features as selected.

        HtmlGenericControl contentsdiv = new HtmlGenericControl("div");
        contentsdiv.ID = "contentsdiv";

        //This block marks the spot on the map.
        string label = string.Empty;
        ESRI.ArcGIS.Geometry.IPoint ip = point;
        ESRI.ArcGIS.Geometry.IGeometry ig_projected = g.unprojectGeometry((ESRI.ArcGIS.Geometry.IGeometry)ip, 102603, 4019);
        ESRI.ArcGIS.Geometry.IPoint ip_projected = (ESRI.ArcGIS.Geometry.IPoint)ig_projected;
        label = "Lat: " + Math.Round(ip_projected.Y, 6).ToString() + "\r\nLong: " + Math.Round(ip_projected.X, 6).ToString();


        /*g.addPoint(ip, 0, 255, 128, ESRI.ArcGIS.ADF.ArcGISServer.esriSimpleMarkerStyle.esriSMSDiamond);
        ESRI.ArcGIS.ADF.ArcGISServer.PointN pn = (ESRI.ArcGIS.ADF.ArcGISServer.PointN)ESRI.ArcGIS.ADF.ArcGISServer.Converter.ComObjectToValueObject(ip, g._sc, typeof(ESRI.ArcGIS.ADF.ArcGISServer.PointN));
        g.addText(label, (ESRI.ArcGIS.ADF.ArcGISServer.Geometry)pn, 0, 0, 0, 8);*/

        if (dt.Rows.Count > 0)
        {

            Table ht = new Table();
            ht.CssClass = "resulttable";
            contentsdiv.Controls.Add(ht);

            TableRow tr_h = new TableRow();
            TableHeaderCell thc_anum9 = new TableHeaderCell();
            TableHeaderCell thc_l1surnam = new TableHeaderCell();
            TableHeaderCell thc_l2block = new TableHeaderCell();
            TableHeaderCell thc_l3surnum = new TableHeaderCell();
            TableHeaderCell thc_l4surnam = new TableHeaderCell();

            thc_anum9.Text = "Abstract Number";
            thc_l1surnam.Text = "Survey Name";
            thc_l2block.Text = "Block/League/Porcion";
            thc_l3surnum.Text = "Section/Labor/Share";
            thc_l4surnam.Text = "Grantee";

            tr_h.Cells.Add(thc_anum9);
            tr_h.Cells.Add(thc_l1surnam);
            tr_h.Cells.Add(thc_l2block);
            tr_h.Cells.Add(thc_l3surnum);
            tr_h.Cells.Add(thc_l4surnam);

            ht.Rows.Add(tr_h);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                TableRow tr = new TableRow();
                TableCell tc_anum9 = new TableCell();
                TableCell tc_l1surnam = new TableCell();
                TableCell tc_l2block = new TableCell();
                TableCell tc_l3surnum = new TableCell();
                TableCell tc_l4surnam = new TableCell();

                if (!(dt.Rows[i]["ANUM9"] is System.DBNull)) tc_anum9.Text = dt.Rows[i]["ANUM9"].ToString();
                if (!(dt.Rows[i]["L1SURNAM"] is System.DBNull)) tc_l1surnam.Text = dt.Rows[i]["L1SURNAM"].ToString();
                if (!(dt.Rows[i]["L2BLOCK"] is System.DBNull)) tc_l2block.Text = dt.Rows[i]["L2BLOCK"].ToString();
                if (!(dt.Rows[i]["L3SURNUM"] is System.DBNull)) tc_l3surnum.Text = dt.Rows[i]["L3SURNUM"].ToString();
                if (!(dt.Rows[i]["L4SURNAM"] is System.DBNull)) tc_l4surnam.Text = dt.Rows[i]["L4SURNAM"].ToString();

                tr.Cells.Add(tc_anum9);
                tr.Cells.Add(tc_l1surnam);
                tr.Cells.Add(tc_l2block);
                tr.Cells.Add(tc_l3surnum);
                tr.Cells.Add(tc_l4surnam);

                ht.Rows.Add(tr);
            }
        }

        string html = null;

        //Now, we're ready to perform updating of the control.
        System.IO.StringWriter sw = new System.IO.StringWriter();

        System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);

        contentsdiv.RenderControl(htw);

        if (dt.Rows.Count > 0)
        {
            html = sw.ToString();
        }
        else
        {
            html = "";
        }

        AJAXCasingRequestResponse acrr = new AJAXCasingRequestResponse();

        acrr.detailsHTML = html;
        acrr.latitude = lat;
        acrr.longitude = lon;

        return acrr;
    }

    [WebMethod]
    public AJAXCasingRequestResponse identifyQuery(double lon, double lat, int layerID)
    {
        string datasetname = TCEQFuncs.getDataSetName(layerID);

        GISHandler g = getGISHandler();

        ESRI.ArcGIS.ADF.Web.Geometry.Point pt = new ESRI.ArcGIS.ADF.Web.Geometry.Point(lon, lat);
        ESRI.ArcGIS.ADF.ArcGISServer.Point padf = (ESRI.ArcGIS.ADF.ArcGISServer.Point)g.translateWebToADF(pt);

        ESRI.ArcGIS.Geometry.IPoint ip1 = (ESRI.ArcGIS.Geometry.IPoint)g.translateADFToAO(padf);

        //Project to the TSMS coord system that the base data use.
        ESRI.ArcGIS.Geometry.IGeometry ig_projected_1 = g.project((ESRI.ArcGIS.Geometry.IGeometry)ip1, constants.GCS_COORDINATE_SYSTEM_ID, 3081);
        ESRI.ArcGIS.Geometry.IPoint point = (ESRI.ArcGIS.Geometry.IPoint)ig_projected_1;

        ESRI.ArcGIS.Geometry.IPolygon poly = g.bufferPoint(point, 50);

        DataTable dt = g.performBaseFeatureDataQuery("", poly, ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelIntersects, datasetname);

        HtmlGenericControl contentsdiv = new HtmlGenericControl("div");
        contentsdiv.ID = "contentsdiv";

        if (dt.Rows.Count > 0)
        {
            //contentsdiv.Controls.Add(dg);
            Table t = new Table();

            TableRow trh = new TableRow();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                TableHeaderCell thc = new TableHeaderCell();

                thc.Text = dt.Columns[i].ColumnName;

                trh.Cells.Add(thc);
            }

            t.Rows.Add(trh);

            for (int i = 0; i < dt.Rows.Count; i++)
            {

                TableRow tr = new TableRow();

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    TableCell tc = new TableCell();

                    tc.Text = dt.Rows[i][j].ToString();
                    tr.Cells.Add(tc);
                }

                t.Rows.Add(tr);
            }

            contentsdiv.Controls.Add(t);

        }
        else
        {
            HtmlGenericControl z = new HtmlGenericControl("span");
            z.InnerText = "There are no results to display.";

            contentsdiv.Controls.Add(z);
        }

        contentsdiv.Attributes.Add("class", "detailsdiv");
        contentsdiv.Attributes.Add("style", "overflow:scroll;");

        string html = null;

        //Now, we're ready to perform updating of the control.
        System.IO.StringWriter sw = new System.IO.StringWriter();

        System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);

        contentsdiv.RenderControl(htw);

        html = sw.ToString();

        AJAXCasingRequestResponse acrr = new AJAXCasingRequestResponse();

        acrr.latitude = lat;
        acrr.longitude = lon;
        acrr.detailsHTML = html;

        return acrr;
    }

    [WebMethod]
    public GetCountiesAJAXResponse getCountyEnvelope(int countyID)
    {
        GISHandler g = getGISHandler();

        //Here, we're going to deal with the little county selector dealie.
        DataTable dt_counties = g.performBaseFeatureDataQuery("OBJECTID=" + countyID, null, ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelUndefined, "sde.SDE.TCEQ_Counties");

        GetCountiesAJAXResponse gcar = new GetCountiesAJAXResponse();

        CountyPropertiesAjaxResponse[] counties = new CountyPropertiesAjaxResponse[1];

        if (dt_counties.Rows.Count > 0)
        {
            CountyPropertiesAjaxResponse county = new CountyPropertiesAjaxResponse();

            county.countyName = dt_counties.Rows[0]["NAME"].ToString();

            ESRI.ArcGIS.Geometry.IGeometry geom = (ESRI.ArcGIS.Geometry.IGeometry)dt_counties.Rows[0]["SHAPE"];

            ESRI.ArcGIS.ADF.Web.Geometry.Polygon pgw = (ESRI.ArcGIS.ADF.Web.Geometry.Polygon)g.convertGeometryTypeWeb(geom);

            ESRI.ArcGIS.ADF.Web.Geometry.Envelope ie = g.getEnvelope(pgw);

            ESRI.ArcGIS.ADF.Web.Geometry.Point pne = new ESRI.ArcGIS.ADF.Web.Geometry.Point(ie.XMax, ie.YMax);

            ESRI.ArcGIS.ADF.Web.Geometry.Point psw = new ESRI.ArcGIS.ADF.Web.Geometry.Point(ie.XMin, ie.YMin);

            ESRI.ArcGIS.ADF.ArcGISServer.Geometry adfne = g.translateWebToADF((ESRI.ArcGIS.ADF.Web.Geometry.Geometry)pne);
            ESRI.ArcGIS.ADF.ArcGISServer.Geometry adfsw = g.translateWebToADF((ESRI.ArcGIS.ADF.Web.Geometry.Geometry)psw);

            ESRI.ArcGIS.Geometry.IPoint ipne = (ESRI.ArcGIS.Geometry.IPoint)g.translateADFToAO(adfne);
            ESRI.ArcGIS.Geometry.IPoint ipsw = (ESRI.ArcGIS.Geometry.IPoint)g.translateADFToAO(adfsw);

            ESRI.ArcGIS.Geometry.IPoint ipne_uproj = (ESRI.ArcGIS.Geometry.IPoint)g.unprojectGeometry((ESRI.ArcGIS.Geometry.IGeometry)ipne, 3081, constants.GCS_COORDINATE_SYSTEM_ID);
            ESRI.ArcGIS.Geometry.IPoint ipsw_uproj = (ESRI.ArcGIS.Geometry.IPoint)g.unprojectGeometry((ESRI.ArcGIS.Geometry.IGeometry)ipsw, 3081, constants.GCS_COORDINATE_SYSTEM_ID);

            county.xMax = ipne_uproj.X;
            county.yMax = ipne_uproj.Y;
            county.xMin = ipsw_uproj.X;
            county.yMin = ipsw_uproj.Y;

            counties[0] = county;
            
        }

        gcar.counties = counties;

        return gcar;
    }

    [WebMethod]
    public GetCountiesAJAXResponse getCounties()
    {
        GISHandler g = getGISHandler();

        //Here, we're going to deal with the little county selector dealie.
        DataTable dt_counties = g.performBaseFeatureDataQuery("hasdata=1", null,ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelUndefined, "sde.SDE.TCEQ_Counties");
        
        GetCountiesAJAXResponse gcar = new GetCountiesAJAXResponse();
        
        CountyPropertiesAjaxResponse[] counties = new CountyPropertiesAjaxResponse[dt_counties.Rows.Count];

        for (int i = 0; i < dt_counties.Rows.Count; i++)
        {
            System.Data.DataRow dr = dt_counties.Rows[i];

            CountyPropertiesAjaxResponse county = new CountyPropertiesAjaxResponse();

            county.countyName = dr["NAME"].ToString();
            county.countyID = System.Convert.ToInt32(dr["OBJECTID"]);

            try
            {
                ESRI.ArcGIS.Geometry.IGeometry geom = (ESRI.ArcGIS.Geometry.IGeometry)dt_counties.Rows[i]["SHAPE"];

                ESRI.ArcGIS.ADF.Web.Geometry.Polygon pgw = (ESRI.ArcGIS.ADF.Web.Geometry.Polygon) g.convertGeometryTypeWeb(geom);

                ESRI.ArcGIS.ADF.Web.Geometry.Envelope ie = g.getEnvelope(pgw);

                ESRI.ArcGIS.ADF.Web.Geometry.Point pne = new ESRI.ArcGIS.ADF.Web.Geometry.Point(ie.XMax, ie.YMax);

                ESRI.ArcGIS.ADF.Web.Geometry.Point psw = new ESRI.ArcGIS.ADF.Web.Geometry.Point(ie.XMin, ie.YMin);

                ESRI.ArcGIS.ADF.ArcGISServer.Geometry adfne = g.translateWebToADF((ESRI.ArcGIS.ADF.Web.Geometry.Geometry)pne);
                ESRI.ArcGIS.ADF.ArcGISServer.Geometry adfsw = g.translateWebToADF((ESRI.ArcGIS.ADF.Web.Geometry.Geometry)psw);

                ESRI.ArcGIS.Geometry.IPoint ipne = (ESRI.ArcGIS.Geometry.IPoint) g.translateADFToAO(adfne);
                ESRI.ArcGIS.Geometry.IPoint ipsw = (ESRI.ArcGIS.Geometry.IPoint) g.translateADFToAO(adfsw);

                ESRI.ArcGIS.Geometry.IPoint ipne_uproj = (ESRI.ArcGIS.Geometry.IPoint) g.unprojectGeometry((ESRI.ArcGIS.Geometry.IGeometry)ipne, 3081, constants.GCS_COORDINATE_SYSTEM_ID);
                ESRI.ArcGIS.Geometry.IPoint ipsw_uproj = (ESRI.ArcGIS.Geometry.IPoint) g.unprojectGeometry((ESRI.ArcGIS.Geometry.IGeometry)ipsw, 3081, constants.GCS_COORDINATE_SYSTEM_ID);

                county.xMax = ipne_uproj.X;
                county.yMax = ipne_uproj.Y;
                county.xMin = ipsw_uproj.X;
                county.yMin = ipsw_uproj.Y;
            }
            catch
            {

                continue;
            }

            counties[i] = county;
        }



        if (counties.Length > 1)
        {
            //Now, sort the array.
            bool sorted = false;

            //Sort the counties array.
            while (!sorted)
            {
                sorted = true;

                for (int i = 1; i < counties.Length; i++)
                {
                    if (counties[i - 1].countyName.CompareTo(counties[i].countyName) > 0)
                    {
                        CountyPropertiesAjaxResponse tempCounty = counties[i];

                        counties[i] = counties[i - 1];

                        counties[i - 1] = tempCounty;

                        sorted = false;
                    }
                }
            }
        }

        gcar.counties = counties;

        
         return gcar;
    }
}

