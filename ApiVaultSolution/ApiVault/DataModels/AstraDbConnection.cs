using Cassandra;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

namespace ApiVault.DataModels
{
    public class AstraDbConnection
    {
        // Session pooling
        private static Cluster? cluster;
        private static ISession? session;

        /*
         * Stablish connection with API Vault database
         */
        public async Task InitializeConnection()
        {
            // New pooling options
            var poolingOptions = new PoolingOptions()
                    .SetCoreConnectionsPerHost(HostDistance.Local, 2)  // Set minimum number of connections for local hosts
                    .SetMaxConnectionsPerHost(HostDistance.Local, 10)  // Set maximum number of connections for local hosts
                    .SetCoreConnectionsPerHost(HostDistance.Remote, 1) // Set minimum number of connections for remote hosts
                    .SetMaxConnectionsPerHost(HostDistance.Remote, 5); // Set maximum number of connections for remote hosts

            // Init cluster
             try
            {
                cluster = await Task.Run(() =>
                {
                    return Cluster.Builder()
                   .WithCloudSecureConnectionBundle("secure-connect-apivault.zip")
                   .WithCredentials(Environment.GetEnvironmentVariable("CLIENT"), Environment.GetEnvironmentVariable("SECRET"))
                   .WithPoolingOptions(poolingOptions)
                   .Build();
                });

                Debug.Print("Initialize Connection Finished");
            }

            catch (Exception ex) 
            {
                Debug.Print("Db Connection error");
            }
        }

        public async Task<ISession> GetSession() 
        {
            if (session == null || session.IsDisposed)
            {
                session = await cluster.ConnectAsync(Environment.GetEnvironmentVariable("CLUSTER"));
            }

            return session;
        }

        // Dispose Connection
        public void DisposeDb()
        {
            if (session != null)
            {
                session.Dispose();
            }
        }
    }
}