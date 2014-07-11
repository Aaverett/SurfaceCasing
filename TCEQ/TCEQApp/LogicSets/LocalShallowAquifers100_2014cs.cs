using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCEQApp.LogicSets
{
    public class LocalShallowAquifers100_2014cs : LocalShallowAquifers100_2013, ILogicSet
    {
        public override object GetFactoryKey()
        {
            return "LocalShallowAquifers100_2014";
        }

        protected override void getData()
        {
            base.getData();

            _alternateTopSentence = "Protect to 100 feet depth.";
        }
    }
}