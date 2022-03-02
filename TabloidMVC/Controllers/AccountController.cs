﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TabloidMVC.Models;
using TabloidMVC.Models.ViewModels;

using TabloidMVC.Repositories;

namespace TabloidMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUserTypesRepository _userTypeRepository;


        public AccountController(
            IUserProfileRepository userProfileRepository, 
            IUserTypesRepository userTypesRepository)
        {
            _userProfileRepository = userProfileRepository;
            _userTypeRepository = userTypesRepository;

        }

        [Authorize]
        public ActionResult Index()
        {
            //getting the current users ID
            int userId = GetCurrentUserId();

            //passing the current user's Id to get a single user

            UserProfile getUser = _userProfileRepository.GetUserById(userId);
            //list to provide all users
            List<UserProfile> users = _userProfileRepository.GetAllUsers();

            
           
            //admin conditional UserType 1 = Admin & current logged in users Id matches the user in the get single user method
            if (getUser.UserType.Id == 1 && userId == getUser.Id)
            {
                return View(users);
            }
            else
            {
                return NotFound();

            }
        }

       



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

<<<<<<< HEAD
        public ActionResult Create()
        {
            List<UserType> types = _userTypeRepository.GetAll();

            UserRegisterFormViewModel vm = new UserRegisterFormViewModel()
            {
                UserProfile = new UserProfile(),
                UserTypes = types
            };

            return View(vm);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserProfile userProfile)
        {
            try
            {
                _userProfileRepository.Add(userProfile);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                List<UserType> types = _userTypeRepository.GetAll();

                UserRegisterFormViewModel vm = new UserRegisterFormViewModel()
                {
                    UserProfile = new UserProfile(),
                    UserTypes = types
                };

                return View(vm);
            }
        }



=======
        // Logout will redirect to home page, where user will have option to log back in
>>>>>>> main
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
