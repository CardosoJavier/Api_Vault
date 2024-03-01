using Cassandra;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiVault.DataModels
{
    internal class AstraDbConnection
    {
        /*
         * Stablish connection with API Vault database
         */
        public static async Task<ISession> Connect()
        {
            return await Task.Run(() => 
            {
                // Get connection data
                string[] connectionData = GetCredentials();

                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "ApiVault.DataModels.secure-connect-apivault.zip";

                // Create a temporary file path
                var tempFilePath = Path.GetTempFileName();

                File.Delete(tempFilePath); // Delete the temp file
                tempFilePath = Path.ChangeExtension(tempFilePath, ".zip"); // Change extension to .zip

                // Extract the embedded ZIP to the temporary file path
                using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
                using (var fileStream = File.Create(tempFilePath))
                {
                    resourceStream.CopyTo(fileStream);
                }

                var cluster = Cluster.Builder()
                    .WithCloudSecureConnectionBundle(tempFilePath)
                    .WithCredentials(connectionData[0], connectionData[1])
                    .Build();

                return cluster.Connect("apivault_space");
            });
        }

        /*
         * Get the credentials from JSON file
         */
        private static string[] GetCredentials()
        {
            // Connection data variables
            string[] connectionData = new string[2];

            var assembly = Assembly.GetExecutingAssembly();
            var resource = "ApiVault.DataModels.ApiVault-token.json";

            using (Stream stream = assembly.GetManifestResourceStream(resource))
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    JsonElement root = doc.RootElement;
                    connectionData[0] = root.GetProperty("clientId").GetString();
                    connectionData[1] = root.GetProperty("secret").GetString();
                }
            }

            return connectionData;

        }
    }
}
