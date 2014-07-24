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

        System.Data.DataTable aquifers = l.Query(null, _coords.GetJValue());

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
                aquiferrecord.top_display_value = aquifers.Rows[i]["bot_display_value"].ToString();
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

        //Add rows to the table.
        t.Rows.Add(longitudeRow);
        t.Rows.Add(latitudeRow);

        
        //Before composing the aquifer rows part of the table, sort the aquifers.
        sortAquiferRecords();

        //Here, we'll compose the aquifer rows.

        for (int i = 0; i < sortedAquiferRecords.Count; i++)
        {
            AquiferRecord ar = sortedAquiferRecords[i];

            if (ar.top_elev != null)
            {
                //Instantiate and assemble our table row.
                TableRow topRow = new TableRow();
                TableCell topLabelCell = new TableCell();
                TableCell topValueCell = new TableCell();
                topRow.Cells.Add(topLabelCell);
                topRow.Cells.Add(topValueCell);

                //Here, we do the above/below thing.
                string sTopMSLAbove = constants.MSL_LABEL_ABOVE;
                if (ar.top_elev < 0) sTopMSLAbove = constants.MSL_LABEL_BELOW;

                if(ar.top_display_value != null)
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

                t.Rows.Add(topRow);
            }
            else if(i == 0)
            {
                //If there's no top value, and this is the first aquifer we encountered, we use the ground elevation.

                if(ar.allow_top_gsfc)
                {
                    //Instantiate and assemble our table row.
                    TableRow topRow = new TableRow();
                    TableCell topLabelCell = new TableCell();
                    TableCell topValueCell = new TableCell();
                    topRow.Cells.Add(topLabelCell);
                    topRow.Cells.Add(topValueCell);

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

                    t.Rows.Add(topRow);
                }
            }

            if (ar.bottom_elev != null)
            {
                TableRow bottomRow = new TableRow();
                TableCell botLabelCell = new TableCell();
                TableCell botValueCell = new TableCell();
                bottomRow.Cells.Add(botLabelCell);
                bottomRow.Cells.Add(botValueCell);

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

                //Now, do the labels.
                if (ar.bottomLabel != null && ar.bottomLabel != string.Empty)
                {
                    botLabelCell.Text = ar.bottomLabel;
                }
                else if (ar.name != null && ar.name != string.Empty)
                {
                    botLabelCell.Text = "Bottom of " + ar.name + ":";
                }

                t.Rows.Add(bottomRow);
            }
        }

        resultsdiv.Controls.Add(t);

        //Now, we do the paragraphs.  These use a similar logic to the table.
        for (int i = 0; i < aquiferrecords.Count; i++)
        {
            AquiferRecord ar = aquiferrecords[i];

            HtmlGenericControl hgc = new HtmlGenericControl("p");

            if (ar.top_elev != null)
            {
                //Here, we do the above/below thing.
                string sTopMSLAbove = constants.MSL_LABEL_ABOVE;
                if (ar.top_elev < 0) sTopMSLAbove = constants.MSL_LABEL_BELOW;

                string sBotMSLAbove = constants.MSL_LABEL_ABOVE;
                if (ar.bottom_elev < 0) sBotMSLAbove = constants.MSL_LABEL_BELOW;

                hgc.InnerText = "The water contained in the " + ar.name + " Formation, which is estimated to occur from a depth of " + Math.Round(Math.Abs(ar.top_gsfc.Value), 0).ToString() + " feet (" + Math.Round(Math.Abs(ar.top_elev.Value), 0).ToString() + " feet " + sTopMSLAbove + " MSL) to " + Math.Round(ar.bottom_gsfc.Value, 0).ToString() + " feet (" + Math.Round(Math.Abs(ar.bottom_elev.Value), 0).ToString() + " feet " + sBotMSLAbove + " MSL) must be isolated from the water above and below.";
            }
            else if (i == 0)
            {
                string sBotMSLAbove = constants.MSL_LABEL_ABOVE;
                if (ar.bottom_elev < 0) sBotMSLAbove = constants.MSL_LABEL_BELOW;

                if (ar.bottom_elev != null && ar.bottom_gsfc != null)
                {
                    hgc.InnerText = "The base of the " + ar.bottomLabel + " is estimated to occur at a depth of " + Math.Round(Math.Abs(ar.bottom_gsfc.Value), 0).ToString() + " feet (" + Math.Round(Math.Abs(ar.bottom_elev.Value), 0).ToString() + " feet " + sBotMSLAbove + " MSL).";
                }
            }

            resultsdiv.Controls.Add(hgc);
        }


        HtmlGenericControl disclaimerparagraph = new HtmlGenericControl("p");
        disclaimerparagraph.InnerHtml = "Note:  This is an approximate recommendation and is subject to confirmation by a <a href=\"http://www.tceq.state.tx.us/nav/permits/surface_casing.html\" target=\"_blank\" title=\"TCEQ Website\">RRC Groundwater Advisory Unit</a> (formerly TCEQ Surface Casing) geologist.  An isolation buffer zone has not been included in these recommendations.";

        resultsdiv.Controls.Add(disclaimerparagraph);
        
        return resultsdiv;
    }

    protected void sortAquiferRecords()
    {
        bool sorted = true;

        AquiferRecord lowest = null;

        sortedAquiferRecords = new List<AquiferRecord>();

        for (int i = 0; i < aquiferrecords.Count; i++)
        {
            if(aquiferrecords[i].bottom_gsfc != null)
            {
                sortedAquiferRecords.Add(aquiferrecords[i]);
            }
        }

        do
        {
            sorted = true;

            for(int i=0; (i + 1) < sortedAquiferRecords.Count; i++)
            {
                if (lowest == null)
                {
                    lowest = sortedAquiferRecords[i];
                }

                
                if (lowest.bottom_gsfc != null && sortedAquiferRecords[i +1].bottom_gsfc != null && lowest.bottom_gsfc.Value > sortedAquiferRecords[i + 1].bottom_gsfc.Value)
                {
                    sorted = false;
                    //If we've got two records out of order, switch them around.
                    AquiferRecord temp = sortedAquiferRecords[i + 1];

                    sortedAquiferRecords[i + 1] = lowest;
                    sortedAquiferRecords[i] = temp;

                    lowest = temp;
                }
                
            }
        }
        while (sorted == false);
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