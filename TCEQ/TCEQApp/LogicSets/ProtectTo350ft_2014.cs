using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCEQApp.LogicSets
{
    public class ProtectTo350ft_2014 : BaseLogicSet
    {
        public override object GetFactoryKey()
        {
            return "ProtectTo350Ft_2014";
        }

        protected override void getData()
        {
            base.getData();

            _showBaseUSDWRow = false;
            _showTopFWIZRow = false;
            _showPBUQWRow = false;
            _showBotFWIZRow = false;

            _alternateTopSentence = "Protect to a depth of 350ft.";
            _alternatePBUQWSentence = "";
            _alternateUSDBSentence = "";

        }
    }
}