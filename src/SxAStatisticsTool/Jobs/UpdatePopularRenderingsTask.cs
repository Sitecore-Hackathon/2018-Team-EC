using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.SecurityModel;
using SxAStatisticsTool.Repositories.Helpers;
using SxAStatisticsTool.Repositories.Factories;

namespace SxAStatisticsTool.Jobs
{
    public class UpdatePopularRenderingsTask
    {
        private IDatabaseReader _databaseReader;
        UpdatePopularRenderingsTask()
        {
            _databaseReader = DatabaseReaderFactory.Build();
        }

        public void Update(Item[] items, Sitecore.Tasks.CommandItem command, Sitecore.Tasks.ScheduleItem schedule)
        {

            var recommendedRenderingsItem = GetRecommendedRenderingItem();
            if (recommendedRenderingsItem == null)
            {
                Log.Debug("Recommended Rendering Item not found");
                return;
            }
            using (new SecurityDisabler())
            {
                recommendedRenderingsItem.Editing.BeginEdit();
                recommendedRenderingsItem[Templates.AvailableRenderings.Fields.Renderings] = GetVisitedItemsRenderings();
                recommendedRenderingsItem.Editing.EndEdit();
            }
        }
        private string GetVisitedItemsRenderings()
        {
            var visitedItems = _databaseReader.GetMostVisitedItems();
            List<string> renderingStringList = new List<string>();
            foreach (KeyValuePair<Guid, int> visitedItem in visitedItems)
            {
                Item item = Sitecore.Context.Database.GetItem(Sitecore.Data.ID.Parse(visitedItem.Key));
                if (item != null)
                {
                    RenderingReference[] renderings = item.Visualization.GetRenderings(Sitecore.Context.Device, true);
                    foreach (var rendering in renderings)
                    {
                        string renderingId = rendering.RenderingID.ToString();
                        renderingStringList.Add(renderingId);
                    }
                }
            }
            return String.Join("|",renderingStringList.Distinct());
        }

        private Item GetRecommendedRenderingItem()
        {
            var database = Sitecore.Data.Database.GetDatabase("master");
            var query = $"{Constants.Paths.DefaultPath}//*[@@templateid='{Templates.AvailableRenderings.Id}' and @@name='{Constants.Values.RecommendedRenderingsItemName}']";
            return database.GetItem(query);
        }
    }


}