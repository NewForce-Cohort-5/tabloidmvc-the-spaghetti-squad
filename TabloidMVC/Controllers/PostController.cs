using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using TabloidMVC.Models;
using TabloidMVC.Models.ViewModels;
using TabloidMVC.Repositories;
using System;

namespace TabloidMVC.Controllers
{
    
    [Authorize]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ICategoryRepository _categoryRepository;

        public PostController(IPostRepository postRepository, ICategoryRepository categoryRepository)
        {
            _postRepository = postRepository;
            _categoryRepository = categoryRepository;
        }
        //this is to view all posts
        public IActionResult Index()
        {
            var posts = _postRepository.GetAllPublishedPosts().OrderByDescending(e => e.PublishDateTime).ToList();
            return View(posts);
        }
       
        //this is to see the list of posts by the logged in user after a user clicks My Posts in the menu
        public IActionResult MyPostsIndex()
        {
            int userId = GetCurrentUserProfileId();

            var postsByUser = _postRepository.GetUserPostById(userId);
            if (postsByUser == null)
            {
                return NotFound();
            }
            return View(postsByUser);
        }


        public IActionResult Details(int id)
        {
            //this is to check to see if there are posts that have been approved
            var post = _postRepository.GetPublishedPostById(id);
            if (post == null)
            {
                //this is to check to see if there are posts that has not been approved

                int userId = GetCurrentUserProfileId();
                post = _postRepository.GetUserPostById(id, userId);
                if (post == null)
                {
                    return NotFound();
                }
            }
            return View(post);
        }
        //This is getting new post form information 
        public IActionResult Create()
        {
            var vm = new PostCreateViewModel();
            vm.CategoryOptions = _categoryRepository.GetAll();
            return View(vm);
        }

        //this is to add the new post from the C# vm to SQL database
        [HttpPost]
        public IActionResult Create(PostCreateViewModel vm)
        {
            try
            {
                //this is setting CreateDateTime to the current date and time
                vm.Post.CreateDateTime = DateAndTime.Now;
                //this is seting the IsApproved to true and stored in the database as BIT value of 1
                vm.Post.IsApproved = true;
                // this is setting UserProfileId to the currentUserId
                vm.Post.UserProfileId = GetCurrentUserProfileId();

                _postRepository.Add(vm.Post);

                return RedirectToAction("Details", new { id = vm.Post.Id });
            } 
            catch
            {
                vm.CategoryOptions = _categoryRepository.GetAll();
                return View(vm);
            }
        }

        //this is to get infor of the post to be removed
        public IActionResult Delete(int id)
        {
            Post post = _postRepository.GetPublishedPostById(id);
                
            int userId = GetCurrentUserProfileId();
             
                if (post == null || post.UserProfileId != userId)
                {
                    return NotFound();
                }
            
            return View(post);
        }

        //this is to remove the selected post from the database. 
        public IActionResult Delete(int id, Post post)
        {
            try
            {
                _postRepository.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(post);
            }
        
 
        }
        private int GetCurrentUserProfileId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}
