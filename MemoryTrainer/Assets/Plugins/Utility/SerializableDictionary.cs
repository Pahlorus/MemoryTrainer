namespace Utility
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;

    [CustomPropertyDrawer(typeof(DrawableDictionary), true)]
    public class DictionaryPropertyDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                var keysProp = property.FindPropertyRelative("_keys");
                return (keysProp.arraySize + 2) * (EditorGUIUtility.singleLineHeight + 1f);
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool expanded = property.isExpanded;
            var r = GetNextRect(ref position);
            property.isExpanded = EditorGUI.Foldout(r, property.isExpanded, label);

            if (expanded)
            {
                int lvl = EditorGUI.indentLevel;
                EditorGUI.indentLevel = lvl + 1;

                var keysProp = property.FindPropertyRelative("_keys");
                var valuesProp = property.FindPropertyRelative("_values");

                int cnt = keysProp.arraySize;
                if (valuesProp.arraySize != cnt) valuesProp.arraySize = cnt;

                for (int i = 0; i < cnt; i++)
                {
                    r = GetNextRect(ref position);
                    //r = EditorGUI.IndentedRect(r);
                    var w0 = EditorGUIUtility.labelWidth; // r.width / 2f;
                    var w1 = r.width - w0;
                    var r0 = new Rect(r.xMin, r.yMin, w0, r.height);
                    var r1 = new Rect(r0.xMax, r.yMin, w1, r.height);

                    var keyProp = keysProp.GetArrayElementAtIndex(i);
                    var valueProp = valuesProp.GetArrayElementAtIndex(i);

                    DrawKey(r0, keyProp);
                    DrawValue(r1, valueProp);
                }

                EditorGUI.indentLevel = lvl;

                r = GetNextRect(ref position);
                var pRect = new Rect(r.xMax - 60f, r.yMin, 30f, EditorGUIUtility.singleLineHeight);
                var mRect = new Rect(r.xMax - 30f, r.yMin, 30f, EditorGUIUtility.singleLineHeight);

                if (GUI.Button(pRect, "+"))
                {
                    AddKeyElement(keysProp);
                    valuesProp.arraySize = keysProp.arraySize;
                }
                if (GUI.Button(mRect, "-"))
                {
                    keysProp.arraySize = Mathf.Max(keysProp.arraySize - 1, 0);
                    valuesProp.arraySize = keysProp.arraySize;
                }
            }
        }

        protected virtual void DrawKey(Rect area, SerializedProperty keyProp)
        {
            EditorGUI.PropertyField(area, keyProp, GUIContent.none, false);
        }

        protected virtual void DrawValue(Rect area, SerializedProperty valueProp)
        {
            EditorGUI.PropertyField(area, valueProp, GUIContent.none, false);
        }




        private Rect GetNextRect(ref Rect position)
        {
            var r = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            var h = EditorGUIUtility.singleLineHeight + 1f;
            position = new Rect(position.xMin, position.yMin + h, position.width, position.height = h);
            return r;
        }


        private static void AddKeyElement(SerializedProperty keysProp)
        {
            keysProp.arraySize++;
            var prop = keysProp.GetArrayElementAtIndex(keysProp.arraySize - 1);

            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                {
                    int value = 0;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).intValue == value)
                        {
                            value++;
                            if (value == int.MaxValue)
                                break;
                            else
                                i = -1;
                        }
                    }
                    prop.intValue = value;
                }
                break;
                case SerializedPropertyType.Boolean:
                {
                    bool value = false;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).boolValue == value)
                        {
                            value = true;
                            break;
                        }
                    }
                    prop.boolValue = value;
                }
                break;
                case SerializedPropertyType.Float:
                {
                    float value = 0f;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).intValue == value)
                        {
                            value++;
                            if (value == int.MaxValue)
                                break;
                            else
                                i = -1;
                        }
                    }
                    prop.floatValue = value;
                }
                break;
                case SerializedPropertyType.String:
                {
                    prop.stringValue = string.Empty;
                }
                break;
                case SerializedPropertyType.Color:
                {
                    Color value = Color.black;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).colorValue == value)
                        {
                            value = ToColor(ToInt(value) + 1);
                            if (value == Color.white)
                                break;
                            else
                                i = -1;
                        }
                    }
                    prop.colorValue = value;
                }
                break;
                case SerializedPropertyType.ObjectReference:
                {
                    prop.objectReferenceValue = null;
                }
                break;
                case SerializedPropertyType.LayerMask:
                {
                    int value = -1;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).intValue == value)
                        {
                            value++;
                            if (value == int.MaxValue)
                                break;
                            else
                                i = -1;
                        }
                    }
                    prop.intValue = value;
                }
                break;
                case SerializedPropertyType.Enum:
                {
                    int value = 0;
                    if (keysProp.arraySize > 1)
                    {
                        var first = keysProp.GetArrayElementAtIndex(0);
                        int max = first.enumNames.Length - 1;

                        for (int i = 0; i < keysProp.arraySize - 1; i++)
                        {
                            if (keysProp.GetArrayElementAtIndex(i).enumValueIndex == value)
                            {
                                value++;
                                if (value >= max)
                                    break;
                                else
                                    i = -1;
                            }
                        }
                    }
                    prop.enumValueIndex = value;
                }
                break;
                case SerializedPropertyType.Vector2:
                {
                    Vector2 value = Vector2.zero;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).vector2Value == value)
                        {
                            value.x++;
                            if (value.x == int.MaxValue)
                                break;
                            else
                                i = -1;
                        }
                    }
                    prop.vector2Value = value;
                }
                break;
                case SerializedPropertyType.Vector2Int:
                {
                    Vector2Int value = Vector2Int.zero;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).vector2IntValue == value)
                        {
                            value.x++;
                            if (value.x == int.MaxValue)
                                break;
                            else
                                i = -1;
                        }
                    }
                    prop.vector2IntValue = value;
                }
                break;
                case SerializedPropertyType.Vector3:
                {
                    Vector3 value = Vector3.zero;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).vector3Value == value)
                        {
                            value.x++;
                            if (value.x == int.MaxValue)
                                break;
                            else
                                i = -1;
                        }
                    }
                    prop.vector3Value = value;
                }
                break;
                case SerializedPropertyType.Vector3Int:
                {
                    Vector3Int value = Vector3Int.zero;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).vector3IntValue == value)
                        {
                            value.x++;
                            if (value.x == int.MaxValue)
                                break;
                            else
                                i = -1;
                        }
                    }
                    prop.vector3IntValue = value;
                }
                break;
                case SerializedPropertyType.Vector4:
                {
                    Vector4 value = Vector4.zero;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).vector4Value == value)
                        {
                            value.x++;
                            if (value.x == int.MaxValue)
                                break;
                            else
                                i = -1;
                        }
                    }
                    prop.vector4Value = value;
                }
                break;
                case SerializedPropertyType.Rect:
                {
                    prop.rectValue = Rect.zero;
                }
                break;
                case SerializedPropertyType.ArraySize:
                {
                    int value = 0;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).arraySize == value)
                        {
                            value++;
                            if (value == int.MaxValue)
                                break;
                            else
                                i = -1;
                        }
                    }
                    prop.arraySize = value;
                }
                break;
                case SerializedPropertyType.Character:
                {
                    int value = 0;
                    for (int i = 0; i < keysProp.arraySize - 1; i++)
                    {
                        if (keysProp.GetArrayElementAtIndex(i).intValue == value)
                        {
                            value++;
                            if (value == char.MaxValue)
                                break;
                            else
                                i = -1;
                        }
                    }
                    prop.intValue = value;
                }
                break;
                case SerializedPropertyType.AnimationCurve:
                {
                    prop.animationCurveValue = null;
                }
                break;
                case SerializedPropertyType.Bounds:
                {
                    prop.boundsValue = default;
                }
                break;
                default:
                    throw new System.InvalidOperationException("Can not handle Type as key.");
            }
        }

        private static void SetPropertyDefault(SerializedProperty prop)
        {
            if (prop == null) throw new System.ArgumentNullException("prop");

            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    prop.intValue = 0;
                    break;
                case SerializedPropertyType.Boolean:
                    prop.boolValue = false;
                    break;
                case SerializedPropertyType.Float:
                    prop.floatValue = 0f;
                    break;
                case SerializedPropertyType.String:
                    prop.stringValue = string.Empty;
                    break;
                case SerializedPropertyType.Color:
                    prop.colorValue = Color.black;
                    break;
                case SerializedPropertyType.ObjectReference:
                    prop.objectReferenceValue = null;
                    break;
                case SerializedPropertyType.LayerMask:
                    prop.intValue = -1;
                    break;
                case SerializedPropertyType.Enum:
                    prop.enumValueIndex = 0;
                    break;
                case SerializedPropertyType.Vector2:
                    prop.vector2Value = Vector2.zero;
                    break;
                case SerializedPropertyType.Vector3:
                    prop.vector3Value = Vector3.zero;
                    break;
                case SerializedPropertyType.Vector4:
                    prop.vector4Value = Vector4.zero;
                    break;
                case SerializedPropertyType.Rect:
                    prop.rectValue = Rect.zero;
                    break;
                case SerializedPropertyType.ArraySize:
                    prop.arraySize = 0;
                    break;
                case SerializedPropertyType.Character:
                    prop.intValue = 0;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    prop.animationCurveValue = null;
                    break;
                case SerializedPropertyType.Bounds:
                    prop.boundsValue = default;
                    break;
                case SerializedPropertyType.Gradient:
                    throw new System.InvalidOperationException("Can not handle Gradient types.");
            }
        }

        private static int ToInt(Color32 color)
        {
            return (color.a & 0xff) << 24 | (color.r & 0xff) << 16 | (color.g & 0xff) << 8 | (color.b & 0xff);
        }

        private static Color ToColor(int value)
        {
            var color = new Color32();
            color.a = (byte)((value >> 24) & 0xff);
            color.r = (byte)((value >> 16) & 0xff);
            color.g = (byte)((value >> 8) & 0xff);
            color.b = (byte)((value) & 0xff);
            return color;
        }
    }

