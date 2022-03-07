using Microsoft.AspNetCore.Authentication;
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
using System.Linq;

using TabloidMVC.Repositories;

namespace TabloidMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUserTypeRepository _userTypeRepository;



        public AccountController(
            IUserProfileRepository userProfileRepository,
            IUserTypeRepository userTypeRepository
            )
        {
            _userProfileRepository = userProfileRepository;
            _userTypeRepository = userTypeRepository;


        }

        [Authorize]
        public ActionResult Index()
        {


            List<UserProfile> activeUsers = new List<UserProfile>();
            {
                List<UserProfile> users = _userProfileRepository.GetAllUsers();

                foreach (UserProfile user in users)
                if(!user.Deactivated)
                activeUsers.Add(user);
            }

            return View(activeUsers);

            
        }


        [Authorize]
        public ActionResult DeactiveList()
        {
            List<UserProfile> deactivatedUsers = new List<UserProfile>();
            {
                List<UserProfile> users = _userProfileRepository.GetAllUsers();

                foreach (UserProfile user in users)
                    if (user.Deactivated)
                        deactivatedUsers.Add(user);
            }

            return View(deactivatedUsers);


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

        public ActionResult Edit(int id)
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
        public ActionResult Edit(int id, UserProfile userProfile)
        {
            //goes back and gets all the user information so I can access later
            userProfile = _userProfileRepository.GetUserById(id);

            try
            {
                //access the User Type name of the selected user and if they aren't an admin runs deactivation
                if (userProfile.UserType.Name != "Admin")
                {
                    _userProfileRepository.ReactivateUser(id);

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


        public ActionResult UserTypeEdit(int id)
        {



            UpdateUserTypeViewModel vm = new UpdateUserTypeViewModel();

             vm.UserProfile = _userProfileRepository.GetUserById(id);
             vm.UserTypes = _userTypeRepository.GetAll();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserTypeEdit(UpdateUserTypeViewModel vm)
        {
            try
            {
              /*  var userTypeId = Request.Form["UserProfile.UserTypeId"];
                vm.UserProfile.UserTypeId = Int32.Parse(userTypeId);

                UserProfile user = new UserProfile();
                {
                    user.UserTypeId = vm.UserProfile.UserTypeId;
                }*/

                _userProfileRepository.UpdateUserType(vm.UserProfile);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View(vm);
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
