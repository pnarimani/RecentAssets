using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RecentAssets.Watchers
{
    public class SceneWatcher : IDisposable
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
            _controller.AddRecentItem(new RecentFile { Guid = guid }, false);
            _window.Refresh();
        }

        public void Dispose()
        {
            EditorSceneManager.sceneClosed -= SceneClosed;
        }
    }
}