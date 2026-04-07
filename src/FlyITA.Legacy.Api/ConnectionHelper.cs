using System.Configuration;
using PCentralLib;

namespace FlyITA.Legacy.Api
{
    /// <summary>
    /// Resolves connection strings the same way the legacy FlyITA app does:
    /// {ROLE}_{Database} from Web.config, decrypted via PCentralConfiguration.
    /// </summary>
    public static class ConnectionHelper
    {
        public static string PerformanceCentral => GetConnectionString("PerformanceCentral");
        public static string ITAEnterprise => GetConnectionString("ITAEnterprise");
        public static string WebRegCustom => GetConnectionString("WebRegCustom");
        public static string WebRegAdmin => GetConnectionString("WebRegAdmin");

        private static string GetConnectionString(string database, bool decrypt = true)
        {
            string role = GetRole();
            string environment = PCentralConfiguration.GetCurrentEnvironment(role, true);
            string key = environment.ToUpper() + "_" + database;

            var connectionSetting = ConfigurationManager.ConnectionStrings[key];
            string connection = connectionSetting?.ConnectionString;

            if (string.IsNullOrWhiteSpace(connection))
            {
                connectionSetting = ConfigurationManager.ConnectionStrings[database];
                connection = connectionSetting?.ConnectionString;
            }

            if (!string.IsNullOrWhiteSpace(connection) && decrypt)
            {
                connection = PCentralConfiguration.decrypt_connection_string(connection);
            }

            return connection ?? string.Empty;
        }

        private static string GetRole()
        {
            return ConfigurationManager.AppSettings["EnvironmentRole"] ?? "CDT";
        }
    }
}
