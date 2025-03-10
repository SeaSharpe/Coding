﻿/*
 * File Name: UserController.cs
 * 
 * Handles user profiles
 */

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
using System.Text;

namespace SeaSharpe_CVGS.Controllers
{
    /// <summary>
    /// This controller handles user profile management, generic user managment is handled 
    /// in <see cref="SeaSharpe_CVGS.Controllers.AccountController"/>.
    /// </summary>
    [Authorize]
    public class UserController : Controller
    {
        /// <summary>
        /// Displays an edit page for updating a profile. Employees can specify a member 
        /// id to modify a member's profile, if left out the currently logged in member id
        /// will be used.
        /// </summary>
        /// <param name="id">member id (Optional)</param>
        /// <returns>Edit view</returns>
        public ActionResult Edit(int? id)
        {
            ProfileViewModel model = new ProfileViewModel
            {
                Member = db.Members.FirstOrDefault(m => m.Id == id || m.User.UserName == User.Identity.Name)
            };

            if (model.Member == null)
            {
                return HttpNotFound();
            }

            var memberAddresses = db.
                Addresses.
                Where(a => a.Member.Id == model.Member.Id).
                OrderBy(a => a.Id);

            // Billing address is the first address Shipping adddress is the second address or a new adress
            model.BillingAddress = memberAddresses.FirstOrDefault() 
                ?? new Address { Member = model.Member };
            model.ShippingAddress = memberAddresses.Skip(1).FirstOrDefault() 
                ?? new Address { Member = model.Member };

            if ( User.IsInRole("Employee") || model.Member.User.UserName == User.Identity.Name )
            {
                return View(model);
            }

            throw new UnauthorizedAccessException("You may not access this profile.");
        }

        /// <summary>
        /// Post back method for the profile page, will update the user, member, and adress 
        /// objects associated with the currently logged in user. If "deleteAddress" is set, 
        /// this action will only delete the address and return the view without making other
        /// changes.
        /// </summary>
        /// <param name="member">The member object</param>
        /// <param name="billingAddress">The member's billing address</param>
        /// <param name="shippingAddress">The member's shipping address</param>
        /// <param name="deleteAddress">A string that contains Billing or Shipping</param>
        /// <returns>Returns to index if successful, otherwise redisplays the 
        /// edit page</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Prefix = "Member")] Member member, 
            [Bind(Prefix = "BillingAddress")] Address billingAddress, 
            [Bind(Prefix = "ShippingAddress")] Address shippingAddress,
            String deleteAddress)
        {
            ProfileViewModel model = new ProfileViewModel { Member = member, BillingAddress = billingAddress, ShippingAddress = shippingAddress };

            if (deleteAddress != null)
            {
                Address deletingAddress = null;
                string addressType = "";
                if (deleteAddress.Contains("Billing"))
                {
                    deletingAddress = billingAddress;
                    addressType = "billing";
                    model.BillingAddress = new Address();
                }
                else if (deleteAddress.Contains("Shipping"))
                {
                    deletingAddress = shippingAddress;
                    addressType = "shipping";
                    model.ShippingAddress = new Address();
                }

                if (deleteAddress != null && deletingAddress.Id != 0)
                {
                    try
                    {
                        db.Entry(deletingAddress).State = EntityState.Deleted;
                        TempData["message"] = string.Format("Deleted {0} address \"{1}\".", addressType, deletingAddress.StreetAddress);
                        db.SaveChanges();
                        ModelState.Clear();
                        return View(model);
                    }
                    catch (Exception e)
                    {
                        TempData["message"] = string.Format("Could not delete {0} address \"{1}\".", addressType, deletingAddress.StreetAddress);
                        return View(model);
                    }
                }
                else
                {
                    TempData["message"] = string.Format("Could not delete {0} address.", addressType);
                    return View(model);
                }
            }

            StringBuilder messageAccumulator = new StringBuilder("");
            bool failedToSaveSomething = false;

            foreach (var address in new Address[] { billingAddress, shippingAddress })
            {
                if (address != null)
                {
                    address.MemberId = member.Id;
                    if (address.Id == 0)
                    {   // Add new address when Id = 0
                        if (!String.IsNullOrWhiteSpace(address.StreetAddress) ||
                            !String.IsNullOrWhiteSpace(address.Region) ||
                            !String.IsNullOrWhiteSpace(address.City) ||
                            !String.IsNullOrWhiteSpace(address.Country) ||
                            !String.IsNullOrWhiteSpace(address.PostalCode))
                        {   // If any of the address fields are not null, try adding it
                            db.Addresses.Add(address);
                        }
                        else if (address == shippingAddress)
                        {   // If the user didn't enter any fields, ignore errors. 
                            RemoveErrors("Shipping");
                        }
                        else if (address == billingAddress)
                        {
                            RemoveErrors("Billing");
                        }
                    }
                    else
                    {   // Update existing address
                        db.Addresses.Attach(address);
                        db.Entry<Address>(address).State = EntityState.Modified;
                    }

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception)
                        {
                            messageAccumulator.Append("Failed to save address.\n");
                            failedToSaveSomething = true;
                        }
                    }
                }
            }

            RemoveErrors(".Member");
            member.UserId = member.User.Id;
            db.Members.Attach(member);
            db.Entry<Member>(member).State = EntityState.Modified;

            db.Users.Attach(member.User);
            db.Entry<ApplicationUser>(member.User).State = EntityState.Unchanged;
            db.Entry<ApplicationUser>(member.User).Property(user => user.FirstName).IsModified = true;
            db.Entry<ApplicationUser>(member.User).Property(user => user.LastName).IsModified = true;
            db.Entry<ApplicationUser>(member.User).Property(user => user.Email).IsModified = true;
            db.Entry<ApplicationUser>(member.User).Property(user => user.PhoneNumber).IsModified = true;
            db.Entry<ApplicationUser>(member.User).Property(user => user.Gender).IsModified = true;
            db.Entry<ApplicationUser>(member.User).Property(user => user.DateOfBirth).IsModified = true;

            if (ModelState.IsValid)
            {
                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    failedToSaveSomething = true;
                    messageAccumulator.Append("Failed to save member\n");
                }
            }
            else
            {   // If we fail to save, record it and detach member/user so that
                // address information can still be saved.
                failedToSaveSomething = true;
                messageAccumulator.Append("Failed to save member\n");
                db.Entry<ApplicationUser>(member.User).State = EntityState.Unchanged;
                db.Entry<Member>(member).State = EntityState.Unchanged;
            }
            
            if (failedToSaveSomething)
            {
                TempData["message"] = messageAccumulator.ToString();
                return View(model);
            }

            TempData["message"] = "Saved profile.";
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Remove all model state errors that match a pattern
        /// </summary>
        /// <param name="pattern">The pattern to compare against each error key</param>
        void RemoveErrors(string pattern)
        {
            var falseErrors = new List<string>();

            foreach (string error in ModelState.Keys)
            {
                if (error.Contains(pattern)) falseErrors.Add(error);
            }

            foreach (var error in falseErrors)
                ModelState.Remove(error);
        }
    }
}
