using UnityEditor;
using UnityEngine;

namespace RecentAssets.Watchers
{
    public class DragAndDropWatcher : IWatcher
    {
        private readonly RecentAssetsDataController _controller;
        
        public DragAndDropWatcher(RecentAssetsDataController controller)
        {
            _controller = controller;
        }
        
        public void OnGUI()
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Link;
            if (Event.current.type == EventType.DragExited)
            {
                var paths = DragAndDrop.paths;
                foreach (var path in paths)
                {
                    var guid = AssetDatabase.AssetPathToGUID(path);
                    _controller.AddRecentItem(guid, true);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}