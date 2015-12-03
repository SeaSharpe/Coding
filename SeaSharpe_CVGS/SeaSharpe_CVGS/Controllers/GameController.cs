<<<<<<< HEAD
﻿using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SeaSharpe_CVGS.Models;
using System.Web.Security;
using System.Collections.Generic;
using System;

namespace SeaSharpe_CVGS.Controllers
{
    public class GameController : Controller
    {
        //Dictionary containing ESRB ratings
        public static Dictionary<string, string> esrbDict = new Dictionary<string, string>
            {
                //TODO: Change key to local image path 
                {"EC", "Early Childhood"},{"E", "Everyone"},{"E10", "Everyone 10+"},{"T", "Teen"},{"M", "Mature"},{"AO", "Adult Only"}
            };
        

        #region Multiple Roles

        /// <summary>
        /// Employee side - list all games
        /// Member side - search feature, list matching games
        /// </summary>
        /// <returns>list of games view</returns>
        public ActionResult Index()
        {
            //User is employee, go to game management page
            if (IsEmployee)
            {
                return RedirectToAction("GameManagement");
            }


            //User is visitor or member, redirect to SearchGames
            else
            {
               return RedirectToAction("SearchGames");
            }        
        }

        /// <summary>
        /// Displays game list page for members/visitors
        /// </summary>
        /// <returns>List of games view</returns>
        public ActionResult SearchGames(string nameSearch, int[] platformSearch, int[] categorySearch, string[] esrbSearch, bool isInclusive = false)
        {           
            IEnumerable<Game> gameList = db.Games.Include(g => g.Platform).Include(g => g.Categories);
            //Name search query
            if(nameSearch != null)
            {
                gameList = gameList.Where(g => g.Name.ToLower().Contains(nameSearch.ToLower()));
            }

            //Platform search query
            if(platformSearch != null)
            {
                gameList = gameList.Where(g => platformSearch.Contains(g.Platform.Id));
            }

            //Category search query
            if (categorySearch != null)
            {
                ICollection<Category> selectedCategories = db.Catagories.Where(c => categorySearch.Contains(c.Id)).ToList();
                //Only returns rows if game has all selected categories 
                if(isInclusive)
                {
                    gameList = gameList.Where(g => g.Categories.Intersect(selectedCategories).Count() == selectedCategories.Count);
                }

                //returns row if game has any of the selected categories
                else
                {
                    gameList = gameList = gameList.Where(g => g.Categories.Intersect(selectedCategories).Any());
                }
                
            }

            //Esrb search query
            if (esrbSearch != null)
            {                
                gameList = gameList.Where(g => esrbSearch.Contains(g.ESRB));
            }
            
            PopulateDropdownData();
            return View(gameList.ToList());
        }

        /// <summary>
        /// Get Single Game
        /// **view shared with PartialCreateReview***
        /// </summary>
        /// <param name="id">game id</param>
        /// <returns>game details view</returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Game game = db.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }

            //Get tempData message from postback
            TempData["message"] = TempData["message"];

            //Check for review in tempdata from postback 
            Review gameReview = (Review)TempData["review"];

            //Get gameReview if it was not stored in tempdata from postback
            if(gameReview == null)
            {
                if(IsMember)
                {
                    //Get current review for this member/game combination
                    gameReview = db.Reviews.FirstOrDefault(r => r.Author.Id == CurrentMember.Id && r.Game_Id == id);
                }                

                //No review for this user, display blank form
                if (gameReview == null)
                {
                    gameReview = new Review();
                }

                //Set gameReview game id to current game
                gameReview.Game_Id = game.Id;  
            }

            //Push game review to view so it can be passed to the partial view for review
            ViewData["isApproved"] = gameReview.IsApproved;
            ViewData["review"] = gameReview;   
                   
            return View(game);
        }        

        #endregion

