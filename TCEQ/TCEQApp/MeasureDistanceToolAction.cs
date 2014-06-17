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


using BEGWebAppLib;

/// <summary>
/// Summary description for MeasureDistanceToolAction
/// </summary>
public class MeasureDistanceToolAction : IMapServerToolAction
{



    #region IMapServerToolAction Members

    public void ServerAction(ESRI.ArcGIS.ADF.Web.UI.WebControls.ToolEventArgs args)
    {
        //Convert to line event args.
        LineEventArgs lea;
        try
        {
            lea = (LineEventArgs)args;
        }
        catch
        {
            return;
        }

        //Get the map control from the page.
        Control c = args.Control.Page.FindControl("Map1");
        Map m = (Map)c;

        //GIS Handler.
        GISHandler g = new GISHandler(m.MapResourceManagerInstance,0);
        
        ESRI.ArcGIS.Geometry.IPoint ig_beg = g.translateScreenCoordsAO(lea.BeginPoint, m);
        ESRI.ArcGIS.Geometry.IPoint ig_fin = g.translateScreenCoordsAO(lea.EndPoint, m);

        //Ok, now we have our two points, so we need to create a line from them and determine its length.

        ESRI.ArcGIS.Geometry.IPoint[] ig_points = new ESRI.ArcGIS.Geometry.IPoint[2];
        ig_points[0] = ig_beg;
        ig_points[1] = ig_fin;

        ESRI.ArcGIS.Geometry.IPolyline ipl = g.makePolyLine(ig_points);

        //Ok, Now we have the polyline object.  We need to convert its units from the map's, which are in TSMS, to something meaningful to the user, such as feet, meters, etc.

        ESRI.ArcGIS.esriSystem.esriUnits eu = getUnits(m.Page);

        double finalmeasurement = g.convertUnits(ipl.Length, constants.MAP_UNITS, eu);

        Decimal d = new Decimal(finalmeasurement);

        //Finally, an easy part.

        //Create a control to house our results
        Table t = new Table();
        t.CssClass = "resulttable";
        
        TableRow trh = new TableRow();
        TableHeaderCell thc = new TableHeaderCell();
        TableHeaderCell clearCell = new TableHeaderCell();
        thc.Text = "Measurement Results";
        thc.ColumnSpan = 2;
        trh.Cells.Add(thc);
        HyperLink hl = new HyperLink();
        hl.NavigateUrl = "javascript:clearMeasurement();";

        TableRow measuredDistanceRow = new TableRow();
        TableCell measuredDistanceCell = new TableCell();
        measuredDistanceCell.ColumnSpan = 2;
        measuredDistanceCell.Text = "Measured Distance: " + Decimal.Round(d, 3).ToString() + " " + getUnitLabel(eu);
        measuredDistanceRow.Cells.Add(measuredDistanceCell);

        TableRow originXRow = new TableRow();
        TableCell originXLabelCell = new TableCell();
        TableCell originXValueCell = new TableCell();
        originXLabelCell.Text = "Origin X";
        originXValueCell.Text = ig_beg.X.ToString();
        originXRow.Cells.Add(originXLabelCell);
        originXRow.Cells.Add(originXValueCell);

        TableRow originYRow = new TableRow();
        TableCell originYLabelCell = new TableCell();
        TableCell originYValueCell = new TableCell();
        originYLabelCell.Text = "Origin Y";
        originYValueCell.Text = ig_beg.Y.ToString();
        originYRow.Cells.Add(originYLabelCell);
        originYRow.Cells.Add(originYValueCell);

        TableRow destXRow = new TableRow();
        TableCell destXLabelCell = new TableCell();
        TableCell destXValueCell = new TableCell();
        destXLabelCell.Text = "Destination X";
        destXValueCell.Text = ig_fin.X.ToString();
        destXRow.Cells.Add(destXLabelCell);
        destXRow.Cells.Add(destXValueCell);

        TableRow destYRow = new TableRow();
        TableCell destYLabelCell = new TableCell();
        TableCell destYValueCell = new TableCell();
        destYLabelCell.Text = "Destination Y";
        destYValueCell.Text = ig_fin.Y.ToString();
        destYRow.Cells.Add(destYLabelCell);
        destYRow.Cells.Add(destYValueCell);

        t.Rows.Add(trh);
        t.Rows.Add(measuredDistanceRow);
        t.Rows.Add(originXRow);
        t.Rows.Add(originYRow);
        t.Rows.Add(destXRow);
        t.Rows.Add(destYRow);

        HtmlGenericControl contentsdiv = new HtmlGenericControl("div");
        contentsdiv.Controls.Add(t);
        //This last section finishes up the div and generates the html to be sent to the browser (which is placed in a callback result)
        contentsdiv.Attributes.Add("class", "detailsdiv");
        contentsdiv.ID = "contentsdiv";

        //Now, we're ready to perform updating of the control.

        //Generate the callback result
        ESRI.ArcGIS.ADF.Web.UI.WebControls.CallbackResult cr = TCEQFuncs.updateControl("div", "contentsdiv", contentsdiv);

        //Add the callbackresult to the map object, which will pass it to the page.
        m.CallbackResults.Add(cr);
        //m.Refresh();
    }

    /// <summary>
    /// This is used to figure out what type of unit the user wants the measurements in.
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    ESRI.ArcGIS.esriSystem.esriUnits getUnits(Page p)
    {
        Control c = p.FindControl("unitselector");

        System.Web.SessionState.HttpSessionState session = p.Session;

        
        ESRI.ArcGIS.esriSystem.esriUnits eu;
        try
        {
            eu = (ESRI.ArcGIS.esriSystem.esriUnits) session["selectedunits"];
        }
        catch
        {
            return ESRI.ArcGIS.esriSystem.esriUnits.esriMiles;
        }

        return eu;
    }

    string getUnitLabel(ESRI.ArcGIS.esriSystem.esriUnits eu)
    {
        switch (eu)
        {
            case ESRI.ArcGIS.esriSystem.esriUnits.esriMiles: return "Miles";
                break;
            case ESRI.ArcGIS.esriSystem.esriUnits.esriKilometers: return "Kilometers";
                break;
            case ESRI.ArcGIS.esriSystem.esriUnits.esriFeet: return "Feet";
                break;
            case ESRI.ArcGIS.esriSystem.esriUnits.esriMeters: return "Meters";
                break;
        }

        return string.Empty;
    }

    #endregion
}
