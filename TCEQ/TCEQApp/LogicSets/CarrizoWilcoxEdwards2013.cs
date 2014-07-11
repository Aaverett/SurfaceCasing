using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace TCEQApp.LogicSets
{
    public class CarrizoWilcoxEdwards2013 : CarrizoWilcox2013, ILogicSet
    {
        protected double _top_edwardsMSL = 0;
        protected double _top_edwardsGSFC = 0;

        public override object GetFactoryKey()
        {
            return "CarrizoWilcoxEdwards2013";
        }

        protected override void getData()
        {
            //
            base.getData();

            ArcGISRESTClient.Layer edwardsLayer = RestClient.GetLayerByName(GetSettingValueFromConfig("EDWARDS_LAYER_NAME"));

            _top_edwardsMSL = edwardsLayer.QueryRasterLayer(_coords.X, _coords.Y);
            _top_edwardsGSFC = _elevation - _top_edwardsMSL;
        }

        public override System.Web.UI.HtmlControls.HtmlGenericControl GenerateOutputControl()
        {
            HtmlGenericControl hgc = base.GenerateOutputControl();

            TableRow trEdwards = new TableRow();
            TableCell tcEdwardsLabelCell = new TableCell();
            TableCell tcEdwardsValueCell = new TableCell();

            string tEdwardsMSLAbove = AboveBelow(_top_edwardsMSL);

            tcEdwardsLabelCell.Text = "Top of Edwards Interval";
            tcEdwardsValueCell.Text = Math.Round(Math.Abs(_top_edwardsGSFC), 0).ToString() + "' Depth (" + Math.Round(Math.Abs(_top_edwardsMSL), 0).ToString() + "' " + tEdwardsMSLAbove + " MSL)";

            trEdwards.Cells.Add(tcEdwardsLabelCell);
            trEdwards.Cells.Add(tcEdwardsValueCell);

            _resultsTable.Rows.Add(trEdwards);

            return hgc;
        }
    }
}