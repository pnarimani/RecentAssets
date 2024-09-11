using System;
using System.Collections.Generic;
using RecentAssets.ClickHandlers;
using RecentAssets.Watchers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace RecentAssets
{
    public class RecentAssetsWindow : EditorWindow
    {
        private const string ActionButtonClass = "ActionButton";

        [SerializeField] private StyleSheet _styleSheet;

        private RecentAssetsDataController _dataController;
        
        private DragAndDropManipulator _manipulator;
        private readonly List<IDisposable> _watchers = new();
        private readonly List<string> _openAssets = new();

        [MenuItem("Tools/Recent Assets")]
        private static void Init()
        {
            var window = GetWindow<RecentAssetsWindow>();
            window.titleContent = new GUIContent("Recent Assets");
            window.Show();
        }

        private void OnEnable()
        {
            _dataController = new RecentAssetsDataController();

            _watchers.Clear();
            _watchers.Add(new SceneWatcher(_dataController, this));
            _watchers.Add(new PrefabWatcher(_dataController, this));
            _watchers.Add(new UndoWatcher(_dataController, this));
        }

        private void OnDisable()
        {
            _dataController.Save();

            foreach (var watcher in _watchers)
                watcher.Dispose();
            _watchers.Clear();
        }

        private void CreateGUI()
        {
            rootVisualElement.Clear();
            rootVisualElement.styleSheets.Add(_styleSheet);

            var pinnedList = CreateListView(_dataController.Data.PinnedFiles);
            pinnedList.reorderable = true;
            var recentList = CreateListView(_dataController.Data.RecentFiles);
            recentList.name = "recent-list";
            rootVisualElement.Add(pinnedList);
            rootVisualElement.Add(new VisualElement { name = "Separator" });
            rootVisualElement.Add(recentList);
            rootVisualElement.Add(new VisualElement { name = "Separator" });
            rootVisualElement.Add(new Button(OnClearClicked) { name = "clear", text = "CLEAR" });

            pinnedList.AddManipulator(new DragAndDropManipulator(this, _dataController));
            recentList.AddManipulator(new DragAndDropManipulator(this, _dataController));
            rootVisualElement.AddManipulator(new DragAndDropManipulator(this, _dataController));
        }

        private ListView CreateListView(List<RecentFile> source)
        {
            return new ListView(source)
            {
                selectionType = SelectionType.None,
                fixedItemHeight = 26,
                makeItem = CreateListItemView,
                bindItem = BindListItemView(source),
            };
        }

        private Action<VisualElement, int> BindListItemView(List<RecentFile> list)
        {
            return (element, i) =>
            {
                var file = list[i];
                element.Q<Button>("open").userData = file;
                element.Q<Button>("remove").userData = file;
                element.Q<Button>("ping").userData = file;
                UpdateOpenButton(file, element);
                UpdatePinButton(file, element);
            };
        }

        private void UpdatePinButton(RecentFile file, VisualElement element)
        {
            var pinButton = element.Q<Button>("pin");
            if (_dataController.IsPinned(file))
                pinButton.AddToClassList("pinned");
            else
                pinButton.RemoveFromClassList("pinned");
            pinButton.userData = file;
        }

        private void UpdateOpenButton(RecentFile file, VisualElement element)
        {
            var path = AssetDatabase.GUIDToAssetPath(file.Guid);
            var open = element.Q<Button>("open");
            open.SetEnabled(!_openAssets.Contains(file.Guid));
            open.userData = file;
            open.text = AssetDatabase.LoadAssetAtPath<Object>(path).name;
            open.Q<Image>().image = AssetDatabase.GetCachedIcon(path);
        }

        private VisualElement CreateListItemView()
        {
            var open = new Button { name = "open" };
            open.Add(new Image());
            open.clicked += () => OnOpenClicked(open);

            var ping = new Button { name = "ping" };
            ping.AddToClassList(ActionButtonClass);
            ping.clicked += () => OnPingClicked(ping);

            var pin = new Button { name = "pin" };
            pin.AddToClassList(ActionButtonClass);
            pin.clicked += () => OnPinClicked(pin);

            var remove = new Button { name = "remove" };
            remove.AddToClassList(ActionButtonClass);
            remove.clicked += () => OnRemoveClicked(remove);

            var root = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            root.Add(open);
            root.Add(ping);
            root.Add(pin);
            root.Add(remove);
            return root;
        }

        private void OnPingClicked(Button button)
        {
            AssetPingHandler.Ping((RecentFile)button.userData);
        }

        private void OnOpenClicked(Button button)
        {
            AssetOpenHandler.Open((RecentFile)button.userData);
        }

        private void OnPinClicked(Button button)
        {
            _dataController.TogglePin((RecentFile)button.userData);
            Refresh();
        }

        private void OnRemoveClicked(Button button)
        {
            _dataController.Remove(((RecentFile)button.userData).Guid);
            Refresh();
        }

        private void OnClearClicked()
        {
            _dataController.Clear();
            Refresh();
        }

        public void Refresh()
        {
            OpenAssetsProvider.UpdateOpenAssets(_openAssets);
            _dataController.RemoveInvalidFiles();
            rootVisualElement.Query<ListView>().ForEach(list => list.Rebuild());
        }
    }
}