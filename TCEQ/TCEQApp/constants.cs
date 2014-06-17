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
/// Summary description for constants
/// </summary>
public class constants
{
    //This is the hostname of the server that is running ArcGIS server.
    public static string SERVER_NAME = "igor";

    //This is the name of the service running on the GIS server
    public static string SERVICE_NAME = "tceq";

    //This is the name of the data frame within the map service that we need to connect to.
    public static string DATA_FRAME_NAME = "(default)";

    public static int MAP_CLICK_FUDGE_DISTANCE = 6;

    //Labels (Stuff the user sees)
    public static string MSL_LABEL_ABOVE = "Above";
    public static string MSL_LABEL_BELOW = "Below";

    public static string CASINGINFO_TABLE_HEADING = "Well Casing Information";
    public static string PROTECT_MESSAGE = "Protect from the land surface to a depth of 100 feet.";

    public static int MAP_COORDINATE_SYSTEM_ID = 102603;
    public static int GCS_COORDINATE_SYSTEM_ID = 4019;

    //This is the unit of measure that coordinates on the map are given in.
    public static ESRI.ArcGIS.esriSystem.esriUnits MAP_UNITS = ESRI.ArcGIS.esriSystem.esriUnits.esriMeters;
    public static ESRI.ArcGIS.esriSystem.esriUnits DEFAULT_DISPLAY_UNITS = ESRI.ArcGIS.esriSystem.esriUnits.esriMiles;
    public static int DEFAULT_MAP_LAYER = 0; //The surveys layer is the default.

    public static string STANDARD_NOTE = "Note:  This is an approximate recommendation and is subject to confirmation by a <a href=\"http://www.rrc.state.tx.us/environmental/environsupport/gau/index.php\" target=\"_blank\" title=\"TCEQ Website\">RRC Groundwater Advisory Unit</a> (formerly TCEQ Surface Casing) geologist.  These recommendations include an isolation buffer.";
}
