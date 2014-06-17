using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// This is the logic for the Creataceous/Permian logic zone from the year 6 iteration.
/// </summary>
public class CretaceousPennsylvanian_year6 : BaseLogicSet, ILogicSet
{

    public override object GetFactoryKey()
    {
        return "CretaceousPennsylvanian_year6";
    }

    protected override void getData()
    {
        base.getData();

        _topgsfc = 0;
        _topmsl = elevation;
    }
}
