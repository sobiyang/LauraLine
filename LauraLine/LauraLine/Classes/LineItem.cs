using LauraLine.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LauraLine.Classes
{
    public class LineItem : MasterVM
    {
        public string Id { get; set; }

        private DateTime _logDate;
        public DateTime LogDate { get
            {
                return _logDate;
            }
            set
            {
                _logDate = value;
                OnPropertyChanged("LogDate");
            }
        }
        public TimeSpan LogTime { get; set; }
        public int LineType { get; set; }
        public bool IsBreastLine { get { return LineType == 1; } }
        public bool IsBottleLine { get { return LineType == 2; } }
        public bool IsDiaperLine { get { return LineType == 3; } }

        public LineItem()
        {
            LogDate = DateTime.Today;
            LogTime = DateTime.Now.TimeOfDay;
        }
    }

    public class BreastLineItem : LineItem
    {
        public int LeftRight { get; set; }

        private int _duration;
        public int Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration = value;
                OnPropertyChanged("Duration");
            }
        }

        public BreastLineItem() : base()
        {
            LeftRight = 0;
            _duration = 10;
            LineType = 1;
        }

        public BreastLineItem(int leftRight, int duration) : base()
        {
            LeftRight = leftRight;
            Duration = duration;
            LineType = 1;
        }
    }

    public class BottleLineItem : LineItem
    {
        private int _amount;
        public int Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
                OnPropertyChanged("Amount");
            }
        }

        public BottleLineItem() : base()
        {
            _amount = 30;
            LineType = 2;
        }

        public BottleLineItem(int amount) : base()
        {
            _amount = amount;
            LineType = 2;
        }
    }

    public class DiaperLineItem : LineItem
    {
        [JsonIgnore]
        public int PoopTimes
        {
            get
            {
                return HasPoop ? 1 : 0;
            }
        }
        public bool HasPoop { get; set; }

        private int _peeTimes;
        public int PeeTimes
        {
            get
            {
                return _peeTimes;
            }
            set
            {
                _peeTimes = value;
                OnPropertyChanged("PeeTimes");
            }
        }

        public DiaperLineItem() : base()
        {
            _peeTimes = 1;
            LineType = 3;
        }

        public DiaperLineItem(bool hasPoop, int peeTimes) : base()
        {
            HasPoop = hasPoop;
            _peeTimes = peeTimes;
            LineType = 3;
        }
    }
}
