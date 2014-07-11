using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace TCEQApp.LogicSets
{
    public class GulfCoast_goliad_2014 : GulfCoast_oakville_2014, ILogicSet
    {

        protected double _base_goliad;
        protected double _base_goliadGSFC;

        public override object GetFactoryKey()
        {
            return "GulfCoast_goliad_2014";
        }

        protected override void getData()
        {
            //
            base.getData();

            _showTopFWIZRow = false;
            _showBotFWIZRow = false;

             //Do anything else.
            ArcGISRESTClient.Layer goliadLayer = RestClient.GetLayerByName(GetSettingValueFromConfig("GOLIAD_LAYER_NAME"));

            //We need to query the oakville dataset.
            _base_goliad = goliadLayer.QueryRasterLayer(_coords.X, _coords.Y);
            _base_goliadGSFC = _elevation - _base_goliad;

            

        }

        /// <summary>
        /// Override of the base logic inherited from the gulf coast base fresh logic type.
        /// </summary>
        /// <returns>HTML generic control containing the result data.</returns>
        public override HtmlGenericControl GenerateOutputControl()
        {
            HtmlGenericControl hgc = base.GenerateOutputControl();

            TableRow trGoliad = new TableRow();
            TableCell tcGoliadLabelCell = new TableCell();
            TableCell tcGoliadValueCell = new TableCell();

            string tGoliadMSLAbove = AboveBelow(_base_goliad);

            tcGoliadLabelCell.Text = "Base of Goliad Isolation Interval";
            tcGoliadValueCell.Text = Math.Round(Math.Abs(_base_goliadGSFC), 0).ToString() + "' Depth (" + Math.Round(Math.Abs(_base_goliad), 0).ToString() + "' " + tGoliadMSLAbove + " MSL)";

            trGoliad.Cells.Add(tcGoliadLabelCell);
            trGoliad.Cells.Add(tcGoliadValueCell);

            _resultsTable.Rows.AddAt(4, trGoliad);

            return hgc;
        }
    }
}