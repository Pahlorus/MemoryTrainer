namespace Utility.MatrixData
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.Callbacks;
#endif

    [Serializable]
    public class ListWrapper<T> : IList<T>
    {
        [SerializeField] private List<T> _values;
        #region IList
        public T this[int index] { get => ((IList<T>)_values)[index]; set => ((IList<T>)_values)[index] = value; }

        public int Count => ((IList<T>)_values).Count;

        public bool IsReadOnly => ((IList<T>)_values).IsReadOnly;

        public void Add(T item)
        {
            ((IList<T>)_values).Add(item);
        }

        public void Clear()
        {
            ((IList<T>)_values).Clear();
        }

        public bool Contains(T item)
        {
            return ((IList<T>)_values).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((IList<T>)_values).CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IList<T>)_values).GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return ((IList<T>)_values).IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ((IList<T>)_values).Insert(index, item);
        }

        public bool Remove(T item)
        {
            return ((IList<T>)_values).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<T>)_values).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<T>)_values).GetEnumerator();
        }
        #endregion IList
    }

    public abstract class MatrixData : ScriptableObject
    {
#if UNITY_EDITOR
        public class MatrixDataWindow : EditorWindow
        {
            [OnOpenAsset(1)]
            public static bool Open(int instanceID, int line)
            {
                var data = EditorUtility.InstanceIDToObject(instanceID) as MatrixData;
                if (data != null)
                {
                    var window = GetWindow<MatrixDataWindow>(true, data.name, true);
                    window.Init(data);
                    return true;
                }
                else
                    return false;
            }

            private const string CellSizePropertyPath = "_cellSize";
            private const string KeysXPropertyPath = "_keysX";
            private const string KeysYPropertyPath = "_keysY";
            private const string ValuesPropertyPath = "_values";

            private MatrixData _matrixData;
            private SerializedObject _serializedObject;

            private SerializedProperty _cellSizeProp;
            private SerializedProperty _keysXProp;
            private SerializedProperty _keysYProp;
            private SerializedProperty _valuesProp;

            private bool _columnAdded = false;
            private bool _rowAdded = false;

            private int _columnRemoved = -1;
            private int _rowRemoved = -1;

            private Vector2 _scroll;
            private Vector2 _cellSize;

            private bool _isError;

            public void Init(MatrixData matrixData)
            {
                _matrixData = matrixData;
                _serializedObject = Editor.CreateEditor(_matrixData).serializedObject;

                _cellSizeProp = _serializedObject.FindProperty(CellSizePropertyPath);
                _keysXProp = _serializedObject.FindProperty(KeysXPropertyPath);
                _keysYProp = _serializedObject.FindProperty(KeysYPropertyPath);
                _valuesProp = _serializedObject.FindProperty(ValuesPropertyPath);

                _isError = !_matrixData.InitLookups();
            }

            private void OnGUI()
            {
                _serializedObject.Update();

                if (_isError)
                    EditorGUI.DrawRect(new Rect(Vector2.zero, maxSize), Color.red);

                EnsureSize();

                var xSize = _keysXProp.arraySize;
                var ySize = _keysYProp.arraySize;

                EditorGUILayout.PropertyField(_cellSizeProp);
                _cellSize = _cellSizeProp.vector2Value;

                GUIStyle tableStyle = new GUIStyle();
                tableStyle.stretchWidth = false;

                GUIStyle cellStyle = new GUIStyle();
                cellStyle.fixedWidth = _cellSize.x;
                cellStyle.fixedHeight = _cellSize.y;
                cellStyle.stretchWidth = false;

                GUIStyle columnHeaderStyle = new GUIStyle();
                columnHeaderStyle.fixedWidth = _cellSize.x;
                columnHeaderStyle.fixedHeight = _cellSize.y;
                columnHeaderStyle.stretchWidth = false;

                GUIStyle rowHeaderStyle = new GUIStyle();
                rowHeaderStyle.fixedWidth = _cellSize.x;
                rowHeaderStyle.fixedHeight = _cellSize.y;
                rowHeaderStyle.stretchWidth = false;

                GUIStyle cornerStyle = new GUIStyle();
                cornerStyle.fixedWidth = _cellSize.x;
                cornerStyle.fixedHeight = _cellSize.y;
                cornerStyle.stretchWidth = false;

                _scroll = EditorGUILayout.BeginScrollView(_scroll, tableStyle);
                EditorGUILayout.BeginHorizontal(tableStyle);
                for (int x = -2; x < xSize; x++)
                {
                    EditorGUILayout.BeginVertical(tableStyle);
                    for (int y = -2; y < ySize; y++)
                    {
                        GUIStyle style;
                        if (y == -1 && x == -1)
                            style = cornerStyle;
                        else if (x == -1)
                            style = rowHeaderStyle;
                        else if (y == -1)
                            style = columnHeaderStyle;
                        else
                            style = cellStyle;
                        EditorGUILayout.BeginVertical(style);
                        if (y == -2 && x == -2)
                            Empty();
                        else if (y == -2 && x == -1)
                            AddColumnHandler();
                        else if (y == -1 && x == -2)
                            AddRowHandler();
                        else if (y == -1 && x == -1)
                            Empty();
                        else if (y == -2)
                            RemoveColumnHandler(x);
                        else if (x == -2)
                            RemoveRowHandler(y);
                        else if (y == -1)
                            ColumnHeaderHandler(x);
                        else if (x == -1)
                            RowHeaderHandler(y);
                        else
                            ValueHandler(x, y);
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();

                HandleDelayedOps();

                if (_serializedObject.ApplyModifiedProperties())
                {
                    _isError = !_matrixData.InitLookups();
                }
            }

            private void EnsureSize()
            {
                var xSize = _keysXProp.arraySize;
                var ySize = _keysYProp.arraySize;

                if (_valuesProp.arraySize != ySize)
                {
                    Debug.LogError($"{_matrixData.name} have wrong data Y size", _matrixData);
                    _valuesProp.arraySize = ySize;
                }
                for (int y = 0; y < ySize; y++)
                {
                    var prop = _valuesProp.GetArrayElementAtIndex(y).FindPropertyRelative(ValuesPropertyPath);
                    if (prop.arraySize != xSize)
                    {
                        Debug.LogError($"{_matrixData.name} have wrong data X size", _matrixData);
                        prop.arraySize = ySize;
                    }

                }
            }

            private void HandleDelayedOps()
            {
                var xSize = _keysXProp.arraySize;
                var ySize = _keysYProp.arraySize;
                if (_columnAdded)
                {
                    _keysXProp.InsertArrayElementAtIndex(xSize);
                    for (int y = 0; y < ySize; y++)
                        _valuesProp.GetArrayElementAtIndex(y).FindPropertyRelative(ValuesPropertyPath).InsertArrayElementAtIndex(xSize);

                    _columnAdded = false;
                    return;
                }
                if (_rowAdded)
                {
                    _keysYProp.InsertArrayElementAtIndex(ySize);
                    _valuesProp.InsertArrayElementAtIndex(ySize);
                    _valuesProp.GetArrayElementAtIndex(ySize).FindPropertyRelative(ValuesPropertyPath).arraySize = xSize;

                    _rowAdded = false;
                    return;
                }
                if (_columnRemoved >= 0)
                {
                    _keysXProp.DeleteArrayElementAtIndexExplicit(_columnRemoved);
                    for (int y = 0; y < ySize; y++)
                        _valuesProp.GetArrayElementAtIndex(y).FindPropertyRelative(ValuesPropertyPath).DeleteArrayElementAtIndexExplicit(_columnRemoved);

                    _columnRemoved = -1;
                    return;
                }
                if (_rowRemoved >= 0)
                {
                    _keysYProp.DeleteArrayElementAtIndexExplicit(_rowRemoved);
                    _valuesProp.DeleteArrayElementAtIndexExplicit(_rowRemoved);

                    _rowRemoved = -1;
                    return;
                }
            }

            private void AddColumnHandler()
            {
                if (GUILayout.Button("+"))
                    _columnAdded = true;
            }

            private void RemoveColumnHandler(int x)
            {
                if (GUILayout.Button("-"))
                    _columnRemoved = x;
            }

            private void AddRowHandler()
            {
                if (GUILayout.Button("+"))
                    _rowAdded = true;
            }

            private void RemoveRowHandler(int y)
            {
                if (GUILayout.Button("-"))
                    _rowRemoved = y;
            }

            private void Empty()
            {
                EditorGUILayout.LabelField(" ", GUI.skin.label);
            }

            private void ColumnHeaderHandler(int x)
            {
                var color = SetColor(Color.gray);
                EditorGUILayout.PropertyField(_keysXProp.GetArrayElementAtIndex(x), GUIContent.none);
                SetColor(color);
            }

            private void RowHeaderHandler(int y)
            {
                var color = SetColor(Color.gray);
                EditorGUILayout.PropertyField(_keysYProp.GetArrayElementAtIndex(y), GUIContent.none);
                SetColor(color);
            }

            private void ValueHandler(int x, int y)
            {
                var num = 1 - (y % 2) * 0.25f;
                var color = SetColor(new Color(num, num, num));
                EditorGUILayout.PropertyField(_valuesProp.GetArrayElementAtIndex(y).FindPropertyRelative(ValuesPropertyPath).GetArrayElementAtIndex(x), GUIContent.none);
                SetColor(color);
            }

            private Color SetColor(Color color)
            {
                var originalColor = GUI.color;
                GUI.color = color;
                return originalColor;
            }
        }


        [CustomEditor(typeof(MatrixData), true)]
        public class MatrixDataEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                EditorGUILayout.HelpBox("Use open button", MessageType.Info);
            }
        }
#endif

        private void OnEnable()
        {
            InitLookups();
        }

        protected abstract bool InitLookups();

        [SerializeField] protected Vector2 _cellSize = new Vector3(60, 25);
    }

    [Serializable]
    public class MatrixData<TX, TY, TValList, TVal> : MatrixData where TValList : ListWrapper<TVal>
    {
        [SerializeField] private List<TX> _keysX = new List<TX>();
        [SerializeField] private List<TY> _keysY = new List<TY>();

        [SerializeField] private List<TValList> _values;

        private Dictionary<TX, int> _lookUpX = new Dictionary<TX, int>();
        private Dictionary<TY, int> _lookUpY = new Dictionary<TY, int>();

        protected override bool InitLookups()
        {
            var inited = true;
            inited &= InitLookup(_keysX, _lookUpX);
            inited &= InitLookup(_keysY, _lookUpY);
            return inited;
        }

        private bool InitLookup<T>(List<T> keys, Dictionary<T, int> lookup)
        {
            try
            {
                lookup.Clear();
                for (int i = 0; i < keys.Count; i++)
                    lookup.Add(keys[i], i);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                return false;
            }

        }

        public bool TryGet(TX keyX, TY keyY, out TVal val)
        {
            if (TryFind(keyX, keyY, out var x, out var y))
            {
                val = Get(x, y);
                return true;
            }
            else
            {
                val = default;
                return false;
            }
        }

        private bool TryFind(TX keyX, TY keyY, out int x, out int y)
        {
            if (_lookUpX.TryGetValue(keyX, out x) && _lookUpY.TryGetValue(keyY, out y))
                return true;
            else
            {
                x = default;
                y = default;
                return false;
            }
        }

        private TVal Get(int x, int y)
        {
            return _values[y][x];
        }

        /*
        private void Set(int x, int y, TVal val)
        {
            _values[y][x] = val;
        }
        */
    }

}