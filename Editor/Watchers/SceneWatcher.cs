using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RecentAssets.Watchers
{
    public class SceneWatcher : IWatcher
    {
        private readonly RecentAssetsDataController _controller;
        private readonly RecentAssetsWindow _window;

        public SceneWatcher(RecentAssetsDataController controller, RecentAssetsWindow window)
        {
            _window = window;
            _controller = controller;
            EditorSceneManager.sceneClosed += SceneClosed;
        }

        private void SceneClosed(Scene scene)
        {
            if (Application.isPlaying) return;
            var guid = AssetDatabase.AssetPathToGUID(scene.path);
            _controller.AddRecentItem(guid, false);
            _window.Repaint();
        }

        public void Dispose()
        {
            EditorSceneManager.sceneClosed -= SceneClosed;
        }

        public void OnGUI()
        {
        }
    }
}