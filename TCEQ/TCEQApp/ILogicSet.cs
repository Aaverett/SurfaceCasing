using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public interface ILogicSet
{
    object GetFactoryKey();

    HtmlGenericControl GenerateOutputControl();

    ArcGISRESTClient.ArcGISRESTClient RestClient
    {
        set;
        get;
    }

    ArcGISRESTClient.Geometry.Point coords
    {
        set;
        get;
    }

    double depth10K
    {
        get;
    }

    double depth3K
    {
        get;
    }

    double depth1K
    {
        get;
    }

    double elevation
    {
        get;
    }

    double topgsfc
    {
        get;
    }

    double topmsl
    {
        get;
    }

    double botmsl
    {
        get;
    }

    double botgsfc
    {
        get;
    }

    double pbuqwgsfc
    {
        get;
    }

    double usdwmsl
    {
        get;
    }

    double usdwgsfc
    {
        get;
    }

    double latitude
    {
        get;
    }

    double longitude
    {
        get;
    }

    string aquifername
    {
        get;
    }
}