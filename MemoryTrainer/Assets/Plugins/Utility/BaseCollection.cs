#if UNITY_EDITOR
using UnityEditor;
using EditorGUITable;
#endif

using System.Collections.Generic;

using UnityEngine;
using Utility.Attributes;

namespace Utility
{
    public abstract class BaseCollection : WindowedScriptableObject
    {
        /// <summary>
        /// Do not use directly
        /// </summary>
        [HideInInspector] public int Version;

        public abstract IReadOnlyList<ScriptableObject> GetItems();

#if UNITY_EDITOR
        [CustomEditor(typeof(BaseCollection), editorForChildClasses: true)]
        public class CollectionEditor : Editor
        {
            private SerializedProperty _versionProp;

            private void OnEnable()
            {
                _versionProp = serializedObject.FindProperty(nameof(Version));
            }

            public override void OnInspectorGUI()
            {
                if (DrawDefaultInspector())
                {
                    var value = _versionProp.intValue;
                    unchecked { value++; }
                    _versionProp.intValue = value;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
#endif
    }

    public abstract class Collection<T> : BaseCollection where T : ScriptableObject
    {
        public List<T> Items;

        public override IReadOnlyList<ScriptableObject> GetItems()
        {
            return Items;
        }
    }

    public abstract partial class PerCollectionData : WindowedScriptableObject
    {
        /// <summary>
        /// Do not use directly
        /// </summary>
        [HideInInspector] public int Version;
        /// <summary>
        /// Do not use directly
        /// </summary>
        [Require] public BaseCollection Collection;
        /// <summary>
        /// Do not use directly
        /// </summary>
        public List<ScriptableObject> Keys;

        protected virtual void OnEnable()
        {
            if (Collection != null && Version != Collection.Version)
                Debug.LogError("Version mismatch", this);
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(PerCollectionData), editorForChildClasses: true)]
        public class PerCollectionDataEditor : Editor
        {
            private SerializedProperty _versionProp;
            private SerializedProperty _collectionProp;
            private SerializedProperty _keysProp;
            private SerializedProperty _valuesProp;

            private SelectorColumn _itemTitleSelector;

            private GUITableState _tableState;

            private void OnEnable()
            {
                _versionProp = serializedObject.FindProperty(nameof(Version));
                _collectionProp = serializedObject.FindProperty(nameof(Collection));
                _keysProp = serializedObject.FindProperty(nameof(Keys));
                _valuesProp = serializedObject.FindProperty(nameof(PerCollectionData<ScriptableObject>.Values));

                _tableState = new GUITableState($"{target.name}");
                _itemTitleSelector = new SelectFromFunctionColumn((p, i) => new PropertyCell(_keysProp.GetArrayElementAtIndex(i)), TableColumn.Title("Item"), TableColumn.EnabledCells(false));
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                UpdateRepresentation();
                EditorGUILayout.PropertyField(_collectionProp);
                EditorGUILayout.Space();
                DrawItems();
                serializedObject.ApplyModifiedProperties();
            }

            private void UpdateRepresentation()
            {
                var perColData = target as PerCollectionData;
                var collection = perColData.Collection;
                if (collection != null && perColData.Version != collection.Version)
                {
                    var copy = new SerializedObject(target);
                    var copyKeys = copy.FindProperty(nameof(Keys));
                    var copyValues = copy.FindProperty(nameof(PerCollectionData<ScriptableObject>.Values));

                    var dict = new Dictionary<Object, SerializedProperty>();
                    var count = Mathf.Min(copyKeys.arraySize, _valuesProp.arraySize);
                    for (int i = 0; i < count; i++)
                        dict[copyKeys.GetArrayElementAtIndex(i).objectReferenceValue] = copyValues.GetArrayElementAtIndex(i).Copy();

                    _keysProp.ClearArray();
                    _valuesProp.ClearArray();

                    var items = collection.GetItems();
                    count = items.Count;
                    var added = 0;
                    for (int i = 0; i < count; i++)
                    {
                        var item = items[i];
                        if (item != null)
                        {
                            _keysProp.InsertArrayElementAtIndex(added);
                            _keysProp.GetArrayElementAtIndex(added).objectReferenceValue = item;
                            _valuesProp.InsertArrayElementAtIndex(added);
                            var serializedItem = _valuesProp.GetArrayElementAtIndex(added);
                            if (dict.TryGetValue(item, out var itemProp))
                                serializedItem.serializedObject.CopyFromSerializedProperty(itemProp);
                            added++;
                        }
                    }

                    _versionProp.intValue = collection.Version;
                    serializedObject.ApplyModifiedPropertiesWithoutUndo();
                }
            }

            private void DrawItems()
            {
                var props = SerializationHelpers.GetElementsSerializedFields(_valuesProp);
                var selectors = ParsingHelpers.ParseColumns(props);
                selectors.Insert(0, _itemTitleSelector);
                _tableState = GUITableLayout.DrawTable(_tableState, _valuesProp, selectors);
            }
        }
#endif
    }

    public abstract class PerCollectionData<T> : PerCollectionData where T : new()
    {
        /// <summary>
        /// Do not use directly
        /// </summary>
        public List<T> Values;

        [System.NonSerialized] protected bool _inited;
        [System.NonSerialized] protected int _initedVersion;
        [System.NonSerialized] protected Dictionary<ScriptableObject, int> _hash;

        protected void Init()
        {
            if (!_inited || _initedVersion != Version)
            {
                var count = (Values == null || Keys == null) ? 0 : Mathf.Min(Values.Count, Keys.Count);

                if (_hash == null)
                    _hash = new Dictionary<ScriptableObject, int>();
                else
                    _hash.Clear();

                for (int i = 0; i < count; i++)
                    _hash.Add(Keys[i], i);

                _initedVersion = Version;
                _inited = true;
            }
        }

        public bool TryGet(ScriptableObject key, out T value)
        {
            Init();
            var count = Values?.Count ?? 0;
            if (_hash.TryGetValue(key, out var index) && 0<= index && index < count)
            {
                value = Values[index];
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
    }

    public abstract class PerCollectionData<K,V> : PerCollectionData<V> where V : new() where K : ScriptableObject
    {
        public bool TryGet(K key, out V value)
        {
            Init();
            var count = Values?.Count ?? 0;
            if (_hash.TryGetValue(key, out var index) && 0 <= index && index < count)
            {
                value = Values[index];
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
    }
}