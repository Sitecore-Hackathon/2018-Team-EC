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
    internal class DatabaseReader : IDatabaseReader
    {
        /// <summary>
        /// Retrieves a list of the most visited items by recency.
        /// </summary>
        /// <param name="maxItems">Maximum # of items to return. Default: 100.</param>
        /// <param name="days">How many days to go back and search for visits. Default: 60.</param>
        /// <param name="path">When using a template id: search for withing an specific path.</param>
        /// <param name="templateId">Limit results to a subset of items based on the template id.</param>
        /// <returns></returns>
        public Dictionary<Guid, int> GetMostVisitedItems(int maxItems = 100, int days = 60, string path = null, Guid templateId = new Guid())
        {
            var query = string.Empty;

            if (templateId != Guid.Empty)
            {
                var itemIds = GetAllGuidsByTemplateId(new ID(templateId), path);

                if (itemIds != null && itemIds.Any())
                {
                    var stringIds = itemIds.Aggregate(string.Empty, (current, id) => current + (current + "'" + id + "',"));
                    stringIds = stringIds.Substring(0, stringIds.LastIndexOf(",", StringComparison.Ordinal));

                    query =

                        $@"SELECT TOP {maxItems} ItemId, count(*) as cnt FROM Fact_PageViews 
                           WHERE Date > DATEADD(DAY, -{days}, GETDATE()) 
                           AND ItemId IN({stringIds}) GROUP BY ItemId ORDER BY cnt DESC";
                }
            }
            else
            {
                query =
                    $@"SELECT TOP {maxItems} ItemId, count(*) as cnt FROM Fact_PageViews 
                       WHERE Date > DATEADD(DAY, -{days}, GETDATE()) GROUP BY ItemId ORDER BY cnt DESC";
            }

            if (!string.IsNullOrEmpty(query))
            {
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
            return null;
        }

        /// <summary>
        /// Runs sitecore query against an item / database
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="query"></param>
        /// <param name="item">(Optional) Run query relative to this item</param>
        /// <param name="database">(Optional) Sitecore database where to run the query</param>
        /// <returns>IEnumerable of input type T</returns>
        public IEnumerable<T> RunQuery<T>(string query, Item item = null, Database database = null) where T : class
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
        public List<Guid> GetAllGuidsByTemplateId(ID templateId, string path)
        {
            var database = Sitecore.Data.Database.GetDatabase("master");
            if (string.IsNullOrEmpty(path)) path = Constants.Paths.DefaultPath;
            var query = string.Format(@"{0}//*[@@templateid='{1}']", path, templateId);

            if (string.IsNullOrEmpty(query)) return null;
            var results = RunQuery<Item>(query, database: database);
            return results.Select(x => x.ID.Guid).Distinct().ToList();
        }
    }
}