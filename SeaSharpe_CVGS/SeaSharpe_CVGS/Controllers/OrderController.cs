using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SeaSharpe_CVGS.Models;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SeaSharpe_CVGS.Controllers
{
    public class OrderController : Controller
    {
        
        private ApplicationDbContext db = new ApplicationDbContext();
        UserManager<ApplicationUser> userManager;

        /// <summary>
        /// Return Current user
        /// </summary>
        private ApplicationUser CurrentUser
        {
            get
            {
                return userManager.FindById(User.Identity.GetUserId());
            }
        }

        /// <summary>
        /// Returns Current Member
        /// </summary>
        private Member CurrentMember
        {
            get
            {
                return db.Members.FirstOrDefault(m => m.User == CurrentUser);
            }
        }

        /// <summary>
        /// Tells if current user is employee 
        /// </summary>
        private bool IsEmployee
        {
            get
            {
                return db.Employees.Any(u => u.User == CurrentUser);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public OrderController()
        {
            db = new ApplicationDbContext();
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.db));
        }


        #region Multiple Roles
        /// <summary>
        /// checks authorization and redirects to appropriate page
        /// </summary>
        /// <returns>redirect to OrderManagement or OrderHistory methods</returns>
        public ActionResult Index()
        {
            if (Roles.IsUserInRole(@"employee"))
            {
                return RedirectToAction("OrderManagement");
            }
            else if (Roles.IsUserInRole(@"member"))
            {
                //return ViewEvents view
                return RedirectToAction("OrderHistory");
            }
            else
            {
                //send to Account/Login
                return RedirectToAction("LogIn", "Account");
            }
        }
        
        #endregion

        #region Employee Side
        /// <summary>
        /// list all completed orders
        /// </summary>
        /// <returns>Order Management view</returns>
        public ActionResult OrderManagement()
        {
            return View(db.Orders.ToList());
        }

        /// <summary>
        /// list all orders waiting to be processed
        /// </summary>
        /// <returns>Outstanding orders partial view</returns>
        public ActionResult PartialOutstandingOrders()
        {
            return View(db.Orders.ToList());
        }

        /// <summary>
        /// list order items in selected order
        /// </summary>
        /// <returns>selected order partial view</returns>
        public ActionResult PartialSelectedOrder()
        {
            return View();
        }
        /// <summary>
        /// post back order updated to processed
        /// *** No view required ***
        /// </summary>
        /// <param name="order">order object</param>
        /// <returns>Order Management view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update([Bind(Include = "Id,OrderPlacementDate,ShipDate,IsProcessed")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("OrderManagement");
            }
            return View(order);
        }
        #endregion

        #region Member Side
        /// <summary>
        /// list all orders waiting to be processed
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderHistory()
        {
            return View(db.Orders.ToList());
        }

        /// <summary>
        /// Show order items in cart order
        /// </summary>
        /// <param name="id">member id</param>
        /// <returns>Cart view</returns>
        public ActionResult Cart(int? id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //Game game = db.Games.Find(id);

            var game = db.Games.FirstOrDefault();
            Order order = AddSelectedGameToOrder(game);

            //order cant be processed
            //check that member is member of the order 

            if (game == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        Order AddSelectedGameToOrder(Game currentGame)
        {
            var locId = CurrentMember.Id;

            Order order = db.Orders.FirstOrDefault(o => o.Member.Id == locId); //memberId
            OrderItem orderItem = new OrderItem();
            orderItem.Order = order;
            orderItem.Game = currentGame;

            return order;
        }
       



        /// <summary>
        /// Member Side - Order Created when first item is added to cart
        /// **** No view required ****
        /// </summary>
        /// <returns>Cart view</returns>
        public ActionResult Create()
        {
            return RedirectToAction("Cart");
        }

        /// <summary>
        /// Post back for order creation
        /// </summary>
        /// <param name="order">order object</param>
        /// <returns>Cart view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,OrderPlacementDate,ShipDate,IsProcessed")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Cart");
            }

            return View(order);
        }

        /// <summary>
        /// Member side - Add a specific game to cart
        /// ****No view required****
        /// </summary>
        /// <param name="id">game id</param>
        /// <returns>game details view</returns>
        public ActionResult AddToCart(int? id)
        {
            return RedirectToAction("Cart");
        }

        /// <summary>
        /// post back order updated when items added to cart
        /// *** No view required ***
        /// </summary>
        /// <param name="order">order object</param>
        /// <returns>Cart view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCartWithItem([Bind(Include = "Id,OrderPlacementDate,ShipDate,IsProcessed")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Cart");
            }
            return View(order);
        }

        /// <summary>
        /// post back for deletion of cart item
        /// </summary>
        /// <param name="id">orderItem id</param>
        /// <returns>Cart view</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int itemId)
        {
            Order order = db.Orders.Find(itemId);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Cart");
        }

        /// <summary>
        /// post back for deletion of entire cart
        /// </summary>
        /// <param name="id">order id</param>
        /// <returns>Cart view</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCartConfirmed(int orderId)
        {
            Order order = db.Orders.Find(orderId);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Cart");
        }
        
        #endregion
       

        /// <summary>
        /// garbage collection
        /// </summary>
        /// <param name="disposing">garbage</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
