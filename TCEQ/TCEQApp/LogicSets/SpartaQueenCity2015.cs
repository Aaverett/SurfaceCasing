using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCEQApp.LogicSets
{
    public class SpartaQueenCity2015 : BaseLogicSet
    {
        public override object GetFactoryKey()
        {
            return "SpartaQueenCity2015";
        }

        protected override void getData()
        {
            base.getData();

            _topFWIZCaption = "Top of Sparta Isolation Zone";
            string sTopMSLAbove = AboveBelow(topmsl);
            string sBotMSLAbove = AboveBelow(botmsl);
            _alternateTopSentence = "The top of the Sparta isolation zone occurs at " + Decimal.Round(new Decimal(topgsfc), 0).ToString() + " feet (" + Decimal.Round(new Decimal(Math.Abs(topmsl)), 0).ToString() + " feet " + sTopMSLAbove + " MSL). <br />";
            _alternateTopSentence += "The bottom of the fresh water isolation zone occurs at " + Decimal.Round(new Decimal(botgsfc), 0).ToString() + " feet (" + Decimal.Round(new Decimal(Math.Abs(botmsl)), 0).ToString() + " feet " + sBotMSLAbove + " MSL). <br />"; ;

        }
    }
}