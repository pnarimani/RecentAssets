using UnityEditor;
using UnityEditor.SceneManagement;

namespace RecentAssets.ClickHandlers
{
    public class SceneRecentFileClickHandler : IRecentFileClickHandler
    {
        public bool TryHandle(RecentFile file)
        {
            var path = AssetDatabase.GUIDToAssetPath(file.Guid);
            if (string.IsNullOrEmpty(path))
                return false;
            
            var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            if (scene == null)
                return false;
            
            EditorSceneManager.OpenScene(path);
            return true;
        }
    }
}