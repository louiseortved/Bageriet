using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Bageriet.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Bageriet.Extensions
{
    public class IdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        static RoleManager<IdentityRole> _staticRoleManager;
        static UserManager<ApplicationUser> _staticUserManager;

        public IdentityService(ApplicationDbContext context)
        {
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            _staticRoleManager = _roleManager;
            _staticUserManager = _userManager;
        }


        //=====Users=====//
        public static List<ApplicationUser> GetUsers()
        {
            var users = _staticUserManager.Users
                        .Include(u => u.Roles)
                        .ToList();
            return users;
        }

        public List<ApplicationUser> GetUsersByOrder(string filter)
        {
            var users = new List<ApplicationUser> { };
            switch (filter)
            {
                default:
                    users = _userManager.Users
                        .Include(u => u.Roles)
                        .OrderBy(u => u.Id)
                        .ToList();
                    break;
                case "DateCreated":
                    users = _userManager.Users
                        .Include(u => u.Roles)
                        .ToList();
                    break;

            }


            return users;
        }

        public List<ApplicationUser> GetUsersByOrderInverted(string filter)
        {
            var users = new List<ApplicationUser> { };
            switch (filter)
            {
                default:
                    users = _userManager.Users
                        .Include(u => u.Roles)
                        .OrderByDescending(u => u.Id)
                        .ToList();
                    break;
                case "DateCreated":
                    users = _userManager.Users
                        .Include(u => u.Roles)
                        .ToList();
                    break;

            }


            return users;
        }

        public ApplicationUser GetUserById(string Id)
        {
            var user = _userManager.Users.Where(u => u.Id == Id)
                        .Include(u => u.Roles)
                        .First();
            return user;
        }

        public ApplicationUser GetUserByEmail(string Email)
        {
            var user = _userManager.Users.Where(u => u.Email == Email)
                        .Include(u => u.Roles)
                        .First();
            return user;
        }

        public bool CreateUser(string username, string email, string password)
        {
            var iresult = _userManager.Create(new ApplicationUser
            { Email = email, UserName = username.Replace(" ", "") }, password);

            return iresult.Succeeded;
        }
        public bool CreateUser(string username, string email, string password, string phonenumber)
        {
            var iresult = _userManager.Create(new ApplicationUser
            { Email = email, UserName = username.Replace(" ", ""), PhoneNumber = phonenumber }, password);

            return iresult.Succeeded;
        }

        public bool DeleteUser(string id)
        {
            var user = _userManager.FindById(id);
            var iresult = _userManager.Delete(user);

            return iresult.Succeeded;
        }

        //=====Roles=====//
        public List<IdentityRole> GetRolesList()
        {
            var Roles = _roleManager.Roles.ToList();
            return Roles;
        }
        public static List<IdentityRole> GetRoles()
        {
            return _staticRoleManager.Roles.ToList();
        }
        public bool CreateRole(string rolename)
        {
            var iresult = _roleManager.Create(new IdentityRole { Name = rolename });
            return iresult.Succeeded;
        }

        public static string GetUserRole(ApplicationUser user)
        {
            var RoleName = GetRoles().Where(r => r.Id == user.Roles.First().RoleId).First().Name.ToString();

            return RoleName;
        }

        public static List<IdentityRole> GetUserRoles(ApplicationUser user)
        {
            var UserRoles = new List<IdentityRole>();
            //var RoleNames = GetRoles().Where(r => r.Users == user.Roles.First().RoleId).ToList();
            foreach (var role in GetRoles())
            {
                if (role.Users.Any(r => r.UserId == user.Id))
                {
                    UserRoles.Add(role);
                }
            }


            return UserRoles;
        }

        public string GetUserRoleString(ApplicationUser user)
        {
            var RoleName = GetRoles().Where(r => r.Id == user.Roles.First().RoleId).First().Name.ToString();

            return RoleName;
        }

        public bool RoleExists(string rolename)
        {
            return _roleManager.RoleExists(rolename);
        }

        public bool AddUserToRole(string userid, string rolename)
        {
            var iresult = _userManager.AddToRole(userid, rolename);
            return iresult.Succeeded;
        }

        public bool RemoveUserFromRole(string userid, string rolename)
        {
            var iresult = _userManager.RemoveFromRole(userid, rolename);
            return iresult.Succeeded;
        }

        public static bool IsUserInRole(string userid, string rolename)
        {
            return _staticUserManager.IsInRole(userid, rolename);
        }

        public bool IsEmailInRole(string Email, string rolename)
        {
            ApplicationUser user = _userManager.FindByEmail(Email);
            return _staticUserManager.IsInRole(user.Id, rolename);
        }

        public List<ApplicationUser> GetUsersInRole(string rolename)
        {
            var usersInRole = new List<ApplicationUser>();
            var allusers = _staticUserManager.Users.ToList();
            foreach (var users in allusers)
            {
                if (_staticUserManager.IsInRole(users.Id, rolename))
                {
                    usersInRole.Add(users);
                }
            }
            return usersInRole;
        }

        public List<ApplicationUser> GetUsersNotInRole(string rolename)
        {
            var usersInRole = new List<ApplicationUser>();
            var allusers = _staticUserManager.Users.ToList();
            foreach (var users in allusers)
            {
                if (!_staticUserManager.IsInRole(users.Id, rolename))
                {
                    usersInRole.Add(users);
                }
            }
            return usersInRole;
        }
    }
}