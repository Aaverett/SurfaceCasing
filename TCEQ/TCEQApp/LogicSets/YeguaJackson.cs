using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

/// <summary>
/// Summary description for YeguaJackson
/// </summary>
public class YeguaJackson : BaseLogicSet, ILogicSet
{
    #region ILogicSet Members

    /// <summary>
    /// This returns the key that is used to figure out what class to create by the class factory.  This value must be stored in the DB.
    /// </summary>
    /// <returns></returns>
    public override object GetFactoryKey()
    {
        return "YeguaJackson";
    }

    #endregion

    protected override void getData()
    {
        //Perform the basic getData logic.
        base.getData();

        //With the data values retrieved from the DB, we can calculate our output vals.
        _topmsl = elevation;
        _topgsfc = 0;
    }
}
