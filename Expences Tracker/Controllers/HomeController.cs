using Expences_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Expences_Tracker.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public IActionResult Search(string searchType)
		{
			if (searchType == "Transactions" || searchType == "transactions")
			{
				return RedirectToAction("Index", "Transaction");
			}
			else if (searchType == "Categories" || searchType == "categories")
			{
				return RedirectToAction("Index", "Category");
			}

			// Default action if no valid search type is found
			return RedirectToAction("Index", "Home");
		}

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