        #region Employee Side
=======
﻿using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SeaSharpe_CVGS.Models;
using System.Web.Security;
using System.Collections.Generic;
using System;

namespace SeaSharpe_CVGS.Controllers
{
    public class GameController : Controller
    {
        //Dictionary containing ESRB ratings
        public static Dictionary<string, string> esrbDict = new Dictionary<string, string>
            {
                {"EC", "Early Childhood"},{"E", "Everyone"},{"E10", "Everyone 10+"},{"T", "Teen"},{"M", "Mature"},{"AO", "Adult Only"}
            };
        

        #region Multiple Roles

        /// <summary>
        /// Employee side - list all games
        /// Member side - search feature, list matching games
        /// </summary>
        /// <returns>list of games view</returns>
        public ActionResult Index()
        {
            // to be uncommented when roles are made
            //User is employee, redirect to GameManagement

            //if (Roles.IsUserInRole(@"employee"))
            //{
            //    return RedirectToAction("GameManagement");
            //}

            if (Roles.IsUserInRole(@"employee"))
            {
            return RedirectToAction("GameManagement");
            }


            //User is visitor or member, redirect to SearchGames
            //else
            //{
               return RedirectToAction("SearchGames");
            //}        
        }

        /// <summary>
        /// Displays game list page for members/visitors
        /// </summary>
        /// <returns>List of games view</returns>
        public ActionResult SearchGames(string nameSearch, int[] platformSearch, int[] categorySearch, string[] esrbSearch, bool isInclusive = false)
        {           
            IEnumerable<Game> gameList = db.Games.Include(g => g.Platform).Include(g => g.Categories);
            //Name search query
            if(nameSearch != null)
            {
                gameList = gameList.Where(g => g.Name.Contains(nameSearch));
            }

            //Platform search query
            if(platformSearch != null)
            {
                gameList = gameList.Where(g => platformSearch.Contains(g.Platform.Id));
            }

            //Category search query
            if (categorySearch != null)
            {
                ICollection<Category> selectedCategories = db.Catagories.Where(c => categorySearch.Contains(c.Id)).ToList();
                //Only returns rows if game has all selected categories 
                if(isInclusive)
                {
                    gameList = gameList.Where(g => g.Categories.Intersect(selectedCategories).Count() == selectedCategories.Count);
                }

                //returns row if game has any of the selected categories
                else
                {
                    gameList = gameList = gameList.Where(g => g.Categories.Intersect(selectedCategories).Any());
                }
                
            }

            //Esrb search query
            if (esrbSearch != null)
            {                
                gameList = gameList.Where(g => esrbSearch.Contains(g.ESRB));
            }
            
            PopulateDropdownData();
            return View(gameList.ToList());
        }

        /// <summary>
        /// Get Single Game
        /// **view shared with PartialCreateReview***
        /// </summary>
        /// <param name="id">game id</param>
        /// <returns>game details view</returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }

            //Get tempData message from postback
            TempData["message"] = TempData["message"];

            //Check for review in tempdata from postback 
            Review gameReview = (Review)TempData["review"];

            //Get gameReview if it was not stored in tempdata from postback
            if(gameReview == null)
            {
                //Temporary pull a review which mimics existing review for this user
                //TODO: replace this with review from current user if it exists
                gameReview = db.Reviews.FirstOrDefault();

                //No review for this user, display blank form
                if (gameReview == null)
                {
                    //WORK IN PROGRESS: CHECK FOR EXISTING REVIEW FOR THIS USER
                    gameReview = new Review();
                }               
            }

            gameReview.Game_Id = game.Id;            

            //Push game review to view so it can be passed to the partial view for review
            ViewData["review"] = gameReview;          
            return View(game);
        }
>>>>>>> refs/remotes/origin/master

