using LauraLine.Utilities;
using LauraLine.ViewModels;
using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LauraLine
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : MasterDetailPage
    {
        public HomePage()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as HomePageMenuItem;
            if (item == null)
                return;

            var page = (Page)Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;

            Detail = new NavigationPage(page);
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }

        private void MasterDetailPage_Appearing(object sender, EventArgs e)
        {
            if (Application.Current.Properties.ContainsKey("synced"))
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
    }
}