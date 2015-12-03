using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SeaSharpe_CVGS.Models;
using System.Data.Entity.Validation;

namespace SeaSharpe_CVGS.Controllers
{
    public class ReviewController : Controller
    {
        #region Multiple Roles
        /// <summary>
        /// checks authorization and redirects to appropriate page
        /// </summary>
        /// <returns>redirect to Review Management or ReviewsRating methods</returns>
        public ActionResult Index()
        {
            if (User.IsInRole("Employee"))
            {
                return RedirectToAction("ReviewManagement");
            }
            
            else
            {
                //return SearchGames view
                return RedirectToAction("ReviewsRating");
            }
            
        }   
     
        /// <summary>
        /// List all reviews for a game and show average rating
        /// </summary>
        /// <returns>ReviewsRating view</returns>
        //Available for all users
        public ActionResult ReviewsRating(int id)
        {            
            //Get list of all reviews/ratings for selected game
            IQueryable<Review> gameReviews = db.Reviews.Where(r => r.Game_Id == id && r.IsApproved);
            ViewData["averageRating"] = "No ratings for this game.";
            if(gameReviews.Count() > 0)
            {
                //Calculate average based on all reviews and ratings
                ViewData["averageRating"] = gameReviews.Average(r => r.Rating);
            }
            //TODO: only show approved reviews
            return View(gameReviews.ToList());
        }

        /// <summary>
        /// Display details for selected review on ReviewsRating page
        /// </summary>
        /// <returns>PartialReviewDetails view</returns>
        //Available to all users
        public PartialViewResult PartialReviewDetails(int id)
        {
            Review review = db.Reviews.Find(id);
            return PartialView(review);
        }
        #endregion

        #region Employee Side       
        /// <summary>
        /// List all reviews awaiting review
        /// </summary>
        /// <returns>ReviewManagement view</returns>
        [Authorize(Roles = "Employee")]
        public ActionResult ReviewManagement()
        {
            List<Review> reviewList = db.Reviews.Where(r => r.Aprover == null && r.Subject != null).ToList();
            return View(reviewList);
        }

        /// <summary>
        /// displays the currently selected review
        /// </summary>
        /// <returns>PartialSelectedReview view</returns>
        //Available to all users
        [Authorize(Roles = "Employee")]
        public PartialViewResult PartialSelectedReview(int id)
        {
            Review review = db.Reviews.Find(id);
            return PartialView(review);
        }
        /// <summary>
        /// post back for updating review to Accepted
        /// **** No view required ****
        /// </summary>
        /// <param name="review">Review object</param>
        /// <returns>Review Management view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public ActionResult PartialSelectedReview(int Id, bool IsApproved)
        {
            try
            {
                Review review = db.Reviews.FirstOrDefault(r => r.Id == Id);
                review.IsApproved = IsApproved;
                review.Aprover = CurrentEmployee;
                //Update the model to include binded changes
                ModelState.Clear();
                TryValidateModel(review);
                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
            }

            catch (DbEntityValidationException e)
            {
                TempData["message"] = e.EntityValidationErrors.First().ValidationErrors.First().ToString();
            }
            
            return RedirectToAction("ReviewManagement");
        }        

        /// <summary>
        /// post back - review rejected by employee, delete review
        /// **** no view required ****
        /// </summary>
        /// <param name="id">review id</param>
        /// <returns>Review Management view</returns>
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Review review = db.Reviews.Find(id);
        //    db.Reviews.Remove(review);
        //    db.SaveChanges();
        //    return RedirectToAction("ReviewManagement");
        //}
        // NOTE: I don't think we need this as reviews are never technically 'deleted' just created and updated to various states -Peter T
        #endregion

        #region Member Side      
        /// <summary>
        /// Review/Rate a specific game form
        /// **displayed on game details view***
        /// </summary>
        /// <returns>PartialCreateReview view</returns>
        /// Can be viewed by non-members but if they try to submit they will be required to login
        public PartialViewResult PartialCreateReview(int id)
        {            
            return PartialView();
        }

        /// <summary>
        /// Postback for partialCreateReview, creates a new review or updates existing one if it is just a rating
        /// </summary>
        /// <returns>GameDetails view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Member")]
        public ActionResult PartialCreateReview([Bind(Include = "Id,Rating,Subject,Body,Game_Id")] Review review)
        {
            try
            {
                //Set current game 
                Game reviewGame = db.Games.FirstOrDefault(g => g.Id == review.Game_Id);
                review.Game = reviewGame;

                //Set current member to the author of the review
                review.Author = CurrentMember;
               
                //Update the model to include binded changes
                ModelState.Clear();
                TryValidateModel(review);

                //Check if model is valid
                if(ModelState.IsValid)
                {
                    Review originalReview = db.Reviews.FirstOrDefault(r => r.Id == review.Id);
                    
                    //Add new review if one does not already exist
                    if (originalReview == null)
                    {
                        db.Reviews.Add(review);
                        TempData["message"] = "Review added.";
                    }

                    //Update if review already exists
                    else
                    {
                        //Detach original review from database
                        db.Entry(originalReview).State = EntityState.Detached;

                        //Set approver to null
                        review.Aprover = null;

                        //Update the review
                        db.Entry(review).State = EntityState.Modified;
                        TempData["message"] = "Review modified.";
                    }

                    //Save db changes
                    db.SaveChanges();                    
                }

            }                

            //Return message to member if exception
            catch (Exception e)
            {
                TempData["message"] = "Error creating review: " + e.GetBaseException().Message;
            }

            TempData["review"] = review;
            return RedirectToAction("Details", "Game", new { id = review.Game_Id });
        }

        /// <summary>
        /// post back for review creation
        /// **** review must be validated by employee before appears in Reviews/Rating list ****
        /// </summary>
        /// <param name="review">Review object</param>
        /// <returns></returns>
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Member")]
        //public ActionResult Create([Bind(Include = "Id,Rating,Subject,Body")] Review review)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Reviews.Add(review);
        //        db.SaveChanges();
        //        return RedirectToAction("Game/Details");
        //    }

        //    return View(review);
        //}
        // NOTE: I do not think we need this method/view anymore as it is being handle by PartialCreateReview
        #endregion
        
    }
}
