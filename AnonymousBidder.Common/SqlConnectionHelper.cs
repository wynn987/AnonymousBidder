using System;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Web.Configuration;

namespace AnonymousBidder.Common
{
    public static class SqlConnectionHelper
    {
        private static string[] connectionStrings;
       
        private static string ServerName
        {
            get
            {
                if (connectionStrings == null)
                {
                    connectionStrings = new string[4];
                    var parent = Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName;
                    StreamReader sr = new StreamReader(parent + "\\abc.txt");
                    sr.ReadLine();
                    sr.ReadLine();
                    sr.ReadLine();
                    connectionStrings[0] = sr.ReadLine();
                    connectionStrings[1] = sr.ReadLine();
                    connectionStrings[2] = Sercurity.Decrypt(sr.ReadLine());
                    connectionStrings[3] = Sercurity.Decrypt(sr.ReadLine());
                }
                return connectionStrings[0];
            }
        }
        private static string DatabaseName
        {
            get
            {
                if (connectionStrings == null)
                {
                    connectionStrings = new string[4];
                    var parent = Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName;
                    StreamReader sr = new StreamReader(parent + "\\abc.txt");
                    sr.ReadLine();
                    sr.ReadLine();
                    sr.ReadLine();
                    connectionStrings[0] = sr.ReadLine();
                    connectionStrings[1] = sr.ReadLine();
                    connectionStrings[2] = Sercurity.Decrypt(sr.ReadLine());
                    connectionStrings[3] = Sercurity.Decrypt(sr.ReadLine());
                }
                return connectionStrings[1];
            }
        }
        private static string Password
        {
            get
            {
                if (connectionStrings == null)
                {
                    connectionStrings = new string[4];
                    var parent = Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName;
                    StreamReader sr = new StreamReader(parent + "\\abc.txt");
                    sr.ReadLine();
                    sr.ReadLine();
                    sr.ReadLine();
                    connectionStrings[0] = sr.ReadLine();
                    connectionStrings[1] = sr.ReadLine();
                    connectionStrings[2] = Sercurity.Decrypt(sr.ReadLine());
                    connectionStrings[3] = Sercurity.Decrypt(sr.ReadLine());
                }
                return connectionStrings[3];
            }
        }
        private static string UserId
        {
            get
            {
                if (connectionStrings == null)
                {
                    connectionStrings = new string[4];
                    var parent = Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName;
                    StreamReader sr = new StreamReader(parent + "\\abc.txt");
                    sr.ReadLine();
                    sr.ReadLine();
                    sr.ReadLine();
                    connectionStrings[0] = sr.ReadLine();
                    connectionStrings[1] = sr.ReadLine();
                    connectionStrings[2] = Sercurity.Decrypt(sr.ReadLine());
                    connectionStrings[3] = Sercurity.Decrypt(sr.ReadLine());
                }
                return connectionStrings[2];
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
