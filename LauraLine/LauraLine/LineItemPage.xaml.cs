using LauraLine.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LauraLine
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LineItemPage : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }

        public LineItemPage()
        {
            InitializeComponent();
        }

        private void ContentPage_Disappearing(object sender, EventArgs e)
        {
            var lineItemVM = BindingContext as LineItemVM;
            Task.Run(() => lineItemVM.SaveClicked());
        }
    }
}
