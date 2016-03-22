using System;
using Budget.Interfaces;

namespace Budget
{
    public class Expence : IExpence {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public Interval Interval { get; set; }
        public string Title { get; set; }
    }
}