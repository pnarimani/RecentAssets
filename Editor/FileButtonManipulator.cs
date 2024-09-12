using RecentAssets.ClickHandlers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecentAssets
{
    public class FileButtonManipulator : PointerManipulator
    {
        private const float DragThreshold = 50;
        private Vector3 _pointerDownPosition;

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if ((evt.position - _pointerDownPosition).sqrMagnitude > DragThreshold)
            {
                if (IsLeftMouseButtonDown(evt))
                    StartDragging();
            }
        }

        private static bool IsLeftMouseButtonDown(PointerMoveEvent evt)
        {
            return (evt.pressedButtons & 1) == 1;
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if ((evt.position - _pointerDownPosition).sqrMagnitude < DragThreshold)
                AssetOpenHandler.Open(((RecentFile)target.userData));
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            _pointerDownPosition = evt.position;
        }

        private static void OnDragUpdate(DragUpdatedEvent _)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }

        private void StartDragging()
        {
            var file = (RecentFile)target.userData;
            var path = AssetDatabase.GUIDToAssetPath(file.Guid);
            var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (obj == null)
                return;

            DragAndDrop.PrepareStartDrag();
            DragAndDrop.objectReferences = new[] { obj };
            DragAndDrop.paths = path != string.Empty ? new[] { path } : new string[] { };
            DragAndDrop.StartDrag(obj.name);
        }
    }
}