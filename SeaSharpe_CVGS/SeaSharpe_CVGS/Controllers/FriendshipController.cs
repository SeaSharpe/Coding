/*
 * File Name: Friendship Controller.cs
 *  
 * Revision History:
 *      25-Nov-2015: Manuel Lopez. Wrote code
 *      26-Nov-2015: Manuel Lopez. Wrote code, Commented
 */

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SeaSharpe_CVGS.Models;

namespace SeaSharpe_CVGS.Controllers
{
    public class FriendshipController : Controller
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FriendshipController()
        {
        }

        #region Members
        /// <summary>
        /// Search functionality for finding new friends
        /// Includes partial views for Friends and Family Lists
        /// and if a search was done will include a list with the search results
        /// <param name="nameSearch">Name to search. Will search first and last name</param>
        /// <returns>Search/Show Friends view</returns>
        public ActionResult Index(string nameSearch)
        {
            //Friender: Requester
            //Friendee: Is the one receiving the request 
            Member currentMem = CurrentMember;

            var friends = db.Friendships.Where(a => a.FrienderId == currentMem.Id && a.IsFamilyMember == false).ToList();
            var family = db.Friendships.Where(a => a.FrienderId == currentMem.Id && a.IsFamilyMember == true).ToList();

            var allFriends = db.Friendships.Where(a => a.FrienderId == currentMem.Id).ToList();

            var searchList = SearchPerson(nameSearch);
            searchList = RemoveCurrentFriendees(searchList, GiveMeFriendees(allFriends));

            ViewData.Add("friends", SetMutualFriendShips(friends));
            ViewData.Add("family", SetMutualFriendShips(family));
            ViewData.Add("search", searchList);

            //use it to hide or not the Search Results
            ViewBag.found = IsSearchFound(searchList, nameSearch);

            return View(friends);
        }

