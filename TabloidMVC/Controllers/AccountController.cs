using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TabloidMVC.Models;
using TabloidMVC.Repositories;

namespace TabloidMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserProfileRepository _userProfileRepository;

        public AccountController(IUserProfileRepository userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }

        [Authorize]
        public ActionResult Index()
        {
            int userId = GetCurrentUserId();

            UserProfile getUser = _userProfileRepository.GetUserById(userId);

            List<UserProfile> users = _userProfileRepository.GetAllUsers();

            //pass the current user into get Users by type
           

            if (getUser.UserType.Id == 1 && userId == getUser.Id)
            {
                return View(users);
            }
            else
            {
                return NotFound();

            }
        }

        // GET: DogsController/Details/5
       



        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Credentials credentials)
        {
            var userProfile = _userProfileRepository.GetByEmail(credentials.Email);

            if (userProfile == null)
            {
                ModelState.AddModelError("Email", "Invalid email");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userProfile.Id.ToString()),
                new Claim(ClaimTypes.Email, userProfile.Email),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        //get the current user & parse id
        public int GetCurrentUserId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}
