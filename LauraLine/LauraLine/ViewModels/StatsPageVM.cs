using LauraLine.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LauraLine.ViewModels
{
    public class StatsPageVM : MasterVM
    {
        public List<DailyStats> Stats { get; set; }

        public void RefreshStats()
        {
            var tempList = ViewModelLocator.mainPageVM.AllLineItems.ToList();
            var datas = tempList.OrderByDescending(t => t.ItemDate)
                .Select(t => t.ItemDate).Distinct();
            Stats.Clear(); ;

            foreach (var date in datas)
            {
                try
                {
                    var dailyLineItems = tempList.Where(vm => vm.ItemDate == date);
                    Stats.Add(new DailyStats
                    {
                        Date = date,
                        SumBreastDuration = dailyLineItems
                        .Sum(vm => (vm.LineItem as BreastLineItem)?.Duration),
                        AvgBreastDuration = dailyLineItems
                        .Average(vm => (vm.LineItem as BreastLineItem)?.Duration),
                        SumBottleAmount = dailyLineItems
                        .Sum(vm => (vm.LineItem as BottleLineItem)?.Amount),
                        PoopTimes = dailyLineItems
                        .Sum(vm => (vm.LineItem as DiaperLineItem)?.PoopTimes),
                        PeeTimes = dailyLineItems
                        .Sum(vm => (vm.LineItem as DiaperLineItem)?.PeeTimes)
                    });
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                }
            }

            OnPropertyChanged("Stats");
        }

        public StatsPageVM() : base()
        {
            Stats = new List<DailyStats>();
        }
    }
}
