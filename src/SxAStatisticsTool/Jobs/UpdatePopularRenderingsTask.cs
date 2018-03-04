using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.SecurityModel;
using SxAStatisticsTool.Repositories.Factories;
using SxAStatisticsTool.Repositories.Helpers;

namespace SxAStatisticsTool.Jobs
{
    public class UpdatePopularRenderingsTask
    {
        private IDatabaseReader _databaseReader;

        private Database database;

        public UpdatePopularRenderingsTask()
        {
            database = Database.GetDatabase("master");
            _databaseReader = DatabaseReaderFactory.Build();
        }

        public void Update(Item[] items, Sitecore.Tasks.CommandItem command, Sitecore.Tasks.ScheduleItem schedule)
        {

            var recommendedRenderingsItems = GetRecommendedRenderingItems();
            if (recommendedRenderingsItems == null)
            {
                Log.Debug("Recommended Rendering Item not found");
                return;
            }
            using (new SecurityDisabler())
            {
                foreach (var rd in recommendedRenderingsItems)
                {
                    var item = database.GetItem(rd.ItemId);
                    item.Editing.BeginEdit();
                    item[Templates.AvailableRenderings.Fields.Renderings] = GetVisitedItemsRenderings();
                    item.Editing.EndEdit();
                }
            }
        }

        private string GetVisitedItemsRenderings()
        {
            var visitedItems = _databaseReader.GetMostVisitedItems(database: database);
            List<string> renderingStringList = new List<string>();
            foreach (KeyValuePair<Guid, int> visitedItem in visitedItems)
            {
                Item item = database.GetItem(ID.Parse(visitedItem.Key));
                if (item != null)
                {
                    DeviceRecords devices = database.Resources.Devices;
                    DeviceItem defaultDevice = devices.GetAll().First(d => d.Name.ToLower() == "default");
                    RenderingReference[] renderings = item.Visualization.GetRenderings(defaultDevice, true);
                    foreach (var rendering in renderings)
                    {
                        string renderingId = rendering.RenderingID.ToString();
                        renderingStringList.Add(renderingId);
                    }
                }
            }
            return String.Join("|", renderingStringList.Distinct());
        }

        private List<SearchResultItem> GetRecommendedRenderingItems()
        {
            ISearchIndex index = ContentSearchManager.GetIndex("sitecore_master_index");
            using (IProviderSearchContext context = index.CreateSearchContext())
            {
                var results = context.GetQueryable<SearchResultItem>()
                    .Where(x => x.TemplateId.Guid == Templates.AvailableRenderings.Id.Guid &&
                                x.Name == Constants.Values.RecommendedRenderingsItemName);

                return results.ToList();
            }
        }
    }
}