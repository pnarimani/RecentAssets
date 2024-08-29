using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace RecentAssets
{
    public class AssetFilter
    {
        private readonly List<string> _bannedList = RecentAssetsPreferences.GetBannedPatternsList();

        public bool ShouldAdd(string guid)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);

            if (!IsGameAsset(path))
                return false;

            if (_bannedList.Count == 0)
                return true;

            var directory = Path.GetDirectoryName(path);
            foreach (var pattern in _bannedList)
            {
                foreach (var foundGuid in AssetDatabase.FindAssets(pattern, new[] { directory }))
                {
                    if (foundGuid == guid)
                        return false;
                }
            }

            return true;
        }

        private static bool IsGameAsset(string path)
        {
            return path.StartsWith("Assets") || path.StartsWith("Packages");
        }
    }
}