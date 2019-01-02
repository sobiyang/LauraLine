using System;

namespace LauraLine
{

    public class HomePageMenuItem
    {
        public HomePageMenuItem()
        {
            TargetType = typeof(CurrentRecordPage);
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }

        public Type TargetType { get; set; }
    }
}