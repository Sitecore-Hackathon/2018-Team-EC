using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.XConnectDataTool.app_code.Services.DataImporter.Services;

namespace Sitecore.XConnectDataTool.app_code.Services.DataImporter.Factories
{
    public class DataImporterFactory
    {
        /// <summary>
        /// Builds a concrete instance of the Wordpresservice repository.
        /// </summary>
        public static IDataImporterService Build()
        {
            return new DataImporterService();
        }
    }
}