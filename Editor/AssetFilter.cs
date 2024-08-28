using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RecentAssets
{
    public class AssetFilter
    {
        private readonly List<string> _bannedList = RecentAssetsPreferences.GetBannedPatternsList();

        public bool ShouldAdd(string guid)
        {
            if (_bannedList.Count == 0)
                return true;

            var path = AssetDatabase.GUIDToAssetPath(guid);
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
    }
}