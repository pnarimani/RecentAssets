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
            var file = new RecentFile { Guid = AssetDatabase.AssetPathToGUID(obj.assetPath) };
            _controller.AddRecentItem(file, false);
            _window.Refresh();
        }

        private void OnStageClosed(PrefabStage obj)
        {
            _window.Refresh();
        }

        public void Dispose()
        {
            PrefabStage.prefabStageClosing -= OnStageClosed;
            PrefabStage.prefabStageOpened -= OnStageOpened;
        }
    }
}