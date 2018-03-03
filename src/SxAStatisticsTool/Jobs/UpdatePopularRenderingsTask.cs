using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.SecurityModel;

namespace SxAStatisticsTool.Jobs
{
    public class UpdatePopularRenderingsTask
    {
        public void Update(Item[] items, Sitecore.Tasks.CommandItem command, Sitecore.Tasks.ScheduleItem schedule)
        {
            var webDatabase = Sitecore.Data.Database.GetDatabase("web");
            var recommendedRenderingsItem = webDatabase.GetItem(Templates.RecommendedRenderings.Id);
            if (recommendedRenderingsItem == null)
            {
                Log.Debug("Empty item");
                return;
            }
            using (new SecurityDisabler())
            {
                recommendedRenderingsItem.Editing.BeginEdit();
                recommendedRenderingsItem[Templates.RecommendedRenderings.Fields.Renderings] = "";
                recommendedRenderingsItem.Editing.EndEdit();
            }
        }
    }
}