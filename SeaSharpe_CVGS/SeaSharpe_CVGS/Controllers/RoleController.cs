using SeaSharpe_CVGS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace SeaSharpe_CVGS.Controllers
{
    public class RoleController : RoleProvider
    {
        private ApplicationDbContext _dbContext;

        public ApplicationDbContext DbContext
        {
            get
            {
                return _dbContext ?? new ApplicationDbContext();
            }

            protected set
            {
                _dbContext = value;
            }
        }

        enum Roles
        {
            None, Member, User, Employee
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                return typeof(RoleController).FullName;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            return Enum.GetNames(typeof(Roles));
        }

        public override string[] GetRolesForUser(string username)
        {
            var roles = new System.Collections.Generic.List<string>();
            if (DbContext.Members.Any(m => m.User.UserName == username)) 
                roles.Add(Enum.GetName(typeof(Roles), Roles.Member));
            if (DbContext.Employees.Any(e => e.User.UserName == username)) 
                roles.Add(Enum.GetName(typeof(Roles), Roles.Employee));
            if (DbContext.Users.Any(u => u.UserName == username)) 
                roles.Add(Enum.GetName(typeof(Roles), Roles.User));
            return roles.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            Roles role = Roles.None;
            Enum.TryParse<Roles>(roleName, out role);
            switch (role)
            {
                case Roles.Member:
                    return DbContext.Members.Select(m => m.User.UserName).ToArray();
                case Roles.User:
                    return DbContext.Users.Select(u => u.UserName).ToArray();
                case Roles.Employee:
                    return DbContext.Employees.Select(e => e.User.UserName).ToArray();
                default:
                    return new string[0];
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            return GetUsersInRole(roleName).Contains(username);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            foreach (string role in Enum.GetNames(typeof(Roles)))
            {
                if (role.ToLower() == roleName.ToLower()) return true;
            }
            return false;
        }
    }
}