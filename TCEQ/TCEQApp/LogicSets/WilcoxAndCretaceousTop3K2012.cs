using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;

namespace TCEQApp.LogicSets
{
    public class WilcoxAndCretaceousTop3K2012 : BaseLogicSet, ILogicSet
    {

        #region ILogicSet Members

        object ILogicSet.GetFactoryKey()
        {
            return "WilcoxAndCretaceousTop3K2012";
        }

        /// <summary>
        /// This retrieves the necessary information from the GDB.
        /// </summary>
        protected override void getData()
        {
            //Now we need to begin collecting the data necessary to generate our output div.
            FetchBasicValues();

            //With the data values retrieved from the DB, we can calculate our output vals.
            //_topmsl = _depth1kTop;
            //_topgsfc = elevation - _depth1kTop;

            _topmsl = elevation;
            _topgsfc = 0;

            _botmsl = _depth1kBottom;
            _botgsfc = elevation - _depth1kBottom;

            //The "Protected base of usable quality water.
            _pbuqwmsl = _depth3k;
            _pbuqwgsfc = elevation - _depth3k;

            //The base of USDW
            _usdwmsl = _depth10k;
            _usdwgsfc = elevation - _depth10k;

            _doTopSentence = false;
            _botFWIZValueString = "See base of usable quality water";

        }

        #endregion
    }
}