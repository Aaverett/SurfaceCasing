using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TCEQApp
{
    public partial class printmap : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                double lat, lon;

                int zoomLevel;

                if (Page.Request["lat"] != null && Page.Request["lat"] != string.Empty)
                {
                    lat = System.Convert.ToDouble(Page.Request["lat"]);
                }
                else lat = 30.0;

                if (Page.Request["lon"] != null && Page.Request["lon"] != string.Empty)
                {
                    lon = System.Convert.ToDouble(Page.Request["lon"]);
                }
                else lon = -97.5;

                if (Page.Request["z"] != null && Page.Request["z"] != string.Empty)
                {
                    zoomLevel = System.Convert.ToInt32(Page.Request["z"]);
                }
                else zoomLevel = 8;

                //Now, generate the pre init script.
                string preinit = "<script language=\"javascript\" type=\"text/javascript\">";
                
                preinit += "var initialLat = " + lat + ";\r\n";
                preinit += "var initialLon = " + lon + ";\r\n";
                preinit += "var zoomLevel = " + zoomLevel + ";\r\n";

                if (Page.Request["mkrlat"] != null && Page.Request["mkrlat"] != string.Empty)
                {
                    preinit += "var mkrlat = " + System.Convert.ToDouble(Page.Request["mkrlat"]).ToString() + ";\r\n";
                }
                else preinit += "var mkrlat = null;";

                if (Page.Request["mkrlon"] != null && Page.Request["mkrlon"] != string.Empty)
                {
                    preinit += "var mkrlon = " + System.Convert.ToDouble(Page.Request["mkrlon"]).ToString() + ";\r\n";
                }
                else preinit += "var mkrlon = null;";

                preinit += "</script>";

                ClientScript.RegisterClientScriptBlock(typeof(String), "preinit", preinit);
                //RegisterClientScriptBlock("preinit", preinit);

                /*string contentcode = string.Empty;

                if(Request["contentcode"] != null && Request["contentcode"] != "")
                {
                    contentcode = HttpUtility.UrlDecode(Request["contentcode"]);
                }*/

                //contentdiv.InnerHtml = contentcode;
            }
        }
    }
}