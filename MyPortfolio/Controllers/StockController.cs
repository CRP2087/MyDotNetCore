using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyPortfolio.Models;
using MyPortfolio.Services;

namespace MyPortfolio.Controllers
{
    public class StockController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly StockPriceService _stockPriceService;
        public StockController(ApplicationDbContext context, StockPriceService stockPriceService)
        {
            _context = context;
            _stockPriceService = stockPriceService;
        }

        // GET: StockController1
        public async Task<IActionResult> Index()
        {
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var userStocks = await _context.StockList.Where(s => s.Username == username).ToListAsync();

            //var stocks = await _context.StockList.ToListAsync(); // or use .ToList() if not async
            return View(userStocks);
            //return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetLatestPrices()
        {
            var username = HttpContext.Session.GetString("Username");

            var stocks = await _context.StockList
                .Where(s => s.Username == username)
                .ToListAsync();
            Dictionary<string, double> data = new Dictionary<string, double>();

            
            try
            {
                data = await _stockPriceService.GetPriceAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            foreach (var stock in stocks)
            {
                foreach(var item in data)
                {
                    if (item.Key.ToString() == stock.Symbol.ToUpper())
                        stock.CurrentPrice = (decimal)item.Value;
                }
            }


            return Json(stocks.Select(s => new
            {
                s.Symbol,
                s.CurrentPrice,
                ProfitLoss = (s.CurrentPrice - s.BuyPrice) * s.Quantity
            }));
        }

        // GET: StockController1/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: StockController1/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Stock stock)
        {
            stock.Notes = "WelCome";
            stock.CurrentPrice = 0;

            if (ModelState.IsValid)
            {
                _context.StockList.Add(stock);
                await _context.SaveChangesAsync();
                TempData["ShowToast"] = true;
                return RedirectToAction("Index");
            }
            return View(stock);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var stock = await _context.StockList.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }
            return View(stock);
        }

        // POST: StockController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Stock stock)
        {
            if (id != stock.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the logged-in username (assuming it's stored in session or claims)
                    var username = HttpContext.Session.GetString("Username"); // or from User.Identity.Name

                    // Fetch the stock record that matches both ID and Username
                    var existingStock = await _context.StockList
                        .FirstOrDefaultAsync(s => s.Id == id && s.Username == username);

                    if (existingStock == null)
                        return NotFound();

                    // Update the fields manually
                    existingStock.Symbol = stock.Symbol;
                    existingStock.BuyPrice = stock.BuyPrice;
                    existingStock.Quantity = stock.Quantity;
                    existingStock.CurrentPrice = stock.CurrentPrice;
                    existingStock.BuyDate = stock.BuyDate;
                    existingStock.CompanyName = stock.CompanyName;
                    
                    // ... add all other fields you allow to be updated

                    await _context.SaveChangesAsync();
                    //return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Problem("Concurrency issue occurred while updating.");
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: StockController1/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: StockController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var stock = await _context.StockList.FindAsync(id);
                
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Alerts()
        {
            return View(_context.StockList);
        }
    }
}
