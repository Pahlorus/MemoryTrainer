#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;

using EditorGUITable;

using UnityEngine;


namespace Utility
{
    [Serializable]
    public struct FormatEntry
    {
        public string Key;
        public string Prefix;
        public string Format;
        public string Suffix;
    }

    [CreateAssetMenu(fileName = nameof(Formatter), menuName = "ScriptableObjects/Utility/" + nameof(Formatter))]
    public class Formatter : WindowedScriptableObject
    {

#if UNITY_EDITOR
        [CustomEditor(typeof(Formatter))]
        public class FormatterEditor : Editor
        {
            private string _testFormat;

            private string _testString;
            private float _testFloat;
            private int _testInt;

            private string _lastError;

            public override void OnInspectorGUI()
            {
                var formatter = target as Formatter;
                if (DrawDefaultInspector())
                {
                    formatter._version++;
                }
                if (formatter._version != formatter._buildVersion)
                {
                    if (formatter._autoRebuild || GUILayout.Button("Rebuild"))
                        formatter.BuildFormatter();
                }
                else
                {
                    //TEST
                    EditorGUILayout.LabelField("Test", EditorStyles.boldLabel);
                    _testFormat = EditorGUILayout.TextArea(_testFormat);
                    _testString = EditorGUILayout.TextField("Test String 0", _testString);
                    _testFloat = EditorGUILayout.FloatField("Test Float 1", _testFloat);
                    _testInt = EditorGUILayout.IntField("Test Int 2", _testInt);
                    if (!string.IsNullOrEmpty(_testFormat))
                    {
                        try
                        {
                            var style = new GUIStyle(EditorStyles.helpBox);
                            style.richText = true;
                            style.fontSize = 14;
                            EditorGUILayout.TextArea(string.Format(formatter.GetFormatter(), _testFormat, _testString, _testFloat, _testInt), style);
                            _lastError = null;
                        }
                        catch (Exception e)
                        {
                            _lastError = e.Message;
                        }
                    }
                    if (!string.IsNullOrEmpty(_lastError))
                        EditorGUILayout.HelpBox(_lastError, MessageType.Error);

                }
            }
        }
#endif
        [SerializeField, HideInInspector] private int _version;
        [SerializeField] private bool _autoRebuild;
        [ReorderableTable(key = nameof(Formatter))]
        [SerializeField] private List<FormatEntry> _formatEntries;
        [SerializeField] private CustomFormatter _customFormatter;

        private int _buildVersion;

        public CustomFormatter GetFormatter()
        {
            if (_customFormatter == null)
            {
                BuildFormatter();
            }
            return _customFormatter;
        }

        private void BuildFormatter()
        {
            _customFormatter = new CustomFormatter();
            var rules = new Dictionary<string, FormatEntry>(_formatEntries.Count);
            foreach (var entry in _formatEntries)
            {
                rules[entry.Key] = entry;
            }
            _customFormatter.FormattingRules = rules;
            _buildVersion = _version;
        }
    }
}