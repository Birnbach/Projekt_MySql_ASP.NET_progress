using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication3.Models;
using WebApplication3.Baza;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using demo.MVC.Class;
using demo.MVC.Models;
using Microsoft.AspNetCore.Html;


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
            string cityID = "3081368";      // Wroclaw
            string apiKey = "38832353be066e5198365b401c09da9f";

            HttpWebRequest apiRequest = WebRequest.Create("http://api.openweathermap.org/data/2.5/weather?id=" + cityID + "&APPID=" + apiKey + "&units=metric") as HttpWebRequest;
            string apiResponse = "";
            using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                apiResponse = reader.ReadToEnd();
            }
            ResponseWeather rootObject = JsonConvert.DeserializeObject<ResponseWeather>(apiResponse);
            StringBuilder sb = new StringBuilder();
            sb.Append("<table style=\"margin-left:auto;margin-right:auto;\">");
            sb.Append("<tr><td>Location:</td><td>" + rootObject.name + "</td></tr>");
            sb.Append("<tr><td>Wind:</td><td>" + rootObject.wind.speed + " m/s</td></tr>");
            sb.Append("<tr><td>Temperature:</td><td>" + (int)rootObject.main.temp + "°C</td></tr>");
            sb.Append("<tr><td>Humidity:</td><td>" + rootObject.main.humidity + "%</td></tr>");
            sb.Append("<tr><td>Pressure:</td><td>" + rootObject.main.pressure + " hPa</td></tr>");
            sb.Append("<tr><td>Overcast:</td><td>" + rootObject.weather[0].description + "</td></tr>");

            ViewBag.HtmlOutput = sb.ToString();
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public ViewResult BookView(string sortOrder, string searchString)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.IdSortParm = sortOrder == "Id_asc" ? "Id_desc" : "Id_asc";
            ViewBag.GenSortParm = sortOrder == "Gen_asc" ? "Gen_desc" : "Gen_asc";
            ViewBag.AutSortParm = sortOrder == "Aut_asc" ? "Aut_desc" : "Aut_asc";
            using (var context = new LibraryContext())
            {
                var boooks = from bk in context.books
                               select bk;

                if (!String.IsNullOrEmpty(searchString))
                {
                    boooks = boooks.Where(bk => bk.Title.Contains(searchString)
                                           || bk.Author.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "name_desc":
                        boooks = boooks.OrderByDescending(bk => bk.Title);
                        break;
                    case "Id_asc":
                        boooks = boooks.OrderBy(bk => bk.BooksID);
                        break;
                    case "Id_desc":
                        boooks = boooks.OrderByDescending(bk => bk.BooksID);
                        break;
                    case "Gen_asc":
                        boooks = boooks.OrderBy(s => s.Genre);
                        break;
                    case "Gen_desc":
                        boooks = boooks.OrderByDescending(bk => bk.Genre);
                        break;
                    case "Aut_asc":
                        boooks = boooks.OrderBy(s => s.Author);
                        break;
                    case "Aut_desc":
                        boooks = boooks.OrderByDescending(bk => bk.Author);
                        break;
                    default:
                        boooks = boooks.OrderBy(s => s.Title);
                        break;
                }
                return View(boooks.ToList());
            }
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

                if (userID > 0)
                {
                    _logger.LogInformation("User {User} logged in at {Time}", model.username, DateTime.Now);

                    return View("UserDashboard");
                }
                else
                {
                    _logger.LogError("Error while logging in.");
                    return View("LoginError");
                }
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
                if (result > 0)
                {
                    _logger.LogInformation("User {User} succesfully signed up at {Time}", model.username, DateTime.Now);
                    return View("SignUpAction");
                }
                else
                {
                    _logger.LogError("Error while signing up.");
                    return View("SignUpError");
                }
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
        public IActionResult Edit(int Id, int available, string Title,string Genre, int Edition)
        {
            Book book = new Book();
            book.BooksID = Id;
            book.Available = available;
            _logger.LogWarning("Book with Id {Id} is about to be edited. Title: {Title}, Genre: {Genre}, Edition: {Edition} etc... at {Time} ", Id, Title ,Genre, Edition, DateTime.Now);
            return View(book);
        }
        [HttpPost]
        public IActionResult Edit(Book book)
        {

            using (var context = new LibraryContext())
            {
                context.books.Update(book);
                context.SaveChanges();
            }
            //ConnectionDB conn = HttpContext.RequestServices.GetService(typeof(WebApplication3.Baza.ConnectionDB)) as ConnectionDB;
           // conn.EditBook(book.BooksID, book.Title, book.Author, book.Edition, book.Genre, book.Available);

            _logger.LogWarning("Edited book with Id: {Id}. Now it's: Title: {Title}, Genre: {Genre}, Edition: {Edition} etc... at {Time}", book.BooksID, book.Title,book.Genre, book.Edition, DateTime.Now);
            return RedirectToAction("BookView");
        }

        [HttpGet]
        public ActionResult Delete(int Id, int available, string author, string title, int edition, string genre)
        {
            Book book = new Book();
            book.BooksID = Id;
            book.Title = title;
            book.Available = available;
            book.Genre = genre;
            book.Author = author;
            book.Edition = edition;

            return View(book);

        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int Id)
        {
           // ConnectionDB conn = HttpContext.RequestServices.GetService(typeof(WebApplication3.Baza.ConnectionDB)) as ConnectionDB;
           // conn.DeleteBook(Id);
           using (var context = new LibraryContext())
            {
                context.books.Remove(context.books.Find(Id));
                context.SaveChanges();
            }

            _logger.LogWarning("Deleted book with Id: {Id} at {Time}", Id, DateTime.Now);


            return RedirectToAction("BookView");
        }
        public ActionResult Create(int Id)
        {
            Book book = new Book();
            using (var context = new LibraryContext())
            {
                var query = context.books.OrderByDescending(bk => bk.BooksID).FirstOrDefault();
                book.BooksID = query.BooksID + 1;
            }
                return View(book);
        }

        [HttpPost]
        public ActionResult Create(Book book)
        {
            
            using (var context = new LibraryContext())
            {
                var query = context.books.OrderByDescending(bk => bk.BooksID).FirstOrDefault();
                book.BooksID = query.BooksID + 1;

                context.books.Add(book);
                context.SaveChanges();
            }
           // ConnectionDB conn = HttpContext.RequestServices.GetService(typeof(WebApplication3.Baza.ConnectionDB)) as ConnectionDB;
           // conn.CreateBook(book.BooksID, book.Title, book.Author, book.Edition, book.Genre, book.Available);
           // var message = "Created book with Id: " + book.BooksID.ToString();
            _logger.LogWarning("Created book with Id: {Id}, Title: {Title}, Edition: {Edition} etc... at {Time}", book.BooksID, book.Title,book.Edition, DateTime.Now);

            return RedirectToAction("BookView");
        }
        public IActionResult Sort()
        {
            using (var context = new LibraryContext())
            {
                var list = context.books.OrderByDescending(bk => bk.BooksID);
                context.SaveChanges();

            }
            return RedirectToAction("BookView");
        }
    }
}
