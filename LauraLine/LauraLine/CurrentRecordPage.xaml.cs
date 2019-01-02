using LauraLine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LauraLine
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrentRecordPage : ContentPage
    {
        public CurrentRecordPage()
        {
            InitializeComponent();
        }

        private void BtnSync_Clicked(object sender, EventArgs e)
        {
            Task.Run(SQLiteHandler.instance.SyncAsync)
                .ContinueWith(t => DisplayAlert("结果", "同步成功", "好"), TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}