using UnityEditor;
using UnityEngine;

namespace RecentAssets.ClickHandlers
{
    public class AssetOpenHandler
    {
        public bool Open(RecentFile file)
        {
            var path = AssetDatabase.GUIDToAssetPath(file.Guid);
            if (string.IsNullOrEmpty(path))
                return false;
            
            var prefab = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (prefab == null)
                return false;
            
            AssetDatabase.OpenAsset(prefab);
            return true;
        }
    }
}