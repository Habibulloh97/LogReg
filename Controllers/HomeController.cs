using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LogReg.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;



namespace LogReg.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
        public HomeController(MyContext context)
        {
            _context=context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if (ModelState.IsValid)
            {
                if(_context.Users.Any(e=> e.Email== newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email is already registered!");
                    return View("Index");
                }
                else{
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password=Hasher.HashPassword(newUser, newUser.Password);
                    _context.Add(newUser);
                    _context.SaveChanges();
                    HttpContext.Session.SetInt32("loggedIn", newUser.UserId);
                    return RedirectToAction("Success");
                }
            }
            else{
                return View("Index");
            }
        }
        [HttpPost("login")]
        public IActionResult Login(LoginUser logUser)
        {
            if (ModelState.IsValid)
            {
                User UserInDb = _context.Users.FirstOrDefault(u=> u.Email == logUser.LEmail);
                if (UserInDb == null)
                {
                    ModelState.AddModelError("LEmail", "Email or Password is incorrect!");
                    return View("Index");
                }
                PasswordHasher<LoginUser> Hasher = new PasswordHasher<LoginUser>();
                PasswordVerificationResult result= Hasher.VerifyHashedPassword(logUser, UserInDb.Password, logUser.LPassword);
                if(result== 0)
                {
                    ModelState.AddModelError("LEmail", "Email or Password is incorrect!");
                    return View("Index");
                }
                else{
                    HttpContext.Session.SetInt32("loggedIn", UserInDb.UserId);
                    return RedirectToAction("Success");
                }
            }
            else {
                return View("Index");
            }
        }

        [HttpGet("Success")]
        public IActionResult Success()
        {
            int? loggedin=HttpContext.Session.GetInt32("loggedIn");
            if (loggedin!= null)
            {
                ViewBag.User= _context.Users.FirstOrDefault(a => a.UserId == (int) loggedin);
                return View();
            }
            else
            {
                return RedirectToAction ("Index");
            }
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
}
}
