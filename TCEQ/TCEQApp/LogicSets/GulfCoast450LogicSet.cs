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
/// Summary description for GulfCoast450LogicSet
/// </summary>
public class GulfCoast450LogicSet : GulfCoast350LogicSet, ILogicSet
{
    public GulfCoast450LogicSet()
    {
        _protectdepth = 450;
    }

    #region ILogicSet Members

    object ILogicSet.GetFactoryKey()
    {
        return "GulfCoast450";
    }

    #endregion
}
