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
/// Summary description for GulfCoast550LogicSet
/// </summary>
public class GulfCoast550LogicSet : GulfCoast350LogicSet, ILogicSet
{
    public GulfCoast550LogicSet()
    {
        _protectdepth = 550;
    }

    public override object GetFactoryKey()
    {
        return "GulfCoast550";
    }
}
