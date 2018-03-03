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
        }




    }
}