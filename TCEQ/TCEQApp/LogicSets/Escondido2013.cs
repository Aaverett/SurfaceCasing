using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCEQApp.LogicSets
{
    public class Escondido2013 : BaseLogicSet, ILogicSet
    {
        public override object GetFactoryKey()
        {
            return "Escondido2013";
        }

        protected override void getData()
        {
            base.getData();

            _botFWIZValueString = "N/A";
            _baseUSDWValueString = "N/A";
            _showTopFWIZRow = false;

            _alternateTopSentence = string.Empty;
            _alternateUSDBSentence = string.Empty;
        }
    }
}