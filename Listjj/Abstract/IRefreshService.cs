using System;


namespace Listjj.Abstract
{
    public interface IRefreshService
    {
      event Action RefreshRequested;
      void CallRequestRefresh();
    }
}