namespace RecentAssets.ClickHandlers
{
    public interface IRecentFileClickHandler
    {
        bool TryHandle(RecentFile file);
    }
}