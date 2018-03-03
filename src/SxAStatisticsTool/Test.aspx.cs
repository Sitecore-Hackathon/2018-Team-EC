using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Xdb.Reporting;
using SxAStatisticsTool.Repositories.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SxAStatisticsTool
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var t = Util.GetMostVisitedItems();
            var t2 = Util.GetMostVisitedItems(templateId: new Guid("{76036F5E-CBCE-46D1-AF0A-4143F9B557AA}"));
        }




    }
}