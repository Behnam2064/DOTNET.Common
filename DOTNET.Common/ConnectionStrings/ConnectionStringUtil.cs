using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DOTNET.Common.ConnectionStrings
{
    public class ConnectionStringUtil
    {
        /// <summary>
        /// Get User Id from connection string (SQL Server)
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        public string? GetUserId(string ConnectionString)
        {
            string? username = GetProperty(ConnectionString, "User ID");
            if (!string.IsNullOrEmpty(username))
            {
                return username;
            }
            else
            {
                return GetProperty(ConnectionString, "Uid");// MySQL
            }
        }

        /// <summary>
        /// Get Password from connection string (SQL Server)
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        public string GetPassword(string ConnectionString)
        {
            var Pwd = GetProperty(ConnectionString, "Password");
            if (!string.IsNullOrEmpty(Pwd))
            {
                return Pwd;
            }
            else
            {
                return GetProperty(ConnectionString, "Pwd");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        public string? GetDatabaseName(string ConnectionString)
        {

            string Catalog = GetProperty(ConnectionString, "Catalog");
            if (!string.IsNullOrWhiteSpace(Catalog) && !string.IsNullOrEmpty(Catalog))
                return Catalog;

            string Database = GetProperty(ConnectionString, "Database");

            if (!string.IsNullOrWhiteSpace(Database) && !string.IsNullOrEmpty(Database))
                return Database;


            return null;
        }

        public string? GetDataSource(string ConnectionString)
        {
            //The name or network address of the instance of SQL Server to which to connect. The port number can be specified after the server name:
            //server=tcp:servername, portnumber
            string ServerAddress = GetProperty(ConnectionString, "Data Source");
            if (!string.IsNullOrEmpty(ServerAddress))
                return ServerAddress;

            //When specifying a local instance, always use (local). To force a protocol, add one of the following prefixes:
            //np:(local), tcp:(local), lpc:(local)

            ServerAddress = GetProperty(ConnectionString, "Server");

            if (!string.IsNullOrEmpty(ServerAddress))
                return ServerAddress;

            //Beginning in .NET Framework 4.5, you can also connect to a LocalDB database as follows:
            //server=(localdb)\\myInstance

            ServerAddress = GetProperty(ConnectionString, "Addr");

            if (!string.IsNullOrEmpty(ServerAddress))
                return ServerAddress;

            //Please read below link
            //https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring?view=dotnet-plat-ext-7.0
            ServerAddress = GetProperty(ConnectionString, "Network Address");

            if (!string.IsNullOrEmpty(ServerAddress))
                return ServerAddress;


            return null;
        }

        /// <summary>
        /// try find CREATE DATABASE and read [MyDatabase]
        /// 
        /// string text = "CREATE DATABASE [MyDatabase]";
        /// string pattern = @"(?<=CREATE\s+DATABASE\s+)\s*\[(?<dbName>[^\]]+)\]";
        ///
        /// Match match = Regex.Match(text, pattern);
        ///
        /// if (match.Success)
        /// {
        ///     Console.WriteLine(match.Groups["dbName"].Value);
        /// }
        /// </summary>
        /// <param name="SqlScript"></param>
        /// <returns>MyDatabase</returns>
        private string GetDatabaseNameFromCreationScript(string SqlScript)
        {
            //ChatGDP
            // Empty space \s+
            //string pattern = @"(?<=CREATE DATABASE \[)[^\]]+(?=\])";              // space sensitive "]"
            //string pattern = @"(?<=CREATE\s+DATABASE\s+)\s*\[([^\]]+)\]";         // not space sensitive but include [ ]"

            string pattern = @"(?<=CREATE\s+DATABASE\s+)\s*\[(?<dbName>[^\]]+)\]";  // not include [ ] and not space sensitive "

            Match match = Regex.Match(SqlScript, pattern);

            if (match.Success)
            {
                return match.Groups["dbName"].Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="propertyName">Send like this 
        /// Failover Partner => Partner
        /// Initial Catalog  => Catalog
        /// Provider => Provider 
        /// </param>
        /// <returns>Return property value like 'DbTest', 'sa', '198.168.1.10', and ...</returns>
        public string? GetProperty(string ConnectionString, string propertyName)
        {
            Match match = GetPropertyRegex(ConnectionString, propertyName);

            if (match != null && match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="propertyName">Send like this 
        /// Failover Partner => Partner
        /// Initial Catalog  => Catalog
        /// Provider => Provider 
        /// </param>
        /// <returns>Return full property value like 'Database= DbTest','User Id= sa', 'Data Source= 198.168.1.10', and ...</returns>
        public string? GetFullProperty(string ConnectionString, string propertyName)
        {
            Match match = GetPropertyRegex(ConnectionString, propertyName);

            if (match != null && match.Success)
            {
                return match.Groups[0].Value;
            }

            return null;
        }

        public Match GetMatchMetadataFromConnectionString(string ConnectionString)
        {
            string reg = @"(metadata)*(.*)(connection\s*string\s*=)";

            return Regex.Match(ConnectionString, reg);

        }
        public string RemoveSQLServerMetadata(string ConnectionString)
        {
            var match = GetMatchMetadataFromConnectionString(ConnectionString);

            if (match.Success)
            {
                return ConnectionString.Replace(match.Value, string.Empty);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get all part of connection string as MatchCollection
        /// Do not support metadata
        /// </summary>
        /// <param name="connectionString">Your connection string</param>
        /// <param name="regexOptions">Your regex option to search properties</param>
        /// <returns>a list of property and value without ;</returns>
        public IList<string> GetAllPropertiesAsList(string connectionString, RegexOptions regexOptions)
        {
            MatchCollection matches = GetPropertiesAsMatchCollection(connectionString, regexOptions);

            IList<string> result = new List<string>();

            foreach (var item in matches)
            {
                var match = item as Match;

                result.Add(match.Value);
            }

            return result;
        }

        /// <summary>
        /// Get all part of connection string as MatchCollection
        /// Do not support metadata
        /// </summary>
        /// <param name="connectionString">Your connection string</param>
        /// <param name="regexOptions">Your regex option to search properties</param>
        /// <returns></returns>
        public MatchCollection GetPropertiesAsMatchCollection(string connectionString, RegexOptions regexOptions)
        {
            string reg = @"([a-zA-Z]\s*)*=\s*([^;]+)";

            return Regex.Matches(connectionString, reg, regexOptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="propertyName">Send like this 
        /// Failover Partner => Partner
        /// Initial Catalog  => Catalog
        /// Provider => Provider 
        /// </param>
        /// <returns>/returns>
        public Match GetPropertyRegex(string ConnectionString, string propertyName)
        {
            /// Difference between Initial Catalog and Database keyword in connection string
            /// https://stackoverflow.com/questions/12238548/difference-between-initial-catalog-and-database-keyword-in-connection-string
            /// The only difference is the name.
            /// These can be used interchangeably.
            /// https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlconnectionstringbuilder.initialcatalog?view=dotnet-plat-ext-7.0&redirectedfrom=MSDN#System_Data_SqlClient_SqlConnectionStringBuilder_InitialCatalog
            /// This property corresponds to the "Initial Catalog" and "database" keys within the connection string.

            #region If has two part
            // like 
            // Data Source | Integrated Security | Network Library | Initial Catalog
            //  ignore empty space between word 

            string pn = "";
            StringBuilder stringBuilder = new StringBuilder();
            if (propertyName.Split(' ') != null && propertyName.Split(' ').Length > 1)
            {
                var ps = propertyName.Split(' ');

                for (int i = 0; i < ps.Length; i++)//Data\s*Source\s*=\s*([^;]+)
                    if (!string.IsNullOrEmpty(ps[i]) && !string.IsNullOrWhiteSpace(ps[i]))
                        stringBuilder.Append((i > 0 ? @"\s*" : "") + ps[i]);


                pn = stringBuilder.ToString() + @"\s*";
            }
            else
            {
                pn = propertyName + @"\s*";
            }


            #endregion


            string PatternCatalog = $@"{pn}=\s*([^;]+)";
            //string PatternCatalogDatabase = @"Database\s*=\s*([^;]+)";

            /*Match match =*/
            return Regex.Match(ConnectionString, PatternCatalog, RegexOptions.IgnoreCase);
            /*
                        if (match.Success)
                        {
                            return match.Groups[1].Value;
                        }

                        return null;*/
        }

        public string ReplaceDataSourceProperty(string ConnectionString, string NewPropertyAndValue, bool IsConationProperty)
        {
            string? ServerAddress = GetProperty(ConnectionString, "Data Source");
            if (!string.IsNullOrEmpty(ServerAddress) && !string.IsNullOrWhiteSpace(ServerAddress))
            {
                if (!IsConationProperty)
                {
                    NewPropertyAndValue = "Data Source=" + NewPropertyAndValue;
                }
                return ReplaceProperty(ConnectionString, "Data Source", NewPropertyAndValue);
            }


            //When specifying a local instance, always use (local). To force a protocol, add one of the following prefixes:
            //np:(local), tcp:(local), lpc:(local)

            ServerAddress = GetProperty(ConnectionString, "Server");

            if (!string.IsNullOrEmpty(ServerAddress) && !string.IsNullOrWhiteSpace(ServerAddress))
            {
                if (!IsConationProperty)
                {
                    NewPropertyAndValue = "Server=" + NewPropertyAndValue;
                }
                return ReplaceProperty(ConnectionString, "Server", NewPropertyAndValue);
            }

            //Beginning in .NET Framework 4.5, you can also connect to a LocalDB database as follows:
            //server=(localdb)\\myInstance

            ServerAddress = GetProperty(ConnectionString, "Addr");

            if (!string.IsNullOrEmpty(ServerAddress) && !string.IsNullOrWhiteSpace(ServerAddress))
            {
                if (!IsConationProperty)
                {
                    NewPropertyAndValue = "Addr=" + NewPropertyAndValue;
                }
                return ReplaceProperty(ConnectionString, "Addr", NewPropertyAndValue);
            }

            //Please read below link
            //https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring?view=dotnet-plat-ext-7.0
            ServerAddress = GetProperty(ConnectionString, "Network Address");

            if (!string.IsNullOrEmpty(ServerAddress) && !string.IsNullOrWhiteSpace(ServerAddress))
            {
                if (!IsConationProperty)
                {
                    NewPropertyAndValue = "Network Address=" + NewPropertyAndValue;
                }
                return ReplaceProperty(ConnectionString, "Network Address", NewPropertyAndValue);
            }

            return string.Empty;

        }
        public string ReplaceProperty(string ConnectionString, string PropertyName, string NewPropertyAndValue)
        {
            var match = GetPropertyRegex(ConnectionString, PropertyName);
            if (match != null && match.Success)
            {
                if (!string.IsNullOrEmpty(NewPropertyAndValue) && !string.IsNullOrWhiteSpace(NewPropertyAndValue) && !NewPropertyAndValue.EndsWith(";"))
                    NewPropertyAndValue = NewPropertyAndValue + ";";

                return Regex.Replace(ConnectionString, $@"{PropertyName}=\s*([^;]+);", NewPropertyAndValue, RegexOptions.IgnoreCase);
            }
            else
            {
                return string.Empty;
            }
        }

        private bool IsConnectionStringBelongCurrentSystem(string cs)
        {

            /*
             Active Directory environment
            SQL Server authentication can be used on the same machine as SQL Server or on a remote connection.
            If you work in an Active Directory environment, Windows authentication is recommended to use. 
            If you work in a non-Active Directory environment, you can utilize SQL Server authentication for database connections.
             */

            /*string IntegratedSecurity = GetConnectionStringProperty(cs, "Integrated Security");
            if (!IntegratedSecurity.IsNull() && IntegratedSecurity.ToLower().Contains("true"))
                return true;*/


            string DataSource = GetDataSource(cs); //GetConnectionStringProperty(cs, "Server");
            DataSource = DataSource?.Trim();
            DataSource = DataSource?.ToLower();
            if (!string.IsNullOrEmpty(DataSource))
                throw new ArgumentNullException("DataSource", "DataSource is empty.(Server name)");

            if (DataSource.Contains(@"\")) // The connection string contains of instance name like n_server-pc\\sql2019
            {
                //n_server-pc\\sql2019
                DataSource = DataSource.Substring(0, DataSource.IndexOf(@"\"));
            }

            if (DataSource.Equals(".") || DataSource.Equals("localhost"))
                return true;
            else
            {
                //var dnsName = Dns.GetHostName();
                string CurrentSystemName = System.Environment.MachineName;
                if (!string.IsNullOrEmpty(CurrentSystemName))
                {
                    CurrentSystemName = System.Net.Dns.GetHostName();
                }

                CurrentSystemName = CurrentSystemName?.ToLower();

                if (DataSource.Equals(CurrentSystemName))
                    return true;


                if (IPAddress.TryParse(DataSource, out IPAddress ipAddress))
                {
                    throw new NotImplementedException();

                    /*
                                        var temp = UtilityNetwork.GetNetworkInterfaceAndRightIPv4Address();
                                        foreach (var item in temp)
                                        {
                                            if (item.Address.ToString().Equals(DataSource))
                                                return true;
                                        }
                    */
                }

                return false;

            }

        }

    }
}
