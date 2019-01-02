using LauraLine.Classes;
using LauraLine.Utilities;
using LauraLine.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace LauraLine.ViewModels
{
    public class LineItemGroup: MasterVM
    {
        public string Heading { get; set; }
        public int LineItemsHeight
        {
            get
            {
                int height = 0;
                foreach(var lineItem in LineItems)
                {
                    height += lineItem.HeightRequest;
                }

                return height;
            }
        }
        public ObservableCollection<LineItemVM> LineItems { get; set; }
        public LineItemGroup(string heading)
        {
            Heading = heading;
            LineItems = new ObservableCollection<LineItemVM>();
            LineItems.CollectionChanged += LineItems_CollectionChanged;
        }

        private void LineItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove) {
                foreach (var vm in e.OldItems)
                {
                    (vm as LineItemVM).PropertyChanged -= LineItemVM_PropertyChanged;
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var vm in e.NewItems)
                {
                    (vm as LineItemVM).PropertyChanged += LineItemVM_PropertyChanged; ;
                }
            }
            OnPropertyChanged("LineItemsHeight");
        }

        private void LineItemVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsExpanded")
                OnPropertyChanged("LineItemsHeight");
        }
    }

    public class MainPageVM : MasterVM
    {
        public ObservableCollection<LineItemVM> LineItems { get; set; }
        public ObservableCollection<LineItemVM> AllLineItems { get; set; }
        public ICommand AddLineActionCommand { get; set; }

        async void AddLineActionClicked()
        {
            var action = await CurrentPage.DisplayActionSheet("添加事项", "取消", null, "母乳", "配方奶", "尿布");
            var noChange = false;
            var vm = new LineItemVM();
            switch (action)
            {
                case "母乳":
                    vm.LineItem = new BreastLineItem();
                    break;
                case "配方奶":
                    vm.LineItem = new BottleLineItem();
                    break;
                case "尿布":
                    vm.LineItem = new DiaperLineItem();
                    break;
                default:
                    noChange = true;
                    break;
            }

            if (!noChange)
            {
                LineItems.Add(vm);
                vm.ViewDetailsClicked();
            }
        }

        /*
        public void AddLineItem(LineItemVM vm)
        {
            LineItemGroup group;
            var heading = vm.LineItem.LogDate.ToString("yyyy-MM-dd");
            if (LineItemGroups.Any(g => g.Heading == heading))
            {
                group = LineItemGroups.Single(g => g.Heading == heading);
                group.LineItems.Insert(0, vm);
            }
            else
            {
                group = new LineItemGroup(heading);
                group.LineItems.Add(vm);
                LineItemGroups.Add(group);
            }
        }
        */

        public void AddLineItemBatch(IEnumerable<LineItem> lineItems)
        {
            foreach (var line in lineItems)
            {
                var vm = new LineItemVM();
                vm.LineItem = line;
                LineItems.Add(vm);
            }

            OnPropertyChanged("LineItems");
        }

        public async Task RefreshDataAsync()
        {
            IsLoading = true;
            LineItems.Clear();

            var listBreastLine = await SQLiteHandler.instance.breastLineTable.ToListAsync();
            var listBottleLine = await SQLiteHandler.instance.bottleLineTable.ToListAsync();
            var listDiaperLine = await SQLiteHandler.instance.diaperLineTable.ToListAsync();

            AddLineItemBatch(listBreastLine);
            AddLineItemBatch(listBottleLine);
            AddLineItemBatch(listDiaperLine);

            SortData();

            IsLoading = false;
        }

        public void SortData()
        {
            IsLoading = true;

            var tempAllItems = LineItems
                .OrderByDescending(l => l.LineItem.LogDate)
                .ThenByDescending(l => l.LineItem.LogTime)
                .ToList();
            AllLineItems = new ObservableCollection<LineItemVM>(tempAllItems);

            var tempItems = tempAllItems.Where(l => (DateTime.Now - l.LineItem.LogDate.Add(l.LineItem.LogTime)) <= TimeSpan.FromDays(1));
            LineItems = new ObservableCollection<LineItemVM>(tempItems);

            OnPropertyChanged("LineItems");
            OnPropertyChanged("AllLineItems");

            ViewModelLocator.statsPageVM.RefreshStats();

            IsLoading = false;
        }

        public MainPageVM() : base()
        {
            AddLineActionCommand = new Command(AddLineActionClicked);
            LineItems = new ObservableCollection<LineItemVM>();
            AllLineItems = new ObservableCollection<LineItemVM>();
        }
    }
}
