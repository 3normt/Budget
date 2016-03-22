using System;
using System.Collections.Generic;

namespace Budget.Interfaces
{
    public interface IBudget{
        List<IIncome> Income { get; set; } 
        void AddExpence(IExpence expence);
        IEnumerable<Payment> GetPayments();
        int PayDay { get; set; }
        void AddIncome(IIncome income);
        decimal GetTotalIncomeForYear(int i);
        void AddIncomeRange(decimal monthlyAmount, DateTime dateFirstIncome, DateTime dateLastIncome, string titleForIncome);
        decimal GetTotalExpenceAmount(int year);
        decimal GetBalanceForDate(DateTime date);
        void AddExpenceRange(string title, decimal amount, DateTime firstMonthOfYear, DateTime lastMonthOfYear);
        void AddIncome(int year, decimal amount, int paydayForMonth, string title);
        void SetBalance(DateTime date, decimal newBalance);
    }
}