        /// <summary>
        /// Displays game list page for employees
        /// </summary>
        /// <returns>List of games view</returns>
<<<<<<< HEAD
        [Authorize(Roles = "Employee")]
=======
>>>>>>> refs/remotes/origin/master
        public ActionResult GameManagement()
        {
            IEnumerable<Game> gameList = db.Games.Include(g => g.Platform).Include(g => g.Categories);
            return View(gameList.ToList());
<<<<<<< HEAD
        }

        /// <summary>
        /// Employee Side - Add a game
        /// </summary>
        /// <returns>return add/edit game view</returns>
        [Authorize(Roles = "Employee")]
        public ActionResult Create()
        {
            PopulateDropdownData();
            return View();
        }

        /// <summary>
        /// Employee Side - post back for game creation, attempt to save to db
        /// </summary>
        /// <param name="game">game object</param>
        /// <returns>view of games' list</returns>
        //ADD EMPLOYY AUTH
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public ActionResult Create([Bind(Include = "Id,Name,ReleaseDate,SuggestedRetailPrice, ImagePath, Publisher, ESRB")] Game game, int? Platform, int[] Categories)
        {            
            try
            {
                //Add game platform if value not null
                if(Platform != null)
                {
                    Platform gamePlatform = db.Platforms.Find(Platform);
                    game.Platform = gamePlatform;
                }
                
                //Add game categories if value not null
                if(Categories != null)
                {
                    ICollection<Category> gameCategories = (ICollection<Category>)db.Catagories.Where(c => Categories.Contains(c.Id)).ToList();
                    game.Categories = gameCategories;
                }     

                //Update the model state to reflect manual addition of platforms and categories
                ModelState.Clear();
                TryValidateModel(game);

                if (ModelState.IsValid)
                {
                    db.Games.Add(game);
                    db.SaveChanges();
                    return RedirectToAction("GameManagement");
                }
            }

            //Return message to employee if exception
            catch(Exception e)
            {
                TempData["message"] = "Error creating game: " + e.GetBaseException().Message;                
            }

            Create();
            return View(game);
        }

        /// <summary>
        /// Employee side - edit game
        /// </summary>
        /// <param name="id">game id</param>
        /// <returns>add/edit game view</returns>
        [Authorize(Roles = "Employee")]
        public ActionResult Edit(int? id)
        {
            //Attempt to access edit page without game id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Game game = db.Games.Find(id);

            //Game with current id does not exist
            if (game == null)
            {
                return HttpNotFound();
            }

            //Selected values for game categories on edit view
            ViewData["Categories"] = game.Categories.Select(c => c.Id);

            //Select value for game platform on edit view
            ViewData["Platform"] = game.Platform.Id;

            PopulateDropdownData();
            return View(game);
        }

        /// <summary>
        /// Employee side - post back for edit game, save to db
        /// </summary>
        /// <param name="game">game object</param>
        /// <returns>list of games view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public ActionResult Edit([Bind(Include = "Id,Name,ReleaseDate,SuggestedRetailPrice, ImagePath, Publisher, ESRB, Categories, Platform_id")] Game game, int[] Categories)
        {
            try
            {
                //Add platform object for model state
                game.Platform = db.Platforms.Find(game.Platform_Id);

                //Update game categories
                if(Categories == null) Categories = new int[] {};
                ICollection<Category> gameCategories = (ICollection<Category>)db.Catagories.Where(c => Categories.Contains(c.Id)).ToList();
                Game originalGame = db.Games.Find(game.Id);
                originalGame.Categories.Clear();
                foreach (Category c in gameCategories)
                {
                    originalGame.Categories.Add(c);
                }
                db.SaveChanges();
                db.Entry(originalGame).State = EntityState.Detached;

                //Update the model to include binded changes
                ModelState.Clear();
                TryValidateModel(game);
                if (ModelState.IsValid)
                {
                    db.Entry(game).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["message"] = "Game with ID: " + game.Id + " updated.";
                    return RedirectToAction("GameManagement");
                }
            }

            catch (Exception e)
            {
                TempData["message"] = e.GetBaseException().Message;
            }            

            Edit(game.Id);
            return View(game);
        }

