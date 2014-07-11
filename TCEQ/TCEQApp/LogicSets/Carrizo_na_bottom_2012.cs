using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCEQApp.LogicSets
{
    public class Carrizo_na_bottom_2012 : BaseLogicSet, ILogicSet
    {
        public override object GetFactoryKey()
        {
            return "Carrizo_na_bottom_2012";
        }

        protected override void getData()
        {
            //
            base.getData();

            //Do anything else.
            ArcGISRESTClient.Layer carrizoTopLayer = RestClient.GetLayerByName(GetSettingValueFromConfig("CARRIZO_TOP_LAYER_NAME"));
            double czTop = carrizoTopLayer.QueryRasterLayer(_coords.X, _coords.Y);

            _topmsl = czTop;
            _topgsfc = elevation - czTop;

            string sTopMSLAbove = AboveBelow(topmsl);

            //_showTopFWIZRow = false;
            _topFWIZCaption = "Top of Carrizo Isolation Interval";
            _botFWIZValueString = "N/A";
            _aquiferFieldCaption = "Aquifer (at BUQ)";
            _alternateTopSentence = "The top of the Carrizo isolation interval is at " + Decimal.Round(new Decimal(topgsfc), 0).ToString() + " feet (" + Decimal.Round(new Decimal(Math.Abs(topmsl)), 0).ToString() + " feet " + sTopMSLAbove + " MSL).";
        }
    }
}