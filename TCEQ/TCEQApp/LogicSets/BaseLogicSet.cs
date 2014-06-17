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
/// Summary description for BaseLogicSet
/// </summary>
public class BaseLogicSet : ILogicSet
{
    //Data members used for the basic calculations.
    //Note:  The name depth is misleading.  The numbers are actually relative to mean sea level.

    protected double _depth10k = 0;
    protected double _depth3k = 0;
    protected double _depth1kTop = 0;
    protected double _depth1kBottom = 0;
    protected double _elevation = 0;
    protected double _topmsl = 0;
    protected double _topgsfc = 0;
    protected double _botmsl = 0;
    protected double _botgsfc = 0;
    protected double _pbuqwmsl = 0;
    protected double _pbuqwgsfc = 0;
    protected double _usdwmsl = 0;
    protected double _usdwgsfc = 0;
    protected double _latitude = 0;
    protected double _longitude = 0;
    protected string _aquifername = string.Empty;    
    protected BEGWebAppLib.GISHandler _g;
    protected ESRI.ArcGIS.Geometry.IPoint _coords;

    protected string _aquiferFieldCaption = "Aquifer";
    protected string _topFWIZCaption = "Top of fresh water isolation zone";
    protected string _botFWIZCaption = "Bottom of fresh water isolation zone";
    protected string _pbuqwCaption = "Protected base of usable quality water";
    protected string _baseUSDWCaption = "Base of USDW";

    protected string _aquiferValueString = null;
    protected string _topFWIZValueString = null;
    protected string _botFWIZValueString = null;
    protected string _pbuqwValueString = null;
    protected string _baseUSDWValueString = null;

    protected bool _showAquiferRow = true;
    protected bool _showTopFWIZRow = true;
    protected bool _showBotFWIZRow = true;
    protected bool _showPBUQWRow = true;
    protected bool _showBaseUSDWRow = true;

    protected bool _doTopSentence = true;
    protected bool _doPBUQWSentence = true;
    protected bool _doBaseUSDWSentence = true;

    protected string _alternateTopSentence = null;
    protected string _alternatePBUQWSentence = null;
    protected string _alternateUSDBSentence = null;

    protected System.Web.UI.WebControls.Table _resultsTable = null;
    protected System.Web.UI.HtmlControls.HtmlGenericControl _resultsDiv;

    public BaseLogicSet()
    {
       
    }

    /// <summary>
    /// This is used to set up an error message in the control c.
    /// </summary>
    /// <param name="c">The control to be filled with the error message.</param>
    /// <param name="message">The message to show to the user.</param>
    protected void SetupErrorMessage(HtmlGenericControl c, string message)
    {

    }

    /// <summary>
    /// This retrieves the necessary information from the GDB.
    /// </summary>
    protected virtual void getData()
    {
        //Now we need to begin collecting the data necessary to generate our output div.
        _elevation = g.queryRaster(_coords, "sde.SDE.tceq_elev");
        _depth10k = g.queryRaster(_coords, "sde.SDE.tceq_10k");
        _depth3k = g.queryRaster(_coords, "sde.SDE.tceq_3k");
        _depth1kTop = g.queryRaster(_coords, "sde.SDE.tceq_tops");
        _depth1kBottom = g.queryRaster(_coords, "sde.SDE.tceq_1k");
        

         //There's one feature that we need to retrieve as well.
        DataTable aqs = g.performBaseFeatureDataQuery(string.Empty, (ESRI.ArcGIS.Geometry.IGeometry)_coords, ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelWithin, "sde.SDE.TCEQ_Aquifers");
        if (aqs != null && aqs.Rows.Count > 0)
        {
            _aquifername = (string)aqs.Rows[0]["Name"];
        }

        //To display the coordinates in lat/long, we need to reproject our feature to a GCS coord system.
        ESRI.ArcGIS.Geometry.IGeometry ig_projected = g.unprojectGeometry((ESRI.ArcGIS.Geometry.IGeometry)_coords, 102603, 4019);
        ESRI.ArcGIS.Geometry.IPoint ip_projected = (ESRI.ArcGIS.Geometry.IPoint)ig_projected;

        _latitude = ip_projected.Y;
        _longitude = ip_projected.X;

        //With the data values retrieved from the DB, we can calculate our output vals.
        _topmsl =  _depth1kTop;
        _topgsfc = elevation - _depth1kTop;

        _botmsl = _depth1kBottom;
        _botgsfc = elevation - _depth1kBottom;
        
        //The "Protected base of usable quality water.
        _pbuqwmsl =  _depth3k;
        _pbuqwgsfc = elevation - _depth3k;
        
        //The base of USDW
        _usdwmsl = _depth10k;
        _usdwgsfc = elevation - _depth10k;
    
    }

