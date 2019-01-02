using System;

namespace LauraLine.Classes
{
    public class DailyStats
    {
        public string Date { get; set; }
        public double? AvgBreastDuration { get; set; }
        public int? SumBreastDuration { get; set; }
        public int? SumBottleAmount { get; set; }
        public int? PoopTimes { get; set; }
        public int? PeeTimes { get; set; }
    }
}
