<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="printmap.aspx.cs" Inherits="TCEQApp.printmap" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TCEQ Surface Casing Estimator</title>
    <link rel="Stylesheet" href="style/print.css" />
    <link rel="stylesheet" href="style/print_print.css" media="print"/>
    <script language="javascript" src="scripts/tceq.js"></script>
    <script language="javascript" type="text/javascript">
        var gmap = null;

        var maindiv;
        var toolbardiv;
        var mapdiv;
        var legenddiv;


        var serviceURL = "http://igor.beg.utexas.edu/ArcGIS/rest/services/TCEQ/MapServer";

        var QueryString = function () {
            // This function is anonymous, is executed immediately and 
            // the return value is assigned to QueryString!
            var query_string = {};
            var query = window.location.search.substring(1);
            var vars = query.split("&");
            for (var i = 0; i < vars.length; i++) {
                var pair = vars[i].split("=");
                // If first entry with this name
                if (typeof query_string[pair[0]] === "undefined") {
                    query_string[pair[0]] = pair[1];
                    // If second entry with this name
                } else if (typeof query_string[pair[0]] === "string") {
                    var arr = [query_string[pair[0]], pair[1]];
                    query_string[pair[0]] = arr;
                    // If third or later entry with this name
                } else {
                    query_string[pair[0]].push(pair[1]);
                }
            }
            return query_string;
        } ();

        function initialize() {

            var z = parseInt(QueryString.z);
            
            //Create the set of map options.
            var mapOptions = {
                center: new google.maps.LatLng(QueryString.lat, QueryString.lng),
                zoom: z,
                mapTypeId: google.maps.MapTypeId.HYBRID
            };

            //Create the map object.
            gmap = new google.maps.Map(document.getElementById("mapdiv"), mapOptions);

            dynMapOv = new gmaps.ags.MapOverlay(serviceURL, { name: 'ArcGIS', opacity: 0.5 });

            /*google.maps.event.addListener(dynMapOv, 'drawstart', function () {
                showWorkingDiv(true);
            });
            google.maps.event.addListener(dynMapOv, 'drawend', function () {
                hideWorkingDiv(true);
            });*/

            dynMapOv.setMap(gmap);

            //If we had coords for the marker...
            if (mkrlon != null && mkrlat != null) {
                casingMarker = new google.maps.Marker({
                    position: new google.maps.LatLng(mkrlat, mkrlon),
                    map: gmap,
                    title: 'Point of Interest'
                });
            }

            /*if (window.contentcode != undefined) {
                $("contentdiv").html(window.contentcode);
            }*/

            var el = window.opener.document.getElementById("printbuffer");

            var h = el.innerHTML;

            $("#contentdiv").html(h);

           
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">

    <asp:ScriptManager ID="scriptManager1" runat="server">
         <Scripts>
            <asp:ScriptReference Path="scripts/jquery-1.4.1.js" />
            <asp:ScriptReference Path="http://maps.googleapis.com/maps/api/js?key=AIzaSyAs2QuaICPc7hIgkozq5lWMNM1sliVXDPM&sensor=true" />
            <asp:ScriptReference Path="scripts/arcgislink.js" />
        </Scripts>
    </asp:ScriptManager>

    <div>
        <div class="unprintablediv">
            <a href="#" onclick="doPrintPage();">Print</a>
        </div>
    
        <!-- The Title --> 
        <div class="titlediv">
            <span class="titlespan">Surface Casing Estimator</span>
        </div>

        <div class="maindiv">
            <div id="mapdiv" class="mapdiv">
                
            </div>
        </div>    
        
        <div class="tocdiv">
        </div>

        <div id="contentdiv"></div>
    </div>

     <script type="text/javascript">
         Sys.Application.add_load(initialize);
    </script>
    </form>
</body>
</html>
