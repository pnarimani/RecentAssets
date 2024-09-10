using UnityEditor;

namespace RecentAssets
{
    public class RecentAssetsDataController
    {
        private readonly AssetFilter _filter = new();
        public RecentAssetsData Data { get; } = new();

        public void Save()
        {
            Data.Save();
        }

        public void RemoveInvalidFiles()
        {
            Data.PinnedFiles.RemoveAll(IsInvalidAsset);
            Data.RecentFiles.RemoveAll(IsInvalidAsset);
        }

        public void Clear()
        {
            Data.RecentFiles.Clear();
        }

        private static bool IsInvalidAsset(RecentFile s)
        {
            if (string.IsNullOrEmpty(s.Guid))
                return true;
            var path = AssetDatabase.GUIDToAssetPath(s.Guid);
            if (string.IsNullOrEmpty(path))
                return true;
            var guid = AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets);
            return guid != s.Guid;
        }

        public void Remove(string guid)
        {
            Data.RecentFiles.RemoveAll(x => x.Guid == guid);
            Data.PinnedFiles.RemoveAll(x => x.Guid == guid);
        }

        public void TogglePin(RecentFile file)
        {
            if (IsPinned(file))
            {
                Data.PinnedFiles.Remove(file);
                Data.RecentFiles.Add(file);
            }
            else
            {
                Data.RecentFiles.Remove(file);
                Data.PinnedFiles.Add(file);
            }
        }

        public bool IsPinned(RecentFile file)
        {
            return Data.PinnedFiles.Contains(file);
        }

        public void AddRecentItem(RecentFile file, bool force)
        {
            if (!force && !_filter.ShouldAdd(file.Guid))
                return;

            if (Data.PinnedFiles.Contains(file))
                return;

            Data.RecentFiles.Remove(file);
            Data.RecentFiles.Insert(0, file);
            
            RemoveExtraFiles();
            Save();
        }

        private void RemoveExtraFiles()
        {
            var files = Data.RecentFiles;
            while(files.Count > RecentAssetsPreferences.MaxRecentAssets)
                files.RemoveAt(files.Count - 1);
        }
    }
}