using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RecentAssets
{
    public class RecentAssetsDataController
    {
        private const string ListPersistenceKey = "RecentAssets_RecentFiles";

        private readonly AssetFilter _filter = new();
        private readonly List<RecentFile> _files;

        public RecentAssetsDataController()
        {
            var json = EditorPrefs.GetString(ListPersistenceKey, "{}");
            _files = JsonUtility.FromJson<WrappedList<RecentFile>>(json).List;
        }

        public void Save()
        {
            var json = JsonUtility.ToJson(new WrappedList<RecentFile> { List = _files });
            EditorPrefs.SetString(ListPersistenceKey, json);
        }

        public IReadOnlyList<RecentFile> GetFiles()
        {
            _files.RemoveAll(IsInvalidAsset);
            return _files;
        }

        public void DeleteRecentFiles()
        {
            for (var i = _files.Count - 1; i >= 0; i--)
            {
                if (_files[i].IsPinned)
                    continue;
                _files.RemoveAt(i);
            }
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

        public void Remove(RecentFile file)
        {
            Remove(file.Guid);
        }

        private void Remove(string guid)
        {
            for (var i = _files.Count - 1; i >= 0; i--)
            {
                if (_files[i].Guid == guid)
                {
                    _files.RemoveAt(i);
                    break;
                }
            }
        }

        public void Pin(RecentFile file)
        {
            file.IsPinned = true;
        }

        public void AddRecentItem(string guid, bool isPinned)
        {
            if (!_filter.ShouldAdd(guid))
                return;

            if (_files.Any(s => s.Guid == guid && s.IsPinned))
                return;

            Remove(guid);
            _files.Add(new RecentFile
            {
                Guid = guid,
                IsPinned = isPinned,
            });

            RemoveExtraFiles();
        }

        private void RemoveExtraFiles()
        {
            var maxFiles = RecentAssetsPreferences.MaxRecentAssets + _files.Count(a => a.IsPinned);
            var extra = Mathf.Max(0, _files.Count - maxFiles);
            for (var i = 0; i < _files.Count; i++)
            {
                if (extra == 0)
                    break;

                if (!_files[i].IsPinned)
                {
                    _files.RemoveAt(i);
                    extra--;
                    i--;
                }
            }
        }
    }
}
