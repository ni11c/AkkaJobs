using Agridea.Calendar;
using Agridea.News;
using Agridea.Security;
using Agridea.Service;
using Agridea.Web.UI;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Agridea
{
    public partial interface IAgrideaService : IService
    {
        #region Calendar
        IQueryable<CalendarEventDisplayItem> GridCalendarFor(SearchCalendarEventInput search);
        #endregion

        #region News
        IList<NewsItem> GetAllNewsOrderByDateAndValidity();
        void DeleteFile(string filePath);
        string GetSaveFileName(string path, string filename, HttpServerUtilityBase server);
        #endregion

        #region Security
        #region Users
        User GetUserByUserName(string userName);
        User GetUserByEmail(string email);
        User CreateUser(string userName, string password, string email, out MembershipCreateStatus status);
        void Block(string userName, bool blocked = true);
        void SetPassword(int id, string password);
        void DeleteUser(string userName);
        bool ValidateUser(string userName, string password);
        void ChangePassword(string userName, string newPassword);
        bool CanChangePassword(string userName, string oldPassword, string newPassword);
        void ChangePassword(string userName, string oldPassword, string newPassword);
        void SetCurrentUserLastActivity(User user);
        int GetNumberOfUsersOnline(int userIsOnlineTimeWindow);
        #endregion

        #region  Roles
        Role GetRoleByName(string roleName);
        string[] GetRolesForUser(string userName);
        string[] GetUsersInRole(string roleName);
        bool IsUserInRole(string userName, string roleName);
        Role GetDisconnectedRole();
        #endregion

        #region Permissions
        void RemoveAllPermissionsForRole(string roleName);
        IAgrideaService AddPermission(string roleName,  string itemName);
        bool HasPermission(string roleName, string itemName);
        #endregion

        #endregion
    }
}
