using System;
using System.Configuration;

namespace SmartBank.DataAccess
{
    internal static class DataAccessSettings
    {
        public static string ConnectionString
        {
            get
            {
                ConnectionStringSettings settings =
                    ConfigurationManager.ConnectionStrings["SmartBankDB"];

                if (settings == null)
                    throw new InvalidOperationException("Connection string 'SmartBankDB' was not found.");

                return settings.ConnectionString;
            }
        }
    }
}
