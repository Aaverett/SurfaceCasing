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

/// <summary>
/// Summary description for IdentifyToolAction
/// </summary>
public class IdentifyToolAction : IMapServerToolAction
{

    #region IMapServerToolAction Members

    public void ServerAction(ToolEventArgs args)
    {
        //Basic stuff here - we need handles on the controls on the page.
        string layername;
        Map m = (Map)args.Control;
        Page p = m.Page;
        MapResourceManager mrm = m.MapResourceManagerInstance;

        GISHandler g = new GISHandler(mrm, 0);

        g.mapctl = (ESRI.ArcGIS.ADF.Web.UI.WebControls.Map) args.Control;

        PointEventArgs pea = (PointEventArgs)args;

        System.Drawing.Point pt = pea.ScreenPoint;

        //pt.X = pt.X + 6;
        //pt.Y = pt.Y - 8;

        //translate the point that the user clicked from screen coords to an ArcGIS polygon.
        //Use the point fudge method to buffer the screen coords of our click and return a corresponding polygon.
        ESRI.ArcGIS.Geometry.IPolygon poly = g.pointFudgeAO(pt, m, constants.MAP_CLICK_FUDGE_DISTANCE, 3081, 3081);

        //We need to retrieve the layer name before we hit the DB.

        int layerid = (int)p.Session["selectedlayer"];

        string datasetname = TCEQFuncs.getDataSetName(layerid);

        //Hit the GeoDB.  This will perform our query and return the results as a convenient datatable.
        DataTable dt = g.performBaseFeatureDataQuery("", poly, ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelIntersects, datasetname);

        /*ESRI.ArcGIS.ADF.Web.Geometry.Geometry geom_web = ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ToAdfGeometry(polyn);

        DataTable dt_layers = g.getLayersList();

        string layername1 = (string)dt_layers.Rows[layerid]["layername"];

        DataTable dt = g.performMapServiceQuery("", geom_web, layername1);*/

        //Mark the location the UserControl clicked.

        string label = string.Empty;
        ESRI.ArcGIS.Geometry.IPoint ip = poly.ToPoint;
        ESRI.ArcGIS.Geometry.IGeometry ig_projected = g.unprojectGeometry((ESRI.ArcGIS.Geometry.IGeometry)ip, 102603, 4019);
        ESRI.ArcGIS.Geometry.IPoint ip_projected = (ESRI.ArcGIS.Geometry.IPoint)ig_projected;
        label = "Lat: " + Math.Round(ip_projected.Y, 6).ToString() + "\r\nLong: " + Math.Round(ip_projected.X, 6).ToString();

        g.clearGraphics();
        g.addPoint(ip, 0, 255, 128, ESRI.ArcGIS.ADF.ArcGISServer.esriSimpleMarkerStyle.esriSMSDiamond);
        ESRI.ArcGIS.ADF.ArcGISServer.PointN pn = (ESRI.ArcGIS.ADF.ArcGISServer.PointN) ESRI.ArcGIS.ADF.ArcGISServer.Local.Converter.ComObjectToValueObject(ip, g._sc, typeof(ESRI.ArcGIS.ADF.ArcGISServer.PointN));
        g.addText(label, (ESRI.ArcGIS.ADF.ArcGISServer.Geometry)pn, 0, 0, 0, 8);

        //We need to set those features as selected.

        HtmlGenericControl contentsdiv = new HtmlGenericControl("div");
        contentsdiv.ID = "contentsdiv";

        if (dt.Rows.Count > 0)
        {
            //Now, we need to assemble a 
            //DataGrid dg = new DataGrid();

            //dg.DataSource = dt;
            //dg.DataBind();

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

        //Generate the callback result
        ESRI.ArcGIS.ADF.Web.UI.WebControls.CallbackResult cr = TCEQFuncs.updateControl("div", "contentsdiv", contentsdiv);

        //Add the callbackresult to the map object, which will pass it to the page.
        m.CallbackResults.Add(cr);

        //Refresh the map
        m.Refresh();
    }

    #endregion
}
