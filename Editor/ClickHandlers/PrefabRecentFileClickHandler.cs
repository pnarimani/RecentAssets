using UnityEditor;
using UnityEngine;

namespace RecentAssets.ClickHandlers
{
    public class PrefabRecentFileClickHandler : IRecentFileClickHandler
    {
        public bool TryHandle(RecentFile file)
        {
            var path = AssetDatabase.GUIDToAssetPath(file.Guid);
            if (string.IsNullOrEmpty(path))
                return false;
            
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
                return false;
            
            AssetDatabase.OpenAsset(prefab);
            return true;
        }
    }
}