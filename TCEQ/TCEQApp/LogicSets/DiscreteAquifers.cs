using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

/// <summary>
/// Displays a list of the aquifers and their top and bottom depths
/// 
/// </summary>
public class DiscreteAquifers : BaseLogicSet
{
    protected List<AquiferRecord> aquiferrecords;
    protected List<AquiferRecord> sortedAquiferRecords;

    public override object GetFactoryKey()
    {
        return "DiscreteAquifers";
    }

    protected override void getData()
    {
        base.getData();

        List<AquiferRecord> rs = new List<AquiferRecord>();

        //Now, we'll get the aquifer data.
        ArcGISRESTClient.Layer l = RestClient.GetLayerByName(GetSettingValueFromConfig("AQUIFER_LAYER_NAME"));

        System.Data.DataTable aquifers = l.Query(null, _coords.GetJToken());

        //For each aquifer we found...
        for (int i = 0; i < aquifers.Rows.Count; i++)
        {
            //Instantiate an aquifer record.
            AquiferRecord aquiferrecord = new AquiferRecord();

            //Get the strings from the aquifer record.
            if (aquifers.Columns.Contains("Name") && aquifers.Rows[i]["Name"] is string)
            {
                aquiferrecord.name = (string)aquifers.Rows[i]["Name"];
            }

            if (aquifers.Columns.Contains("top_label") && aquifers.Rows[i]["top_label"] is string)
            {
                aquiferrecord.topLabel = (string)aquifers.Rows[i]["top_label"];
            }

            if (aquifers.Columns.Contains("bot_label") && aquifers.Rows[i]["bot_label"] is string)
            {
                aquiferrecord.bottomLabel = (string)aquifers.Rows[i]["bot_label"];
            }

            if (aquifers.Columns.Contains("allow_top_gsfc") && !(aquifers.Rows[i]["allow_top_gsfc"] is System.DBNull))
            {
                int ii = System.Convert.ToInt32(aquifers.Rows[i]["allow_top_gsfc"]);

                if(ii == 0)
                {
                    aquiferrecord.allow_top_gsfc = false;
                }
            }

            if (aquifers.Columns.Contains("top_display_value") && !(aquifers.Rows[i]["top_display_value"] is System.DBNull))
            {
                aquiferrecord.top_display_value = aquifers.Rows[i]["top_display_value"].ToString();
            }

            if (aquifers.Columns.Contains("bot_display_value") && !(aquifers.Rows[i]["bot_display_value"] is System.DBNull))
            {
                aquiferrecord.bot_display_value = aquifers.Rows[i]["bot_display_value"].ToString();
            }

            if(aquifers.Columns.Contains("excludeFromList") && !(aquifers.Rows[i]["excludeFromList"] is System.DBNull))
            {
                if(aquifers.Rows[i]["excludeFromList"].ToString() == "1")
                {
                    aquiferrecord.excludeFromList = true;
                }
            }

            if (aquifers.Columns.Contains("excludeFromTable") && !(aquifers.Rows[i]["excludeFromTable"] is System.DBNull))
            {
                if (aquifers.Rows[i]["excludeFromTable"].ToString() == "1")
                {
                    aquiferrecord.excludeFromTable = true;
                }
            }

            if (aquifers.Columns.Contains("excludeFromText") && !(aquifers.Rows[i]["excludeFromText"] is System.DBNull))
            {
                if (aquifers.Rows[i]["excludeFromText"].ToString() == "1")
                {
                    aquiferrecord.excludeFromText = true;
                }
            }

            //Now, we process the data we retrieved from the db.
            if(aquifers.Columns.Contains("top_raster") && aquifers.Columns.Contains("bot_raster"))
            {
                if (aquifers.Rows[i]["top_raster"] is string)
                {
                    string top_raster_name = (string) aquifers.Rows[i]["top_raster"];

                    if(top_raster_name != string.Empty)
                    {
                        try
                        {
                            ArcGISRESTClient.Layer topRaster = RestClient.GetLayerByName(top_raster_name);

                            if (topRaster != null)
                            {
                                double top_val = topRaster.QueryRasterLayer(_coords.X, _coords.Y);
                                aquiferrecord.top_elev = top_val;

                                aquiferrecord.top_gsfc = elevation - top_val;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                
                if (aquifers.Rows[i]["bot_raster"] is string)
                {
                        
                    string bot_raster_name = (string)aquifers.Rows[i]["bot_raster"];

                    if (bot_raster_name != string.Empty)
                    {
                        try
                        {
                            ArcGISRESTClient.Layer botRaster = RestClient.GetLayerByName(bot_raster_name);

                            if (botRaster != null)
                            {
                                double bot_val = botRaster.QueryRasterLayer(_coords.X, _coords.Y);
                                aquiferrecord.bottom_elev = bot_val;

                                aquiferrecord.bottom_gsfc = elevation - bot_val;
                            }
                        }
                        catch
                        {
                        }
                    }
                }

                //Finally, place the data in the list.
                rs.Add(aquiferrecord);
            }
        }

        aquiferrecords = rs;
    }

    public override System.Web.UI.HtmlControls.HtmlGenericControl GenerateOutputControl()
    {
        //Before composing the aquifer rows part of the table, sort the aquifers.
        sortAquiferRecords();

        HtmlGenericControl resultsdiv = new HtmlGenericControl("div");
        resultsdiv.Attributes.Add("class", "detailsdiv");

        //Now we're ready to compose our results into a set of controls so that we can have ASP.NET display them.
        Table t = new Table();
        t.CssClass = "resulttable";
        t.ID = "resulttable";
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

        //Compose the aquifer names row
        TableCell aquiferLabelCell = new TableCell();
        TableCell aquiferValueCell = new TableCell();
        aquiferRow.Cells.Add(aquiferLabelCell);
        aquiferRow.Cells.Add(aquiferValueCell);
        aquiferLabelCell.Text = "Aquifers";
        aquiferValueCell.Text = ComposeAquiferNamesList();

        //Add rows to the table.
        t.Rows.Add(longitudeRow);
        t.Rows.Add(latitudeRow);
        t.Rows.Add(aquiferRow);

        //Here, we'll compose the aquifer rows.
        for (int i = 0; i < sortedAquiferRecords.Count; i++)
        {
            bool isFirst = false;

            string name = sortedAquiferRecords[i].name;

            if(i == 0)
            {
                isFirst = true;
            }

            if (!sortedAquiferRecords[i].excludeFromTable)
            {

                TableRow topRow = PrepareTopRow(sortedAquiferRecords[i], isFirst);
                TableRow botRow = PrepareBottomRow(sortedAquiferRecords[i], isFirst);

                if (topRow != null)
                {
                    t.Rows.Add(topRow);
                }

                if (botRow != null)
                {
                    t.Rows.Add(botRow);
                }
            }
        }

        //Add the table to the final output.
        resultsdiv.Controls.Add(t);

        //Now, we do the paragraphs.  These use a similar logic to the table.
        for (int i = 0; i < sortedAquiferRecords.Count; i++)
        {
            AquiferRecord ar = sortedAquiferRecords[i];

            if (!ar.excludeFromText)
            {
                HtmlGenericControl hgc = new HtmlGenericControl("p");

                if (ar.top_elev != null)
                {
                    //Here, we do the above/below thing.
                    string sTopMSLAbove = constants.MSL_LABEL_ABOVE;
                    if (ar.top_elev < 0) sTopMSLAbove = constants.MSL_LABEL_BELOW;

                    string sBotMSLAbove = constants.MSL_LABEL_ABOVE;
                    if (ar.bottom_elev < 0) sBotMSLAbove = constants.MSL_LABEL_BELOW;

                    hgc.InnerText = "The " + ar.name + ", occurs from a depth of " + Math.Round(Math.Abs(ar.top_gsfc.Value), 0).ToString() + " feet (" + Math.Round(Math.Abs(ar.top_elev.Value), 0).ToString() + " feet " + sTopMSLAbove + " MSL) to " + Math.Round(ar.bottom_gsfc.Value, 0).ToString() + " feet (" + Math.Round(Math.Abs(ar.bottom_elev.Value), 0).ToString() + " feet " + sBotMSLAbove + " MSL).";
                }
                else if (ar.bottom_elev != 0)
                {
                    string sBotMSLAbove = constants.MSL_LABEL_ABOVE;
                    if (ar.bottom_elev < 0) sBotMSLAbove = constants.MSL_LABEL_BELOW;

                    string bottomLabel = ar.bottomLabel;

                    if ((bottomLabel == string.Empty || bottomLabel == null) && ar.name != null)
                    {
                        bottomLabel = ar.name;
                    }

                    if (ar.bottom_elev != null && ar.bottom_gsfc != null)
                    {
                        hgc.InnerText = "The base of the " + bottomLabel + " is estimated to occur at a depth of " + Math.Round(Math.Abs(ar.bottom_gsfc.Value), 0).ToString() + " feet (" + Math.Round(Math.Abs(ar.bottom_elev.Value), 0).ToString() + " feet " + sBotMSLAbove + " MSL).";
                    }
                }

                resultsdiv.Controls.Add(hgc);
            }           
        }


        HtmlGenericControl disclaimerparagraph = new HtmlGenericControl("p");
        disclaimerparagraph.InnerHtml = "Note:  This is an approximate recommendation and is subject to confirmation by a <a href=\"http://www.tceq.state.tx.us/nav/permits/surface_casing.html\" target=\"_blank\" title=\"TCEQ Website\">RRC Groundwater Advisory Unit</a> (formerly TCEQ Surface Casing) geologist.  An isolation buffer zone has not been included in these recommendations.";

        resultsdiv.Controls.Add(disclaimerparagraph);
        
        return resultsdiv;
    }

    protected TableRow PrepareBottomRow(AquiferRecord ar, bool isfirst)
    {
        TableRow ret = null;

        //Get ready to do the bottom row.
        TableRow bottomRow = new TableRow();
        TableCell botLabelCell = new TableCell();
        TableCell botValueCell = new TableCell();
        bottomRow.Cells.Add(botLabelCell);
        bottomRow.Cells.Add(botValueCell);

        //Set up the bottom row label.
        if (ar.bottomLabel != null && ar.bottomLabel != string.Empty)
        {
            botLabelCell.Text = ar.bottomLabel;
            //ret = bottomRow;
        }
        else if (ar.name != null && ar.name != string.Empty)
        {
            botLabelCell.Text = "Base of " + ar.name + ":";
            //ret = bottomRow;
        }

        //If we have a display value given, we display that.  Simple enough.
        if (ar.bot_display_value != null)
        {
            botValueCell.Text = ar.bot_display_value;

            ret = bottomRow;
        }
        else if (ar.bottom_elev != null) //Otherwise, do the business where we display an actual value.
        {
            string sBotMSLAbove = constants.MSL_LABEL_ABOVE;
            if (ar.bottom_elev < 0) sBotMSLAbove = constants.MSL_LABEL_BELOW;

            if (ar.bot_display_value != null) //If there's a display value
            {
                botValueCell.Text = ar.bot_display_value;
            }
            else
            {
                if (ar.bottom_elev != null && ar.bottom_gsfc != null)
                {
                    botValueCell.Text = Math.Round(Math.Abs(ar.bottom_gsfc.Value), 0).ToString() + "' Depth (" + Math.Round(Math.Abs(ar.bottom_elev.Value), 0).ToString() + "' " + sBotMSLAbove + " MSL)";
                }
            }

            ret = bottomRow;
        }

        return ret;
    }

    protected TableRow PrepareTopRow(AquiferRecord ar, bool isfirst)
    {
        TableRow ret = null;

        TableRow topRow = new TableRow();
        TableCell topLabelCell = new TableCell();
        TableCell topValueCell = new TableCell();
        topRow.Cells.Add(topLabelCell);
        topRow.Cells.Add(topValueCell);

        if (ar.top_elev != null)
        {
            //Here, we do the above/below thing.
            string sTopMSLAbove = constants.MSL_LABEL_ABOVE;
            if (ar.top_elev < 0) sTopMSLAbove = constants.MSL_LABEL_BELOW;

            if (ar.top_display_value != null)
            {
                topValueCell.Text = ar.top_display_value;
            }
            else
            {
                if (ar.top_gsfc != null && ar.top_elev != null)
                {
                    topValueCell.Text = Math.Round(Math.Abs(ar.top_gsfc.Value), 0).ToString() + "' Depth (" + Math.Round(Math.Abs(ar.top_elev.Value), 0).ToString() + "' " + sTopMSLAbove + " MSL)";
                }
            }
            //Now, do the labels.
            if (ar.topLabel != null && ar.topLabel != string.Empty)
            {
                topLabelCell.Text = ar.topLabel;
            }
            else if (ar.name != null && ar.name != string.Empty)
            {
                topLabelCell.Text = "Top of " + ar.name + ":";
            }

            ret = topRow;
        }
        else if (isfirst)
        {
            //If there's no top value, and this is the first aquifer we encountered, we use the ground elevation.

            if (ar.allow_top_gsfc)
            {
                //Here, we do the above/below thing.
                string sTopMSLAbove = constants.MSL_LABEL_ABOVE;
                if (ar.top_elev < 0) sTopMSLAbove = constants.MSL_LABEL_BELOW;

                topValueCell.Text = Math.Round(_elevation, 0).ToString() + "'";

                //Now, do the labels.
                if (ar.topLabel != null && ar.topLabel != string.Empty)
                {
                    topLabelCell.Text = ar.topLabel;
                }
                else if (ar.name != null && ar.name != string.Empty)
                {
                    topLabelCell.Text = "Ground elevation:";
                }

                ret = topRow;
            }
        }

        return ret;
    }

    protected string ComposeAquiferNamesList()
    {
      string ret = string.Empty;

        List<AquiferRecord> listedAquifers = new List<AquiferRecord>();

        for (int i = 0; i < sortedAquiferRecords.Count; i++)
        {
            if(sortedAquiferRecords[i].excludeFromList == false)
            {
                listedAquifers.Add(sortedAquiferRecords[i]);
            }
        }

        for (int i = 0; i < listedAquifers.Count; i++ )
        {
            ret += listedAquifers[i].name;

            if(i < listedAquifers.Count - 1)
            {
                ret += ", ";
            }
        }

        return ret;
    }

    protected void sortAquiferRecords()
    {
        bool sorted = true;

        AquiferRecord lowest = null;

        sortedAquiferRecords = new List<AquiferRecord>();
        List<AquiferRecord> displayValueAquiferRecords = new List<AquiferRecord>();
        List<AquiferRecord> atEndAquiferRecords = new List<AquiferRecord>();

        for (int i = 0; i < aquiferrecords.Count; i++)
        {
            if(aquiferrecords[i].bottom_gsfc != null)
            {
                sortedAquiferRecords.Add(aquiferrecords[i]);
            }
            else if (aquiferrecords[i].bot_display_value != null) { 
                //If it doesn't have a bottom depth, we can't sort it, so we'll put it at the end.  
                //We store those in a list for now.
                displayValueAquiferRecords.Add(aquiferrecords[i]);
            }
            else
            {
                atEndAquiferRecords.Add(aquiferrecords[i]);
            }
        }

        lowest = sortedAquiferRecords[0];

        do
        {
            sorted = true;

            for(int i=0; (i + 1) < sortedAquiferRecords.Count; i++)
            {
                                
                if (sortedAquiferRecords[i].bottom_gsfc.Value > sortedAquiferRecords[i + 1].bottom_gsfc.Value)
                {
                    sorted = false;
                    //If we've got two records out of order, switch them around.
                    AquiferRecord temp = sortedAquiferRecords[i + 1];

                    sortedAquiferRecords[i + 1] = sortedAquiferRecords[i];
                    sortedAquiferRecords[i] = temp;
                }
                
            }
        }
        while (sorted == false);

        //Tack the unsortable ones on the end.
        for (int i = 0; i < displayValueAquiferRecords.Count; i++)
        {
            sortedAquiferRecords.Add(displayValueAquiferRecords[i]);
        }

        //Tack the unsortable ones on the end.
        for(int i=0; i < atEndAquiferRecords.Count; i++)
        {
            sortedAquiferRecords.Add(atEndAquiferRecords[i]);
        }
    }

    protected class AquiferRecord
    {
        public string name;

        public double? top_elev;
        public double? bottom_elev;

        public double? top_gsfc;
        public double? bottom_gsfc;

        public string topLabel;
        public string bottomLabel;

        public bool allow_top_gsfc;

        public string top_display_value = null;
        public string bot_display_value = null;

        public bool excludeFromList = false;
        public bool excludeFromTable = false;
        public bool excludeFromText = false;

        public AquiferRecord()
        {
            name = null;
            topLabel = null;
            bottomLabel = null;

            top_elev = null;
            bottom_elev = null;

            top_gsfc = null;
            bottom_gsfc = null;

            allow_top_gsfc = true;
        }
    }
}