        /// <summary>
        /// Employee -side post back for delete game.  **no delete view, delete button on list of games view***
        /// ****No view required****
        /// </summary>
        /// <param name="id">game id</param>
        /// <returns>list of games view</returns>
        [Authorize(Roles = "Employee")]
        public ActionResult Delete(int id)
        {
            Game game = db.Games.Find(id);
            try
            {
                //Remove the gameCategories associated with the game being deleted
                game.Categories.Clear();

                //Remove game and save changes
                db.Games.Remove(game);
                db.SaveChanges();
                TempData["message"] = game.Name + " and it's dependencies have been deleted.";
            }

            catch (Exception e)
            {
                TempData["message"] = "Error deleting game: " + e.GetBaseException().Message;
            }
            
            return RedirectToAction("GameManagement");
        }
        
        /// <summary>
        /// Member side - Add a specific game to wish list
        /// ****No view required****
        /// </summary>
        /// <param name="id">game id</param>
        /// <returns>game details view</returns>
        [Authorize(Roles = "Member")]
        public ActionResult AddToWishList(int? id)
        {
            return RedirectToAction("Details");
        }
        /// <summary>
        /// Member side - Download a specific game
        /// ****No view required****
        /// </summary>
        /// <param name="id">game id</param>
        /// <returns>game details view</returns>
        [Authorize(Roles = "Member")]
        public ActionResult Download(int? id)
        {
            return RedirectToAction("Details");
        }
        
        #endregion

