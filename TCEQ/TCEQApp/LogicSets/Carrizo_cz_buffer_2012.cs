using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCEQApp.LogicSets
{
    public class Carrizo_cz_buffer_2012 : BaseLogicSet, ILogicSet
    {
        public override object GetFactoryKey()
        {
            return "Carrizo_cz_buffer_2012";
        }

        protected override void getData()
        {
            //
            base.getData();

            double czTop = g.queryRaster(_coords, "sde.sde.TCEQ_CARRIZO_TOP");

            _topmsl = czTop;
            _topgsfc = elevation - czTop;

            

            string sTopMSLAbove = AboveBelow(topmsl);
            string sBotMSLAbove = AboveBelow(botmsl);

            //Do anything else.
            _topFWIZCaption = "Top of Carrizo Isolation Interval";
            _aquiferFieldCaption = "Aquifer (at BUQ)";
            _alternateTopSentence = "The top of the Carrizo isolation interval is at " + Decimal.Round(new Decimal(topgsfc), 0).ToString() + " feet (" + Decimal.Round(new Decimal(Math.Abs(topmsl)), 0).ToString() + " feet " + sTopMSLAbove + " MSL).<br /><br />";
            _alternateTopSentence += "The bottom of the fresh water isolation zone occurs at " + Decimal.Round(new Decimal(botgsfc), 0).ToString() + " feet (" + Decimal.Round(new Decimal(Math.Abs(botmsl)), 0).ToString() + " feet " + sBotMSLAbove + " MSL).";
        }
    }
}