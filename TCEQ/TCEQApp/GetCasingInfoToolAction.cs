using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ESRI.ArcGIS.ADF.Web.UI.WebControls;
using ESRI.ArcGIS.ADF.Web.UI.WebControls.Tools;
using ESRI.ArcGIS.ADF.Web.Geometry;

using BEGWebAppLib;

/// <summary>
/// Summary description for GetCasingInfoToolAction
/// </summary>
public class GetCasingInfoToolAction : IMapServerToolAction
{
    #region IMapServerToolAction Members

    public void ServerAction(ToolEventArgs args)
    {
        //When the user uses this tool, they are trying to look up a particular well's data and display its log.
        PointEventArgs pargs;
        try
        {
            pargs = (PointEventArgs)args;
        }
        catch
        {
            //We should probably provide some sort of error handling here.  For now, we'll just abort.
            return;
        }

        //Grab the point from the set of args.
        System.Drawing.Point sp = pargs.ScreenPoint;

        //next, we need the map that sent the event.
        Map m = (Map)args.Control;

        //Create a GISHandler.
        GISHandler g = new GISHandler(m.MapResourceManagerInstance, 0);
        g.mapctl = m;

        //We now have most of the data necessary to begin.  The first thing we need to do is get the actual coords of the point the user clicked.
        ESRI.ArcGIS.Geometry.IPoint ip = g.translateScreenCoordsAO(sp, m);
        ESRI.ArcGIS.ADF.Web.Geometry.Point pt = g.translateScreenCoords(sp, m);

        //Now, we need to hit the GeoDB again, to find out what logic we need to use for that point.
        DataTable dt = g.performBaseFeatureDataQuery("", (ESRI.ArcGIS.Geometry.IGeometry)ip, ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelWithin, "sde.SDE.TCEQ_Logic");

        //Label the point the user clicked.
        ESRI.ArcGIS.ADF.ArcGISServer.PointN pn = (ESRI.ArcGIS.ADF.ArcGISServer.PointN)ESRI.ArcGIS.ADF.ArcGISServer.Local.Converter.ComObjectToValueObject(ip, g._sc, typeof(ESRI.ArcGIS.ADF.ArcGISServer.PointN));
        string label = string.Empty;
        ESRI.ArcGIS.Geometry.IGeometry ig_projected = g.unprojectGeometry((ESRI.ArcGIS.Geometry.IGeometry)ip, constants.MAP_COORDINATE_SYSTEM_ID, constants.GCS_COORDINATE_SYSTEM_ID);
        ESRI.ArcGIS.Geometry.IPoint ip_projected = (ESRI.ArcGIS.Geometry.IPoint)ig_projected;
        label = "Casing Query\r\nLat: " + Math.Round(ip_projected.Y, 6).ToString() + "\r\nLong: " + Math.Round(ip_projected.X, 6).ToString();

        //Here, we mark the spot on the map where the user clicked.
        g.clearGraphics();
        g.addPoint(pt, System.Drawing.Color.HotPink, ESRI.ArcGIS.ADF.Web.Display.Symbol.MarkerSymbolType.Cross, 19, label);
        //g.addPoint(ip, 255, 128, 128, ESRI.ArcGIS.ADF.ArcGISServer.esriSimpleMarkerStyle.esriSMSCross);
        //g.addText(label, (ESRI.ArcGIS.ADF.ArcGISServer.Geometry)pn, 0, 0, 0, 8);


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
            contentsdiv = (HtmlGenericControl)args.Control.Page.FindControl("contentsdiv");

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
            trtc.Text = "There are no data available for the location you have selected.  Please select a different location within one of the counties shown to contain data, or contact TCEQ for further information.";
            trt.Cells.Add(trtc);
            t.Rows.Add(trt);

            //Add the table to our div.
            contentsdiv.Controls.Add(t);

        }
        

        contentsdiv.Attributes.Add("class", "detailsdiv");
        contentsdiv.ID = "contentsdiv";

        //Now, we're ready to perform updating of the control.

        //Generate the callback result
        ESRI.ArcGIS.ADF.Web.UI.WebControls.CallbackResult cr = TCEQFuncs.updateControl("div", "contentsdiv", contentsdiv);

        //Add the callbackresult to the map object, which will pass it to the page.
        m.CallbackResults.Add(cr);
        //m.Refresh();
    }

    #endregion
}
