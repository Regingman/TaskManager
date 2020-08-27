using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        string userId = "";

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _logger = logger;
            _context = context;
        }


        public IActionResult Index()
        {
            //string[] rolesArray;
            // var 
            //Appl.Identity.GetUserId();
            //if (Roles.IsUserInRole(userName, "Administrator"))
            //{
            //    return RedirectToAction("Index", "AdminApp");
            //}
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
