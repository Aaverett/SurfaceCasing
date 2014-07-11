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
/// Logic set for the hog mountain aquifer business.
/// This is an extension of the Year5 logic.
/// </summary>
public class HogMountain : BaseLogicSet, ILogicSet
{
    private double _top_hog_mountain = 0.0;
    private double _top_hog_mountainmsl = 0.0;
    private double _top_hog_mountaingsfc = 0.0;
    private double _bottom_hog_mountain = 0.0;
    private double _bottom_hog_mountainmsl = 0.0;
    private double _bottom_hog_mountaingsfc = 0.0;

    public override object GetFactoryKey()
    {
        return "HogMountain";
    }

    protected override void getData()
    {
        //First, do the base getData business.  
        base.getData();

        //In this section, the top of the FWIZ is the ground surface.
        _topmsl = elevation;
        _topgsfc = 0;

        ArcGISRESTClient.Layer hogMountainLayer = RestClient.GetLayerByName(GetSettingValueFromConfig("HOG_MOUNTAIN_TOP_LAYER_NAME"));
        ArcGISRESTClient.Layer depth3KLayer = RestClient.GetLayerByName(GetSettingValueFromConfig("DEPTH_3K_LAYER_NAME"));

        _top_hog_mountainmsl = hogMountainLayer.QueryRasterLayer(_coords.X, _coords.Y);
        _top_hog_mountaingsfc = elevation - _top_hog_mountainmsl;

        _bottom_hog_mountainmsl = depth3KLayer.QueryRasterLayer(_coords.X, _coords.Y);
        _bottom_hog_mountaingsfc = elevation - _bottom_hog_mountainmsl;
    }

    public override HtmlGenericControl GenerateOutputControl()
    {
        //First, do the basic logic business.  This is actually the same, but we need to add the hog mountain lines as well.
        HtmlGenericControl hgc = base.GenerateOutputControl();

        Control c = hgc.FindControl("resulttable");
        
        //Why the heck doesn't findcontrol work here?
        Control c_fwiz = null;
        for (int i = 0; i < hgc.Controls.Count; i++)
        {
            if (hgc.Controls[i].ID == "fwizparagraph")
            {
                c_fwiz = hgc.Controls[i];
            }
        }

        //If we didn't find the table, just abort.  Bad juju would happen otherwise.
        if (c == null || !(c is Table))
        {
            if (hgc.Controls[0] is Table && hgc.Controls[0].ID == "resulttable") c = hgc.Controls[0];
        }

        if (c == null || !(c is Table)) return hgc;

        string sHMtnMSLAbove = constants.MSL_LABEL_ABOVE;
        string sBHMtnMSLAbove = constants.MSL_LABEL_ABOVE;
        if (_top_hog_mountainmsl < 0) sHMtnMSLAbove = constants.MSL_LABEL_BELOW;
        if (_bottom_hog_mountainmsl < 0) sBHMtnMSLAbove = constants.MSL_LABEL_BELOW;

        Table t = (Table)c;

        TableRow tr1 = new TableRow();
        TableCell thc1 = new TableCell();
        TableCell tc1 = new TableCell();
        thc1.Text = "Top of Hog Mountain Sandstone: ";
        tc1.Text = Math.Round(Math.Abs(_top_hog_mountaingsfc), 0).ToString() + "' Depth (" + Math.Round(Math.Abs(_top_hog_mountainmsl), 0).ToString() + "' " + sHMtnMSLAbove + " MSL)";
        tr1.Cells.Add(thc1);
        tr1.Cells.Add(tc1);
        t.Rows.Add(tr1);


        TableRow tr2 = new TableRow();
        TableCell thc2 = new TableCell();
        TableCell tc2 = new TableCell();
        thc2.Text = "Base of Hog Mountain Sandstone: ";
        tc2.Text = Math.Round(Math.Abs(_bottom_hog_mountaingsfc), 0).ToString() + "' Depth (" + Math.Round(Math.Abs(_bottom_hog_mountainmsl), 0).ToString() + "' " + sBHMtnMSLAbove + " MSL)";
        tr2.Cells.Add(thc2);
        tr2.Cells.Add(tc2);
        t.Rows.Add(tr2);

        if (c_fwiz != null && c_fwiz is HtmlGenericControl)
        {
            string newfwizstring = "The fresh water contained in the " + aquifername + " aquifer unit, which is estimated to occur from a depth of " + Decimal.Round(new Decimal(_top_hog_mountaingsfc), 0).ToString() + " feet (" + Decimal.Round(new Decimal(Math.Abs(_top_hog_mountainmsl)), 0).ToString() + " feet " + sHMtnMSLAbove + " MSL) to " + Decimal.Round(new Decimal(_bottom_hog_mountaingsfc), 0).ToString() + " feet (" + Decimal.Round(new Decimal(Math.Abs(_bottom_hog_mountainmsl)), 0).ToString() + " feet " + sBHMtnMSLAbove + " MSL), must be isolated from the water above and below.";

            HtmlGenericControl hgc_fwiz = (HtmlGenericControl)c_fwiz;

            hgc_fwiz.InnerText = newfwizstring;
        }

        return hgc;
    }
}