        /// <summary>
        /// Add a Friend to CurrentMember FriendShip
        /// </summary>
        /// <returns>Index view</returns>
        public ActionResult AddFriend(string userName)
        {
            Friendship newFriendship = new Friendship();
            newFriendship.Friender = CurrentMember;

            var friendee = db.Members.FirstOrDefault(a => a.User.UserName == userName);
            newFriendship.Friendee = friendee;

            db.Friendships.Add(newFriendship);
            db.SaveChanges();

            TempData["message"] = friendee.User.FirstName
                + " " + friendee.User.LastName + " added as a friend";

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Add a Friend to CurrentMember FriendShip as a family
        /// </summary>
        /// <returns>Index view</returns>
        public ActionResult AddFamily(string userName)
            {
            Friendship newFriendship = new Friendship();
            newFriendship.Friender = CurrentMember;

            var friendee = db.Members.FirstOrDefault(a => a.User.UserName == userName);
            newFriendship.Friendee = friendee;
            newFriendship.IsFamilyMember = true;

            db.Friendships.Add(newFriendship);
                db.SaveChanges();

            TempData["message"] = friendee.User.FirstName 
                + " " + friendee.User.LastName + " added as family";

                return RedirectToAction("Index");
            }

        /// <summary>
        /// Display wishlist of selected friend
        /// </summary>
        /// <param name="id">friendee id</param>
        /// <returns>Wishlist view</returns>
        public ActionResult Details(int id)
        {
            var friendeeMember = db.Members.FirstOrDefault(a => a.Id == id);

            ViewBag.FullName = friendeeMember.User.FirstName + " " + friendeeMember.User.LastName;

            var wishListGames = db.WishLists.Where(w => w.MemberId == id).ToList();
            var games = PullGamesWithId(wishListGames);

            return View(games);
        }

       /// <summary>
        /// Post back for delete friendship
       /// </summary>
        /// <param name="id">FrienderId</param>
       /// <returns>Search/Show Friends view</returns>
        public ActionResult Delete(int id)
        {
            Friendship friendship = db.Friendships.FirstOrDefault
                (f => f.FriendeeId == id && f.FrienderId == CurrentMember.Id);

            TempData["message"] = friendship.Friendee.User.FirstName
                + " " + friendship.Friendee.User.LastName + "'s friendship removed";

            db.Friendships.Remove(friendship);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //TODO move to cart
        //id is the game id wanted to move to the cart
        public ActionResult MoveToCart(int id)
        {
            return RedirectToAction("Index");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Will pull a game list with the a list of WishList objects
        /// </summary>
        /// <param name="wishListGames"></param>
        /// <returns></returns>
        List<Game> PullGamesWithId(List<WishList> wishListGames)
        {
            List<Game> res = new List<Game>();
            var allGames = db.Games.ToList();

            for (int i = 0; i < wishListGames.Count; i++)
            {
                if (allGames.Contains(wishListGames[i].Game))
                {
                    res.Add(wishListGames[i].Game);
                }
            }
            return res;
        }

        /// <summary>
        /// Given a list of FriendShips will define a List of MutualFriendShip
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        List<MutualFriendShip> SetMutualFriendShips(List<Friendship> list)
        {
            List<MutualFriendShip> res = new List<MutualFriendShip>();

            for (int i = 0; i < list.Count; i++)
            {
                var newMutualFriendShip = new MutualFriendShip(list[i], db, CurrentMember);
                res.Add(newMutualFriendShip);
            }

            return res.OrderBy(a => a.Friendship.Friendee.User.LastName).
                ThenBy(a => a.Friendship.Friendee.User.FirstName).ToList();
        }

        /// <summary>
        /// Will remove current friends on 'searchList'
        /// </summary>
        /// <param name="searchList">The whole search list found</param>
        /// <param name="currentFriendees">The currentFriendees of the CurrentMember</param>
        /// <returns>A list with searchList minus the currentFriendees</returns>
        private List<Member> RemoveCurrentFriendees(List<Member> searchList, List<Member> currentFriendees)
        {
            if (searchList == null || currentFriendees == null)
            {
                return null;
            }

            for (int i = 0; i < currentFriendees.Count; i++)
            {
                searchList.Remove(currentFriendees[i]);
            }
            return searchList;
        }

        /// <summary>
        /// Will return the list friendees of a Friendship list 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        List<Member> GiveMeFriendees(List<Friendship> list)
        {
            List<Member> res = new List<Member>();
            for (int i = 0; i < list.Count; i++)
            {
                res.Add(list[i].Friendee);
            }
            return res;
        }

        /// <summary>
        /// Will return true if 'list.Count' is greater than zero
        /// </summary>
        /// <param name="list"></param>
        /// <returns>emtpy string if the param 'nameSearch' was not used. "found" if list.Count > 0
        /// other wise "notFound"</returns>
        string IsSearchFound(List<Member> list, string nameSearch)
        {
            //means no search was executed
            if (nameSearch == null || nameSearch.Trim() == "" || list == null)
            {
                return "";
            }

            //at least one item was found
            if (list.Count > 0)
            {
                return "found";
            }

            //not item found 
            return "notFound";
        }

        /// <summary>
        /// Will break 'searchName' into words if are space separated and will passed it to SearchPersonByWord
        /// </summary>
        /// <param name="searchName"></param>
        /// <returns></returns>
        List<Member> SearchPerson(string searchName)
        {
            if (searchName == null || searchName.Trim() == "")
            {
                return null;
            }

            var words = searchName.Split(' ');
            List<Member> res = new List<Member>();

            for (int i = 0; i < words.Length; i++)
            {
                res.AddRange(SearchPersonByWord(words[i].Trim()));
            }
            res = res.Distinct().ToList();

            return res.OrderBy(a => a.User.LastName).
                ThenBy(a => a.User.FirstName).ToList();
        }

        /// <summary>
        /// Will return a list of Members that either have a same or like first/last name
        ///  
        /// </summary>
        /// <param name="searchName">Name to search</param>
        /// <returns></returns>
        List<Member> SearchPersonByWord(string searchName)
        {
            return db.Members.Where(n => n.User.FirstName.Contains(searchName)
                || n.User.LastName.Contains(searchName)).ToList();
        }

        #endregion
    }

    /// <summary>
    /// Class used to hold a friendShip and define if a friendship is mutual
    /// </summary>
    public class MutualFriendShip
    {
        public Friendship Friendship;
        public bool IsMutual;//bool that indicates if a friendship is mutual
        private ApplicationDbContext db;
        private Member currentMember;

        /// <summary>
        /// Given all param will define if has a Mutual FriendShip with param 'currentMember'
        /// </summary>
        /// <param name="friendship"></param>
        /// <param name="db"></param>
        /// <param name="currentMember"></param>
        public MutualFriendShip(Friendship friendship, ApplicationDbContext db, Member currentMember)
        {
            this.currentMember = currentMember;
            this.db = db;
            Friendship = friendship;
            DefineIfMutual();
        }

        /// <summary>
        /// Will Define if a Friendship is mutual 'IsMutual' class field
        /// </summary>
        void DefineIfMutual()
        {
            //will find the friendShip of current Member and other member the other way around
            var friendeeFriends =
                db.Friendships.FirstOrDefault(a => a.FrienderId == Friendship.FriendeeId && a.FriendeeId == currentMember.Id);

            if (friendeeFriends != null)
            {
                IsMutual = true;
            }
        }
    }
}
