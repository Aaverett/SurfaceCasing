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
/// Summary description for GetWellLogToolAction
/// </summary>
public class GetWellLogToolAction : IMapServerToolAction
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
        Map m = (Map) args.Control;

        //Create a GISHandler.
        //GISHandler g = new GISHandler(constants.SERVICE_NAME, constants.SERVER_NAME, constants.DATA_FRAME_NAME);
        GISHandler g = new GISHandler(m.MapResourceManagerInstance, 0);
        g.mapctl = m;

        //Use the point fudge method to buffer the screen coords of our click and return a corresponding polygon.
        ESRI.ArcGIS.Geometry.IPolygon poly = g.pointFudgeAO(sp, m, constants.MAP_CLICK_FUDGE_DISTANCE, 3081, 3081);

        DataTable dt = g.performBaseFeatureDataQuery("", poly, ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelContains, "sde.SDE.TCEQ_QWells");

        //This next section is used to label the point the user clicked on the map, and mark its latitude and longitude.
        ESRI.ArcGIS.Geometry.IPoint ip = g.translateScreenCoordsAO(sp,m);
        ESRI.ArcGIS.ADF.Web.Geometry.Point pnw = g.translateScreenCoords(sp, m);
        ESRI.ArcGIS.ADF.ArcGISServer.PointN pn = (ESRI.ArcGIS.ADF.ArcGISServer.PointN) ESRI.ArcGIS.ADF.ArcGISServer.Local.Converter.ComObjectToValueObject(ip, g._sc, typeof(ESRI.ArcGIS.ADF.ArcGISServer.PointN));
        string label = string.Empty;
        ESRI.ArcGIS.Geometry.IGeometry ig_projected = g.unprojectGeometry((ESRI.ArcGIS.Geometry.IGeometry)ip, 102603, 4019);
        ESRI.ArcGIS.Geometry.IPoint ip_projected = (ESRI.ArcGIS.Geometry.IPoint)ig_projected;
        label = "Log Image Query\r\nLat: " + Math.Round(ip_projected.Y,6).ToString() + "\r\nLong: " + Math.Round(ip_projected.X,6).ToString();

        //Here, we mark the spot on the map where the user clicked.
        g.clearGraphics();
        g.addPoint(pnw, System.Drawing.Color.Turquoise, ESRI.ArcGIS.ADF.Web.Display.Symbol.MarkerSymbolType.Triangle, 19, label);
        //g.addPoint(ip, 0, 255, 128, ESRI.ArcGIS.ADF.ArcGISServer.esriSimpleMarkerStyle.esriSMSDiamond);
        //g.addText(label, (ESRI.ArcGIS.ADF.ArcGISServer.Geometry)pn, 0, 0, 0, 8);

        if (dt != null) //Make sure that there was actual data.
        {
            HtmlGenericControl contentsdiv = (HtmlGenericControl)args.Control.Page.FindControl("contentsdiv");

            contentsdiv.Visible = true;

            contentsdiv.Attributes.Add("class", "detailsdiv");

            //If contentsdiv isn't found, abort the procedure.  Maybe provide error handling?
            if (contentsdiv == null) return;

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

            //Now, we're ready to perform updating of the control.

            //Generate the callback result
            ESRI.ArcGIS.ADF.Web.UI.WebControls.CallbackResult cr = TCEQFuncs.updateControl("div", "contentsdiv", contentsdiv);

            //Add the callbackresult to the map object, which will pass it to the page.
            m.CallbackResults.Add(cr);
        }
        m.Refresh();
    }

    #endregion
}
