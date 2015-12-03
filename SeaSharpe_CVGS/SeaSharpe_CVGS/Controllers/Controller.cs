using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SeaSharpe_CVGS.Models;

namespace SeaSharpe_CVGS.Controllers
{
    public class Controller : System.Web.Mvc.Controller
    {
        ApplicationDbContext _db;
        ApplicationSignInManager _signInManager;
        ApplicationUserManager _userManager;

        /// <summary>
        /// For backwards compatibility
        /// </summary>
        public ApplicationDbContext db
        {
            get
            {
                return _db ?? (_db = new ApplicationDbContext());
            }

            protected set
            {
                _db = value;
            }
        }

        public ApplicationDbContext DbContext
        {
            get
            {
                return _db ?? (_db = new ApplicationDbContext());
            }

            protected set
            {
                _db = value;
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            protected set
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
            protected set
            {
                _userManager = value;
            }
        }

<<<<<<< HEAD
        public ApplicationUser CurrentUser
        {
            get
            {
                return DbContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
=======
        public ApplicationUser CurrentUser 
        {
            get
            {
                return DbContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name); 
>>>>>>> refs/remotes/origin/master
            }
        }

        public Member CurrentMember
<<<<<<< HEAD
        {
            get
            {
                return DbContext.Members.FirstOrDefault(m => m.User.UserName == User.Identity.Name);
=======
        { 
            get 
            {
                return DbContext.Members.FirstOrDefault(m => m.User.UserName == User.Identity.Name); 
>>>>>>> refs/remotes/origin/master
            }
        }

        public Employee CurrentEmployee
        {
            get
            {
                return DbContext.Employees.FirstOrDefault(e => e.User.UserName == User.Identity.Name);
            }
        }

        public bool IsAuthenticated { get { return User.Identity.IsAuthenticated; } }

        public bool IsEmployee { get { return CurrentEmployee != null; } }

<<<<<<< HEAD
        public bool IsMember { get { return CurrentMember != null; } }
=======
        public bool IsMember { get { return CurrentMember != null;} }
>>>>>>> refs/remotes/origin/master

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_db != null)
                {
                    _db.Dispose();
                    _db = null;
                }

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
    }
}