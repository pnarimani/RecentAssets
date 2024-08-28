using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace RecentAssets
{
    public static class OpenAssetsProvider
    {
        public static List<string> GetOpenAssets()
        {
            var result = new List<string>();

            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null)
                result.Add(AssetDatabase.AssetPathToGUID(stage.assetPath));

            for (var index = 0; index < SceneManager.sceneCount; index++)
                result.Add(AssetDatabase.AssetPathToGUID(SceneManager.GetSceneAt(index).path));

            return result;
        }
    }
}