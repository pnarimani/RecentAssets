using System;
using System.Collections.Generic;
using RecentAssets.ClickHandlers;
using RecentAssets.Watchers;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RecentAssets
{
    public class RecentAssetsWindow : EditorWindow
    {
        private Vector2 _scrollPos;
        private GUIStyle _choiceButtonStyle;
        private GUIStyle _closeButtonStyle;

        private RecentAssetsDataController _dataController;
        private static readonly List<IDisposable> _watchers = new();
        private static readonly List<IRecentFileClickHandler> _clickHandlers = new()
        {
            new SceneRecentFileClickHandler(),
            new PrefabRecentFileClickHandler(),
            new GeneralAssetHighlighter(),
        };


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

        private void OnGUI()
        {
            InitializeStyles();

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            
            DrawAllFiles(true);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            DrawAllFiles(false);

            EditorGUILayout.EndScrollView();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            DrawClearButton();
        }

        private void InitializeStyles()
        {
            _closeButtonStyle = new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset(0, 0, 0, 0),
            };

            _choiceButtonStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
            };
        }

        private void DrawClearButton()
        {
            GUI.backgroundColor = new Color(1, 0, 0, 0.4f);
            if (GUILayout.Button(new GUIContent("CLEAR RECENT"), GUILayout.Height(24)))
            {
                _dataController.DeleteRecentFiles();
                Repaint();
            }

            GUI.backgroundColor = Color.white;
        }

        private void DrawAllFiles(bool drawPinned)
        {
            var presentAssets = OpenAssetsProvider.GetOpenAssets();

            var lastIconSize = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(new Vector2(15, 15));

            var files = _dataController.GetFiles();
            for (var i = files.Count - 1; i >= 0; i--)
            {
                var file = files[i];
                if (file.IsPinned != drawPinned)
                    continue;
                DrawFileRow(file, presentAssets);
            }

            EditorGUIUtility.SetIconSize(lastIconSize);
        }

        private void DrawFileRow(RecentFile file, List<string> presentAssets)
        {
            EditorGUILayout.BeginHorizontal();

            if (presentAssets.Contains(file.Guid))
                EditorGUI.BeginDisabledGroup(true);

            DrawFileButton(file);
            DrawPinButton(file);
            DrawRemoveButton(file);

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawRemoveButton(RecentFile file)
        {
            GUI.backgroundColor = new Color(1, 0, 0, 0.4f);
            var icon = EditorGUIUtility.IconContent("d_winbtn_win_close");
            if (GUILayout.Button(icon, _closeButtonStyle, GUILayout.Width(24), GUILayout.Height(24)))
            {
                _dataController.Remove(file);
                Repaint();
            }

            GUI.backgroundColor = Color.white;
        }

        private void DrawPinButton(RecentFile file)
        {
            var icon = file.IsPinned
                ? EditorGUIUtility.IconContent("pinned")
                : EditorGUIUtility.IconContent("pin");
            GUI.backgroundColor = file.IsPinned ? new Color(0, 1, 0, 0.4f) : Color.white;
            if (GUILayout.Button(icon, _closeButtonStyle, GUILayout.Width(24), GUILayout.Height(24)))
            {
                _dataController.Pin(file);
                Repaint();
            }
        }

        private void DrawFileButton(RecentFile file)
        {
            var path = AssetDatabase.GUIDToAssetPath(file.Guid);
            var fileName = AssetDatabase.LoadAssetAtPath<Object>(path).name;
            var icon = AssetDatabase.GetCachedIcon(path);

            if (!GUILayout.Button(new GUIContent(fileName, icon), _choiceButtonStyle, GUILayout.Height(24)))
                return;

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            foreach (var handler in _clickHandlers)
            {
                if (handler.TryHandle(file))
                    break;
            }
        }

        public void Reload()
        {
            OnDisable();
            OnEnable();
        }
    }
}