#endif

    public abstract class DrawableDictionary
    {

    }

    [Serializable()]
    public class SerializableDictionaryBase<TKey, TValue> : DrawableDictionary, IDictionary<TKey, TValue>, UnityEngine.ISerializationCallbackReceiver
    {

        #region Fields

        [NonSerialized()]
        private Dictionary<TKey, TValue> _dict;
        [NonSerialized()]
        private IEqualityComparer<TKey> _comparer;

        #endregion

        #region CONSTRUCTOR

        public SerializableDictionaryBase()
        {

        }

        public SerializableDictionaryBase(IEqualityComparer<TKey> comparer)
        {
            _comparer = comparer;
        }

        #endregion

        #region Properties

        public IEqualityComparer<TKey> Comparer
        {
            get { return _comparer; }
        }

        #endregion

        #region IDictionary Interface

        public int Count
        {
            get { return (_dict != null) ? _dict.Count : 0; }
        }

        public void Add(TKey key, TValue value)
        {
            if (_dict == null) _dict = new Dictionary<TKey, TValue>(_comparer);
            _dict.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            if (_dict == null) return false;
            return _dict.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get
            {
                if (_dict == null) _dict = new Dictionary<TKey, TValue>(_comparer);
                return _dict.Keys;
            }
        }

        public bool Remove(TKey key)
        {
            if (_dict == null) return false;
            return _dict.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_dict == null)
            {
                value = default;
                return false;
            }
            return _dict.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get
            {
                if (_dict == null) _dict = new Dictionary<TKey, TValue>(_comparer);
                return _dict.Values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (_dict == null) throw new KeyNotFoundException();
                return _dict[key];
            }
            set
            {
                if (_dict == null) _dict = new Dictionary<TKey, TValue>(_comparer);
                _dict[key] = value;
            }
        }

        public void Clear()
        {
            if (_dict != null) _dict.Clear();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            if (_dict == null) _dict = new Dictionary<TKey, TValue>(_comparer);
            (_dict as ICollection<KeyValuePair<TKey, TValue>>).Add(item);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            if (_dict == null) return false;
            return (_dict as ICollection<KeyValuePair<TKey, TValue>>).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (_dict == null) return;
            (_dict as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (_dict == null) return false;
            return (_dict as ICollection<KeyValuePair<TKey, TValue>>).Remove(item);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            if (_dict == null) return default;
            return _dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_dict == null) return System.Linq.Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
            return _dict.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            if (_dict == null) return System.Linq.Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
            return _dict.GetEnumerator();
        }

        #endregion

        #region ISerializationCallbackReceiver

        [UnityEngine.SerializeField()]
        private TKey[] _keys;
        [UnityEngine.SerializeField()]
        private TValue[] _values;

        void UnityEngine.ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_keys != null && _values != null)
            {
                if (_dict == null) _dict = new Dictionary<TKey, TValue>(_keys.Length, _comparer);
                else _dict.Clear();
                for (int i = 0; i < _keys.Length; i++)
                {
                    if (i < _values.Length)
                        _dict[_keys[i]] = _values[i];
                    else
                        _dict[_keys[i]] = default;
                }
            }

            _keys = null;
            _values = null;
        }

        void UnityEngine.ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (_dict == null || _dict.Count == 0)
            {
                _keys = null;
                _values = null;
            }
            else
            {
                int cnt = _dict.Count;
                _keys = new TKey[cnt];
                _values = new TValue[cnt];
                int i = 0;
                var e = _dict.GetEnumerator();
                while (e.MoveNext())
                {
                    _keys[i] = e.Current.Key;
                    _values[i] = e.Current.Value;
                    i++;
                }
            }
        }

        #endregion

    }
}