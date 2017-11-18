using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Bageriet.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public string ImageUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Adress { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Profession { get; set; }
        public string Description { get; set; }


    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Ingredient> Ingredients { get; set; }
     
    
        public virtual DbSet<Newsletter> Newsletters { get; set; }
        public virtual DbSet<Subscriber> Subscribers { get; set; }
    

        

        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Like> Likes { get; set; }

        public virtual DbSet<Mail> Mails { get; set; }
        public virtual DbSet<Slider> Sliders { get; set; }
        //public virtual DbSet<Contact> Contacts { get; set; }

        public virtual DbSet<About> About { get; set; }

    }
}