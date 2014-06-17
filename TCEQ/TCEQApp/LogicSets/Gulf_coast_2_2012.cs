using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace TCEQApp.LogicSets
{
    public class Gulf_coast_2_2012 : BaseLogicSet, ILogicSet
    {
        public override object GetFactoryKey()
        {
            return "Gulf_coast_2_2012";
        }

        protected override void getData()
        {
            //
            base.getData();

            //Do anything else.

            _showTopFWIZRow = false;
            _botFWIZValueString = "N/A";
            _aquiferFieldCaption = "Aquifer (at BUQ)";
            _doTopSentence = false;
        }
    }
}