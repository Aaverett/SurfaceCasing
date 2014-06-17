using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ESRI.ArcGIS.ADF.Web.UI.WebControls;
using ESRI.ArcGIS.ADF.Web.UI.WebControls.Tools;

/// <summary>
/// Summary description for PrintMapCommand
/// </summary>
public class PrintMapCommand : IMapServerCommandAction
{

    #region IServerAction Members

    public void ServerAction(ToolbarItemInfo info)
    {
        Map m = (Map)info.BuddyControls[0];

        try
        {
            

            ESRI.ArcGIS.ADF.Web.Geometry.Envelope env = m.Extent;

            HttpContext.Current.Session["printextent"] = env;
        }
        catch
        {

        }
        ESRI.ArcGIS.ADF.Web.UI.WebControls.CallbackResult cr = null;

        HtmlGenericControl hgc = new HtmlGenericControl("script");

        //hgc.InnerHtml = "<script language=\"javascript\">openPrintViewer();</script>";

        string js = "openPrintViewer();";

        cr = TCEQFuncs.doJavascriptCallback(js);

        m.CallbackResults.Add(cr);

        //m.Refresh();
    }

    #endregion
}
