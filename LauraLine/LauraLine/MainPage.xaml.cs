using LauraLine.Utilities;
using LauraLine.ViewModels;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LauraLine
{
    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void ContentPage_Appearing(object sender, System.EventArgs e)
        {
            if(Application.Current.Properties.ContainsKey("synced"))
                Task.Run(ViewModelLocator.mainPageVM.RefreshDataAsync);
            else
            {
                Task.Run(SQLiteHandler.instance.SyncAsync).ContinueWith(t =>
                {
                    DisplayAlert("结果", "同步成功", "好");
                    Application.Current.Properties["synced"] = true;
                    Application.Current.SavePropertiesAsync();
                }, TaskScheduler.FromCurrentSynchronizationContext())
                .ContinueWith(t => ViewModelLocator.mainPageVM.RefreshDataAsync());
            }
        }

        private async void ContentPage_Disappearing(object sender, System.EventArgs e)
        {
            await SQLiteHandler.instance.SyncAsync();
        }

        private void BtnSync_Clicked(object sender, System.EventArgs e)
        {
            Task.Run(SQLiteHandler.instance.SyncAsync).ContinueWith(t => DisplayAlert("结果", "同步成功","好"), TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
