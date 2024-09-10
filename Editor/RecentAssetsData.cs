using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RecentAssets
{
    public class RecentAssetsData
    {
        private static string UniqueId { get; } = Application.dataPath.GetHashCode().ToString();
        private static string RecentPersistenceKey { get; } = $"RecentAssets_{UniqueId}_RecentFiles";
        private static string PinnedPersistenceKey { get; } = $"RecentAssets_{UniqueId}_PinnedFiles";

        public List<RecentFile> PinnedFiles { get; } = LoadList(PinnedPersistenceKey);
        public List<RecentFile> RecentFiles { get; } = LoadList(RecentPersistenceKey);

        public void Save()
        {
            SaveList(PinnedFiles, PinnedPersistenceKey);
            SaveList(RecentFiles, RecentPersistenceKey);
        }

        private static List<RecentFile> LoadList(string key)
        {
            return JsonUtility.FromJson<WrappedList<RecentFile>>(EditorPrefs.GetString(key, "{}")).List;
        }

        private static void SaveList(List<RecentFile> list, string key)
        {
            EditorPrefs.SetString(key, JsonUtility.ToJson(new WrappedList<RecentFile> { List = list }));
        }
    }
}