using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace RecentAssets
{
    public static class OpenAssetsProvider
    {
        public static void UpdateOpenAssets(List<string> result)
        {
            result.Clear();
            
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null)
                result.Add(AssetDatabase.AssetPathToGUID(stage.assetPath));

            for (var index = 0; index < SceneManager.sceneCount; index++)
                result.Add(AssetDatabase.AssetPathToGUID(SceneManager.GetSceneAt(index).path));
        }
    }
}