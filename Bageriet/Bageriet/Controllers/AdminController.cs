using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Bageriet.Extensions;
using Bageriet.Models;
using Bageriet.ViewModels;
using Microsoft.AspNet.Identity;

namespace Bageriet.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin
        public ActionResult Index()
        {
            DashboardVM vm = new DashboardVM()
            {
                ProductCount = db.Products.Count(),
                MailCount = db.Mails.Where(x => x.IsNew).ToList().Count,
                NewsCount = db.News.Count(),
                UserCount = db.Users.Count()
            };

            return View(vm);
        }

        #region Products

        public ActionResult ProductList()
        {

            List<Category> allCategories = db.Categories.ToList();

            return View(allCategories);
        }



        [HttpGet]
        public ActionResult ProductCreate()
        {
            ProductVM vm = new ProductVM()
            {
               
                Categories = db.Categories.ToList(),
            
            };


            return View(vm);
        }





        [HttpPost]
        public ActionResult ProductCreate(string Title, string Description,
            int CategoryId, string ImageUrl, HttpPostedFileBase file)
        {


            var fileComplete = "";
            if (file != null)
            {

                var fileExtension = Path.GetExtension(file.FileName);
                var fileName = Guid.NewGuid().ToString();
                fileComplete = fileName + fileExtension;
                var path = Path.Combine(Server.MapPath("/Uploads/Products/"), fileComplete);
                file.SaveAs(path);

            }

            Product prod = new Product
            {
                Title = Title,
                Description = Description,

                ImageUrl = file.FileName
            };



            prod.CategoryId = CategoryId;
            prod.ImageUrl = fileComplete;

            db.Products.Add(prod);
            db.SaveChanges();




            return RedirectToAction("ProductList");
        }


        [HttpGet]
        public ActionResult DetailsProduct(int id)
        {
            //////Product prod = db.Products.Find(id);
            IngredientVM vm = new IngredientVM()
            {
                Product = db.Products.Find(id),

                //Ingredienses = db.Ingredients.ToList(),

                //Ingredient = db.Ingredients.Find(id),

            };
            return View(vm);
        }




        [HttpGet]
        public ActionResult
            ProductEdit(int? id) //id´et laves nullable - så kan vi lave en if sætning (tjekke på) om id´et er der.
        {
            ProductVM vm = new ProductVM()
            {
                //Categories = db.Categories.Where(x => x.ParentId != null).ToList(),
                //Ingredienses = db.Ingredienses.ToList(),
                Product = db.Products.Find(id),
                //Brands = db.Brands.ToList()
            };
            return View(vm);
        }


        [HttpPost]
        public ActionResult
            ProductEdit(int id, string Title, string Description, HttpPostedFileBase file)
        {

            Product prod = db.Products.Find(id);

            var fileComplete = "";
            if (file != null && file.ContentLength > 0)
            {
                System.IO.File.Delete(HostingEnvironment.ApplicationPhysicalPath + "/Uploads/Products/" + prod.ImageUrl);
                var fileExtension = Path.GetExtension(file.FileName);
                var fileName = Guid.NewGuid().ToString();
                fileComplete = fileName + fileExtension;
                var path = Path.Combine(Server.MapPath("/Uploads/Products/"), fileComplete);
                file.SaveAs(path);
                prod.ImageUrl = fileComplete;
            }

            prod.Title = Title;
            prod.Description = Description;


            db.SaveChanges();

            return RedirectToAction("ProductList");

        }



        public ActionResult ProductDelete(int id)
        {
            Product prod = db.Products.Find(id);
            System.IO.File.Delete(HostingEnvironment.ApplicationPhysicalPath + "/Uploads/Products/" + prod.ImageUrl);

            db.Products.Remove(prod);
            db.SaveChanges();

            return RedirectToAction("ProductList");
        }


        #endregion



        #region Category

        public ActionResult CategoryList()
        {
            List<Category> allCategories = db.Categories.ToList();

            return View(allCategories);
        }



        [HttpGet]
        public ActionResult CategoryEdit(int? id)
        {
            Category cat = db.Categories.Find(id);
            return View(cat);
        }

        [HttpPost]
        public ActionResult CategoryEdit(int? id, string Title)
        {
            Category cat = db.Categories.Find(id);

            cat.Title = Title;

            db.SaveChanges();
            return RedirectToAction("CategoryList");
        }

        [HttpGet]
        public ActionResult CategoryCreate()
        {
            List<Category> cat = db.Categories.ToList();
            return View(cat);
        }



        [HttpPost]
        public ActionResult CategoryCreate(string Title, int? ParentId)
        {
            Category cat = new Category();

            cat.Title = Title;

            db.Categories.Add(cat);
            db.SaveChanges();
            return RedirectToAction("CategoryList");
        }


        public ActionResult CategoryDelete(int id)
        {
            Category cat = db.Categories.Find(id);

            db.Categories.Remove(cat);
            db.SaveChanges();

            return RedirectToAction("CategoryList");

        }

        #endregion


        #region ingredients





        [HttpPost]
        public ActionResult _AddIngredients(Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {

                db.Ingredients.Add(ingredient);
                db.SaveChanges();

           
            }



            return RedirectToAction("DetailsProduct", "Admin", new { id = ingredient.ProductId });


        }



        public ActionResult IngredientsList()
        {
            List<Ingredient> allIng = db.Ingredients.ToList();
            return View(allIng);
        }


        [HttpGet]
        public ActionResult IngredientsEdit(int? id)
        {
            Ingredient ing = db.Ingredients.Find(id);

            return View(ing);
        }

        [HttpPost]
        public ActionResult IngredientsEdit(int? id, string Name, string Amount)
        {
            Ingredient ing = db.Ingredients.Find(id);


            ing.Name = Name;
            ing.Amount = Amount;

            db.SaveChanges();
            return RedirectToAction("DetailsProduct", "Admin", new { id = ing.ProductId });
        }



        public ActionResult IngredientsDelete(int id)
        {

            Ingredient ing = db.Ingredients.Find(id);
            var ProdId = ing.ProductId;
            db.Ingredients.Remove(ing);
            db.SaveChanges();

            return RedirectToAction("DetailsProduct", "Admin", new { id = ProdId });

        }

        #endregion



        #region Newsletter



        public ActionResult NewsletterList()
        {
            List<Newsletter> allNewsletters = db.Newsletters.ToList();
            return View(allNewsletters);
        }


        [HttpGet]
        public ActionResult NewsletterCreate()
        {

            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult NewsletterCreate(string Title, string HtmlContent)
        {

            Newsletter newsletter = new Newsletter()
            {
                Title = Title,
                HtmlContent = HtmlContent,

            };

            db.Newsletters.Add(newsletter);
            db.SaveChanges();

            return RedirectToAction("NewsletterList");
        }



        public ActionResult NewsletterDelete(int id)
        {
            Newsletter news = db.Newsletters.Find(id);


            db.Newsletters.Remove(news);
            db.SaveChanges();

            return RedirectToAction("NewsletterList");
        }




        [HttpGet]
        [ValidateInput(false)]
        public ActionResult NewsletterSend(int? id)
        {
            NewsletterVM vm = new NewsletterVM()   //en instansiering af newsletter
            {
                Newsletter = db.Newsletters.Find(id),
                Subscribers = db.Subscribers.ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult NewsletterSend(string[] Email, string HtmlContent, string title)
        {

            foreach (var item in Email)
            {
                MailMessage message = new MailMessage();

                message.From = new MailAddress("l.ortved@gmail.com");
                message.To.Add(new MailAddress(item));

                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.UTF8;
                message.Subject = title;
                message.Body = HtmlContent;

                SmtpClient client = new SmtpClient();
                client.Send(message);
            }



            return RedirectToAction("NewsletterList");
        }


        #endregion


        #region News


        public ActionResult NewsList()
        {

            NewsVM vm = new NewsVM()
            {
                NewsList = db.News.ToList(),


            };

            return View(vm);
        }



        public ActionResult CreateNews()
        {

            return View();
        }

        [HttpPost]
        public ActionResult CreateNews(string UserId, string Title, string Content, string ImageUrl, HttpPostedFileBase file)
        {
            var fileComplete = "";
            if (file != null)
            {

                var fileExtension = Path.GetExtension(file.FileName);
                var fileName = Guid.NewGuid().ToString();
                fileComplete = fileName + fileExtension;
                var path = Path.Combine(Server.MapPath("/Uploads/News/"), fileComplete);
                file.SaveAs(path);
            }


            News news = new News()
            {
                Title = Title,
                Content = Content,
                ImageUrl = file.FileName,
                UserId = User.Identity.GetUserId()

            };

            news.ImageUrl = fileComplete;

            db.News.Add(news);
            db.SaveChanges();
            return RedirectToAction("NewsList");

        }



        public ActionResult NewsDetail(int id)
        {
            News news = db.News.Find(id);

            return View(news);
        }



        public ActionResult DeleteNews(int id)
        {
            News news = db.News.Find(id);

            db.Entry(news).State = EntityState.Modified;

            db.News.Remove(news);
            db.SaveChanges();


            return RedirectToAction("NewsList");
        }

        #endregion

        #region Comments

        public ActionResult _CreateComment()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult _CreateComment(Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.UserId = User.Identity.GetUserId();
                db.Comments.Add(comment);db.SaveChanges();
            }
            return RedirectToAction("Newsdetail", "Admin", new { id = comment.NewsId });
        }


        public ActionResult DeleteComment(int id)
        {
            Comment Comment = db.Comments.Find(id);
            var NewsId = Comment.NewsId;
            db.Comments.Remove(Comment);
            db.SaveChanges();


            return RedirectToAction("NewsDetail", "Admin", new { id = NewsId });
        }
      


        #endregion


        #region Mails


        public ActionResult MailList()
        {
            return View();
        }


        public PartialViewResult _MailNav()
        {
            MailVM VM = new MailVM
            {
                InboxCount = db.Mails.Where(x => x.IsActive).ToList().Count,
                TrashCount = db.Mails.Where(x => x.IsActive != true).ToList().Count
            };
            return PartialView(VM);
        }

        public PartialViewResult _MailList(string query, string list)
        {
            if (query == null)
            {
                query = "";
            }
            if (list == null)
            {
                list = "";
            }
            MailVM VM = new MailVM
            {
                InboxList = db.Mails.Where(x => x.IsActive).OrderByDescending(x => x.IsNew).ToList(),
                TrashList = db.Mails.Where(x => x.IsActive != true).OrderByDescending(x => x.IsNew).ToList()
            };
            if (list == "Inbox")
            {
                VM.InboxList = db.Mails.Where(x => x.IsActive && x.Name.Contains(query)).OrderByDescending(x => x.IsNew).ToList();
            }
            else if (list == "Trash")
            {
                VM.TrashList = db.Mails.Where(x => x.IsActive != true && x.Name.Contains(query)).OrderByDescending(x => x.IsNew).ToList();
            }

            return PartialView(VM);
        }


        public PartialViewResult _GetMail(int? id)
        {

            Mail Mail = db.Mails.Find(id);
            if (id != null)
            {
                Mail.IsNew = false;
                db.Entry(Mail).State = EntityState.Modified;
                db.SaveChanges();
            }
            return PartialView(Mail);
        }


        public PartialViewResult _TrashMail(int id)
        {
            Mail Mail = db.Mails.Find(id);
            if (Mail.IsActive)
            {
                Mail.IsActive = false;
            }
            else
            {
                db.Mails.Remove(Mail);
            }
            db.SaveChanges();
            return PartialView("_GetMail");
        }

        #endregion



        #region Content

        #region Slider


        public ActionResult SliderList()
        {
            List<Slider> allSliders = db.Sliders.ToList();
            return View(allSliders);
        }


        [HttpGet]
        public ActionResult EditSlider(int id)
        {
            Slider slider = db.Sliders.Find(id);
            return View(slider);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSlider(int id, string ImageUrl, HttpPostedFileBase file)
        {
            Slider slider = db.Sliders.Find(id);
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    Helper.DeleteFile(slider.ImageUrl, "~/Uploads/Slider/");
                    var fileName = Guid.NewGuid().ToString();
                    Helper.UploadFile(file, fileName, "~/Uploads/Slider/");
                    slider.ImageUrl = fileName + "_" + file.FileName;
                }

                db.Entry(slider).State = EntityState.Modified;
                db.SaveChanges();
                return (RedirectToAction("SliderList"));

            }
            else
            {
                return (RedirectToAction("EditSlider", new { Id = slider.Id }));
            }



        }



        public ActionResult SliderCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SliderCreate(IEnumerable<HttpPostedFileBase> files)
        {


            foreach (var item in files)
            {
                if (item != null && item.ContentLength > 0)
                {
                    var filename = Path.GetFileName(item.FileName);
                    var path = Path.Combine(Server.MapPath("/Uploads/Slider/"), filename);
                    item.SaveAs(path);

                    Slider img = new Slider()
                    {
                        ImageUrl = filename,

                    };

                    db.Sliders.Add(img);
                    db.SaveChanges();

                }
            }
            return RedirectToAction("SliderList");
        }





        public ActionResult DeleteSlider(int id)
        {
            Slider slider = db.Sliders.Find(id);
            System.IO.File.Delete(HostingEnvironment.ApplicationPhysicalPath + "/Uploads/Slider/" + slider.ImageUrl);

            db.Sliders.Remove(slider);
            db.SaveChanges();


            return RedirectToAction("SliderList");
        }


        #endregion


        #region About


        public ActionResult AboutList()
        {

            List<About> about = db.About.ToList();

            return View(about);
        }



        public ActionResult CreateAbout()
        {

            return View();
        }

        [HttpPost]
        public ActionResult CreateAbout(About About, HttpPostedFileBase File)
        {
            if (ModelState.IsValid)
            {
                if (File != null && File.ContentLength > 0)
                {
                    //If About have images

                    //create and save new image
                    //Helper.UploadFile(File, "", "FilePath");
                    //Article.ArticleImage = File.FileName;
                }
                try
                {
                    db.About.Add(About);
                    db.SaveChanges();

                    return RedirectToAction("AboutList");
                }
                catch
                {
                    return RedirectToAction("CreateAbout");
                }
            }


            return RedirectToAction("CreateAbout");

        }

        public ActionResult DetailsAbout(int Id)
        {
            var about = db.About.Find(Id);

            return View(about);
        }

        public ActionResult EditAbout(int Id)
        {
            var about = db.About.Find(Id);
            ;
            return View(about);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAbout(About About)
        {
            if (ModelState.IsValid)
            {
                db.Entry(About).State = EntityState.Modified;
                db.SaveChanges();
                return (RedirectToAction("AboutList"));
            }
            else
            {
                return (RedirectToAction("EditAbout", new { Id = About.Id }));
            }
        }

        public ActionResult DeleteAbout(int id)
        {
            About about = db.About.Find(id);

            db.About.Remove(about);
            db.SaveChanges();


            return RedirectToAction("AboutList");
        }

        #endregion

        #endregion





    }
}