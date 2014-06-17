using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for Year5
/// </summary>
public class Year5 : BaseLogicSet, ILogicSet
{
    public override HtmlGenericControl GenerateOutputControl()
    {
        //Label strings - we go ahead and initialize these here because...  We have no obvious reason not to.
        string sTopMSLAbove = constants.MSL_LABEL_ABOVE;
        string sBotMSLAbove = constants.MSL_LABEL_ABOVE;
        string sPBUQWMSLAbove = constants.MSL_LABEL_ABOVE;
        string sUSDWMSLAbove = constants.MSL_LABEL_ABOVE;

        //Here, we do the above/below thing.
        if (topmsl < 0) sTopMSLAbove = constants.MSL_LABEL_BELOW;

        if (botmsl < 0) sBotMSLAbove = constants.MSL_LABEL_BELOW;

        if (_pbuqwmsl < 0) sPBUQWMSLAbove = constants.MSL_LABEL_BELOW;

        if (_usdwmsl < 0) sUSDWMSLAbove = constants.MSL_LABEL_BELOW;

        //This will be the control that gets returned.
        HtmlGenericControl resultsdiv = new HtmlGenericControl("div");
        resultsdiv.Attributes.Add("class", "detailsdiv");

        //Now we're ready to compose our results into a set of controls so that we can have ASP.NET display them.
        Table t = new Table();
        t.CssClass = "resulttable";
        TableRow headingrow = new TableRow();
        TableHeaderCell thc = new TableHeaderCell();
        thc.Text = constants.CASINGINFO_TABLE_HEADING;
        thc.ColumnSpan = 2;
        headingrow.Cells.Add(thc);
        t.Rows.Add(headingrow);

        TableRow longitudeRow = new TableRow();
        longitudeRow.ID = "longitudeRow";
        TableRow latitudeRow = new TableRow();
        latitudeRow.ID = "latitudeRow";
        TableRow aquiferRow = new TableRow();
        aquiferRow.ID = "aquiferRow";
        TableRow fwizTopRow = new TableRow();
        fwizTopRow.ID = "fwizTopRow";
        TableRow fwizBotRow = new TableRow();
        fwizBotRow.ID = "fwizBotRow";
        TableRow pbuqwRow = new TableRow();
        pbuqwRow.ID = "pbuqwRow";
        TableRow usdwRow = new TableRow();
        usdwRow.ID = "usdwRow";

        //The longitude row
        TableCell longitudeLabelCell = new TableCell();
        longitudeLabelCell.ID = "longitudeLabelCell";
        longitudeLabelCell.Text = "Longitude";
        TableCell longitudeValueCell = new TableCell();
        longitudeValueCell.ID = "longitudeValueCell";
        longitudeValueCell.Text = Math.Round(longitude, 6).ToString();
        longitudeRow.Cells.Add(longitudeLabelCell);
        longitudeRow.Cells.Add(longitudeValueCell);

        //The latitude row
        TableCell latitudeLabelCell = new TableCell();
        latitudeLabelCell.ID = "latitudeLabelCell";
        latitudeLabelCell.Text = "Latitude";
        TableCell latitudeValueCell = new TableCell();
        latitudeValueCell.ID = "latitudeLabelCell";
        latitudeValueCell.Text = Math.Round(latitude, 6).ToString();
        latitudeRow.Cells.Add(latitudeLabelCell);
        latitudeRow.Cells.Add(latitudeValueCell);

        //The aquifer row
        TableCell aquiferLabelCell = new TableCell();
        aquiferLabelCell.ID = "aquiferLabelCell";
        TableCell aquiferValueCell = new TableCell();
        aquiferValueCell.ID = "aquiferValueCell";
        aquiferLabelCell.Text = "Aquifer";
        aquiferValueCell.Text = aquifername;
        aquiferRow.Cells.Add(aquiferLabelCell);
        aquiferRow.Cells.Add(aquiferValueCell);

        //The Fresh water isolation zone top row
        TableCell fwizTopLabelCell = new TableCell();
        fwizTopLabelCell.ID = "fwizTopLabelCell";
        TableCell fwizTopValueCell = new TableCell();
        fwizTopValueCell.ID = "fwizTopValueCell";
        fwizTopLabelCell.Text = "Top of Fresh Water Isolation Zone";
        fwizTopValueCell.Text = Math.Round(Math.Abs(topgsfc), 0).ToString() + "' Depth (" + Math.Round(Math.Abs(topmsl), 0).ToString() + "' " + sTopMSLAbove + " MSL)";
        fwizTopRow.Cells.Add(fwizTopLabelCell);
        fwizTopRow.Cells.Add(fwizTopValueCell);

        //The Fresh water isolation zone bottom row
        TableCell fwizBotLabelCell = new TableCell();
        fwizBotLabelCell.ID = "fwizBotLabelCell";
        TableCell fwizBotValueCell = new TableCell();
        fwizBotValueCell.ID = "fwizBotValueCell";
        fwizBotLabelCell.Text = "Bottom of Fresh Water Isolation Zone";
        fwizBotValueCell.Text = "See base of usable quality water";
        fwizBotRow.Cells.Add(fwizBotLabelCell);
        fwizBotRow.Cells.Add(fwizBotValueCell);

        //Protected base of usable quality qater row
        TableCell pbuqwLabelCell = new TableCell();
        pbuqwLabelCell.ID = "pbuqwLabelCell";
        TableCell pbuqwValueCell = new TableCell();
        pbuqwValueCell.ID = "pbuqwValueCell";
        pbuqwLabelCell.Text = "Protected base of usable quality water";
        pbuqwValueCell.Text = Math.Round(Math.Abs(pbuqwgsfc), 0).ToString() + "' Depth (" + Math.Round(Math.Abs(pbuqwmsl), 0).ToString() + "' " + sPBUQWMSLAbove + " MSL)";
        pbuqwRow.Cells.Add(pbuqwLabelCell);
        pbuqwRow.Cells.Add(pbuqwValueCell);

        //USDW row
        TableCell usdwLabelCell = new TableCell();
        usdwLabelCell.ID = "usdwLabelCell";
        TableCell usdwValueCell = new TableCell();
        usdwValueCell.ID = "usdwValueCell";
        usdwLabelCell.Text = "Base of USDW";
        usdwValueCell.Text = Math.Round(Math.Abs(usdwgsfc), 0).ToString() + "' Depth (" + Math.Round(Math.Abs(usdwmsl), 0).ToString() + "' " + sUSDWMSLAbove + " MSL)";
        usdwRow.Cells.Add(usdwLabelCell);
        usdwRow.Cells.Add(usdwValueCell);

        //Add all the rows to the table.
        t.Rows.Add(longitudeRow);
        t.Rows.Add(latitudeRow);
        //t.Rows.Add(elevRow);
        t.Rows.Add(aquiferRow);
        t.Rows.Add(fwizTopRow);
        t.Rows.Add(fwizBotRow);
        t.Rows.Add(pbuqwRow);
        t.Rows.Add(usdwRow);

        resultsdiv.Controls.Add(t);

        //Finally, we need to compose the paragraph form thing.
        string fwizstring, pbuqwstring, usdwstring;

        //This is like option zero from the year 3 TCEQ app.
        //Here, we compose the output strings.
        //fwizstring = "The fresh water contained in the " + aquifername + " aquifer unit, which is estimated to occur from a depth of " + Decimal.Round(new Decimal(topgsfc), 0).ToString() + " feet (" + Decimal.Round(new Decimal(Math.Abs(topmsl)), 0).ToString() + " feet " + sTopMSLAbove + " MSL) to " + Decimal.Round(new Decimal(botgsfc), 0).ToString() + " feet (" + Decimal.Round(new Decimal(Math.Abs(botmsl)), 0).ToString() + " feet " + sBotMSLAbove + " MSL), must be isolated from the water above and below.";

        pbuqwstring = "The base of usable quality water is estimated to occur  at a depth of " + Decimal.Round(new Decimal(pbuqwgsfc), 0).ToString() + " feet (" + Decimal.Round(new Decimal(Math.Abs(pbuqwmsl)), 0).ToString() + " feet " + sPBUQWMSLAbove + " MSL).";

        usdwstring = "The USDW (10,000 ppm TDS) is estimated to occur at a depth of " + Decimal.Round(new Decimal(usdwgsfc), 0).ToString() + " feet (" + Decimal.Round(new Decimal(Math.Abs(usdwmsl)), 0).ToString() + "' " + sUSDWMSLAbove + " MSL).";

        //Now we need to create controls to house these.
        /*HtmlGenericControl fwizparagraph = new HtmlGenericControl("p");
        fwizparagraph.InnerText = fwizstring;*/

        HtmlGenericControl pbuqwparagraph = new HtmlGenericControl("p");
        pbuqwparagraph.InnerText = pbuqwstring;

        HtmlGenericControl usdwparagraph = new HtmlGenericControl("p");
        usdwparagraph.InnerText = usdwstring;

        HtmlGenericControl disclaimerparagraph = new HtmlGenericControl("p");
        disclaimerparagraph.InnerHtml = constants.STANDARD_NOTE;

        //resultsdiv.Controls.Add(fwizparagraph);
        resultsdiv.Controls.Add(pbuqwparagraph);
        resultsdiv.Controls.Add(usdwparagraph);
        resultsdiv.Controls.Add(disclaimerparagraph);

        //Return our results div.
        return resultsdiv;
    }

