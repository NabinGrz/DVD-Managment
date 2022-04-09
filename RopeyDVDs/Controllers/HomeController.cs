using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RopeyDVDs.Data;
using RopeyDVDs.Models;
using RopeyDVDs.Services;
using System.Diagnostics;
using System.Security.Claims;

namespace RopeyDVDs.Controllers
{
    public class HomeController : Controller
    {
        private readonly RopeyDVDsContext dataBaseContext;
        private readonly UserServices _userService;

        private readonly ILogger<HomeController> _logger;

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