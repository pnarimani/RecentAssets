using UnityEditor;

namespace RecentAssets.ClickHandlers
{
    public class AssetPingHandler
    {
        public static void Ping(RecentFile file)
        {
            var path = AssetDatabase.GUIDToAssetPath(file.Guid);
            if (string.IsNullOrEmpty(path))
                return;

            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            if (obj == null)
                return;

            EditorGUIUtility.PingObject(obj);
        }
    }
}