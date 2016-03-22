using System;
using Budget.Interfaces;

namespace Budget
{
    public class Income :IIncome{
     
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}