    protected override void getData()
    {
        /*//Now we need to begin collecting the data necessary to generate our output div.
        _depth10k = g.queryRaster(_coords, "sde.SDE.tceq_10k");
        _depth3k = g.queryRaster(_coords, "sde.SDE.tceq_3k");
        _depth1kTop = g.queryRaster(_coords, "sde.SDE.tceq_tops");
        _depth1kBottom = g.queryRaster(_coords, "sde.SDE.tceq_1k");
        _elevation = g.queryRaster(_coords, "sde.SDE.tceq_elev");

        //There's one feature that we need to retrieve as well.
        DataTable aqs = g.performBaseFeatureDataQuery(string.Empty, (ESRI.ArcGIS.Geometry.IGeometry)_coords, ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelWithin, "sde.SDE.TCEQ_Aquifers");
        if (aqs != null && aqs.Rows.Count > 0)
        {
            _aquifername = (string)aqs.Rows[0]["Name"];
        }

        //To display the coordinates in lat/long, we need to reproject our feature to a GCS coord system.
        ESRI.ArcGIS.Geometry.IGeometry ig_projected = g.unprojectGeometry((ESRI.ArcGIS.Geometry.IGeometry)_coords, 102603, 4019);
        ESRI.ArcGIS.Geometry.IPoint ip_projected = (ESRI.ArcGIS.Geometry.IPoint)ig_projected;

        _latitude = ip_projected.X;
        _longitude = ip_projected.Y;

        //With the data values retrieved from the DB, we can calculate our output vals.
        _topmsl = elevation;
        _topgsfc = 0;

        _botmsl = _depth1kBottom;
        _botgsfc = elevation - _depth1kBottom;

        //The "Protected base of usable quality water.
        _pbuqwmsl = _depth3k;
        _pbuqwgsfc = elevation - _depth3k;

        //The base of USDW
        _usdwmsl = _depth10k;
        _usdwgsfc = elevation - _depth10k;*/

        base.getData();

        _topmsl = elevation;
        _topgsfc = 0;

    }

    public override object GetFactoryKey()
    {
        return "Year5";
    }

}
