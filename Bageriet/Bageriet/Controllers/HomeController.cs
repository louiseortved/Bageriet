using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bageriet.Extensions;
using Bageriet.Models;
using Bageriet.ViewModels;
using Microsoft.AspNet.Identity;

namespace Bageriet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IdentityService _identityService;
        public HomeController()
        {
            db = new ApplicationDbContext();
            _identityService = new IdentityService(db);
        }


        public ActionResult Index()
        {
            ContentVM vm = new ContentVM()
            {
                Sliders = db.Sliders.OrderByDescending(x => x.Id).Take(3).OrderBy(x => x.Id).ToList(),
                //Events = db.Events.OrderBy(x => x.DateOfEvent).Take(3).ToList(),
                Abouts = db.About.ToList(),
                News = db.News.OrderByDescending(x => x.Id).Take(3).OrderBy(x => x.Id).ToList(),
                Products = db.Products.OrderByDescending(x => Guid.NewGuid()).Take(8).ToList(),


            };
            return View(vm);

        }



        public ActionResult Login()
        {
            return View();
        }


        public ActionResult Products(int? id)
        {

            List<Product> products = new List<Product>();

            if (id != null)
            {
                products = db.Products.Where(x => x.CategoryId == id).ToList();
            }
            else
            {
                products = db.Products.ToList();
            }

            return View(products);
        }






        public ActionResult Contact()
        {
            About about = db.About.FirstOrDefault();
            return View(about);
        }


        public PartialViewResult _Mail()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void _Mail(string content, string email, string name, string phone)
        {
            if (ModelState.IsValid)
            {
                var mail = new Mail()
                {
                    DateCreated = DateTime.Now,
                    Content = content,
                    Email = email,
                    Name = name,
                    Phone = "36473829",
                
                };


                db.Mails.Add(mail);
                db.SaveChanges();

            }

        }


       




        public ActionResult _AddSubscriber()
        {
            return PartialView();
        }






        public ActionResult ProductDetail(int id)
        {
            Product product = db.Products.Find(id);
            return View(product);
        }


        [HttpPost]
        public ActionResult AddSubscriber(string Email, string Name)
        {
            Subscriber sub = new Subscriber
            {
                Email = Email,
                Name = Name,
            };

            db.Subscribers.Add(sub);
            db.SaveChanges();

            return RedirectToAction("Index");
        }




        public PartialViewResult CategoryList()
        {
            List<Category> categories = db.Categories.ToList();
            return PartialView(categories);
        }


        [HttpPost]
        public ActionResult _CreateComment(Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.UserId = User.Identity.GetUserId();
                db.Comments.Add(comment);
                db.SaveChanges();
            }
            return RedirectToAction("ProductDetail", "Home", new { id = comment.ProductId });
        }

        public PartialViewResult _GetComments(int Id, int? Page)
        {
            int pages;
            if (Page != null)
            {
                pages = (int)Page * 3;
            }
            else
            {
                pages = 0 * 3;
            }
            List<Comment> CommentList = db.Comments.Where(x => x.ProductId == Id).OrderByDescending(x => x.DateCreated).Skip(pages).Take(3).ToList();
            ViewBag.Count = db.Comments.Count(x => x.ProductId == Id);
            ViewBag.Product = Id;
            ViewBag.DateCreated = DateTime.Now;
            return PartialView(CommentList);
        }

        [HttpPost]
        public ActionResult AddLike(Like Like)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser currentUser = _identityService.GetUserById(User.Identity.GetUserId());
                if (db.Likes.Any(x => x.UserId == currentUser.Id && x.ProductId == Like.ProductId))
                {
                    Like like = db.Likes.FirstOrDefault(x => x.ProductId == Like.ProductId && x.UserId == currentUser.Id);
                    db.Likes.Remove(like);
                    db.SaveChanges();

                }
                else
                {
                    Like.UserId = User.Identity.GetUserId();
                    db.Likes.Add(Like);
                    db.SaveChanges();

                }

                return View();
            }
            return View();
        }



        [HttpGet]
        public ActionResult Search(string query)
        {
            ViewBag.query = query;
            var vm = new SearchVM();
            //multiple split words
            //var split = new[] {  };
            if (!string.IsNullOrEmpty(query))
            {
                var array = query.ToLower().Split(new[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);
                //check if query is null
                foreach (var search in array)
                {


                    vm.Ingredients.AddRange(db.Ingredients
                        .Where(s => s.Name.Contains(search))
                        .ToList()); // to list da den ellers retunerer en ienumerable



                    vm.Products.AddRange(db.Products

                        .Where(p => p.Title.Contains(search) || p.Description.Contains(search))
                        .ToList());
                }


                vm.Ingredients = db.Ingredients.Where(x => x.Name.Contains(query)).ToList();
                vm.Products = db.Products.Where(x => x.Title.Contains(query)).ToList();

            }

            return View(vm);
        }


    }
}