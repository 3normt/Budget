using System;

namespace Budget.Interfaces
{
    public class Balance : IBalance {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
    }
}