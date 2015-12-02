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
    [Authorize]
    public class UserController : Controller
    {
        /// <summary>
        /// Displays member profile page
        /// ** contains (2) partial address views**
        /// </summary>
        /// <param name="id">member id</param>
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
        /// post back for edit member
        /// </summary>
        /// <param name="member">member object</param>
        /// <returns>redirect to Game/SearchGames</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Prefix = "Member")] Member member, 
            [Bind(Prefix = "BillingAddress")] Address billingAddress, 
            [Bind(Prefix = "ShippingAddress")] Address shippingAddress)
        {
            var sqlLog = new StringBuilder("");
            db.Database.Log = x => sqlLog.Append(x);
            db.Users.Attach(member.User);
            var userEntry = db.Entry<ApplicationUser>(member.User);
            //member.User = null;
            db.Members.Attach(member);
            var memberEntry = db.Entry<Member>(member);

            memberEntry.State = EntityState.Unchanged;


            memberEntry.Property(m => m.IsEmailMarketingAllowed).IsModified = true;
            userEntry.Property(u => u.FirstName).IsModified = true;
            //userEntry.Property(u => u.LastName).IsModified = true;
            //userEntry.Property(u => u.PhoneNumber).IsModified = true;
            //userEntry.Property(u => u.Gender).IsModified = true;
            //userEntry.Property(u => u.PhoneNumber).IsModified = true;
            userEntry.Property(u => u.UserName).IsModified = false;
            if (billingAddress != null)
            {
                billingAddress.Member = member;
                if (billingAddress.Id == 0)
                {
                    db.Addresses.Add(billingAddress);
                }
                else
                {
                    db.Addresses.Attach(billingAddress);
                    db.Entry<Address>(billingAddress).State = EntityState.Modified;
                }
            }
            foreach (var value in ModelState.Values)
            {
                value.Errors.Clear();
            }

            if (true)
            {
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    StringBuilder sb = new StringBuilder("");
                    foreach (var entity in e.EntityValidationErrors)
                    {
                        foreach (var error in entity.ValidationErrors)
                        {
                            sb.Append(string.Format("{0} -> {1}\n", error.PropertyName, error.ErrorMessage));
                        }
                    }
                    TempData["Message"] = sb.ToString();
                }
            }
            return View(new ProfileViewModel { Member = member, BillingAddress = billingAddress, ShippingAddress = shippingAddress });
        }

        void UpdateMember(Member updateFrom, Member updateTo)
        {
            updateTo.User.FirstName = updateFrom.User.FirstName;
            updateTo.User.LastName = updateFrom.User.LastName;
            updateTo.User.Email = updateFrom.User.Email;
            updateTo.User.Gender = updateFrom.User.Gender;
            updateTo.IsEmailMarketingAllowed = updateFrom.IsEmailMarketingAllowed;
        }

        void UpdateAddress(Address updateFrom, Address updateTo)
        {
            updateTo.PostalCode = updateFrom.PostalCode;
            updateTo.Region = updateFrom.PostalCode;
            updateTo.StreetAddress= updateFrom.PostalCode;
        }
    }
}
