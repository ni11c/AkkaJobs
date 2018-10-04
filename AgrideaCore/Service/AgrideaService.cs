using Agridea.Calendar;
using Agridea.DataRepository;
using Agridea.Diagnostics.Contracts;
using Agridea.News;
using Agridea.ObjectMapping;
using Agridea.Security;
using Agridea.Service;
using Agridea.Web.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Agridea
{
    public partial class AgrideaService : ServiceBase, IAgrideaService
    {
        #region Constants
        protected const string IndexAction = "Index";
        protected const string CreateAction = "Create";
        protected const string EditAction = "Edit";
        protected const string DeleteAction = "Delete";
        #endregion

        #region Members
        #endregion

        #region Initialization
        public AgrideaService(IDataRepository repository)
            : base(repository)
        {
        }
        #endregion



        #region IAgrideaService
        #region Calendar
        public IQueryable<CalendarEventDisplayItem> GridCalendarFor(SearchCalendarEventInput search)
        {
            var allEvents = Repository.All<CalendarEvent>();

            if (!string.IsNullOrWhiteSpace(search.AppliesTo))
                allEvents = allEvents.Where(m => m.AppliesTo.Contains(search.AppliesTo));
            if (!string.IsNullOrWhiteSpace(search.Comment))
                allEvents = allEvents.Where(m => m.Comment.Contains(search.Comment));
            if (!string.IsNullOrWhiteSpace(search.Description))
                allEvents = allEvents.Where(m => m.Description.Contains(search.Description));
            if (search.StartDate.HasValue)
                allEvents = allEvents.Where(m => m.StartDate >= search.StartDate.Value);
            if (search.EndDate.HasValue)
                allEvents = allEvents.Where(m => m.EndDate <= search.EndDate.Value);
            if (!string.IsNullOrWhiteSpace(search.Owner))
                allEvents = allEvents.Where(m => m.Owner.Contains(search.Owner));
            if (!string.IsNullOrWhiteSpace(search.Title))
                allEvents = allEvents.Where(m => m.Title.Contains(search.Title));
            if (search.RecurrenceId > 0)
                allEvents = allEvents.Where(m => m.Recurrence.Id == search.RecurrenceId);
            allEvents = (search.StatusId > 0) ?
                allEvents.Where(m => m.Status.Id == search.StatusId) :
                allEvents.Where(m => m.Status.Code != CalendarEventStatus.Done);

            return allEvents.Map<CalendarEvent, CalendarEventDisplayItem>();
        }
        #endregion

        #region Security
        #region Users
        public User GetUserByUserName(string userName)
        {
            Requires<ArgumentException>.IsNotNull(userName, "UserName is missing");
            return GetFirstOrDefault<User>(m => m.UserName == userName);
        }
        public User GetUserByEmail(string email)
        {
            Asserts<ArgumentException>.IsNotEmpty(email);
            return GetFirstOrDefault<User>(m => m.Email == email);
        }
        public User CreateUser(
            string userName,
            string password,
            string email,
            out MembershipCreateStatus status)
        {
            if (!ValidateUserName(userName, email, 0, out status))
                return null;

            if (!ValidatePassword(password))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            var user = new User
                           {
                               UserName = userName,
                               Password = TransformPassword(password),
                               Email = email,
                               CreationDate = DateTime.Now,
                               LastActivityDate = DateTime.Now,
                               LastPasswordChangeDate = DateTime.Now
                           };

            Add(user);
            Save();
            user = GetUserByUserName(user.UserName);
            status = MembershipCreateStatus.Success;
            return user;
        }
        public void Block(string userName, bool blocked)
        {
            var user = GetUserByUserName(userName);
            Requires<InvalidOperationException>.IsNotNull(user);

            user.Blocked = blocked;
            Modify(user);
            Save();
        }

        public void SetPassword(int id, string password)
        {
            var user = GetById<User>(id);
            user.Password = TransformPassword(password);
            user.LastPasswordChangeDate = DateTime.Now;
            Modify(user).Save();
        }

        public void DeleteUser(string userName)
        {
            var user = GetUserByUserName(userName);
            Requires<InvalidOperationException>.IsNotNull(user);
            CascadeRemove(user);
            Save();
        }

        public bool ValidateUser(string userName, string password)
        {
            var user = GetUserByUserName(userName);
            if (user == null || password == null)
                return false;

            if (ValidateUserInternal(user, password))
            {
                user.LastLoginDate = DateTime.Now;
                user.LastActivityDate = DateTime.Now;
                Modify(user);
                Save();
                return true;
            }

            return false;
        }

        public void ChangePassword(string userName, string newPassword)
        {
            var user = GetUserByUserName(userName);
            Requires<InvalidOperationException>.IsNotNull(user);

            user.Password = TransformPassword(newPassword);
            user.LastPasswordChangeDate = DateTime.Now;
            Modify(user);
            Save();
        }

        public bool CanChangePassword(string userName, string oldPassword, string newPassword)
        {
            var user = GetUserByUserName(userName);
            Requires<InvalidOperationException>.IsNotNull(user);

            return (ValidateUserInternal(user, oldPassword) && ValidatePassword(newPassword));
        }

        public void ChangePassword(string userName, string oldPassword, string newPassword)
        {
            var user = GetUserByUserName(userName);
            Requires<InvalidOperationException>.IsNotNull(user);

            if (ValidateUserInternal(user, oldPassword))
            {
                Requires<ArgumentException>.IsTrue(ValidatePassword(newPassword));

                user.Password = TransformPassword(newPassword);
                user.LastPasswordChangeDate = DateTime.Now;
                Modify(user);
                Save();
            }
        }

        public void SetCurrentUserLastActivity(User user)
        {
            Requires<InvalidOperationException>.IsNotNull(user);

            user.LastActivityDate = DateTime.Now;
            Modify(user);
            Save();
        }

        public IList<User> GetUsersOnLine(int userIsOnlineTimeWindow)
        {
            var usersOnLine = new List<User>();
            foreach (User user in GetAll<User>())
                if (user.IsOnlineSince(userIsOnlineTimeWindow))
                    usersOnLine.Add(user);
            return usersOnLine;
        }

        public int GetNumberOfUsersOnline(int userIsOnlineTimeWindow)
        {
            int count = 0;
            foreach (User user in GetAll<User>())
                if (user.IsOnlineSince(userIsOnlineTimeWindow))
                    count++;
            return count;
        }

        #endregion

        #region Roles
        public Role GetRoleByName(string roleName)
        {
            Asserts<ArgumentException>.IsNotEmpty(roleName);
            return GetFirstOrDefault<Role>(m => m.Name == roleName);
        }

        public string[] GetRolesForUser(string userName)
        {
            Requires<ArgumentException>.IsNotEmpty(userName);
            var user = GetUserByUserName(userName);
            return All<Role>(m => m.UserRoleList.Any(x => x.User.UserName == userName))
                .Select(m => m.Name)
                .ToArray();

        }

        public string[] GetUsersInRole(string roleName)
        {
            return All<User>(x => x.UserRoleList.Any(m => m.Role.Name == roleName))
                .Select(m => m.UserName)
                .ToArray();
        }

        public bool IsUserInRole(string userName, string roleName)
        {
            Requires<ArgumentException>.IsNotEmpty(userName);
            Requires<ArgumentException>.IsNotEmpty(roleName);
            return Any<User>(m => m.UserName == userName && m.UserRoleList.Any(x => x.Role.Name == roleName));
        }


        public virtual Role GetDisconnectedRole()
        {
            return new Role { Name = Role.DisconnectedRole };
        }

        public IList<Role> GetAllRolesByPartialName(string partialRoleName, PaginationOptions options, out int totalCount)
        {
            Requires<ArgumentNullException>.IsNotNull(partialRoleName);
            return Repository.All<Role>(item => item.Name.Contains(partialRoleName))
                .ToOrderedPagination(options, out totalCount);
        }
        public IList<Role> GetAllRoles(PaginationOptions options, out int totalCount)
        {
            return Repository.All<Role>()
                .ToOrderedPagination(options, out totalCount);
        }
        #endregion

        #region Permissions
        public void RemoveAllPermissionsForRole(string roleName)
        {
            Requires<ArgumentException>.IsNotEmpty(roleName);

            var role = GetRoleByName(roleName);
            Requires<InvalidOperationException>.IsNotNull(role, string.Format("Role '{0}' does not exist", roleName));

            ClearPermissions(role);

            Save();
        }

        public IAgrideaService AddPermission(string roleName, string itemName)
        {
            Requires<ArgumentException>.IsNotEmpty(roleName);
            Requires<ArgumentException>.IsNotEmpty(itemName);

            GetRoleByName(roleName).PermissionList.Add(new Permission { ItemName = itemName });
            Save();
            return this;
        }
        public bool HasPermission(string roleName, string itemName)
        {
            Requires<ArgumentException>.IsNotEmpty(roleName);
            Requires<ArgumentException>.IsNotEmpty(itemName);

            Role role = GetRoleByName(roleName);
            Requires<InvalidOperationException>.IsNotNull(role, string.Format("Role '{0}' does not exist", roleName));

            return DoHavePermission(role, itemName);
        }
        #endregion

        #endregion

        #region News

        public IList<NewsItem> GetAllNewsOrderByDateAndValidity()
        {
            return Repository.All<NewsItem>(item => ((!item.ValidityDateEnd.HasValue) || item.ValidityDateEnd >= DateTime.Today) &&
                                                    ((!item.ValidityDateStart.HasValue) || item.ValidityDateStart <= DateTime.Today))
                .OrderByDescending(m => m.CreationDate)
                .ToList();
        }

        public void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public string GetSaveFileName(string path, string filename, HttpServerUtilityBase server)
        {
            var fileName = Path.GetFileName(filename);
            var savefilename = Path.Combine(path, fileName);
            var fileIndex = 0;
            while (File.Exists(server.MapPath(savefilename)))
            {
                fileIndex++;
                fileName = fileIndex.ToString() + "_" + fileName;
                savefilename = Path.Combine(path, fileName);
            }
            return fileName;
        }

        #endregion

        #endregion

        #region Helpers

        #region Security

        protected bool ValidatePassword(string password)
        {
            return true;
        }

        protected bool ValidateUserName(string userName, string email, int excludedId, out MembershipCreateStatus status)
        {
            var isValid = true;
            status = MembershipCreateStatus.Success;

            if (GetAll<User>().Where(user => user.Id.CompareTo(excludedId) != 0).Any(user => string.Equals(user.UserName, userName, StringComparison.OrdinalIgnoreCase)))
            {
                isValid = false;
                status = MembershipCreateStatus.DuplicateUserName;
            }
            return isValid;
        }

        private bool ValidateUserInternal(User user, string password)
        {
            return !user.Blocked && ComparePassword(password, user.Password);
        }

        protected string TransformPassword(string password)
        {
            return PasswordHelper.Hash(password);
        }

        private bool ComparePassword(string clearPassword, string transformedPassword)
        {
            return PasswordHelper.CompareHash(clearPassword, transformedPassword);
        }


        private bool DoHavePermission(Role role, string itemName)
        {
            Asserts<ArgumentException>.IsNotEmpty(itemName);
            return role.PermissionList.FirstOrDefault(x => x.ItemName == itemName) != null;
        }

        protected void ClearPermissions(Role item)
        {
            var permissions = new Permission[item.PermissionList.Count];
            item.PermissionList.CopyTo(permissions, 0);
            foreach (var permission in permissions)
            {
                item.PermissionList.Remove(permission);
                Remove(permission); //cant live alone
            }
        }

        protected void ClearPermission(string permissionItemName)
        {
            var permissions = GetAll<Permission>(p => p.ItemName == permissionItemName);
            foreach (var permission in permissions)
            {
                Remove(permission);
            }
        }
        #endregion
        #endregion
    }
}
