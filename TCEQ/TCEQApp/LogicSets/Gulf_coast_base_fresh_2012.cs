using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCEQApp.LogicSets
{
    public class Gulf_coast_base_fresh_2012 : BaseLogicSet, ILogicSet
    {
        public override object GetFactoryKey()
        {
            return "Gulf_coast_base_fresh_2012";
        }

        protected override void getData()
        {
            //
            base.getData();

            //Do anything else.

            _showTopFWIZRow = false;
            _aquiferFieldCaption = "Aquifer (at BUQ)";
            //_doTopSentence = false;

            string sBotMSLAbove = AboveBelow(botmsl);

            _alternateTopSentence = "The bottom of the fresh water isolation zone occurs at " + Decimal.Round(new Decimal(botgsfc), 0).ToString() + " feet (" + Decimal.Round(new Decimal(Math.Abs(botmsl)), 0).ToString() + " feet " + sBotMSLAbove + " MSL).";
        }
    }
}