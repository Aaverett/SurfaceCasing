using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCEQApp.LogicSets
{
    public class LocalShallowAquifers100_2013 : BaseLogicSet, ILogicSet
    {
        public override object GetFactoryKey()
        {
            return "LocalShallowAquifers100_2013";
        }

        protected override void getData()
        {
            base.getData();

            _showBaseUSDWRow = false;
            _showTopFWIZRow = false;
            _showPBUQWRow = false;
            _showBotFWIZRow = false;

            _alternateTopSentence = "Protect to a minimum of 100'.  Local areas should be protected to 250'.";
            _alternatePBUQWSentence = "";
            _alternateUSDBSentence = "";

        }
    }
}