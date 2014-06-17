//tceq.js
//Aaron Averett - 2007-06-22
//This file contains the javascripts used in the new TCEQ application.

var logviewerwindow = '';

 var sizex = 900;
 var sizey = 650;
    

function openLogViewer(encodedpath)
{
    var address = 'logviewer.aspx?u=' + encodedpath;
    
    if(!logviewerwindow.closed && logviewerwindow.location)
    {
        logviewerwindow.location.href = address;
        logviewerwindow.focus();
    }
    else
    {
        var args;
        args = "width=" + sizex + ", height=" + sizey + ", toolbar=no, location=no, directories=no, status=no, menubar=no, copyhistory=no, scrollbars=yes";
        logviewerwindow = window.open(address, "logviewerwindow", args);
    }
}

//Opens the printable map viewer pane.
function openPrintViewer()
{
    var address = 'printmap.aspx';
   
    if(!logviewerwindow.closed && logviewerwindow.location)
    {
        logviewerwindow.location.href = address;
        logviewerwindow.focus();
    }
    else
    {
        var args;
        args = "width=" + sizex + ", height=" + sizey + ", toolbar=no, location=no, directories=no, status=no, menubar=no, copyhistory=no, scrollbars=yes";
        logviewerwindow = window.open(address, "logviewerwindow", args);
    }
}

function doPrintPage()
{
    window.print();
    
    return false;
}