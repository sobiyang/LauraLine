using LauraLine.Classes;
using LauraLine.Utilities;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace LauraLine.ViewModels
{
    public class LineItemVM : MasterVM
    {
        public int HeightRequest
        {
            get
            {
                if (IsExpanded)
                {
                    if (LineItem.LineType == 1)
                        return 350;
                    else
                        return 300;
                }
                else
                    return 60;
            }
        }

        public string ItemDate
        {
            get
            {
                return LineItem.LogDate.ToString("yyyy-MM-dd");
            }
        }

        public string ItemTime
        {
            get
            {
                return LineItem.LogDate.Date.Add(LineItem.LogTime).ToString("yyyy-MM-dd h:mm tt");
            }
        }

        public string ItemName
        {
            get
            {
                var itemName = "无";
                switch (LineItem.LineType)
                {
                    case 1:
                        var lineItem = (BreastLineItem)LineItem;
                        itemName = $"母乳 ({Constants.LeftRightOptions[lineItem.LeftRight]})";
                        break;
                    case 2:
                        itemName = $"配方奶";
                        break;
                    case 3:
                        itemName = $"尿布";
                        break;
                    default:
                        break;
                }
                return itemName;
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return LineItem.LogTime > TimeSpan.FromHours(12) ? Color.AliceBlue : Color.WhiteSmoke;
            }
        }

        public LineItem LineItem { get; set; }
        public bool IsExpanded { get; set; }
        public string ToggleExpandIcon
        {
            get
            {
                return IsExpanded ? "ic_expand_less.png" : "ic_expand_more.png";
            }
        }


        public ICommand ToggleExpandCommand { get; set; }
        void ToggleExpandClicked()
        {
            IsExpanded = !IsExpanded;
            OnPropertyChanged("IsExpanded");
            OnPropertyChanged("ToggleExpandIcon");
            OnPropertyChanged("HeightRequest");
        }

        public ICommand SaveCommand { get; set; }
        public async void SaveClicked()
        {
            ViewModelLocator.mainPageVM.IsLoading = true;
            switch (LineItem.LineType)
            {
                case 1:
                    if (string.IsNullOrEmpty(LineItem.Id))
                       await SQLiteHandler.instance.breastLineTable.InsertAsync(LineItem as BreastLineItem);
                    else
                       await SQLiteHandler.instance.breastLineTable.UpdateAsync(LineItem as BreastLineItem);
                    break;
                case 2:
                    if (string.IsNullOrEmpty(LineItem.Id))
                       await SQLiteHandler.instance.bottleLineTable.InsertAsync(LineItem as BottleLineItem);
                    else
                       await SQLiteHandler.instance.bottleLineTable.UpdateAsync(LineItem as BottleLineItem);
                    break;
                case 3:
                    if (string.IsNullOrEmpty(LineItem.Id))
                       await SQLiteHandler.instance.diaperLineTable.InsertAsync(LineItem as DiaperLineItem);
                    else
                       await SQLiteHandler.instance.diaperLineTable.UpdateAsync(LineItem as DiaperLineItem);
                    break;
                default:
                    break;
            }

            ViewModelLocator.mainPageVM.SortData();
        }

        public ICommand ViewDetailsCommand { get; set; }
        public async void ViewDetailsClicked()
        {
            var detailsPage = new LineItemPage();
            detailsPage.BindingContext = this;
            await Navigation.PushAsync(detailsPage);
        }

        public LineItemVM() : base()
        {
            ToggleExpandCommand = new Command(ToggleExpandClicked);
            SaveCommand = new Command(SaveClicked);
            ViewDetailsCommand = new Command(ViewDetailsClicked);
        }
    }
}
