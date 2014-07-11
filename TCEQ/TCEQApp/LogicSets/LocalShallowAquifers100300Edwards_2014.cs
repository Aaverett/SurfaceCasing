using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCEQApp.LogicSets
{
    public class LocalShallowAquifers100300Edwards_2014 : LocalShallowAquifers100_2013, ILogicSet
    {
        public override object GetFactoryKey()
        {
            return "LocalShallowAquifers100300Edwards_2014";
        }

        protected override void getData()
        {
            base.getData();

            _alternateTopSentence = "Protect to 100 feet depth.  Local areas should be protected to 300 feet.  Depth to base of Edwards Aquifer is between 800 and 1100 feet.";
        }
    }
}