    #region ILogicSet Members

    public virtual object GetFactoryKey()
    {
        return "default";
    }

    protected string AboveBelow(double val)
    {
        if (val < 0) return constants.MSL_LABEL_BELOW;

        return constants.MSL_LABEL_ABOVE;
    }

    public virtual HtmlGenericControl GenerateOutputControl()
    {
        //This will be the control that gets returned.
        HtmlGenericControl resultsdiv = new HtmlGenericControl("div");
        resultsdiv.Attributes.Add("class", "detailsdiv");

        //Perform sanity checking on the data.
        /*if(_depth1kTop < _depth1kBottom)
        {
            resultsdiv.InnerHtml = "<div style=\"padding: 10px;\"><p>The location you have selected is in an area where the depth at which one or more aquifers occurs could not be computed accurately.  Please contact TCEQ for help.</p></div>";

            return resultsdiv;
        }*/

        //Label strings - we go ahead and initialize these here because...  We have no obvious reason not to.
        string sTopMSLAbove = AboveBelow(topmsl);
        string sBotMSLAbove = AboveBelow(botmsl);
        string sPBUQWMSLAbove = AboveBelow(_pbuqwmsl);
        string sUSDWMSLAbove = AboveBelow(_usdwmsl);
       

        //Now we're ready to compose our results into a set of controls so that we can have ASP.NET display them.
        Table t = new Table();
        t.CssClass = "resulttable";
        t.ID = "resulttable";
        TableRow headingrow = new TableRow();
        TableHeaderCell thc = new TableHeaderCell();
        thc.Text = constants.CASINGINFO_TABLE_HEADING;
        thc.ColumnSpan = 2;
        headingrow.Cells.Add(thc);
        t.Rows.Add(headingrow);

        TableRow longitudeRow = new TableRow();
        longitudeRow.ID = "longitudeRow";
        TableRow latitudeRow = new TableRow();
        latitudeRow.ID = "latitudeRow";
        TableRow aquiferRow = new TableRow();
        aquiferRow.ID = "aquiferRow";
        TableRow fwizTopRow = new TableRow();
        fwizTopRow.ID = "fwizTopRow";
        TableRow fwizBotRow = new TableRow();
        fwizBotRow.ID = "fwizBotRow";
        TableRow pbuqwRow = new TableRow();
        pbuqwRow.ID = "pbuqwRow";
        TableRow usdwRow = new TableRow();
        usdwRow.ID = "usdwRow";

        //The longitude row
        TableCell longitudeLabelCell = new TableCell();
        longitudeLabelCell.ID = "longitudeLabelCell";
        longitudeLabelCell.Text = "Longitude";
        TableCell longitudeValueCell = new TableCell();
        longitudeValueCell.ID = "longitudeValueCell";
        longitudeValueCell.Text = Math.Round(longitude, 6).ToString();
        longitudeRow.Cells.Add(longitudeLabelCell);
        longitudeRow.Cells.Add(longitudeValueCell);

        //The latitude row
        TableCell latitudeLabelCell = new TableCell();
        latitudeLabelCell.ID = "latitudeLabelCell";
        latitudeLabelCell.Text = "Latitude";
        TableCell latitudeValueCell = new TableCell();
        latitudeValueCell.ID = "latitudeLabelCell";
        latitudeValueCell.Text = Math.Round(latitude, 6).ToString();
        latitudeRow.Cells.Add(latitudeLabelCell);
        latitudeRow.Cells.Add(latitudeValueCell);

        //The aquifer row
        if (_showAquiferRow)
        {
            TableCell aquiferLabelCell = new TableCell();
            aquiferLabelCell.ID = "aquiferLabelCell";
            TableCell aquiferValueCell = new TableCell();
            aquiferValueCell.ID = "aquiferValueCell";
            aquiferLabelCell.Text = _aquiferFieldCaption;
            if (_aquiferValueString != null) aquiferValueCell.Text = _aquiferValueString;
            else aquiferValueCell.Text = aquifername;
            aquiferRow.Cells.Add(aquiferLabelCell);
            aquiferRow.Cells.Add(aquiferValueCell);
        }

        //The Fresh water isolation zone top row
        if (_showTopFWIZRow)
        {
            TableCell fwizTopLabelCell = new TableCell();
            fwizTopLabelCell.ID = "fwizTopLabelCell";
            TableCell fwizTopValueCell = new TableCell();
            fwizTopValueCell.ID = "fwizTopValueCell";
            fwizTopLabelCell.Text = _topFWIZCaption;
            if (_topFWIZValueString != null) fwizTopValueCell.Text = _topFWIZValueString;
            else fwizTopValueCell.Text = Math.Round(Math.Abs(topgsfc), 0).ToString() + "' Depth (" + Math.Round(Math.Abs(topmsl), 0).ToString() + "' " + sTopMSLAbove + " MSL)";
            
            fwizTopRow.Cells.Add(fwizTopLabelCell);
            fwizTopRow.Cells.Add(fwizTopValueCell);
        }

        //The Fresh water isolation zone bottom row
        if (_showBotFWIZRow)
        {
            TableCell fwizBotLabelCell = new TableCell();
            fwizBotLabelCell.ID = "fwizBotLabelCell";
            TableCell fwizBotValueCell = new TableCell();
            fwizBotValueCell.ID = "fwizBotValueCell";
            fwizBotLabelCell.Text = _botFWIZCaption;
            if (_botFWIZValueString != null) fwizBotValueCell.Text = _botFWIZValueString;
            else fwizBotValueCell.Text = Math.Round(Math.Abs(botgsfc), 0).ToString() + "' Depth (" + Math.Round(Math.Abs(botmsl), 0).ToString() + "' " + sBotMSLAbove + " MSL)";
            fwizBotRow.Cells.Add(fwizBotLabelCell);
            fwizBotRow.Cells.Add(fwizBotValueCell);
        }

        //Protected base of usable quality qater row
        if (_showPBUQWRow)
        {
            TableCell pbuqwLabelCell = new TableCell();
            pbuqwLabelCell.ID = "pbuqwLabelCell";
            TableCell pbuqwValueCell = new TableCell();
            pbuqwValueCell.ID = "pbuqwValueCell";
            pbuqwLabelCell.Text = _pbuqwCaption;
            if (_pbuqwValueString != null) pbuqwValueCell.Text = _pbuqwValueString;
            else pbuqwValueCell.Text = Math.Round(Math.Abs(pbuqwgsfc), 0).ToString() + "' Depth (" + Math.Round(Math.Abs(pbuqwmsl), 0).ToString() + "' " + sPBUQWMSLAbove + " MSL)";
            pbuqwRow.Cells.Add(pbuqwLabelCell);
            pbuqwRow.Cells.Add(pbuqwValueCell);
        }

        //USDW row
        if (_showBaseUSDWRow)
        {
            TableCell usdwLabelCell = new TableCell();
            usdwLabelCell.ID = "usdwLabelCell";
            TableCell usdwValueCell = new TableCell();
            usdwValueCell.ID = "usdwValueCell";
            usdwLabelCell.Text = _baseUSDWCaption;
            if (_baseUSDWValueString != null) usdwValueCell.Text = _baseUSDWValueString;
            else usdwValueCell.Text = Math.Round(Math.Abs(usdwgsfc), 0).ToString() + "' Depth (" + Math.Round(Math.Abs(usdwmsl), 0).ToString() + "' " + sUSDWMSLAbove + " MSL)";
            usdwRow.Cells.Add(usdwLabelCell);
            usdwRow.Cells.Add(usdwValueCell);
        }

        /*//Ground Elevation - for shits and giggles
        TableRow elevRow = new TableRow();
        elevRow.ID = "elevRow";
        TableCell elevLabelCell = new TableCell();
        elevRow.ID = "elevLabelCell";
        TableCell elevValueCell = new TableCell();
        elevRow.ID = "elevValueCell";
        elevLabelCell.Text = "Ground Elevation";
        elevValueCell.Text = Math.Round(Math.Abs(elevation),0).ToString();
        elevRow.Cells.Add(elevLabelCell);
        elevRow.Cells.Add(elevValueCell);*/

        //Add all the rows to the table.
        t.Rows.Add(longitudeRow);
        t.Rows.Add(latitudeRow);
        //t.Rows.Add(elevRow);
        if(_showAquiferRow) t.Rows.Add(aquiferRow);
        if(_showTopFWIZRow) t.Rows.Add(fwizTopRow);
        if(_showBotFWIZRow) t.Rows.Add(fwizBotRow);
        if(_showPBUQWRow) t.Rows.Add(pbuqwRow);
        if(_showBaseUSDWRow) t.Rows.Add(usdwRow);

        resultsdiv.Controls.Add(t);

        //Finally, we need to compose the paragraph form thing.
        string fwizstring, pbuqwstring, usdwstring;

        //This is like option zero from the year 3 TCEQ app.
        //Here, we compose the output strings.
        fwizstring = "The fresh water contained in the " + aquifername + " aquifer unit, which is estimated to occur from a depth of " + Decimal.Round(new Decimal(topgsfc), 0).ToString() + " feet (" + Decimal.Round(new Decimal(Math.Abs(topmsl)), 0).ToString() + " feet " + sTopMSLAbove + " MSL) to " + Decimal.Round(new Decimal(botgsfc), 0).ToString() + " feet (" + Decimal.Round(new Decimal(Math.Abs(botmsl)), 0).ToString() + " feet " + sBotMSLAbove + " MSL), must be isolated from the water above and below.";

        pbuqwstring = "The base of usable quality water is estimated to occur  at a depth of " + Decimal.Round(new Decimal(pbuqwgsfc), 0).ToString() + " feet (" + Decimal.Round(new Decimal(Math.Abs(pbuqwmsl)), 0).ToString() + " feet " + sPBUQWMSLAbove + " MSL).";

        usdwstring = "The USDW (10,000 ppm TDS) is estimated to occur at a depth of " + Decimal.Round(new Decimal(usdwgsfc), 0).ToString() + " feet (" + Decimal.Round(new Decimal(Math.Abs(usdwmsl)), 0).ToString() + "' " + sUSDWMSLAbove + " MSL). ";

        //Now we need to create controls to house these.

        if(_doTopSentence)
        {
            HtmlGenericControl fwizparagraph = new HtmlGenericControl("p");
            fwizparagraph.ID = "fwizparagraph";
            if (_alternateTopSentence != null) fwizparagraph.InnerHtml= _alternateTopSentence;
            else fwizparagraph.InnerText = fwizstring;
            resultsdiv.Controls.Add(fwizparagraph);
        }

        if (_doPBUQWSentence)
        {
            HtmlGenericControl pbuqwparagraph = new HtmlGenericControl("p");
            pbuqwparagraph.ID = "pbuqwparagraph";
            if (_alternatePBUQWSentence != null) pbuqwparagraph.InnerHtml = _alternatePBUQWSentence;
            else pbuqwparagraph.InnerText = pbuqwstring;
            resultsdiv.Controls.Add(pbuqwparagraph);
        }


        if (_doBaseUSDWSentence)
        {
            HtmlGenericControl usdwparagraph = new HtmlGenericControl("p");
            usdwparagraph.ID = "usdwparagraph";
            if (_alternateUSDBSentence != null) usdwparagraph.InnerHtml = _alternateUSDBSentence;
            else usdwparagraph.InnerText = usdwstring;
            resultsdiv.Controls.Add(usdwparagraph);
        }

        HtmlGenericControl disclaimerparagraph = new HtmlGenericControl("p");
        disclaimerparagraph.ID = "disclaimerparagraph";
        disclaimerparagraph.InnerHtml = constants.STANDARD_NOTE;
        resultsdiv.Controls.Add(disclaimerparagraph);

        //Hang on to handles on these things, in case a subclass needs them.
        _resultsDiv = resultsdiv;
        _resultsTable = t;

        //Return our results div.
        return resultsdiv;
    }

