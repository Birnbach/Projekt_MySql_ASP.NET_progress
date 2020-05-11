using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication3.Models;
using WebApplication3.Baza;
using System.Text;

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
            ConnectionDB conn = HttpContext.RequestServices.GetService(typeof(WebApplication3.Baza.ConnectionDB)) as ConnectionDB;
            conn.EditBook(book.BooksID, book.Title, book.Author, book.Edition, book.Genre, book.Available);

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
            ConnectionDB conn = HttpContext.RequestServices.GetService(typeof(WebApplication3.Baza.ConnectionDB)) as ConnectionDB;
            conn.DeleteBook(Id);

            _logger.LogWarning("Deleted book with Id: {Id} at {Time}", Id, DateTime.Now);


            return RedirectToAction("BookView");
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Book book)
        {
            
            ConnectionDB conn = HttpContext.RequestServices.GetService(typeof(WebApplication3.Baza.ConnectionDB)) as ConnectionDB;
            conn.CreateBook(book.BooksID, book.Title, book.Author, book.Edition, book.Genre, book.Available);

           // var message = "Created book with Id: " + book.BooksID.ToString();
            _logger.LogWarning("Created book with Id: {Id}, Title: {Title}, Edition: {Edition} etc... at {Time}", book.BooksID, book.Title,book.Edition, DateTime.Now);

            return RedirectToAction("BookView");

        }
    }
}
