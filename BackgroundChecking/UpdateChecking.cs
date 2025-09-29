using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.DataTransfer;
using Windows.Services.Store;
using Windows.Storage;

namespace BackgroundChecking
{
    public sealed class UpdateChecking : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            try
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("IsLatestVersion"))
                {
                    var ret = ApplicationData.Current.LocalSettings.Values["IsLatestVersion"];
                }
                else
                    ApplicationData.Current.LocalSettings.Values["IsLatestVersion"] = true;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                deferral.Complete();
            }
        }

        private async Task<bool> CheckIfNeedUpdateAsync()
        {
            StoreContext context = StoreContext.GetDefault();
            var updates = await context.GetAppAndOptionalStorePackageUpdatesAsync();

            if (updates.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}
