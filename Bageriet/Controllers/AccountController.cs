﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Bageriet.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Bageriet.Models;
using Bageriet.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.VisualBasic.ApplicationServices;

namespace Bageriet.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();


        private UserManager<ApplicationUser> Usermanager { get; set; }
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private readonly IdentityService _identityService;

        public AccountController()
        {
            _identityService = new IdentityService(db);
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:

                    if (_identityService.IsEmailInRole(model.Email, "User"))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }





        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                UserManager.AddToRole(user.Id, "User");
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }



        #region Users



        public ActionResult UserList()
        {
            List<ApplicationUser> users = IdentityService.GetUsers();


            return View(users);
        }



        public ActionResult DetailsUser(string Id)
        {

            ApplicationUser user = db.Users.Find(Id);


            return View(user);
        }

        public ActionResult DeleteUser(string id)
        {
            ApplicationUser user = db.Users.Find(id);

            db.Users.Remove(user);
            db.SaveChanges();

            return RedirectToAction("UserList");
        }




        public new ActionResult Profile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ApplicationUser user = _identityService.GetUserById(User.Identity.GetUserId());
               
                return View(user);
            }
        }


       


        [HttpGet]
        public PartialViewResult _EditUser(string id)
        {
           
            ApplicationUser user = db.Users.Find(id);

            return PartialView(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(ApplicationUser User)
        {
            if (ModelState.IsValid)
            {
                db.Entry(User).State = EntityState.Modified;
                db.SaveChanges();
                return (RedirectToAction("Profile", "Account", new {id = User.Id}));
            }
            else
            {
                return (RedirectToAction("Profile"));
            }
        }




        [HttpGet]
        public PartialViewResult _EditProfilePicture(string id)
        {

            ApplicationUser user = db.Users.Find(id);

            return PartialView(user);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfilePicture(string Id, HttpPostedFileBase File)
        {
            ApplicationUser User = _identityService.GetUserById(Id);
            if (ModelState.IsValid)
            {


                if (File != null && File.ContentLength > 0)
                {
                    //If Category have images
                    Helper.DeleteFile(User.ImageUrl, "/Uploads/Users/");
                    int starpos = File.FileName.LastIndexOf("\\") + 1;
                    //create and save new image
                    Helper.UploadFile(File, User.Id, "/Uploads/Users/");
                    User.ImageUrl = User.Id + "_" + File.FileName.Substring(starpos);
                }
                else
                {
                    return RedirectToAction("Profile");
                }
                db.Entry(User).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Profile", "Account", new {id = User.Id});
            }
            else
            {
                return RedirectToAction("Profile");
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditProfilePicture(string Id, HttpPostedFileBase File)
        //{
        //    ApplicationUser user = db.Users.Find(Id);


        //    if (ModelState.IsValid)
        //    {


        //        if (File != null && File.ContentLength > 0)
        //        {
        //            //If Category have images
        //            Helper.DeleteFile(user.ImageUrl, "~/Uploads/Users/");
        //            //create and save new image
        //            Helper.UploadFile(File, user.Id, "~/Uploads/Users/");
        //            user.ImageUrl = user.Id + "_" + File.FileName;
        //        }
        //        else
        //        {
        //            return RedirectToAction("Profile", new { Email = user.Email });
        //        }
        //        db.Entry(User).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Profile", new { Email = user.Email });
        //    }
        //    else
        //    {
        //        return RedirectToAction("Profile", new { Email = user.Email });
        //    }
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditProfile(string Id, HttpPostedFileBase File)
        //{
        //    ApplicationUser User = _identityService.GetUserById(Id);
        //    if (ModelState.IsValid)
        //    {


        //        if (File != null && File.ContentLength > 0)
        //        {
        //            //If Category have images
        //            Helper.DeleteFile(User.ImageUrl, "~/Content/Images/Users/");
        //            //create and save new image
        //            Helper.UploadFile(File, User.Id, "~/Content/Images/Users/");
        //            User.ImageUrl = User.Id + "_" + File.FileName;
        //        }
        //        else
        //        {
        //            return RedirectToAction("Profile", "Account", new { Email = User.Email });
        //        }
        //        db.Entry(User).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Profile", "Home", new { Email = User.Email });
        //    }
        //    else
        //    {
        //        return RedirectToAction("Profile", "Home" ,new { Email = User.Email });
        //    }
        //}




        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditUser(string UserName, string FirstName,string LastName, string Adress, string City, string Country
        //    ,string Profession,string Description,string ImageUrl, HttpPostedFileBase file, string id)
        //{
        //    ApplicationUser user = _identityService.GetUserById(id);

        //    if (ModelState.IsValid)
        //    {

        //        //var user = new ApplicationUser
        //        //{
        //        //    UserName = model.UserName,
        //        //    FirstName = model.FirstName,
        //        //    LastName = model.LastName,
        //        //    Adress = model.Adress,
        //        //    City = model.City,
        //        //    Country = model.Country,
        //        //    PhoneNumber = model.PhoneNumber,
        //        //    Email = model.Email,
        //        //    //ImageUrl = model.ImageUrl,
        //        //    Profession = model.Profession,
        //        //    Description = model.Description

        //        //};


        //        if (file != null && file.ContentLength > 0)
        //        {
        //            //If Category have images
        //            Helper.DeleteFile(user.ImageUrl, "/Uploads/Users/");
        //            //create and save new image
        //            Helper.UploadFile(file, user.Id, "/Uploads/Users/");
        //            user.ImageUrl = user.Id + "_" + file.FileName;
        //        }

        //        //var fileName = Guid.NewGuid().ToString();
        //        //if (file != null && file.ContentLength > 0)
        //        //{

        //        //    Helper.UploadFile(file, fileName, "~/Uploads/Users/");

        //        //    user.ImageUrl = fileName + "_" + file.FileName;
        //        //}


        //        db.Entry(user).State = EntityState.Modified;
        //        //db.Users.Add(user);
        //        db.SaveChanges();
        //        return (RedirectToAction("Profile"));
        //    }
        //    else
        //    {
        //        return (RedirectToAction("Profile", "Account" , new { Email = user.Email }));
        //    }
        //}


        #region Partials
        public PartialViewResult _BlogList(string Id)
        {
            NewsVM vm = new NewsVM()
            {
                //BlogList = db.Blogs.Where(x => x.UserId == Id).ToList(),
                User = db.Users.Find(Id)

            };
            return PartialView(vm);
        }



        #endregion


        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }





        #region UserRoles


        [HttpGet]
        public ActionResult Rolelist()
        {
            var roles = db.Roles.ToList();
            //List<ApplicationUser> AllUsers = db.Users.Include(u => u.Roles).ToList();
            //ViewBag.AllUsers = db.Users.ToList();
            //ViewBag.AllRoles = db.Roles.ToList();
            return View(roles);
        }


        [HttpGet]
        public ActionResult CreateRole()
        {
            return View();
        }


        [HttpPost]
        public ActionResult CreateRole(string roleName)
        {

            try
            {
                db.Roles.Add(new IdentityRole() { Name = roleName });
                db.SaveChanges();
                return RedirectToAction("Rolelist");
            }

            catch (Exception)
            {
                return View();
            }


        }



        public ActionResult DeleteRole(string Id)
        {
            var thisRole = db.Roles.FirstOrDefault(i => i.Id == Id);
            db.Roles.Remove(thisRole);
            db.SaveChanges();
            return RedirectToAction("Rolelist");
        }



        [HttpGet]
        public ActionResult GetRoles()
        {

            var userList = db.Users.OrderBy(n => n.UserName).ToList().Select(e => new SelectListItem()
            {
                Text = e.UserName,
                Value = e.UserName
            });

            ViewBag.Users = userList;
            return View();
        }


        [HttpPost]
        public ActionResult GetRoles(string UserName)
        {

            if (!string.IsNullOrEmpty(UserName))
            {
                ApplicationUser user = db.Users.FirstOrDefault(n => n.UserName.Equals(UserName));

                this.Usermanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.db));
                ViewBag.RolesForThisUser = Usermanager.GetRoles(user.Id);
                var userList = db.Users.OrderBy(n => n.UserName).ToList().Select(e => new SelectListItem()
                {
                    Text = e.UserName,
                    Value = e.UserName
                });

                ViewBag.Users = userList;
            }

            return View();
        }

        [HttpGet]
        public ActionResult RoleAddToUser()
        {
            var userList = db.Users.OrderBy(i => i.UserName).ToList().Select(i => new SelectListItem()
            {
                Text = i.UserName,
                Value = i.Id
            });

            ViewBag.Users = userList;
            var roleList = db.Roles.OrderBy(i => i.Name).ToList().Select(i => new SelectListItem()
            {
                Text = i.Name,
                Value = i.Name
            });

            ViewBag.Roles = roleList;

            return View();
        }


        [HttpPost]
        public ActionResult RoleAddToUsers(string UserId, string RoleName)
        {
            var roleStore = new RoleStore<IdentityRole>(db);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);


            userManager.AddToRole(UserId, RoleName);
            //roleManager.Update();
            return RedirectToAction("GetRoles");
        }



        #endregion



        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}