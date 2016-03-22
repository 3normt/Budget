using System;
using System.Collections.Generic;
using System.Linq;
using Budget.Interfaces;

namespace Budget
{
    public class Budget : IBudget{
        private readonly IDateService DateService;

        public List<IIncome> Income { get; set; }
        public List<IExpence> Expences { get; set; }
        public List<IBalance> Balances { get; set; } 

        public Budget(IDateService dateService)
        {
            this.DateService = dateService;
            InitArrays();
        }

        public Budget()
        {
            InitArrays();
        }

        private void InitArrays()
        {
            Income = new List<IIncome>();
            Expences = new List<IExpence>();
            Balances = new List<IBalance>();
        }

        public void AddExpence(IExpence expence)
        {
            Expences.Add(expence);
        }

        public int PayDay { get; set; }

        IEnumerable<Payment> IBudget.GetPayments()
        {
            throw new NotImplementedException();
        }

        public void AddIncome(IIncome income)
        {
            Income.Add(income);
        }

        public decimal GetTotalIncomeForYear(int year)
        {
            var sum = 0m; 
            foreach (var income in Income.Where(x => x.Date.Year == year))
            {
                sum += income.Amount;
            }
            return sum; 
        }

        public void AddIncomeRange(decimal monthlyAmount, DateTime dateFirstIncome, DateTime dateLastIncome, string titleForIncome)
        {
            // add first income
            Income.Add(new Income
            {
                Amount = monthlyAmount,
                Date = dateFirstIncome,
                Title = titleForIncome
            });

            // add last income
            Income.Add(new Income
            {
                Amount = monthlyAmount,
                Date = dateLastIncome,
                Title = titleForIncome
            });

            // add income inbetween
            var startDate = dateFirstIncome.AddMonths(1);
            while (startDate < dateLastIncome)
            {
                Income.Add(new Income
                {
                    Amount = monthlyAmount,
                    Date = startDate,
                    Title = titleForIncome
                });
                startDate = startDate.AddMonths(1);
            }
        }

        public decimal GetTotalExpenceAmount(int year)
        {
            decimal sum = 0;

            foreach (var ex in Expences.Where(x => x.Date.Year == year))
            {
                sum += ex.Amount;
            }

            return sum;
        }

        public decimal GetBalanceForDate(DateTime date)
        {
            var lastBalanceBeforeDate = Balances.Where(x => x.Date <= date).OrderByDescending(x => x.Date).FirstOrDefault();
            if(lastBalanceBeforeDate != null)
            {
                var sumExpences = Expences.Where(x => x.Date > lastBalanceBeforeDate.Date && x.Date <= date).Sum(x => x.Amount);
                var sumIncome = Income.Where(x => x.Date > lastBalanceBeforeDate.Date && x.Date <= date).Sum(x => x.Amount);
                var sum = sumIncome - sumExpences;
                return lastBalanceBeforeDate.Amount + sum; 
            }
            else
            {
                var sumExpences = Expences.Where(x => x.Date <= date).Sum(x => x.Amount);
                var sumIncome = Income.Where(x => x.Date <= date).Sum(x => x.Amount);
                return sumIncome - sumExpences;
            }
       
        }

        public void AddExpenceRange(string title, decimal amount, DateTime dateFirstIncome, DateTime dateLastIncome)
        {
            // add first income
            Expences.Add(new Expence
            {
                Amount = amount,
                Date = dateFirstIncome,
                Title = title
            });

            // add last income
            Expences.Add(new Expence
            {
                Amount = amount,
                Date = dateLastIncome,
                Title = title
            });

            // add income inbetween
            var startDate = dateFirstIncome.AddMonths(1);
            while (startDate < dateLastIncome)
            {
                Expences.Add(new Expence
                {
                    Amount = amount,
                    Date = startDate,
                    Title = title
                });
                startDate = startDate.AddMonths(1);
            }
        }

        public void AddIncome(int year, decimal amountForAFullYear, int paydayForMonth, string title)
        {
            var monthlyAmount = amountForAFullYear / 12; 
            AddIncomeRange(monthlyAmount, new DateTime(year, 1, paydayForMonth), new DateTime(year, 12, paydayForMonth), title);
        }

        public void SetBalance(DateTime date, decimal newBalance)
        {
            this.Balances.Add(new Balance { Date = date, Amount = newBalance });
        }

        public Payment GetPayments()
        {
            throw new NotImplementedException();
        }
    }
}