using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCEQApp.LogicSets
{
    public class SpartaCarrizo2015 : BaseLogicSet, ILogicSet
    {
        protected double _czTop;
        protected double _czTopGsfc;

        public override object GetFactoryKey()
        {
            return "SpartaCarrizo2015";
        }

        protected override void getData()
        {
            base.getData();

            //Do anything else.
            ArcGISRESTClient.Layer carrizoTopLayer = RestClient.GetLayerByName(GetSettingValueFromConfig("CARRIZO_TOP_LAYER_NAME"));
            double czTop = carrizoTopLayer.QueryRasterLayer(_coords.X, _coords.Y);

            double topCZgsfc = elevation - czTop;

            _czTop = czTop;
            _czTopGsfc = topCZgsfc;

            //_topmsl = czTop;
            //_topgsfc = elevation - czTop;

            string sTopMSLAbove = AboveBelow(topmsl);
            string czTopMSLAbove = AboveBelow(czTop);

            //_showTopFWIZRow = false;
            _topFWIZCaption = "Top of Sparta Isolation Zone";
            _aquiferFieldCaption = "Aquifer (at BUQ)";
            _alternateTopSentence = "The top of the Sparta isolation interval is at " + Decimal.Round(new Decimal(topgsfc), 0).ToString() + " feet (" + Decimal.Round(new Decimal(Math.Abs(topmsl)), 0).ToString() + " feet " + sTopMSLAbove + " MSL).<br /><br />";
            _alternateTopSentence += "The top of the Carrizo isolation interval is at " + Decimal.Round(new Decimal(topCZgsfc), 0).ToString() + " feet (" + Decimal.Round(new Decimal(Math.Abs(czTop)), 0).ToString() + " feet " + czTopMSLAbove + " MSL).";
        }

        public override System.Web.UI.HtmlControls.HtmlGenericControl GenerateOutputControl()
        {
            System.Web.UI.HtmlControls.HtmlGenericControl c = base.GenerateOutputControl();

            string czTopMSLAbove = AboveBelow(_czTop);

            //Create a new table row for the sparta.
            System.Web.UI.WebControls.TableRow tr = new System.Web.UI.WebControls.TableRow();
            System.Web.UI.WebControls.TableCell tc_caption = new System.Web.UI.WebControls.TableCell();
            System.Web.UI.WebControls.TableCell tc_value = new System.Web.UI.WebControls.TableCell();
            tr.Cells.Add(tc_caption);
            tr.Cells.Add(tc_value);
            tc_caption.Text = "Top of Carrizo Isolation Zone";
            tc_value.Text = Math.Round(Math.Abs(_czTopGsfc), 0).ToString() + "' Depth (" + Math.Round(Math.Abs(_czTop), 0).ToString() + "' " + czTopMSLAbove + " MSL)";

            //Add the row to the set of rows.
            _resultsTable.Rows.AddAt(5, tr);

            return c;
        }
    }
}