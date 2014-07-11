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
/// Summary description for TCEQFuncs
/// </summary>
public class TCEQFuncs
{
    /// <summary>
    /// This method generates a callbackresult that can be passed to a control during an asynchronous call, allowing a web control to be updated without refreshing the entire page.
    /// </summary>
    /// <param name="targetID">The ID value of the HTML entity that will recieve the content.</param>
    /// <param name="c">The control you want to update.</param>
    /// <returns>Callback result containg the HTML for control C as it exists right now.</returns>
    /*public static CallbackResult updateControl(string targettag, string targetID, Control c)
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

    public static CallbackResult doJavascriptCallback(string script)
    {
        CallbackResult cr = new CallbackResult(null, null, "javascript", script);

        return cr;
    }*/

    /// <summary>
    /// Ugly hack to get around having to convert from a layer name to a dataset name.
    /// </summary>
    /// <param name="layerIndex"></param>
    /// <returns></returns>
    public static string getDataSetName(int layerIndex)
    {
        switch (layerIndex)
        {
            case 0: return "sde.SDE.TCEQ_QWells";
                break;
            case 1: return "sde.SDE.TCEQ_OGWells";
                break;
            case 2: return string.Empty;
                break;
            case 3: return "sde.SDE.waterbodies";
                break;
            case 4: return "sde.SDE.RIVERS";
                break;
            case 5: return "sde.SDE.TCEQ_Roads";
                break;
            case 6: return "sde.SDE.TCEQ_UrbanAreas";
                break;
            case 7: return "sde.SDE.TCEQ_Surveys";
                break;
            case 8: return "sde.SDE.TCEQ_Counties";
                break;
        }

        return string.Empty;
    }

    public static void NotifyAdminMissingLog(string logfilename)
    {
        try
        {
            System.Web.Mail.MailMessage msgMail = new System.Web.Mail.MailMessage();

            msgMail.To = ConfigurationSettings.AppSettings["admin_email_address"];
            msgMail.From = "Do not reply <donotreply@beg.utexas.edu>";
            msgMail.Subject = "Missing Log Image in TCEQ Surface Casing Application";

            msgMail.BodyFormat = System.Web.Mail.MailFormat.Text;

            msgMail.Body = "A log image within the TCEQ Surface Casing application has been found to be missing.  The filename is " + logfilename;

            System.Web.Mail.SmtpMail.Send(msgMail);
        }
        catch (System.Exception e)
        {

        }
    }

    public static string LocalLogImagePath
    {
        get
        {
            try
            {
                string s = ConfigurationSettings.AppSettings["local_log_image_path"];

                return s;
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    public static string WebLogImagePath
    {
        get
        {
            try
            {
                string s = ConfigurationSettings.AppSettings["web_log_image_path"];

                return s;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
