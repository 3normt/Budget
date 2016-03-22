using System;

namespace Budget.Interfaces
{
    public interface IIncome :INumber {
  
    }

    public interface INumber // todo rename
    {
        decimal Amount { get; set; }
        DateTime Date { get; set; }
        string Title { get; set; }
    }
}