using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace TCEQApp.LogicSets
{
    public class SimpleStringLogicSet : BaseLogicSet, ILogicSet
    {
        object ILogicSet.GetFactoryKey()
        {
            return "SimpleStringLogicSet";
        }

        protected virtual string OutputString
        {
            get
            {
                return string.Empty;
            }
        }

        protected override void getData()
        {
            //There's one feature that we need to retrieve as well.
            ArcGISRESTClient.Layer aquiferLayer = RestClient.GetLayerByName(GetSettingValueFromConfig("AQUIFER_LAYER_NAME"));
            System.Data.DataTable aqs = aquiferLayer.Query(null, _coords.GetJToken());
            if (aqs != null && aqs.Rows.Count > 0)
            {
                _aquifername = (string)aqs.Rows[0]["Name"];
            }

            _latitude = _coords.Y;
            _longitude = _coords.X;
        }

        public virtual HtmlGenericControl GenerateOutputControl()
        {
            //This will be the control that gets returned.
            HtmlGenericControl resultsdiv = new HtmlGenericControl("div");
            resultsdiv.Attributes.Add("class", "detailsdiv");

            //Perform sanity checking on the data.
            /*if(_depth1kTop < _depth1kBottom)
            {
                resultsdiv.InnerHtml = "<div style=\"padding: 10px;\"><p>The location you have selected is in an area where the depth at which one or more aquifers occurs could not be computed accurately.  Please contact TCEQ for help.</p></div>";

                return resultsdiv;
            }*/

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
            TableRow protectRow = new TableRow();
            protectRow.ID = "protectRow";
          
          
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

            //The protect row.
            TableCell protectCell = new TableCell();
            protectCell.ID = "protectCell";
            protectCell.Text = OutputString;
            protectCell.ColumnSpan = 2;
            protectRow.Cells.Add(protectCell);

            //Add all the rows to the table.
            t.Rows.Add(longitudeRow);
            t.Rows.Add(latitudeRow);         
            t.Rows.Add(aquiferRow);
            t.Rows.Add(protectRow);

            resultsdiv.Controls.Add(t);

           
            HtmlGenericControl protectparagraph = new HtmlGenericControl("p");
            protectparagraph.ID = "protectParagraph";
            protectparagraph.InnerText = OutputString;

            HtmlGenericControl disclaimerparagraph = new HtmlGenericControl("p");
            disclaimerparagraph.ID = "disclaimerparagraph";
            disclaimerparagraph.InnerHtml = constants.STANDARD_NOTE;

            resultsdiv.Controls.Add(protectparagraph);
            resultsdiv.Controls.Add(disclaimerparagraph);

            //Return our results div.
            return resultsdiv;
        }
    }
}