using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace SxAStatisticsTool
{
    public class Constants
    {
        public static class Paths
        {
            public static readonly string DefaultPath = Sitecore.Configuration.Settings.GetSetting("SxAStatisticsTool.Paths.DefaultPath");
        }
        public static class Values
        {
            public static readonly string RecommendedRenderingsItemName = Sitecore.Configuration.Settings.GetSetting("SxAStatisticsTool.Values.RecommendedRenderingsItemName");
        }
        public static class Templates
        {
            public static Guid Page = new Guid("{917B68DB-F17A-4941-9D50-849132F5E441}");
        }
    }
}