using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TCEQApp
{
    public partial class logviewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string u = Request["u"];
            string imagename = HttpUtility.UrlDecode(u);

            if (imagename != null && imagename.Trim() != string.Empty)
            {

                if (System.IO.File.Exists(TCEQFuncs.LocalLogImagePath + "/" + imagename))
                {
                    //Now, we create an image control.
                    Image img = new Image();

                    //Set the image's path.
                    img.ImageUrl = TCEQFuncs.WebLogImagePath + "/" + imagename;
                    img.AlternateText = "Log image " + imagename;

                    imageunavailablediv.Visible = false;

                    //Add the image to the main div control.
                    maindiv.Controls.Add(img);
                }
                else
                {
                    TCEQFuncs.NotifyAdminMissingLog(imagename);
                }
            }  
        }
    }
}