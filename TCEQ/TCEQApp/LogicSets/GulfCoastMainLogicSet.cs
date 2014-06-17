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
/// Summary description for GulfCoastMainLogicSet
/// </summary>
public class GulfCoastMainLogicSet : BaseLogicSet, ILogicSet
{
    public GulfCoastMainLogicSet()
    {
    }

    public override object GetFactoryKey()
    {
        return "GulfCoastMain";
    }

    public override double  topgsfc
    {
	    get 
	    {
            return 0;
	    }
    }

    public override double topmsl
    {
        get
        {
            return elevation;
        }
    }
}
