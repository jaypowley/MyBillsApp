using System;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBills.Mvc.Models;
using MyBills.Domain.Entities;
using MyBills.Services;

namespace MyBills.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private const string Format = "MMMM";
        private readonly IUserService _userService;
        private readonly IUserBillService _userBillService;
        private readonly ILoginRegisterService _loginRegisterService;

        public HomeController(IUserService userService, IUserBillService userBillService, ILoginRegisterService loginRegisterService)
        {
            _userService = userService;
            _userBillService = userBillService;
            _loginRegisterService = loginRegisterService;
        }

        [Authorize]
        public async Task<IActionResult> Index(int? month, int? year)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name); // will give the user's userName
            var usViewModel = new UserBillsViewModel();
            var userId = await _userService.GetUserId(userName);
            var navHelper = UserBillService.GetNavigationHelper(month, year);

            ViewBag.PreviousMonth = navHelper.PrevMonth;
            ViewBag.PreviousMonthName = new DateTime(navHelper.CurrentYear, navHelper.PrevMonth, 1).ToString(Format, CultureInfo.InvariantCulture);
            ViewBag.PreviousMonthsYear = navHelper.PrevMonthsYear;
            ViewBag.MonthName = new DateTime(navHelper.CurrentYear, navHelper.CurrentMonth, 1).ToString(Format, CultureInfo.InvariantCulture);
            ViewBag.NextMonth = navHelper.NextMonth;
            ViewBag.NextMonthName = new DateTime(navHelper.CurrentYear, navHelper.NextMonth, 1).ToString(Format, CultureInfo.InvariantCulture);
            ViewBag.NextMonthsYear = navHelper.NextMonthsYear;

            var userBills = await _userBillService.GetMonthlyBillSetByUserIdAndMonthYearAsync(userId, navHelper.CurrentMonth, navHelper.CurrentYear);
            var userDetails = await _userService.GetUserDetailByUserId(userId);

            ViewData["UsersFirstName"] = userDetails.FirstName;
            usViewModel.UserDetails = userDetails;
            usViewModel.UserMonthlyBillSet = userBills;

            return View(usViewModel);
        }

        [Authorize]
        public async Task<IActionResult> PayBill(int billId, int day, int month, int year)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var userId = await _userService.GetUserId(userName);

            await _userBillService.MarkBillAsPaidAsync(billId, userId, day, month, year);

            var referer = Request.Headers["Referer"].ToString();

            if (referer == string.Empty)
            {
                return RedirectToAction("Index");
            }
            return Redirect(referer);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region Auth

        public IActionResult Login()
        {
            var user = new UserLogin();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLogin login)
        {
            if (!ModelState.IsValid) return View(login);            

            if (LoginRegisterService.IsLoginValid(login.Username, login.Password))
            {
                login.IsSuccess = await _loginRegisterService.LoginAsync(login.Username, login.Password);

                if (!login.IsSuccess)
                {
                    ViewBag.ErrorMessage = "Your credentials were not valid. Please try again.";
                }
                else
                {
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Name, login.Username));

                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Your credentials were not valid. Please try again.";
            }

            return View(login);
        }

        public IActionResult Register()
        {
            var user = new UserRegistration();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegistration register)
        {
            if (!ModelState.IsValid) return View(register);            

            if (LoginRegisterService.IsRegistrationValid(register.Password, register.ConfirmPassword, register.Email, register.FriendlyName))
            {
                register.IsSuccess = await _loginRegisterService.RegisterNewUserAsync(register.Email, register.Password, register.FriendlyName);

                if (!register.IsSuccess)
                {
                    ViewBag.ErrorMessage = "Your account could not be created. Please try again.";
                }
                else
                {
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Name, register.Email));

                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.ErrorMessage = register.Password != register.ConfirmPassword ? "Your password and confirmation did not match." : "Your account could not be created. Please try again.";
            }

            return View(register);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}
