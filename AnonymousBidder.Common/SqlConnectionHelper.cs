using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace AnonymousBidder.Common
{
    public static class SqlConnectionHelper
    {
        private static string ServerName
        {
            get
            {
                if (WebConfigurationManager.AppSettings["ServerName"] != null)
                {
                    return WebConfigurationManager.AppSettings["ServerName"];
                }
                return string.Empty;
            }
        }
        private static string DatabaseName
        {
            get
            {
                if (WebConfigurationManager.AppSettings["DatabaseName"] != null)
                {
                    return WebConfigurationManager.AppSettings["DatabaseName"];
                }
                return string.Empty;
            }
        }
        private static string Password
        {
            get
            {
                if (WebConfigurationManager.AppSettings["Password"] != null)
                {
                    return Sercurity.Decrypt(WebConfigurationManager.AppSettings["Password"]);
                }
                return string.Empty;
            }
        }
        private static string UserId
        {
            get
            {
                if (WebConfigurationManager.AppSettings["UserID"] != null)
                {
                    return Sercurity.Decrypt(WebConfigurationManager.AppSettings["UserID"]);
                }
                return string.Empty;
            }
        }
        public static string GetEntityConnectionString()
        {
            string connectionString = new SqlConnectionStringBuilder
            {
                    InitialCatalog = DatabaseName,
                    DataSource = ServerName,
                    IntegratedSecurity = false,
                    UserID = UserId,
                    Password = Password,
                    MultipleActiveResultSets = true,
                    PersistSecurityInfo = true,
            }.ConnectionString;
            return connectionString;
        }
    }
}
