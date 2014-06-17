using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;


namespace TCEQApp.LogicSets
{
    public class CarrizoWilcox2013 : Carrizo_cz_buffer_2012, ILogicSet
    {
        public override object GetFactoryKey()
        {
            return "CarrizoWilcox2013";
        }

        protected override void getData()
        {
            //
            base.getData();

            //Hijack the Top of fresh water boxes.
            _topFWIZCaption = "Top of Carrizo-Wilcox Isolation Interval";
            _topmsl = _elevation;
            _topgsfc = 0;

            _botFWIZValueString = "(N/A)";

        }

        public override System.Web.UI.HtmlControls.HtmlGenericControl GenerateOutputControl()
        {
            HtmlGenericControl hgc = base.GenerateOutputControl();

            return hgc;
        }
    }
}