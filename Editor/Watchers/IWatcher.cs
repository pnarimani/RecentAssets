using System;

namespace RecentAssets.Watchers
{
    public interface IWatcher : IDisposable
    {
        void OnGUI();
    }
}