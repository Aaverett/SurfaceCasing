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
/// Summary description for GetSurveyInfoToolAction
/// </summary>
public class GetSurveyInfoToolAction : IMapServerToolAction
{

    #region IMapServerToolAction Members

    public void ServerAction(ToolEventArgs args)
    {
        Map m = (Map)args.Control;
        Page p = m.Page;
        MapResourceManager mrm = m.MapResourceManagerInstance;

        GISHandler g = new GISHandler(mrm, 0);
        g.mapctl = m;

        PointEventArgs pea = (PointEventArgs)args;

        System.Drawing.Point pt = pea.ScreenPoint;

        pt.X = pt.X + 6;
        pt.Y = pt.Y - 8;

        //translate the point that the user clicked from screen coords to an ArcGIS polygon.
        //Use the point fudge method to buffer the screen coords of our click and return a corresponding polygon.
        ESRI.ArcGIS.Geometry.IPoint point = g.translateScreenCoordsAO(pt, m);

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
            //There's a lot of potential for crash and burn here, so we'll add this try/catch block.
            try
            {

                //Before we compose the output html, let's also set up a marker for the map.
                object o = dt.Rows[0]["SHAPE"];

                g.clearGraphics();
                //ESRI.ArcGIS.ADF.ArcGISServer.Polygon pgn1 = (ESRI.ArcGIS.ADF.ArcGISServer.Polygon)g.translateAOToADF((ESRI.ArcGIS.Geometry.IGeometry)o);
                //ESRI.ArcGIS.ADF.Web.Geometry.Polygon pgn =  (ESRI.ArcGIS.ADF.Web.Geometry.Polygon) ESRI.ArcGIS.ADF.ArcGISServer.Converter.ToAdfGeometry(pgn1);

                ESRI.ArcGIS.ADF.Web.Geometry.Polygon pgn = (ESRI.ArcGIS.ADF.Web.Geometry.Polygon) g.convertGeometryTypeWeb((ESRI.ArcGIS.Geometry.IGeometry)o);
                g.addPolygon(pgn,
                    System.Drawing.Color.GreenYellow,
                    System.Drawing.Color.Black,
                    ESRI.ArcGIS.ADF.Web.Display.Symbol.PolygonFillType.Solid,
                    1,
                    0.5,
                    label);
            }
            catch
            {

            }

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
        else
        {
            HtmlGenericControl z = new HtmlGenericControl("span");
            z.InnerText = "There are no results to display.";
            contentsdiv.Controls.Add(z);

            g.clearGraphics();
            ESRI.ArcGIS.ADF.ArcGISServer.Point pp1 = (ESRI.ArcGIS.ADF.ArcGISServer.Point) g.translateAOToADF(ip);
            ESRI.ArcGIS.ADF.Web.Geometry.Point pp = (ESRI.ArcGIS.ADF.Web.Geometry.Point)ESRI.ArcGIS.ADF.ArcGISServer.Converter.ToAdfGeometry(pp1);

            //pp = ESRI.ArcGIS.ADF.ArcGISServer.

            //ESRI.ArcGIS.ADF.Web.Geometry.Point pp = new ESRI.ArcGIS.ADF.Web.Geometry.Point(
            g.addPoint(pp, System.Drawing.Color.GreenYellow, ESRI.ArcGIS.ADF.Web.Display.Symbol.MarkerSymbolType.Cross, 19, label);
        }

        contentsdiv.Attributes.Add("class", "detailsdiv");

        //Generate the callback result
        ESRI.ArcGIS.ADF.Web.UI.WebControls.CallbackResult cr = TCEQFuncs.updateControl("div", "contentsdiv", contentsdiv);

        //Add the callbackresult to the map object, which will pass it to the page.
        m.CallbackResults.Add(cr);

        //Refresh the map
        m.Refresh();
    }

    #endregion
}