    public virtual ESRI.ArcGIS.Geometry.IPoint coords
    {
        get
        {
            return _coords;
        }

        set
        {
            //Save the value we were passed.
            _coords = value;

            //Retrieve the basic data from the DB.
            getData();
        }
    }

    public virtual double depth10K
    {
        get
        {
            return _depth10k;
        }
    }

    public virtual double depth3K
    {
        get
        {
            return _depth3k;
        }
    }

    public virtual double depth1K
    {
        get
        {
            return _depth1kBottom;
        }
    }

    public virtual double elevation
    {
        get 
        {
            return _elevation;
        }
    }

    public virtual double topgsfc
    {
        get
        {
            return _topgsfc;
        }
    }

    public virtual double topmsl
    {
        get 
        {
            return _topmsl;
        }
    }

    public virtual double botmsl
    {
        get 
        {
            return _botmsl;
        }
    }

    public virtual double botgsfc
    {
        get 
        {
            return _botgsfc;
        }
    }

    public virtual double pbuqwgsfc
    {
        get
        {
            return _pbuqwgsfc;
        }
    }

    public virtual double pbuqwmsl
    {
        get
        {
            return _pbuqwmsl;
        }
    }

    public virtual double usdwmsl
    {
        get 
        {
            return _usdwmsl;
        }
    }

    public virtual double usdwgsfc
    {
        get
        {
            return _usdwgsfc;
        }
    }

    /// <summary>
    /// This is the GISHandler used to talk back and forth with the GDB.
    /// </summary>
    public virtual BEGWebAppLib.GISHandler g
    {
        get
        {
            if (_g == null)
            {
                _g = new BEGWebAppLib.GISHandler(constants.SERVICE_NAME, constants.SERVER_NAME, constants.DATA_FRAME_NAME);
            }

            return _g;
        }

        set
        {
            _g = value;
        }
    }

    public virtual double latitude
    {
        get
        {
            return _latitude;
        }
    }

    public virtual double longitude
    {
        get
        {
            return _longitude;
        }
    }

    public virtual string aquifername
    {
        get
        {
            return _aquifername;
        }
    }


    #endregion
}
