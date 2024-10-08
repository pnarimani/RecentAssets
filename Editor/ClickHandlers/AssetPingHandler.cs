﻿using UnityEditor;

namespace RecentAssets.ClickHandlers
{
    public class AssetPingHandler
    {
        public bool Ping(RecentFile file)
        {
            var path = AssetDatabase.GUIDToAssetPath(file.Guid);
            if (string.IsNullOrEmpty(path))
                return false;
            
            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            if (obj == null)
                return false;
            
            EditorGUIUtility.PingObject(obj);
            return true;
        }
    }
}