using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TabloidMVC.Models;
using TabloidMVC.Repositories;

namespace TabloidMVC.Controllers
{
    public class TagController : Controller
    {
       
            private readonly ITagRepository _tagRepo;
           

            public TagController(ITagRepository tagRepository)
            {
                _tagRepo = tagRepository;
                
            }


            // GET: TagController
            //Currently no permission restrictions for admin only use
            //but will need to figure that out on a later ticket
            public ActionResult Index()
        {
            

            var tags = _tagRepo.GetAllTags().OrderBy(t =>t.Name);
           
            return View(tags);
        }

        

        // GET: TagController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TagController/Create
        public ActionResult Create()
        {
            Tag tag = new Tag();
           
            return View(tag);
        }

        // POST: TagController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tag tag)
        {
            try
            {
                _tagRepo.AddTag(tag);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(tag);
            }
        }



        // GET: TagController/Edit/5
        public ActionResult Edit(int id)
        {
            Tag tag = _tagRepo.GetTagById(id); 
            
           
            return View(tag);
        }

        // POST: TagController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Tag tag)
        {
            try
            {
                _tagRepo.UpdateTag(tag, id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(tag);
            }
        }

        // GET: TagController/Delete/5
        public ActionResult Delete(int id)
        {
            Tag tag = _tagRepo.GetTagById(id);

            return View(tag);
        }

        // POST: TagController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Tag tag)
        {
            
            try
            {
                _tagRepo.DeleteTag(id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(tag);
            }
        }
    }
}
