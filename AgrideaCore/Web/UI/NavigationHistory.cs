using System;
using System.Collections.Generic;

namespace Agridea.Web.UI
{
    [Serializable]
    public class NavigationHistory
    {
        #region Constants
        private const char Slash = '/';
        private const char QueryMark = '?';
        private const string Colon = ":";
        private const string Semicolon = ";";
        #endregion

        #region Members
        private Stack<Uri> history_;
        #endregion

        #region Initialization
        public NavigationHistory()
        {
            history_ = new Stack<Uri>();
        }
        #endregion

        #region Services
        public override string ToString() { return string.Join(Semicolon, history_); }
        public void Add(Uri currentUrl)
        {
            int existingUrlIndex = Find(currentUrl);
            if (existingUrlIndex >= 0 || !FollowsLast(currentUrl))
                EmptyUpto(existingUrlIndex);

            history_.Push(currentUrl);
        }
        public Uri RetreiveBackUrl()
        {
            if (history_.Count == 0) return null;
            return history_.Pop();
        }
        #endregion

        #region Helpers
        private int Find(Uri currentUrl)
        {
            var historyArray = history_.ToArray();
            for (int i = 0; i < history_.Count; i++)
                if (PathWithoutQuery(historyArray[i]) == PathWithoutQuery(currentUrl)) return i;
            return -1;
        }
        private bool FollowsLast(Uri currentUrl)
        {
            if (history_.Count == 0 || currentUrl == null) return false;

            var url = history_.Peek();
            if (url == null) return false;
            var currentUrlComponents = currentUrl.PathAndQuery.Split(Slash);
            var urlComponents = url.PathAndQuery.Split(Slash);
            var currentUrlPath = PathWithoutQuery(currentUrl);
            var urlPath = PathWithoutQuery(url);

            return currentUrlComponents.Length == urlComponents.Length + 1 && currentUrlPath.StartsWith(urlPath);
        }
        private string PathWithoutQuery(Uri url)
        {
            if (url == null) return null;
            var indexOfQuestionMark = url.PathAndQuery.IndexOf(QueryMark);
            if (indexOfQuestionMark == -1) return url.PathAndQuery;
            return url.PathAndQuery.Substring(0, indexOfQuestionMark);
        }
        private void EmptyUpto(int existingUrlIndex)
        {
            if (existingUrlIndex == -1) history_.Clear();

            for (int i = 0; i <= existingUrlIndex; i++)
                history_.Pop();
        }
        #endregion
    }
}