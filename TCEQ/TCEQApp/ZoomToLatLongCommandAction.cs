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
using ESRI.ArcGIS.ADF.Web.Geometry;

/// <summary>
/// Summary description for ZoomToLatLongCommandAction
/// </summary>
public class ZoomToLatLongCommandAction : IMapServerCommandAction
{

    public void ServerAction(ToolbarItemInfo info)
    {
        //When the button is clicked, we need to get a handle on the
        Page p = info.Toolbar.Page;

        //Here, we grab a handle on the contents div, which will be used to display our form.
        Control c = p.FindControl("setToCoordsDiv");
        if (!(c is HtmlGenericControl))
        {
            //If it's not the right kind of control, we abort.
            return;
        }

        //Cast to the HtmlGenericControl type
        HtmlGenericControl contentsdiv = (HtmlGenericControl)c;
        
        //Set the CSS class on the contents div
        contentsdiv.Attributes.Add("class", "detailsdiv");

        //Generate the callback result
        ESRI.ArcGIS.ADF.Web.UI.WebControls.CallbackResult cr = TCEQFuncs.updateControl("div", "contentsdiv", contentsdiv);
       
        Map m = (Map)info.BuddyControls[0];

        //Add the callbackresult to the map object, which will pass it to the page.
        m.CallbackResults.Add(cr);
    }
}
