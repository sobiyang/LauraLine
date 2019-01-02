using LauraLine.Classes;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LauraLine.Utilities
{
    public partial class SQLiteHandler
    {
        public static SQLiteHandler instance = new SQLiteHandler();
        IMobileServiceClient client;
        public IMobileServiceSyncTable<BreastLineItem> breastLineTable;
        public IMobileServiceSyncTable<BottleLineItem> bottleLineTable;
        public IMobileServiceSyncTable<DiaperLineItem> diaperLineTable;

        private SQLiteHandler()
        {
            client = new MobileServiceClient("https://laura-line.azurewebsites.net/");
            var store = new MobileServiceSQLiteStore("localstore.db");
            store.DefineTable<BreastLineItem>();
            store.DefineTable<BottleLineItem>();
            store.DefineTable<DiaperLineItem>();
            client.SyncContext.InitializeAsync(store);
            breastLineTable = client.GetSyncTable<BreastLineItem>();
            bottleLineTable = client.GetSyncTable<BottleLineItem>();
            diaperLineTable = client.GetSyncTable<DiaperLineItem>();
        }

        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await client.SyncContext.PushAsync();

                // The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                // Use a different query name for each unique query in your program.
                await breastLineTable.PullAsync("allBreastLine", breastLineTable.CreateQuery());
                await bottleLineTable.PullAsync("allBottleLine", bottleLineTable.CreateQuery());
                await diaperLineTable.PullAsync("allDiaperLine", diaperLineTable.CreateQuery());
            }
            catch (MobileServicePushFailedException exc)
            {
                if (exc.PushResult != null)
                {
                    syncErrors = exc.PushResult.Errors;
                }
            }

            // Simple error/conflict handling.
            if (syncErrors != null)
            {
                foreach (var error in syncErrors)
                {
                    if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
                    {
                        // Update failed, revert to server's copy
                        await error.CancelAndUpdateItemAsync(error.Result);
                    }
                    else
                    {
                        // Discard local change
                        await error.CancelAndDiscardItemAsync();
                    }

                    Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.Item["id"]);
                }
            }
        }
    }
}
