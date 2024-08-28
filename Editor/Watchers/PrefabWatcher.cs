using System;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace RecentAssets.Watchers
{
    public class PrefabWatcher : IDisposable
    {
        private readonly RecentAssetsDataController _controller;
        private readonly RecentAssetsWindow _window;

        public PrefabWatcher(RecentAssetsDataController controller, RecentAssetsWindow window)
        {
            _window = window;
            _controller = controller;

            PrefabStage.prefabStageOpened += OnStageOpened;
            PrefabStage.prefabStageClosing += OnStageClosed;
        }

        private void OnStageOpened(PrefabStage obj)
        {
            _window.Repaint();
        }

        private void OnStageClosed(PrefabStage obj)
        {
            _controller.AddRecentItem(AssetDatabase.AssetPathToGUID(obj.assetPath), false);
            _window.Repaint();
        }

        public void Dispose()
        {
            PrefabStage.prefabStageClosing -= OnStageClosed;
            PrefabStage.prefabStageOpened -= OnStageOpened;
        }
    }
}