using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCEQApp.LogicSets
{
    public class CarrizoWilcox_nofwiz_2014 : BaseLogicSet, ILogicSet
    {
        public override object GetFactoryKey()
        {
            return "CarrizoWilcox_nofwiz_2014";
        }

        protected override void getData()
        {
            //
            base.getData();

            _showTopFWIZRow = false;
            _doTopSentence = false;
        }
    }
}