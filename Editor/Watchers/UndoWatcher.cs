using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RecentAssets.Watchers
{
    public class UndoWatcher : IDisposable
    {
        private readonly RecentAssetsDataController _controller;
        private readonly RecentAssetsWindow _window;

        public UndoWatcher(RecentAssetsDataController controller, RecentAssetsWindow window)
        {
            _window = window;
            _controller = controller;
            Undo.postprocessModifications += OnPostprocessModifications;
        }

        private UndoPropertyModification[] OnPostprocessModifications(UndoPropertyModification[] modifications)
        {
            foreach (var mod in modifications)
            {
                var asset = GetAsset(mod);
                if (asset == null)
                    continue;
                var path = AssetDatabase.GetAssetPath(asset);
                var guid = AssetDatabase.AssetPathToGUID(path);
                if (string.IsNullOrEmpty(guid))
                    continue;
                _controller.AddRecentItem(guid, false);
            }

            _window.Repaint();

            return modifications;
        }

        private static Object GetAsset(UndoPropertyModification modification)
        {
            var obj = modification.currentValue.target;

            if (obj is Component component)
                obj = component.transform.root.gameObject;

            return obj;
        }

        public void Dispose()
        {
            Undo.postprocessModifications -= OnPostprocessModifications;
        }
    }
}