        #region Helper Methods
        /// <summary>
        /// Helper method to populate dropdown data for various game views
        /// </summary>
        private void PopulateDropdownData()
        {
            //Send platform selectlist to view for dropdown
            ViewData["platformList"] = new SelectList(db.Platforms, "Id", "Name");

            //Send category selectlist to view for listbox
            ViewData["categoryList"] = new SelectList(db.Catagories, "Id", "Name");

            //Send esrb selectlist to view for dropdown
            ViewData["esrbList"] = new SelectList(esrbDict,"Key", "Value");
        }
        #endregion

    }
}
=======
        }

        #endregion

        #region Employee Side

        /// <summary>
        /// Employee Side - Add a game
        /// </summary>
        /// <returns>return add/edit game view</returns>
       
        public ActionResult Create()
        {
            PopulateDropdownData();
            return View();
        }

        /// <summary>
        /// Employee Side - post back for game creation, attempt to save to db
        /// </summary>
        /// <param name="game">game object</param>
        /// <returns>view of games' list</returns>
        //ADD EMPLOYY AUTH
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,ReleaseDate,SuggestedRetailPrice, ImagePath, Publisher, ESRB")] Game game, int? Platform, int[] Categories)
        {            
            try
            {
                //Add game platform if value not null
                if(Platform != null)
                {
                    Platform gamePlatform = db.Platforms.Find(Platform);
                    game.Platform = gamePlatform;
                }
                
                //Add game categories if value not null
                if(Categories != null)
        {
                    ICollection<Category> gameCategories = (ICollection<Category>)db.Catagories.Where(c => Categories.Contains(c.Id)).ToList();
                    game.Categories = gameCategories;
                }     

                //Update the model state to reflect manual addition of platforms and categories
                ModelState.Clear();
                TryValidateModel(game);

            if (ModelState.IsValid)
            {
                db.Games.Add(game);
                db.SaveChanges();
                return RedirectToAction("GameManagement");
            }
            }

            //Return message to employee if exception
            catch(Exception e)
            {
                TempData["message"] = "Error creating game: " + e.GetBaseException().Message;                
            }

            Create();
            return View(game);
        }

        /// <summary>
        /// Employee side - edit game
        /// </summary>
        /// <param name="id">game id</param>
        /// <returns>add/edit game view</returns>
        public ActionResult Edit(int? id)
        {
            //Attempt to access edit page without game id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Game game = db.Games.Find(id);

            //Game with current id does not exist
            if (game == null)
            {
                return HttpNotFound();
            }

            //Selected values for game categories on edit view
            ViewData["Categories"] = game.Categories.Select(c => c.Id);

            //Select value for game platform on edit view
            ViewData["Platform"] = game.Platform.Id;

            PopulateDropdownData();
            return View(game);
        }

        /// <summary>
        /// Employee side - post back for edit game, save to db
        /// </summary>
        /// <param name="game">game object</param>
        /// <returns>list of games view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ReleaseDate,SuggestedRetailPrice, ImagePath, Publisher, ESRB, Categories, Platform_id")] Game game, int[] Categories)
        {
            try
            {
                //Add platform object for model state
                game.Platform = db.Platforms.Find(game.Platform_Id);

                //Update game categories
                if(Categories == null) Categories = new int[] {};
                ICollection<Category> gameCategories = (ICollection<Category>)db.Catagories.Where(c => Categories.Contains(c.Id)).ToList();
                Game originalGame = db.Games.Find(game.Id);
                originalGame.Categories.Clear();
                foreach (Category c in gameCategories)
        {
                    originalGame.Categories.Add(c);
                }
                db.SaveChanges();
                db.Entry(originalGame).State = EntityState.Detached;

                //Update the model to include binded changes
                ModelState.Clear();
                TryValidateModel(game);
            if (ModelState.IsValid)
            {
                db.Entry(game).State = EntityState.Modified;
                db.SaveChanges();
                    TempData["message"] = "Game with ID: " + game.Id + " updated.";
                return RedirectToAction("GameManagement");
            }
            }

            catch (Exception e)
            {
                TempData["message"] = e.GetBaseException().Message;
            }            

            Edit(game.Id);
            return View(game);
        }

        /// <summary>
        /// Employee -side post back for delete game.  **no delete view, delete button on list of games view***
        /// ****No view required****
        /// </summary>
        /// <param name="id">game id</param>
        /// <returns>list of games view</returns>
        public ActionResult Delete(int id)
        {
            Game game = db.Games.Find(id);
            try
            {
                //Remove the gameCategories associated with the game being deleted
                game.Categories.Clear();

                //Remove game and save changes
            db.Games.Remove(game);
            db.SaveChanges();
                TempData["message"] = game.Name + " and it's dependencies have been deleted.";
            }

            catch (Exception e)
            {
                TempData["message"] = "Error deleting game: " + e.GetBaseException().Message;
            }
            
            return RedirectToAction("GameManagement");
        }
        
        /// <summary>
        /// Member side - Add a specific game to wish list
        /// ****No view required****
        /// </summary>
        /// <param name="id">game id</param>
        /// <returns>game details view</returns>
        public ActionResult AddToWishList(int? id)
        {
            return RedirectToAction("Details");
        }
        /// <summary>
        /// Member side - Download a specific game
        /// ****No view required****
        /// </summary>
        /// <param name="id">game id</param>
        /// <returns>game details view</returns>
        public ActionResult Download(int? id)
        {
            return RedirectToAction("Details");
        }
        
        #endregion

        #region Helper Methods
        /// <summary>
        /// Helper method to populate dropdown data for various game views
        /// </summary>
        private void PopulateDropdownData()
        {
            //Send platform selectlist to view for dropdown
            ViewData["platformList"] = new SelectList(db.Platforms, "Id", "Name");

            //Send category selectlist to view for listbox
            ViewData["categoryList"] = new SelectList(db.Catagories, "Id", "Name");

            //Send esrb selectlist to view for dropdown
            ViewData["esrbList"] = new SelectList(esrbDict,"Key", "Value");
        }
        #endregion

    }
}
>>>>>>> refs/remotes/origin/master
