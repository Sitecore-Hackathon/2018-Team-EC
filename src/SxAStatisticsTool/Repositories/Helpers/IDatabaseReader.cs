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
    public interface IDatabaseReader
    {
        /// <summary>
        /// Retrieves a list of the most visited items by recency.
        /// </summary>
        /// <param name="maxItems">Maximum # of items to return. Default: 100.</param>
        /// <param name="days">How many days to go back and search for visits. Default: 60.</param>
        /// <param name="path">When using a template id: search for within an specific path.</param>
        /// <param name="templateId">Limit results to a subset of items based on the template id.</param>
        /// <returns></returns>
        Dictionary<Guid, int> GetMostVisitedItems(int maxItems = 100, int days = 60, string path = null, Guid templateId = new Guid(), Database database = null);



        IEnumerable<T> RunQuery<T>(string query, Item item = null, Database database = null) where T : class;
    }
}