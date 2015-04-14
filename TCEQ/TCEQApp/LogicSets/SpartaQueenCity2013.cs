using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCEQApp.LogicSets
{
    public class SpartaQueenCity2013 : BaseLogicSet, ILogicSet
    {
        public override object GetFactoryKey()
        {
            return "SpartaQueenCity2013";
        }

        protected override void getData()
        {
            base.getData();

            _botFWIZValueString = "N/A";
        }
    }
}