using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RecentAssets
{
    public class RecentAssetsPreferences : SettingsProvider
    {
        private const string BannedPatternsKey = "BannedPatterns";
        private const string Prefix = "RecentAssets-";

        private RecentAssetsSettings _settings;
        private SerializedObject _serializedObject;

        public RecentAssetsPreferences(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(
            path, scopes, keywords)
        {
        }

        public static int MaxRecentAssets
        {
            get => Mathf.Max(0, EditorPrefs.GetInt($"{Prefix}{nameof(MaxRecentAssets)}", 10));
            set => EditorPrefs.SetInt($"{Prefix}{nameof(MaxRecentAssets)}", value);
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();

            var window = EditorWindow.GetWindow<RecentAssetsWindow>();
            if (window != null)
                window.Refresh();
        }

        public override void OnGUI(string searchContext)
        {
            if (_settings == null)
            {
                _settings = ScriptableObject.CreateInstance<RecentAssetsSettings>();
                _settings.BannedPatterns = GetBannedPatternsList();
                _settings.MaxRecentAssets = MaxRecentAssets;
                _serializedObject = new SerializedObject(_settings);
            }

            EditorGUI.BeginChangeCheck();

            var it = _serializedObject.GetIterator();
            it.NextVisible(true);
            while (it.NextVisible(false))
                EditorGUILayout.PropertyField(it, true);

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
                SetList(BannedPatternsKey, _settings.BannedPatterns);
                MaxRecentAssets = _settings.MaxRecentAssets;
            }
        }

        public static List<string> GetBannedPatternsList()
        {
            return GetList(BannedPatternsKey);
        }

        private static void SetList(string key, List<string> value)
        {
            var json = JsonUtility.ToJson(new WrappedList<string> { List = value });
            EditorPrefs.SetString($"{Prefix}{key}", json);
        }

        private static List<string> GetList(string key)
        {
            var json = EditorPrefs.GetString($"{Prefix}{key}", "{}");
            return JsonUtility.FromJson<WrappedList<string>>(json).List;
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettings()
        {
            return new RecentAssetsPreferences("Preferences/Recent Assets", SettingsScope.User);
        }
    }
}