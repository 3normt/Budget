using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Budget.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Budget.Tests
{
    [TestClass]
    public class BudgetTests
    {
        [TestMethod]
        public void Budget_has_income()
        {
            IBudget b = new Budget();

            for (int i = 0; i < 12; i++)
            {
                b.AddIncome(new Income {
                    Amount = 30000,
                    Date = new DateTime(2016, i + 1, 15) 
                });
            }

            decimal totalIncomeFor2016 = b.GetTotalIncomeForYear(2016);

            Assert.AreEqual(expected: 360000, actual: totalIncomeFor2016);
            
        }

        [TestMethod]
        public void Budget_can_add_income_for_period()
        {
            IBudget b = new Budget();
            
            var dateFirstIncome = new DateTime(2016, 1, 16);
            var dateLastIncome = new DateTime(2016, 12, 16);

            b.AddIncomeRange(30000, dateFirstIncome, dateLastIncome, ""); 

            var actual = b.GetTotalIncomeForYear(2016);
            var expected = 360000;
            
            Assert.AreEqual(expected,actual);
        }

        [TestMethod]
        public void Budget_has_no_income_for_next_year()
        {
            IBudget b = new Budget();

            var dateFirstIncome = new DateTime(2016, 1, 16);
            var dateLastIncome = new DateTime(2016, 12, 16);

            b.AddIncomeRange(30000, dateFirstIncome, dateLastIncome, "");

            var actual = b.GetTotalIncomeForYear(2017);
            var expected = 0;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Budget_has_no_income_for_last_year()
        {
            IBudget b = new Budget();

            var dateFirstIncome = new DateTime(2016, 1, 16);
            var dateLastIncome = new DateTime(2016, 12, 16);

            b.AddIncomeRange(30000, dateFirstIncome, dateLastIncome, "");

            var actual = b.GetTotalIncomeForYear(2015);
            var expected = 0;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Budget_has_expence()
        {
            IBudget b = new Budget();
            
            b.AddExpence(new Expence()
            {
                Amount = 4000,
                Date = new DateTime(2016,12,24),
                Interval = Interval.None
            });

            var expectedExpenceAmount = 4000;
            var actualExpenceAmount = b.GetTotalExpenceAmount(year:2016); 

            Assert.AreEqual(expectedExpenceAmount, actualExpenceAmount);
        }

        [TestMethod]
        public void Budget_has_no_expence_for_specific_year()
        {
            IBudget b = new Budget();

            b.AddExpence(new Expence()
            {
                Amount = 4000,
                Date = new DateTime(2016, 12, 24),
                Interval = Interval.None
            });

            var expectedExpenceAmount = 0;
            var actualExpenceAmount = b.GetTotalExpenceAmount(year: 2017);

            Assert.AreEqual(expectedExpenceAmount, actualExpenceAmount);
        }

        [TestMethod]
        public void Budget_balance_for_specific_month()
        {
            IBudget b = new Budget();

            var christmas = new DateTime(2016, 12, 24); 

            b.AddExpence(new Expence()
            {
                Amount = 5000,
                Date = christmas,
                Interval = Interval.None
            });

            b.AddIncome(new Income()
            { 
                Amount = 10000,
                Date = christmas
            });

            var expectedBalance = 5000;
            var actualBalance = b.GetBalanceForDate(date: christmas);

            Assert.AreEqual(expectedBalance, actualBalance);
        }

        [TestMethod]
        public void Budget_balance_for_specific_month_is_less_than_zero()
        {
            IBudget b = new Budget();

            var christmas = new DateTime(2016, 12, 24);

            b.AddExpence(new Expence()
            {
                Amount = 5000,
                Date = christmas,
                Interval = Interval.None
            });

            b.AddIncome(new Income()
            {
                Amount = 10000,
                Date = christmas.AddDays(1)
            });

            var expectedBalance = -5000;
            var actualBalance = b.GetBalanceForDate(date: christmas);

            Assert.AreEqual(expectedBalance, actualBalance);
        }

        [TestMethod]
        public void Budget_has_two_sources_of_income()
        {
            IBudget b = new Budget();

            var firstMonthOfYear = new DateTime(2016,1,15);
            var lastMonthOfYear = new DateTime(2016, 12, 15);

            b.AddIncomeRange(30000, firstMonthOfYear, lastMonthOfYear, titleForIncome: "Lønn, Arne");
            b.AddIncomeRange(26000, firstMonthOfYear, lastMonthOfYear, titleForIncome: "Lønn, Anne");

            Assert.AreEqual(expected: 672000, actual:b.GetTotalIncomeForYear(2016));
        }

        [TestMethod]
        public void Budget_balance_with_only_income_increases_over_time()
        {
            IBudget b = new Budget();

            var firstMonthOfYear = new DateTime(2016, 1, 15);
            var lastMonthOfYear = new DateTime(2016, 12, 15);

            b.AddIncomeRange(30000, firstMonthOfYear, lastMonthOfYear, titleForIncome: "Lønn, Arne");
            b.AddIncomeRange(26000, firstMonthOfYear, lastMonthOfYear, titleForIncome: "Lønn, Anne");

            Assert.AreEqual(0, b.GetBalanceForDate(new DateTime(2016,1,1)));
            Assert.AreEqual(56000, b.GetBalanceForDate(new DateTime(2016, 1, 20)));
            Assert.AreEqual(56000, b.GetBalanceForDate(new DateTime(2016, 2, 14)));
            Assert.AreEqual(112000, b.GetBalanceForDate(new DateTime(2016, 2, 15)));
        }

        [TestMethod]
        public void Budget_balance_with_income_and_expences_increases_over_time_with_expence_range()
        {
            IBudget b = new Budget();

            var firstMonthOfYear = new DateTime(2016, 1, 16);
            var lastMonthOfYear = new DateTime(2016, 12, 16);

            b.AddIncomeRange(1000, firstMonthOfYear, lastMonthOfYear, titleForIncome: "Lønn, Arne");
            b.AddIncomeRange(1000, firstMonthOfYear, lastMonthOfYear, titleForIncome: "Lønn, Anne");

            Assert.AreEqual(24000, b.GetTotalIncomeForYear(2016));
            
            b.AddExpenceRange(title: "Mat", amount:500m, firstMonthOfYear:firstMonthOfYear, lastMonthOfYear:lastMonthOfYear);

            Assert.AreEqual(6000, b.GetTotalExpenceAmount(2016));

            var balanceDecember2016 = b.GetBalanceForDate(new DateTime(2016, 12, 30)); 

            Assert.AreEqual(18000, balanceDecember2016);
        }

        [TestMethod]
        public void Budget_add_income_for_a_year_get_correct_balance()
        {
            IBudget b = new Budget();

            b.AddIncome(year: 2016, amount: 300000, paydayForMonth: 15, title: "Lønn");

            Assert.AreEqual(300000, b.GetBalanceForDate(new DateTime(2016,12,31)));
        }

        [TestMethod]
        public void Budget_balance_next_year()
        {
            IBudget b = new Budget();
            b.AddIncome(year: 2016, amount: 12000, paydayForMonth: 16, title: "Lønn Arne");
            b.AddIncome(year: 2016, amount: 24000, paydayForMonth: 16, title: "Lønn Anne");

            var totalIncome = b.GetTotalIncomeForYear(2016); 
            Assert.AreEqual(expected:36000,actual:totalIncome);

            var firstOfJuly2017 = new DateTime(2017,6,1);
            var balanceForJuneNextYear = b.GetBalanceForDate(firstOfJuly2017);

            Assert.AreEqual(expected:36000, actual: balanceForJuneNextYear);
        }

        [TestMethod]
        public void Budget_balance_can_be_adjusted()
        {
            IBudget b = new Budget();
            b.AddIncome(year: 2016, amount: 12000, paydayForMonth: 16, title: "Lønn Arne");
            b.AddIncome(year: 2016, amount: 24000, paydayForMonth: 16, title: "Lønn Anne");

            var x = new DateTime(2016, 6,20);

            b.SetBalance(date: x, newBalance: 50000);

            var balanceForFirstOfJuly = b.GetBalanceForDate(new DateTime(2016, 7, 1));
            Assert.AreEqual(expected:50000, actual: balanceForFirstOfJuly);
        }

        [TestMethod]
        public void Budget_balance_can_be_adjusted_on_a_date_where_there_is_income()
        {
            IBudget b = new Budget();
            b.AddIncome(year: 2016, amount: 12000, paydayForMonth: 15, title: "Lønn Arne");
            b.AddIncome(year: 2016, amount: 24000, paydayForMonth: 15, title: "Lønn Anne");

            var dateForAdjustment = new DateTime(2016,6,15); // this day, we will also have income (Lønn Arne & Al)

            b.SetBalance(dateForAdjustment, 0);

            // the balance for next month should be 3000,-
            var balanceForJulyOnNextPayment = b.GetBalanceForDate(new DateTime(2016, 7, 15));

            Assert.AreEqual(expected:3000, actual:balanceForJulyOnNextPayment);


        }
    }
}