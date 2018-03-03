using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SxAStatisticsTool.Repositories.Helpers;

namespace SxAStatisticsTool.Repositories.Factories
{
    public class DatabaseReaderFactory
    {
        /// <summary>
        /// Instance of IBlogRepository.
        /// </summary>
        private static IDatabaseReader _databaseReader;

        /// <summary>
        /// Static constructor for the DatabaseReaderFactory class that instantiates the instance of IBlogRepository.
        /// </summary>
        static DatabaseReaderFactory()
        {
            _databaseReader = new DatabaseReader();
        }

        /// <summary>
        /// Returns a instance of class that implements the IDatabaseReader interface.
        /// </summary>
        public static IDatabaseReader Build()
        {
            return _databaseReader;
        }
    }
}