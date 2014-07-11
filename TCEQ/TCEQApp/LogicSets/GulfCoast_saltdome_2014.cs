using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCEQApp.LogicSets
{
    public class GulfCoast_saltdome_2014 : BaseLogicSet, ILogicSet
    {
        public override object GetFactoryKey()
        {
            return "GulfCoast_saltdome_2014";
        }

        protected override void getData()
        {
            //
            base.getData();

            _showTopFWIZRow = false;
            _botFWIZValueString = "N/A";
        }
    }
}