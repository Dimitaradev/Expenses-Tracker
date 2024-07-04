using Expences_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;

namespace Expences_Tracker.Controllers
{
	public class DashBoardController : Controller
	{
		private ApplicationDbContext _context { get; set; }
		public DashBoardController(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<ActionResult> Index()
		{
			// last 7 day transactions
			DateTime startDate = DateTime.Now.AddDays(-6);
			DateTime endDate = DateTime.Today;

			List<Transaction> selectedTransactions = await _context.Transactions
				.Include(x => x.Category)
				.Where(x => x.Date >= startDate && x.Date <= endDate).ToListAsync();

			//Total income
			int totalIncome = selectedTransactions
				.Where(i => i.Category.Type == "Income")
				.Sum(j => j.Amount);
			ViewBag.TotalIncome = totalIncome.ToString("C0");

			//Total expense
			int totalExpenses = selectedTransactions
				.Where(i => i.Category.Type == "Expense")
				.Sum(j => j.Amount);
			ViewBag.TotalExpence = totalExpenses.ToString("C0");

			//Total balance
			int balance = totalIncome - totalExpenses;
			CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
			culture.NumberFormat.CurrencyNegativePattern = 1;
			ViewBag.Balance = String.Format(culture, "{0:C0}", balance);

			//Doughnut chart - expence by category
			ViewBag.DoughnutChartData = selectedTransactions
				.Where(i => i.Category.Type == "Expense")
				.GroupBy(j => j.CategoryId)
				.Select(k => new
				{
					categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
					amount = k.Sum(j => j.Amount),
					formattedAmount = k.Sum(j => j.Amount).ToString("C0"),
				})
				.OrderByDescending(l => l.amount)
				.ToList();

			//Spline chart income vs expense
			//income
			List<SplineChartData> incomeSummary = selectedTransactions
				.Where(i => i.Category.Type == "Income")
				.GroupBy(j => j.Date)
				.Select(k => new SplineChartData()
				{
					day = k.First().Date.ToString("dd-MMM"),
					income = k.Sum(l => l.Amount)
				})
				.ToList();

			//expence
			List<SplineChartData> expenseSummary = selectedTransactions
				.Where(i => i.Category.Type == "Expense")
				.GroupBy(j => j.Date)
				.Select(k => new SplineChartData()
				{
					day = k.First().Date.ToString("dd-MMM"),
					expense = k.Sum(l => l.Amount)
				})
				.ToList();

			//combine income and expense
			string[] lastSevenDays = Enumerable.Range(0, 7)
				.Select(i => startDate.AddDays(i).ToString("dd-MMM"))
				.ToArray();

			ViewBag.SplineChartData = from day in lastSevenDays
									  join income in incomeSummary on day equals income.day into dayIncomeJoined
									  from income in dayIncomeJoined.DefaultIfEmpty()
									  join expense in expenseSummary on day equals expense.day into expenseJoined
									  from expense in expenseJoined.DefaultIfEmpty()
									  select new
									  {
										  day = day,
										  income = income == null ? 0 : income.income,
										  expense = expense == null ? 0 : expense.expense,
									  };

			//Recent Transactions
			ViewBag.RecentTransactions = await _context.Transactions
			.Include(i => i.Category)
			.OrderByDescending(j => j.Date)
			.Take(5)
			.ToListAsync();

			return View();
		}
	}

	public class SplineChartData
	{
		public string day { get; set; }
		public int income { get; set; }
		public int expense { get; set; }
	}

}
