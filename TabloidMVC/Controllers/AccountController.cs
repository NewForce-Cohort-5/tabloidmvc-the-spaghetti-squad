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


        public AccountController(
            IUserProfileRepository userProfileRepository
            )
        {
            _userProfileRepository = userProfileRepository;


        }

        [Authorize]
        public ActionResult Index()
        {
            try
            {
                List<UserProfile> users = _userProfileRepository.GetAllUsers();
                foreach (UserProfile user in users)
                {
                    var Active = !user.Deactivated;
                   

                        return View(Active.ToString());
                    
                }
            }

            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("Index");
        }

    

    public ActionResult Create()
    {
        return View();
    }


       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserProfile userProfile)
        {
            try
            {
                //adds new userProfile

                _userProfileRepository.Add(userProfile);

                userProfile = _userProfileRepository.GetByEmail(userProfile.Email);

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userProfile.Id.ToString()),
                new Claim(ClaimTypes.Email, userProfile.Email),
                new Claim(ClaimTypes.Role, userProfile.UserType.Name)
            };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                //I believe this signs checks and signs us in

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

              

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return View(userProfile);
            }
        }

        public IActionResult Details(int id)
        {
            var user = _userProfileRepository.GetUserById(id);
            
           
            return View(user);
        }

        [Authorize]

        //soft delete or deactivate
        public ActionResult Delete(int id)
        {
            int currentUserId = GetCurrentUserId();

            UserProfile userProfile = _userProfileRepository.GetUserById(id);

            //prevent navigation to delete with param for null users and if current user isn't admin
            if (userProfile != null || User.IsInRole("Admin"))
            {
                return View(userProfile);
                
            }

            return NotFound();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, UserProfile userProfile)
        {
            //goes back and gets all the user information so I can access later
            userProfile = _userProfileRepository.GetUserById(id);

            try
            {
                //access the User Type name of the selected user and if they aren't an admin runs deactivation
                if (userProfile.UserType.Name != "Admin")
                {
                    _userProfileRepository.DeactivateUser(id);

                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                return View(userProfile);
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

            //added conditional to check for deactivation

            if (userProfile == null || userProfile.Deactivated == true)
            {
                ModelState.AddModelError("Email", "Invalid email");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userProfile.Id.ToString()),
                new Claim(ClaimTypes.Email, userProfile.Email),
                new Claim(ClaimTypes.Role, userProfile.UserType.Name)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }



        // Logout will redirect to home page, where user will have option to log back in

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
