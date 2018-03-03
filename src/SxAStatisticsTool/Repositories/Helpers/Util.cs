using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Xdb.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SxAStatisticsTool.Repositories.Helpers
{
    public static class Util
    {
        const string sitesPath = "/sitecore/content/"; //Make configurable

        /// <summary>
        /// Runs sitecore query against an item / database
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="query"></param>
        /// <param name="item">(Optional) Run query relative to this item</param>
        /// <param name="database">(Optional) Sitecore database where to run the query</param>
        /// <returns>IEnumerable of input type T</returns>
        public static IEnumerable<T> RunQuery<T>(string query, Item item = null, Database database = null) where T : class
        {

            //Run query relative to item if available. If not, against db.
            var currentItem = item ?? Sitecore.Context.Item;

            var db = database ?? (item != null ? item.Database : Sitecore.Context.Database);
            IEnumerable<Item> results = currentItem != null
                ? currentItem.Axes.SelectItems(query)
                : db.SelectItems(query);

            return results == null ? null : results.Select(result => result as T).ToList();
        }


        /// <summary>
        /// Retrieves all item guids that are instanced from a template id
        /// </summary>
        /// <param name="templateId">Template id to filter items</param>
        /// <param name="path">(Optional) Path to limit the query to</param>
        /// <returns>List of Guid</returns>
        public static List<Guid> GetAllGuidsByTemplateId(ID templateId, string path)
        {
            if (string.IsNullOrEmpty(path)) path = sitesPath;
            var query = string.Format(@"{0}//*[@@templateid='{1}']", path, templateId);

            if (string.IsNullOrEmpty(query)) return null;
            var results = RunQuery<Item>(query);
            return results.Select(x => x.ID.Guid).Distinct().ToList();
        }


        public static T CastItem<T>(this Item item) where T : class
        {
            return item as T;
            //return SitecoreService.Cast<T>(item);
        }

        public static Dictionary<Guid, int> GetMostVisitedItems(int maxItems = 3, int days = 30, string path = null, Guid templateId = new Guid())
        {
            var query =
                string.Format(
                    "SELECT TOP {0} ItemId, count(*) as cnt FROM Fact_PageViews WHERE Date > DATEADD(DAY, -{1}, GETDATE()) GROUP BY ItemId ORDER BY cnt DESC",
                    maxItems, days);

            var dataQuery = new ReportDataQuery(query);
            var provider = (ReportDataProviderBase)Factory.CreateObject("reporting/dataProvider", true);
            var response = provider.GetData("reporting", dataQuery, CachingPolicy.WithCacheDisabled);

            var items = response.GetDataTable().Rows;

            var results = new Dictionary<Guid, int>();

            for (var i = 0; i < items.Count; i++)
            {
                results.Add(Guid.Parse(System.Convert.ToString(items[i]["ItemId"])), System.Convert.ToInt32(items[i]["cnt"]));
            }

            return results;
        }


    }
}