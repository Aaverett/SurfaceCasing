<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="Stylesheet" type="text/css" href="style/style2.css" />
    <title>Surface Casing Estimator</title>
    <script language="javascript" type="text/javascript">

        var toolbarheight = 75;
        var bordertotal = 0;
        var maindivmargin = 30;
        var clickBufferSize = 8;

        var gmap = null;
        var dynMapOv = null;
        var mapService = null;
        var serviceURL = "http://coastal.beg.utexas.edu:6080/arcgis/rest/services/surfacecasing/surfacecasing_client/MapServer";
        var ext = null;
        var qtask = null;
        var query = null;
        var overlays = null;
        var toolMode = "casing";

        var initialLat = 30.3;
        var initialLon = -97.7;

        var maindiv;
        var toolbardiv;
        var mapdiv;
        var legenddiv;

        var legendOpenHeight = 100;
        var legendOpen = true;

        var mapIsTransparent = 0.5;

        var casingbutton;
        var identifybutton;
        var logbutton;

        var activeTool;

        var casingMarker;

        var workingdivRequests = 0;
        var workingmapRequests = 0;

        var identifyLayerID = 0;

        var secondaryUIColor = "#cfd0d1";
        var buttonHighlightColor = secondaryUIColor;
        var buttonTextColor = "#000000";
        var highlightedButtonTextColor = "#000000";

        var logviewerwindow = null;

        var iWindow = null;

        function resizeDivs() {
            //Set up the  the height of the map
            maindiv = $("#maindiv");
            mapdiv = $("#mapdiv");
            toolbardiv = $("#toolbardiv");
            legenddiv = $("#legenddiv");
            var legenddiv_contents = $("#legenddiv_contents");

            var divH, divW, windowH, windowW;

            windowH = $(window).height();
            windowW = $(window).width();

            divH = windowH - maindivmargin;
            divW = windowW - maindivmargin;

            maindiv.width(divW);
            maindiv.height(divH);

            maindiv.offset({
                top: (windowH - divH) / 2,
                left: (windowW - divW) / 2
            });

            mapdiv.css({ width: divW - bordertotal, height:  divH - 175});

            toolbardiv.css({ position: "absolute", width: divW - bordertotal, height: toolbarheight - bordertotal, top: mapdiv.height() + 80});

            legendOpenHeight = divH - 375;

            legenddiv.css({ right: bordertotal + 10, top: bordertotal + 125, height: legendOpenHeight, width: 200 });
            legenddiv_contents.css({height: legendOpenHeight - 29});
        }

        function handleAJAXError(args) {
            hideWorkingDiv();
            alert("An error occurred that prevents completion of your request." + args);
        }

        function initialize() {

            resizeDivs();

            $(window).resize(function () {
                resizeDivs();
            });

            //Create the set of map options.
            var mapOptions = {
                center: new google.maps.LatLng(initialLat, initialLon),
                zoom: 8,
                mapTypeId: google.maps.MapTypeId.HYBRID
            };

            //Create the map object.
            gmap = new google.maps.Map(document.getElementById("mapdiv"), mapOptions);

            //Set up events for the map to handle.
            google.maps.event.addListener(gmap, "click", mapClicked);

            dynMapOv = new gmaps.ags.MapOverlay(serviceURL, { name: 'ArcGIS', opacity: 0.5 });

            google.maps.event.addListener(dynMapOv, 'drawstart', function () {
                showWorkingDiv(true);
            });
            google.maps.event.addListener(dynMapOv, 'drawend', function () {
                hideWorkingDiv(true);
            });

            dynMapOv.setMap(gmap);
            
            mapService = dynMapOv.getMapService();

            google.maps.event.addListenerOnce(mapService, "load", initMapServiceCompleted);

            casingbutton = addToolbarButton("casingbutton", "Casing Data", activateTool, doCasingQuery, "assets/casing_1.gif", "assets/casing_0.gif", "assets/casing_2.gif");
            logbutton = addToolbarButton("logbutton", "View Log", activateTool, doLogQuery, "assets/log_1.gif", "assets/log_0.gif", "assets/log_2.gif");
            surveyInfoButton = addToolbarButton("surveyinfobutton", "Survey Info", activateTool, doSurveyInfoQuery, "assets/survey_1.gif", "assets/survey_0.gif", "assets/survey_2.gif");
            identifybutton = addToolbarButton("identifybutton", "Identify", activateTool, doIdentifyQuery, "assets/identify_1.gif", "assets/identify_0.gif", "assets/identify_2.gif");
            
            addToolbarSpacer("spacer1");
            mapOpaqueButton = addToolbarButton("opaqueButton", "Cycle Overlay", toggleMapTransparency, null);//, "assets/identify_1.gif", "assets/identify_0.gif", "assets/identify_2.gif"););
            
            addToolbarSpacer("spacer2");
            fullExtentButton = addToolbarButton("fullextentbutton", "Full Extent", zoomToFullExtent, null, "assets/fullextent_1.gif", "assets/fullextent_0.gif", "assets/fullextent_2.gif");
            goToLatLongButton = addToolbarButton("latlongButton", "Go to Lat/Long", goToLatLon, null, "assets/xy_1.gif", "assets/xy_0.gif", "assets/xy_2.gif");
            goToCountyButton = addToolbarButton("countyButton", "Go to County", goToCounty, null, null, null, null);

            addToolbarSpacer("spacer3");
            addToolbarButton("measurebutton", "Measure", activateTool, doMeasureDistance, "assets/measure_1.gif", "assets/measure_0.gif", "assets/measure_2.gif");

            addToolbarSpacer("spacer4");
            addToolbarButton("printbutton", "Print Map", doPrintMap, null, "assets/print_1.gif", "assets/print_0.gif", null);

            casingbutton.click();

        }

        function initMapServiceCompleted() {
            
            //Here, we set up the legend.
            var toc = '';

            for (var i = 0; i < mapService.layers.length; i++) {
                var legendIndex = 0;

                var imagecode = "";

                

                if (mapService.layers[i].legend != undefined && mapService.layers[i].legend) {
                    var imgurl = mapService.layers[i].url + "/images/" + mapService.layers[i].legend[legendIndex].url;
                    imagecode = '<td style="vertical-align: middle; width: 40px;"><img src="' + imgurl + '" alt="Legend swatch for ' + mapService.layers[i].name + '"/></td>';
                }

                var selectedStyle = "";

                if (mapService.layers[i].id == identifyLayerID) selectedStyle = ' style="background-color: #999999;"';

                toc += '<div class="legend_entry" id="layer_legend_entry_' + mapService.layers[i].id + '"' + selectedStyle + '><table style="width: 100%;"><tr>' + imagecode + '<td style="vertical-align: middle; width: 20px;"><input type="checkbox" id="layer' + mapService.layers[i].id + '"';
                if (mapService.layers[i].visible)
                    toc += ' checked="checked"';
                toc += ' onclick="setLayerVis()"></td><td style="vertical-align: middle; width: 110px;">' + mapService.layers[i].name + '</td>';

                //Do the layer selection selector deal.

                var imgurl_identify = "assets/identify_0.gif";

                if (mapService.layers[i].id == identifyLayerID) imgurl_identify = "assets/identify_1.gif";

                toc += '<td style="width: 30px;"><a href="#" onclick="selectIdentifyLayer(' + mapService.layers[i].id + ');"><img src="' + imgurl_identify + '" /></a></td>';

                toc += '</tr></table></div>';
            }
            document.getElementById('legenddiv_contents').innerHTML = toc;

        }

        function setLayerVis() {
            var service = mapService;
            for (var i = 0; i < service.layers.length; i++) {
                var el = document.getElementById('layer' + service.layers[i].id);
                service.layers[i].visible = (el.checked === true);
            }
            dynMapOv.refresh();
        }

        function showWorkingDiv(maprequest) {

            if (maprequest == true) {
                workingmapRequests++;
            }
            else {
                workingdivRequests++;
            }

            document.getElementById('drawing').style.visibility = 'visible';
        }

        function hideWorkingDiv(maprequest) {
            if (maprequest) {
                workingmapRequests = 0;
            }
            else workingdivRequests--;

            if (workingdivRequests <= 0 && workingmapRequests == 0) {
                document.getElementById('drawing').style.visibility = 'hidden';
            }

            if (workingdivRequests < 0) workingdivRequests = 0;
        }

        function doCasingQuery(event) {

            if (casingMarker != null) casingMarker.setMap(null);
            
            casingMarker = new google.maps.Marker({
                position: new google.maps.LatLng(event.latLng.lat(), event.latLng.lng()),
                map: gmap,
                title: 'Casing Query'
            });

            showWorkingDiv(false);
            TCEQAjaxService.casingQuery(event.latLng.lng(), event.latLng.lat(), handleCasingQueryResult, handleAJAXError);
        }

        function handleCasingQueryResult(args) {
            hideWorkingDiv(false);

            var windowOptions = { content: args.detailsHTML +
            "<div>The estimator site contains data from multiple regional datasets that are spliced together, resulting in some local, narrow (<500 ft) gaps in data at seams and edges. We suggest obtaining casing estimates for several locations that are greater than 500 ft apart for your area of interest.</div>"
            };

            if (iWindow != null) iWindow.setMap(null);

            iWindow = new google.maps.InfoWindow(windowOptions);
            
            iWindow.open(gmap, casingMarker);

            google.maps.event.addListener(casingMarker, 'click', function () {
                if (iWindow != null) iWindow.open(gmap, casingMarker);
            });
        }

        function doLogQuery(event) {

            showWorkingDiv(false);

            var bufferDistance = getBufferDistance();

            //We don't 
            TCEQAjaxService.logQuery(event.latLng.lng(), event.latLng.lat(), bufferDistance, handleLogQueryResult, handleAJAXError);
        }

        function handleLogQueryResult(args) {
            hideWorkingDiv(false);

            if (args.detailsHTML == "") alert("No log was found at the given location.");
            
            if (casingMarker != null) casingMarker.setMap(null);

            casingMarker = new google.maps.Marker({
                position: new google.maps.LatLng(args.latitude, args.longitude),
                map: gmap,
                title: 'Casing Query'
            });

            var windowOptions = { content: args.detailsHTML
            };

            if (iWindow != null) iWindow.setMap(null);

            iWindow = new google.maps.InfoWindow(windowOptions);

            iWindow.open(gmap, casingMarker);

            google.maps.event.addListener(casingMarker, 'click', function () {
                if (iWindow != null) iWindow.open(gmap, casingMarker);
            });
        }

        function doSurveyInfoQuery(event) {
            showWorkingDiv(false);
            TCEQAjaxService.surveyInfoQuery(event.latLng.lng(), event.latLng.lat(), handleSurveyInfoQuery, handleAJAXError);
        }

        function handleSurveyInfoQuery(args) {
            hideWorkingDiv(false);

            if (args.blurbHTML == "") alert("No survey was found at the selected location.");

            if (casingMarker != null) casingMarker.setMap(null);

            casingMarker = new google.maps.Marker({
                position: new google.maps.LatLng(args.latitude, args.longitude),
                map: gmap,
                title: 'Survey Info Query'
            });

            var windowOptions = { content: args.detailsHTML
            };

            if (iWindow != null) iWindow.setMap(null);

            iWindow = new google.maps.InfoWindow(windowOptions);

            iWindow.open(gmap, casingMarker);

            google.maps.event.addListener(casingMarker, 'click', function () {
                if (iWindow != null) iWindow.open(gmap, casingMarker);
            });
        }

        function getBufferDistance() {
            var zoomLevel = gmap.getZoom();

            var bounds = gmap.getBounds();

            var bwidth = bounds.getNorthEast().lng() - bounds.getSouthWest().lng();

            var div = gmap.getDiv();

            var pwidth = $(div).width();

            var dpp = bwidth / pwidth;

            var bufferDistance = dpp * clickBufferSize;

            return bufferDistance;
        }

        function doIdentifyQuery(event) {
            showWorkingDiv(false);

            var bufferDistance = getBufferDistance();

            TCEQAjaxService.identifyQuery(event.latLng.lng(), event.latLng.lat(), identifyLayerID, bufferDistance, handleIdentifyQueryResponse, handleAJAXError);
        }

        function handleIdentifyQueryResponse(args) {
            hideWorkingDiv(false);

            if (args.blurbHTML == "") alert("No survey was found at the selected location.");

            if (casingMarker != null) casingMarker.setMap(null);

            casingMarker = new google.maps.Marker({
                position: new google.maps.LatLng(args.latitude, args.longitude),
                map: gmap,
                title: 'Identify'
            });

            var windowOptions = { content: args.detailsHTML
            };

            if (iWindow != null) iWindow.setMap(null);

            iWindow = new google.maps.InfoWindow(windowOptions);

            iWindow.open(gmap, casingMarker);

            google.maps.event.addListener(casingMarker, 'click', function () {
                if (iWindow != null) iWindow.open(gmap, casingMarker);
            });
        }

        function selectIdentifyLayer(newID) {
            var oldStripe = $('#layer_legend_entry_' + identifyLayerID);
            var newStripe = $('#layer_legend_entry_' + newID);
            
            identifyLayerID = newID;

            //Update the UI to reflect the change.
            oldStripe.css("background-color", "#dddddd");
            newStripe.css("background-color", "#999999");
        }

        function zoomToFullExtent() {
          
            if (gmap != null) {
                var sw = new google.maps.LatLng(25.7, -106.7);
                var ne = new google.maps.LatLng(36.5,-93.45);
                var bounds = new google.maps.LatLngBounds(sw, ne);

                gmap.setZoom(6);

                gmap.panToBounds(bounds);
            }
        }

        function toggleMapTransparency() {
            if (mapIsTransparent == 0.5) {
                mapIsTransparent = 1.0;
            }
            else if (mapIsTransparent == 1.0) {
                mapIsTransparent = 0.0;
            }
            else {
                mapIsTransparent = 0.5;
            }

            if (dynMapOv != null) {
                dynMapOv.setOpacity(mapIsTransparent);
            }
        }

        function goToLatLon() {
            //First, we need to center the object in the window.

            var windowH = $(window).height();
            var windowW = $(window).width();

            var top = (windowH - 300) / 2;
            var left = (windowW - 400) / 2;

            $('#goToLatLonDialog').css("top", top);
            $('#goToLatLonDialog').css("left", left);
            $('#goToLatLonDialog').css("display", "block");

            var lat = $('#latitudeBox').attr('value',gmap.getCenter().lat());
            var lon = $('#longitudeBox').attr('value', gmap.getCenter().lng());
        }

        function goToLatLongCancel() {
            $('#goToLatLonDialog').css("display", "none");
        }

        function goToLatLonGoClicked() {
            var lat = $('#latitudeBox').attr('value');
            var lon = $('#longitudeBox').attr('value');

            if (gmap != null) {
                var ll = new google.maps.LatLng(lat, lon);

                gmap.panTo(ll);

                if (casingMarker != null) casingMarker.setMap(null);

                casingMarker = new google.maps.Marker({
                    position: new google.maps.LatLng(lat, lon),
                    map: gmap,
                    title: 'Identify'
                });

                var windowOptions = { content: "Latitude: " + lat + "<br />" + "Longitude: " + lon
                };

                if (iWindow != null) iWindow.setMap(null);

                iWindow = new google.maps.InfoWindow(windowOptions);

                google.maps.event.addListener(iWindow, 'closeclick', function () {
                    if (casingMarker != null) casingMarker.setMap(null);
                }
                );

                iWindow.open(gmap, casingMarker);
            }

            goToLatLongCancel();
        }

        var gotCounties = false;

        function goToCounty() {

            if ($('#goToCountyDialog').css("display") == "block") {
                cancelGoToCounty();
                return;
            }

            var windowH = $(window).height();
            var windowW = $(window).width();

            var top = (windowH - 300) / 2;
            var left = (windowW - 400) / 2;

            $('#goToCountyDialog').css("top", top);
            $('#goToCountyDialog').css("left", left);
            $('#goToCountyDialog').css("display", "block");

            $("#goToCountyContentsDiv").width($('#goToCountyDialog').width());
            $("#goToCountyContentsDiv").height($('#goToCountyDialog').height() - 29);

            //Get the list of counties.
            if (!gotCounties) {
                var html = "<div id=\"countiesWorkingDiv\">Working... Please wait...</div>";
                showWorkingDiv(false);
                TCEQAjaxService.getCounties(handleGotAllCounties, handleAJAXError, handleAJAXError);

                $("#goToCountyContentsDiv").append(html);

                $("#countiesWorkingDiv").show("fade", {}, 500);
            }
        }

        function handleGotAllCounties(args) {
            //var html = "";
            
            hideWorkingDiv(false);

            $("#countiesWorkingDiv").remove();

            for (var i = 0; i < args.counties.length; i++) {

                var county = args.counties[i];
                var divid = "county_" + county.countyID;
                var html = "<div id=\"" + divid + "\" class=\"legend_entry\">" + county.countyName + "</div>";

                $("#goToCountyContentsDiv").append(html);

                var element = $("#" + divid);

                element.bind("click", doGoToCounty, element);

                element.data("countyID", county.countyID);
            }

            //$("#goToCountyContentsDiv").append(html);

            gotCounties = true;
        }

        function doGoToCounty(ecounty) {

            showWorkingDiv(false);

            //alert(this.data("countyID"));

            TCEQAjaxService.getCountyEnvelope($(this).data("countyID"), handleGetCountyEnvelope, handleAJAXError);

            cancelGoToCounty();
        }


        function handleGetCountyEnvelope(args) {
            

            for (var i = 0; i < args.counties.length; i++) {

                var county = args.counties[i];
                
                var ne = new google.maps.LatLng(county.yMax, county.xMax);
                var sw = new google.maps.LatLng(county.yMin, county.xMin);

                var b = new google.maps.LatLngBounds(sw, ne);

                var xCenter, yCenter;
                xCenter = county.xMin + (county.xMax - county.xMin) / 2;
                yCenter = county.yMin + (county.yMax - county.yMin) / 2;

                var centerpoint = new google.maps.LatLng(yCenter, xCenter);

                //gmap.panToBounds(b);
                gmap.panTo(centerpoint);
                gmap.setZoom(11);

                if (casingMarker != null) casingMarker.setMap(null);

                casingMarker = new google.maps.Marker({
                    position: centerpoint,
                    map: gmap,
                    title: 'Casing Query'
                });

                break;
            }

            hideWorkingDiv(false);
        }

        function cancelGoToCounty() {           
            //$("#goToCountyDialog").hide("fade", {}, 500);
            $('#goToCountyDialog').css("display", "none");
        }

        var measureMarker1;
        var measureMarker2;
        var measureline;
        var measureWindow;

        function doMeasureDistance(event) {
        
            if(measureMarker1 != null && measureMarker2 != null)
            {
                clearLastMeasurement();
            }

            if (measureMarker1 == null) {
                measureMarker1 = new google.maps.Marker({
                    position: new google.maps.LatLng(event.latLng.lat(), event.latLng.lng()),
                    map: gmap,
                    title: 'Measure Origin',
                    icon: new google.maps.MarkerImage("assets/markers/paleblue_MarkerA.png")
                });

                
            }
            else {
                //The second point.
                measureMarker2 = new google.maps.Marker({
                    position: new google.maps.LatLng(event.latLng.lat(), event.latLng.lng()),
                    map: gmap,
                    title: 'Measure Destination',
                    icon: new google.maps.MarkerImage("assets/markers/paleblue_MarkerB.png")
                });

                var loc1 = measureMarker1.getPosition();
                var loc2 = measureMarker2.getPosition();

                var points = Array(new google.maps.LatLng(loc1.lat(), loc1.lng()), new google.maps.LatLng(loc2.lat(), loc2.lng()));

                var lineOptions = {path: points, map: gmap, title: 'Measure Line', strokeColor: "#00ffff", strokeOpacity: 0.5};

                measureline = new google.maps.Polyline(lineOptions);

                

                //compute the distance (no longer requires an asynchronous call!)
                var R = 6378137;
                var dLat = (loc1.lat() - loc2.lat()) * Math.PI / 180;
                var dLng = (loc1.lng() - loc2.lng()) * Math.PI / 180;
                var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
                Math.cos(loc1.lat() * Math.PI / 180) * Math.cos(loc2.lat() * Math.PI / 180) *
                Math.sin(dLng / 2) * Math.sin(dLng / 2);
                var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
                var d = R * c;

                //Display the distance
                
                var windowOptions = { content: "<p>A to B: " + (d/ 1000).toFixed(2) + " Km.</p>"
                };

                if (measureWindow = null) measureWindow.setMap(null);

                measureWindow = new google.maps.InfoWindow(windowOptions);

                measureWindow.open(gmap, measureMarker2);

                google.maps.event.addListener(measureMarker1, 'click', function () {
                    measureWindow.open(gmap, measureMarker2);
                }
                );

                google.maps.event.addListener(measureMarker2, 'click', function () {
                    measureWindow.open(gmap, measureMarker2);
                }
                );
                
            }
        }

        function clearLastMeasurement() {

            if (measureMarker1 != null) {
                //Release the first point marker
                measureMarker1.setMap(null);
                measureMarker1 = null;
            }

            if (measureline != null) {
                //Release the line.
                measureline.setMap(null);
                measureline = null;
            }

            if (measureMarker2 != null) {
                //Release the second point marker
                measureMarker2.setMap(null);
                measureMarker2 = null;
            }
        }

        function activateTool() {

            if ($(this).data("active") != true) {
                $(this).css("background", buttonHighlightColor);
                $(this).css("color", highlightedButtonTextColor);
                if (activeTool != undefined && activeTool != null && activeTool != $(this)) deactivateTool(activeTool);

                activeTool = $(this);

                activeTool.data("active", true);

                $("#" + $(this).data("imageid")).attr("src", $(this).data("downimage"));
            }
        }

        function deactivateTool(tool) {
            tool.css("background", secondaryUIColor);
            tool.css("color", buttonTextColor);
            tool.data("active", false);

            $("#" + tool.data("imageid")).attr("src", tool.data("upimage"));

            //When switching tools, clear any annotations that were added for measurement.
            clearLastMeasurement();
        }

        function addToolbarSpacer(id) {
            toolbardiv.append("<div class=\"toolbarspacer\" id=\"" + id + "\">&nbsp;</div>");
        }

        function addToolbarButton(id, label, clickAction, toolaction, upimage, overimage, downimage) {
            if (toolbardiv == null) return;

            var html = "<div class=\"toolbarbutton\" id=\"" + id + "\">";
            
            html += label;

            if(upimage != undefined && upimage != null) {

                html += "<br /><img id=\"" + id + "_image\" src=\"" + upimage + "\" alt=\""+label+"\">";
            }

            html += "</div>";

            toolbardiv.append(html);

            var button = $("#" + id);

            var backgroundcolor = secondaryUIColor;


            //display: "inline-block", 
            button.css({"background": backgroundcolor});
            button.css("text-align", "center");
            button.css("font-weight", "bold");
            button.css("margin-right", "5px");
            button.css("margin-left", "5px");
            button.css("color", buttonTextColor);

            button.data("active", false);

            button.data("imageid", id + "_image");
            if(upimage != undefined && upimage != null) button.data("upimage", upimage);
            if(overimage != undefined && overimage != null) button.data("overimage", overimage);
            if(downimage != undefined && downimage != null) button.data("downimage", downimage);

            //Set up the events.
            button.click(clickAction);
            button.mouseenter(function () {

                if ($(this).data("active") == false) {
                    $(this).css("background", buttonHighlightColor);
                    $(this).css("color", highlightedButtonTextColor);

                    var id = $(this).data("imageid");

                    //Switch the image to the "over" image
                    $("#" + $(this).data("imageid")).attr("src", $(this).data("overimage"));
                }
            });
            button.mouseleave(function () {

            
                if ($(this).data("active") == false) {
                    $(this).css("background", secondaryUIColor);
                    $(this).css("color", buttonTextColor);
                    //Switch the image to the "up" image
                    $("#" + $(this).data("imageid")).attr("src", $(this).data("upimage"));
                }
            });

            if (toolaction != null) {
                button.data("toolaction", toolaction);
            }

            return button;
        }

        function mapClicked(event) {
            
            if (activeTool.data("toolaction") != null) {
                
                var f = activeTool.data("toolaction");

                f(event);
            }
        }

        function toggleLegend() {

            if (legendOpen) {
                //Hide the legend.

                $("#legenddiv").animate({ height: 25 }, 500, 'swing', null);

                legendOpen = false;

                $("#legendtogglelink").html("Show");

                $("#legenddiv_contents").css("display", "none");
            }
            else {
                
                $("#legenddiv").animate({ height: legendOpenHeight}, 500, 'swing', null);
                legendOpen = true;

                $("#legendtogglelink").html("Hide");

                $("#legenddiv_contents").css("display", "block");
            }
        }

        function openLogViewer(log) {
            var address = 'logviewer.aspx?u=' + log;

            var sizex = 900;
            var sizey = 700;

            if (logviewerwindow != null && !logviewerwindow.closed && logviewerwindow.location) {
                logviewerwindow.location.href = address;
                logviewerwindow.focus();
            }
            else {
                var args;
                args = "width=" + sizex + ", height=" + sizey + ", toolbar=no, location=no, directories=no, status=no, menubar=no, copyhistory=no, scrollbars=yes";
                logviewerwindow = window.open(address, "logviewerwindow", args);
            }
        }

        //Opens the printable map viewer pane.
        function openPrintViewer() {

            var c = gmap.getCenter();

            var lat = c.lat();
            var lng = c.lng();
            var z = gmap.getZoom();


            var sizex = 900;
            var sizey = 700;


            var address = 'printmap.aspx?lat=' + lat + '&lng=' + lng + '&z=' + z;

            if (casingMarker != null) {
                address = address + "&mkrlat=" + casingMarker.getPosition().lat() + "&mkrlon=" + casingMarker.getPosition().lng() ;
            }

            if (iWindow != null) {
                var c = iWindow.getContent();

                //address = address + "&contentcode=" + encodeURIComponent(c)

                $("#printbuffer").html(c);
            }

            if (logviewerwindow != null && !logviewerwindow.closed && logviewerwindow.location) {
                logviewerwindow.location.href = address;
                logviewerwindow.focus();
            }
            else {
                var args;
                args = "width=" + sizex + ", height=" + sizey + ", toolbar=no, location=no, directories=no, status=no, menubar=no, copyhistory=no, scrollbars=yes";
                logviewerwindow = window.open(address, "logviewerwindow", args);
            }
        }

        function doPrintMap() {
            openPrintViewer();
        }

        function openHelpWindow() {
            var sizex = 1050;
            var sizey = 700;


            var address = 'help.htm';


            if (!logviewerwindow.closed && logviewerwindow.location) {
                logviewerwindow.location.href = address;
                logviewerwindow.focus();
            }
            else {
                var args;
                args = "width=" + sizex + ", height=" + sizey + ", toolbar=no, location=no, directories=no, status=no, menubar=no, copyhistory=no, scrollbars=yes";
                logviewerwindow = window.open(address, "logviewerwindow", args);
            }

            return false;
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="scripts/jquery-1.4.1.js" />
            <asp:ScriptReference Path="http://maps.googleapis.com/maps/api/js?key=AIzaSyAs2QuaICPc7hIgkozq5lWMNM1sliVXDPM&sensor=true" />
            <asp:ScriptReference Path="scripts/arcgislink.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/TCEQAjaxService.asmx" />
        </Services>
    </asp:ScriptManager>
    
    <div id="maindiv">
        <div id="helplinkdiv">
            <a href="help.htm" onclick="openHelpWindow(); return false;">Help</a>
        </div>
        <div id="headingdiv">
            <img src="assets/heading.jpg" alt="Heading Image" />
        </div>
        <div id="drawing">
            <table style="height: 100%; width: 100%;">
                <tr>
                    <td style="vertical-align: middle; text-align: center;">
                        <img src="assets/throbber-slow.gif" alt="Working throbber" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="mapdiv">
        </div>
        <div id="toolbardiv">
        </div>
        <div id="legenddiv">
            <div id="legenddiv_heading"><a href="#" id="legendtogglelink" onclick="toggleLegend(); return false;" >Hide</a>Legend</div>
            <div id="legenddiv_contents"></div>
        </div>
    </div>

    <div id="goToLatLonDialog">
        <div id="legenddiv_heading">Go to Latitude/Longitude</div>
        <table>
            <tr>
                <td>Longitude (X): </td>
                <td><input type="text" name="longitudeBox" id="longitudeBox" /></td>
            </tr>
            <tr>
                <td>Latitude (Y): </td>
                <td><input type="text" name="latitudeBox" id="latitudeBox"  /></td>
            </tr>
            <tr>
                <td colspan="2">

                    <input type="button" value="Go" onclick="goToLatLonGoClicked();"/> 
                    <input type="button" value="Cancel" onclick="goToLatLongCancel();" />
                </td>
            </tr>
        </table>
    </div>

    <div id="goToCountyDialog">
        <div id="legenddiv_heading"><a href="#" id="countyDialogToggleLink" onclick="cancelGoToCounty(); return false;">Hide</a>Go to County</div>
        <div id="goToCountyContentsDiv">
        </div>
    </div>

    </form>

    <div id="printbuffer" style="display: none;">
    </div>

    <script type="text/javascript">
        Sys.Application.add_load(initialize);
    </script>
</body>
</html>
