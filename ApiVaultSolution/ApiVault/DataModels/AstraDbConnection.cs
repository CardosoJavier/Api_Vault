using Cassandra;
using System.Diagnostics;
using System.Threading.Tasks;
using System;

namespace ApiVault.DataModels
{
    internal class AstraDbConnection
    {
        // Session pooling
        private static Cluster? cluster;
        private static ISession? session;

        /*
         * Stablish connection with API Vault database
         */
        public static async Task Connect()
        {
            if (session != null && !session.IsDisposed) 
            {
                return;
            }

            else
            {
                await Task.Run(() =>
                {

                    var watch = System.Diagnostics.Stopwatch.StartNew();

                    var clusterWatch = System.Diagnostics.Stopwatch.StartNew();
                    cluster = Cluster.Builder()
                        .WithCloudSecureConnectionBundle("secure-connect-apivault.zip")
                        .WithCredentials(Environment.GetEnvironmentVariable("CLIENT"), Environment.GetEnvironmentVariable("SECRET"))
                        .Build();

                    clusterWatch.Stop();
                    Debug.Print($"GetCredentials(): {clusterWatch.ElapsedMilliseconds} ms");


                    session = cluster.Connect("apivault_space");
                    watch.Stop();
                    Debug.Print($"Cluster and session: {watch.ElapsedMilliseconds} ms");
                });
            }
        }

        public static async Task<ISession> GetSession() 
        {
            if (session == null || session.IsDisposed) 
            {
                await Connect();

                return session;
            }

            return session;
        }

        // Dispose Connection
        public static void DisposeDb()
        {
            if (session != null)
            {
                session.Dispose();
            }
        }
    }
}