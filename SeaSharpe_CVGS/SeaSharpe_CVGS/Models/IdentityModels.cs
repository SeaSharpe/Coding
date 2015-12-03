using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace SeaSharpe_CVGS.Models
{
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        [NotMapped]
        public override bool PhoneNumberConfirmed { get; set; }
        [Required] // NOT NULL
        [Column(TypeName = "char"), StringLength(1)] // CHAR(1)
        public string Gender { get; set; }

        [Display(Name = "First Name")]
        [Required, MinLength(1), MaxLength(50)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required, MinLength(1), MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [Display(Name = "Date of Registration")]
        public DateTime DateOfRegistration { get; set; }
    }

    public partial class Member
    {
        public virtual ApplicationUser User { get; set; }
        public int Id { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsEmailMarketingAllowed { get; set; }
        public int StripeID { get; set; }
        public virtual ICollection<Friendship> Friendships { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }

    public partial class Employee
    {
        public virtual ApplicationUser User { get; set; }
        public int Id { get; set; }
    }

    public partial class Address
    {
        public int Id { get; set; }
        public virtual Member Member { get; set; }
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
    }

    public partial class Order
    {
        public int Id { get; set; }
        [Display(Name = "Billing Address")]
        public virtual Address BillingAddress { get; set; }
        [Display(Name = "Shipping Address")]
        public virtual Address ShippingAddress { get; set; }
        public virtual Member Member { get; set; }
        [Display(Name = "Approver")]
        public virtual Employee Aprover { get; set; } //todo: correct to Approver
        [Display(Name = "Order Placement Date")]
        public DateTime? OrderPlacementDate { get; set; }
        [Display(Name = "Ship Date")]
        public DateTime? ShipDate { get; set; }
        [Display(Name = "Processed?")]
        public bool IsProcessed { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }

    public partial class OrderItem
    {
        [Key, Column("Game_Id", Order = 0)]
        public int GameId { get; set; }
        [Key, Column("Order_Id", Order = 1)]
        public int OrderId { get; set; }
        public virtual Game Game { get; set; }
        public virtual Order Order { get; set; }
        [Display(Name = "Sale Price")]
        public decimal SalePrice { get; set; }
    }

    public partial class WishList
    {
        [Key, Column("Member_Id", Order = 0)]
        public int MemberId { get; set; }
        [Key, Column("Game_Id", Order = 1)]
        public int GameId { get; set; }
        public virtual Game Game { get; set; }
        public virtual Member Member { get; set; }
    }

    public partial class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Image Path")]
        public string ImagePath { get; set; }
        [MinLength(0), MaxLength(50)]
        public string Publisher { get; set; }
        [MinLength(0), MaxLength(4)]
        public string ESRB { get; set; }
        [Display(Name = "Release Date")]
        public DateTime ReleaseDate { get; set; }
        [Display(Name = "Price")]
        [DisplayFormat(DataFormatString="{0:c}")]
        public decimal SuggestedRetailPrice { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual Platform Platform { get; set; }
        [ForeignKey("Platform")]
        public int Platform_Id { get; set; }
    }

    public partial class Review
    {
        public int Id { get; set; }
        public virtual Game Game { get; set; }
        public virtual Member Author { get; set; }
        [Display(Name = "Approver")]
        public virtual Employee Aprover { get; set; } // todo: correct spelling to Approver
        public float Rating { get; set; }
        public string Subject { get; set; }
        [ForeignKey("Game")]
        public int Game_Id { get; set; }
        public string Body { get; set; }
        public bool IsApproved { get; set; }
    }

    public partial class Friendship
    {
        [Key, Column("Friendee_Id", Order = 0)]
        [ForeignKey("Friendee")]
        public int FriendeeId { get; set; }
        [Key, Column("Friender_Id", Order = 1)]
        [ForeignKey("Friender")]
        public int FrienderId { get; set; }
        public virtual Member Friendee { get; set; }
        public virtual Member Friender { get; set; }
        [Display(Name = "Family Member?")]
        public bool IsFamilyMember { get; set; }
    }

    public partial class Platform
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Game> Games { get; set; }
    }

    public partial class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Game> Games { get; set; }
    }

    public partial class Event
    {
        public int Id { get; set; }
        public virtual Employee Employee { get; set; }
        public string Location { get; set; }
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public virtual ICollection<Member> Attendies { get; set; }
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.ManyToManyCascadeDeleteConvention>();
            modelBuilder.Entity<Member>().
                HasMany(member => member.Friendships).
                WithRequired(friendship => friendship.Friendee).
                WillCascadeOnDelete(false);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        [Display(Name = "Categories")]
        public DbSet<Category> Catagories { get; set; } // to do: correct spelling to Categories
        [Display(Name = "Wish Lists")]
        public DbSet<WishList> WishLists { get; set; }
        [Display(Name = "Order Items")]
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}