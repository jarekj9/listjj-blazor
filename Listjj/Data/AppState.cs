using System;


namespace Listjj.Data
{
    public class AppState
    {
        public bool IsLoggedIn { get; set; }
        public string UserName { get; set; }
        public Guid UserId { get; set; }
        public Guid RecentCategoryId { get; set; }
        public event Action OnChange;

        public void SetLogin(bool login, Guid id, string user)
        {
            IsLoggedIn = login;
            UserName = user;
            UserId = id;
            NotifyStateChanged();
        }

        private void NotifyStateChanged()
        {
            OnChange?.Invoke();
        }
    }
}