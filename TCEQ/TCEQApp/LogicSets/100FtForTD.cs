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
/// Summary description for _100FtForTD
/// </summary>
public class _100FtForTD : BaseLogicSet, ILogicSet
{
    public override object GetFactoryKey()
    {
        return "100 feet for TD";
    }

    public override HtmlGenericControl GenerateOutputControl()
    {
        //I guess we have to do this the hard way.  I wanted to simply intercept and alter the output from the base method, but apparently, I have to completely override it.

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
        TableRow fwizRow = new TableRow();
        fwizRow.ID = "fwizRow";
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
    
        //This is the part that's changed from the base version.
        //The Fresh water isolation zone top row
        TableCell fwizLabelCell = new TableCell();
        fwizLabelCell.ID = "fwizLabelCell";
        TableCell fwizValueCell = new TableCell();
        fwizValueCell.ID = "fwizValueCell";
        fwizLabelCell.Text = "Fresh Water Isolation Zone";
        fwizValueCell.Text = constants.PROTECT_MESSAGE;
        fwizRow.Cells.Add(fwizLabelCell);
        fwizRow.Cells.Add(fwizValueCell);

        
        //Protected base of usable quality qater row
        TableCell pbuqwLabelCell = new TableCell();
        pbuqwLabelCell.ID = "pbuqwLabelCell";
        TableCell pbuqwValueCell = new TableCell();
        pbuqwValueCell.ID = "pbuqwValueCell";
        pbuqwLabelCell.Text = "Protected base of usable quality water";
        //pbuqwValueCell.Text = Math.Round(Math.Abs(pbuqwgsfc), 0).ToString() + "' Depth (" + Math.Round(pbuqwmsl, 0).ToString() + "' " + sPBUQWMSLAbove + " MSL)";
        pbuqwValueCell.Text = constants.PROTECT_MESSAGE;
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

        //Ground Elevation - for shits and giggles
        TableRow elevRow = new TableRow();
        elevRow.ID = "elevRow";
        TableCell elevLabelCell = new TableCell();
        elevRow.ID = "elevLabelCell";
        TableCell elevValueCell = new TableCell();
        elevRow.ID = "elevValueCell";
        elevLabelCell.Text = "Ground Elevation";
        elevValueCell.Text = Math.Round(Math.Abs(elevation), 0).ToString();
        elevRow.Cells.Add(elevLabelCell);
        elevRow.Cells.Add(elevValueCell);

        //Add all the rows to the table.
        t.Rows.Add(longitudeRow);
        t.Rows.Add(latitudeRow);
        t.Rows.Add(elevRow);
        t.Rows.Add(aquiferRow);
        t.Rows.Add(fwizRow);
        t.Rows.Add(pbuqwRow);
        t.Rows.Add(usdwRow);

        resultsdiv.Controls.Add(t);

        //Finally, we need to compose the paragraph form thing.
        string fwizstring, pbuqwstring, usdwstring;

        //This is like option zero from the year 3 TCEQ app.
        //Here, we compose the output strings.
        fwizstring = "The fresh water must be protected from the ground surface to a depth of 100 feet.";

        pbuqwstring = "Protect from the ground surface to a depth of 100 feet.";

        usdwstring = "The USDW (10,000 ppm TDS) is estimated to occur at a depth of " + Decimal.Round(new Decimal(usdwgsfc), 0).ToString() + " feet (" + Decimal.Round(new Decimal(Math.Abs(usdwmsl)), 0).ToString() + "' " + sUSDWMSLAbove + " MSL).";

        //Now we need to create controls to house these.
        HtmlGenericControl fwizparagraph = new HtmlGenericControl("p");
        fwizparagraph.InnerText = fwizstring;

        HtmlGenericControl pbuqwparagraph = new HtmlGenericControl("p");
        pbuqwparagraph.InnerText = pbuqwstring;

        HtmlGenericControl usdwparagraph = new HtmlGenericControl("p");
        usdwparagraph.InnerText = usdwstring;

        HtmlGenericControl disclaimerparagraph = new HtmlGenericControl("p");
        disclaimerparagraph.InnerHtml = constants.STANDARD_NOTE;

        resultsdiv.Controls.Add(fwizparagraph);
        resultsdiv.Controls.Add(pbuqwparagraph);
        resultsdiv.Controls.Add(usdwparagraph);
        resultsdiv.Controls.Add(disclaimerparagraph);

        //Return our results div.
        return resultsdiv;
    }
}
