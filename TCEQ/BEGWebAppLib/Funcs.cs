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

namespace BEGWebAppLib
{
/// <summary>
/// Summary description for Funcs
/// </summary>
    public class Funcs
    {
        /// <summary>
        /// This method generates a callbackresult that can be passed to a control during an asynchronous call, allowing a web control to be updated without refreshing the entire page.
        /// </summary>
        /// <param name="targetID">The ID value of the HTML entity that will recieve the content.</param>
        /// <param name="c">The control you want to update.</param>
        /// <returns>Callback result containg the HTML for control C as it exists right now.</returns>
        public static ESRI.ArcGIS.ADF.Web.UI.WebControls.CallbackResult updateControl(string targettag, string targetID, Control c)
        {
            //Create our html writer.  This will be used to generate the HTML for our control.
            System.IO.StringWriter sw = new System.IO.StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            c.RenderControl(hw);
            hw.Flush();
            string htmlcode = sw.ToString();

            //These two steps generate a callbackresult object.  This gets passed back to the browser, and drawn on the page.                                        
            CallbackResult cr = new CallbackResult(targettag, targetID, "content", htmlcode);

            return cr;
        }
    }
}
