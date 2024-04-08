using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using EventEase.Data;


namespace EventEase.Controllers
{
    public class LoginController : Controller
    {

        //Declare AppDbContent
        private readonly ApplicationDbContext _context;
        public const string SessionKeyId = "_lid";
        public const string SessionKeyRole = "_lrole";

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(new Login());
        }
        [HttpPost]
        public IActionResult Index(Login loggedin)
        {
            Data.EF.Login l = _context.login.FirstOrDefault(x =>
            x.Username == loggedin.Username
            && x.Password == loggedin.Password);
            if (l == null)
            {
                ViewData["Error"] = "Username ose password nuk eshte i sakte!";
                return View(new Login());
            }
            else
            {
                HttpContext.Session.SetInt32(SessionKeyId, l.id);
                HttpContext.Session.SetString(SessionKeyRole, l.Role.ToString());
                if (l.Role == Enums.Role.Admin)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove(SessionKeyId);
            HttpContext.Session.Remove(SessionKeyRole);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            Register register = new Register();
            return View(register);
        }

        [HttpPost]
        public IActionResult Register(Register client)
        {

            if (ClientValidation(client))
            {
                Data.EF.Login login = new Data.EF.Login
                {
                    Password = client.Password,
                    Username = client.Username,
                    Role = Enums.Role.Client
                };
                _context.login.Add(login);
                _context.SaveChanges();

                Data.EF.Users newclient = new Data.EF.Users
                {
                    LoginId = login.id,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    E_mail = client.E_mail,
                };

                _context.Add(newclient);
                _context.SaveChanges();

                return RedirectToAction("Index", "Login");
            }
            else
            {
                return View(client);
            }
        }

        public bool ClientValidation(Register client)
        {
            if (_context.login.FirstOrDefault(x => x.Username == client.Username) != null)
            {
                ViewData["Error"] = "Already exist an user with username: " + client.Username;
                return false;
            }

            if (client.Password != client.ConfirmPassword)
            {
                ViewData["Error"] = "Please Confirm password again";
                client.Password = "";
                client.ConfirmPassword = "";
                return false;
            }


            return true;
        }
    }
}
