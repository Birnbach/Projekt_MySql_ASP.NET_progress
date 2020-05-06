using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication3.Models;
using WebApplication3.Baza;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult BookView()
        {
            ConnectionDB conn = HttpContext.RequestServices.GetService(typeof(WebApplication3.Baza.ConnectionDB)) as ConnectionDB;
            return View(conn.GetAllBooks());
        }
        public IActionResult AdminView()
        {
            ConnectionDB conn = HttpContext.RequestServices.GetService(typeof(WebApplication3.Baza.ConnectionDB)) as ConnectionDB;
            return View(conn.GetAllAdmins());
        }
        public IActionResult ReaderView()
        {
            ConnectionDB conn = HttpContext.RequestServices.GetService(typeof(WebApplication3.Baza.ConnectionDB)) as ConnectionDB;
            return View(conn.GetAllReaders());
        }
        public IActionResult RentalView()
        {
            ConnectionDB conn = HttpContext.RequestServices.GetService(typeof(WebApplication3.Baza.ConnectionDB)) as ConnectionDB;
            return View(conn.GetAllRentals());
        }

        public IActionResult LogIn()
        {            
            return View();
        }

        [HttpPost]
        public ActionResult UserDashboard(userModel model)
        {
            //Debug.Write("Check1");
            if (ModelState.IsValid)
            {
                ConnectionDB conn = HttpContext.RequestServices.GetService(typeof(WebApplication3.Baza.ConnectionDB)) as ConnectionDB;
                int userID = conn.checkUserID(model);
                // Debug.Write(userID + "\n");
                if(userID > 0) return View("UserDashboard");
                else return View("LoginError");
            }
            return View();
        }

        [HttpPost]
        public ActionResult SignUpAction(signupForm model)
        {
            //Debug.Write("Check1");
            if (ModelState.IsValid)
            {
                ConnectionDB conn = HttpContext.RequestServices.GetService(typeof(WebApplication3.Baza.ConnectionDB)) as ConnectionDB;
                int result = conn.createUser(model);
                Console.WriteLine(result);
                if(result > 0) return View("SignUpAction");
                else return View("SignUpError");
            }
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }

        public IActionResult UserDashboard()
        {            
            return View();
        }

        public IActionResult LoginError()
        {            
            return View();
        }

        public IActionResult SignUpError()
        {            
            return View();
        }
    }
}
