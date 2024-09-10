using UnityEditor;
using UnityEngine.UIElements;

namespace RecentAssets
{
    public class DragAndDropManipulator : PointerManipulator
    {
        private readonly RecentAssetsWindow _window;
        private readonly RecentAssetsDataController _controller;
        
        public DragAndDropManipulator(RecentAssetsWindow window, RecentAssetsDataController controller)
        {
            _controller = controller;
            _window = window;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
            target.RegisterCallback<DragPerformEvent>(OnDragPerform);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
            target.UnregisterCallback<DragPerformEvent>(OnDragPerform);
        }

        private static void OnDragUpdate(DragUpdatedEvent _)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        }

        private void OnDragPerform(DragPerformEvent _)
        {
            var paths = DragAndDrop.paths;
            foreach (var path in paths)
            {
                var file = new RecentFile { Guid = AssetDatabase.AssetPathToGUID(path)};
                if (_controller.IsPinned(file))
                    continue;
                _controller.AddRecentItem(file, true);
                _controller.TogglePin(file);
            }
            _window.Refresh();
        }
    }
}