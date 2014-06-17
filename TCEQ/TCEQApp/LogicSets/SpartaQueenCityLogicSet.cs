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
/// Summary description for SpartaQueenCityLogicSet
/// </summary>
public class SpartaQueenCityLogicSet : BaseLogicSet, ILogicSet
{
    public SpartaQueenCityLogicSet()
    {

    }


    #region ILogicSet Members

    /// <summary>
    /// This returns the key that is used to figure out what class to create by the class factory.  This value must be stored in the DB.
    /// </summary>
    /// <returns></returns>
    public override object GetFactoryKey()
    {
        return "Sparta-Queen City";
    }

    #endregion

    
}
