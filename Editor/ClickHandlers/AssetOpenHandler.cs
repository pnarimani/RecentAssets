using UnityEditor;
using UnityEngine;

namespace RecentAssets.ClickHandlers
{
    public class AssetOpenHandler
    {
        public static void Open(RecentFile file)
        {
            var path = AssetDatabase.GUIDToAssetPath(file.Guid);
            if (string.IsNullOrEmpty(path))
                return;

            var prefab = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (prefab == null)
                return;

            AssetDatabase.OpenAsset(prefab);
        }
    }
}