namespace Utility.Attributes
{
    using UnityEngine;

#if UNITY_EDITOR
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using System.IO;
    using Cysharp.Threading.Tasks;
#endif

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class RequireAttribute : MultiPropertyAttribute
    {
        private Color _color;

#if UNITY_EDITOR
        internal override void OnPreGUI(ref Rect position, SerializedProperty property, ref GUIContent label)
        {
            _color = GUI.color;
            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
                GUI.color = Color.red;
        }

        internal override void OnPostGUI(Rect position, SerializedProperty property)
        {
            GUI.color = _color;
        }
#endif
    }

#if UNITY_EDITOR
    //[CustomPropertyDrawer(typeof(RequireAttribute))]
    public static class RequireAttributeDrawer/* : PropertyDrawer*/
    {
        [System.Flags]
        private enum ValidateTargets
        {
            Scenes = 1,
            Prefabs = 2,
            ScriptableObjects = 4,
            All = Scenes | Prefabs | ScriptableObjects,
        }

        private const BindingFlags _FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        private static float _minRange;
        private static float _maxRange;

        private static bool _canceled;
        private static float _waitInterval = 1f / 60f;
        private static int _errorCount;

        private static string _prefix;
        private static string _progressText;
        private static float _progress;

        private static ValidateTargets _validateTargets;

        /*
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var color = GUI.color;
            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
                GUI.color = Color.red;
            EditorGUI.PropertyField(position, property, label);
            GUI.color = color;
        }
        */

        [MenuItem("Assets/Validate Requred Fields/All")]
        private static void CheckRequiredAll()
        {
            _validateTargets = ValidateTargets.All;
            RunValidation();
        }
        [MenuItem("Assets/Validate Requred Fields/Scenes")]
        private static void CheckRequiredScenes()
        {
            _validateTargets = ValidateTargets.Scenes;
            RunValidation();
        }
        [MenuItem("Assets/Validate Requred Fields/Prefabs")]
        private static void CheckRequiredPrefabs()
        {
            _validateTargets = ValidateTargets.Prefabs;
            RunValidation();
        }
        [MenuItem("Assets/Validate Requred Fields/ScriptableObjects")]
        private static void CheckRequiredScriptableObjects()
        {
            _validateTargets = ValidateTargets.ScriptableObjects;
            RunValidation();
        }

        [MenuItem("Assets/Validate Requred Fields/Cancel")]
        private static void CancelValidation()
        {
            _canceled = true;
            EditorUtility.ClearProgressBar();
        }

        private static void RunValidation()
        {
            _canceled = false;
            ReportProgressAsync().Forget();
            ValidateAsync().Forget();
        }

        private static int Index(ValidateTargets validateTargets)
        {
            switch (validateTargets)
            {
                case ValidateTargets.Scenes: return 0;
                case ValidateTargets.Prefabs: return 1;
                case ValidateTargets.ScriptableObjects: return 2;
                default: return -1;
            }
        }

        private static int CountFlags(ValidateTargets validateTargets, int from = 0, int to = 32)
        {
            var value = (int)validateTargets;
            int count = 0;
            int mask = 1;
            for (int i =0; i<32;i++)
            {
                if ((value & mask) != 0 && from <= i && i <= to)
                        count++;
                mask <<= 1;
            }
            return count;
        }

        private static async UniTaskVoid ReportProgressAsync()
        {
            _minRange = 0;
            _maxRange = 0;
            _progress = 0;
            _prefix = null;
            _progressText = null;

            var time = EditorApplication.timeSinceStartup;

            while (!_canceled)
            {
                _canceled = EditorUtility.DisplayCancelableProgressBar("Validating", $"{_prefix} {_progressText}", Mathf.Lerp(_minRange, _maxRange, _progress));

                var newTime = EditorApplication.timeSinceStartup;
                if (newTime > time + _waitInterval)
                {
                    time = newTime;
                    await UniTask.Yield();
                }
            }
            EditorUtility.ClearProgressBar();
        }

        private static async UniTaskVoid ValidateAsync()
        {
            _errorCount = 0;

            var targets = CountFlags(_validateTargets);

            if (_validateTargets.HasFlag(ValidateTargets.Scenes))
            {
                if (_canceled)
                    return;

                var (min, max) = SetMinMax(ValidateTargets.Scenes, targets);

                var sceneCount = EditorSceneManager.sceneCount;
                for (int i = 0; i < sceneCount; i++)
                {
                    var scene = EditorSceneManager.GetSceneAt(i);
                    _prefix = $"Scene {scene.name}";
                    if (scene.IsValid() && scene.isLoaded)
                    {
                        if (_canceled)
                            return;

                        var rootObjects = scene.GetRootGameObjects();
                        var length = rootObjects.Length;
                        for (int j = 0; j < length; j++)
                        {
                            if (_canceled)
                                return;

                            var sceneMin = Mathf.Lerp(min, max, (i + 0f) / sceneCount);
                            var sceneMax = Mathf.Lerp(min, max, (i + 1f) / sceneCount);

                            _minRange = Mathf.Lerp(sceneMin, sceneMax, (j + 0f) / length);
                            _maxRange = Mathf.Lerp(sceneMin, sceneMax, (j + 1f) / length);
                            _progress = 0;

                            GameObject obj = rootObjects[j];
                            var text = $"{j}/{length}";
                            var childs = obj.GetComponentsInChildren<Component>(includeInactive: true);
                            await CheckObjectsAsync(ToEnumerable(childs, text));
                        }
                    }
                }
            }
            if (_validateTargets.HasFlag(ValidateTargets.Prefabs))
            {
                if (_canceled)
                    return;

                (_minRange, _maxRange) = SetMinMax(ValidateTargets.Prefabs, targets);
                _prefix = "Prefab";

                await CheckObjectsAsync(GetAllInstances("Prefab"));
            }
            if (_validateTargets.HasFlag(ValidateTargets.ScriptableObjects))
            {
                if (_canceled)
                    return;

                (_minRange, _maxRange) = SetMinMax(ValidateTargets.ScriptableObjects, targets);
                _prefix = "ScriptableObject";

                await CheckObjectsAsync(GetAllInstances("ScriptableObject"));
            }

            _canceled = true;
            Debug.Log($"Validation done. ErrorCount: {_errorCount}");

            #region Helpers

            (float min, float max) SetMinMax(ValidateTargets current, int targets_)
            {
                var index = Index(current);
                var from = CountFlags(_validateTargets, to: index - 1);
                var to = CountFlags(_validateTargets, from: index + 1);

                var min = (from + 0f) / targets_;
                var max = 1 - (to + 0f) / targets_;

                return (min, max);
            }

            void DisplayProgress(string text, int index, int length)
            {
                _progressText = text;
                _progress = index / (length - 1f);
            }

            IEnumerable<Object> GetAllInstances(string typeName)
            {
                var guids = AssetDatabase.FindAssets("t:" + typeName);
                int length = guids.Length;
                for (int i = 0; i < length; i++)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    var objects = AssetDatabase.LoadAllAssetsAtPath(path);
                    DisplayProgress($"{i}/{length} {Path.GetFileNameWithoutExtension(path)}", i, length);
                    foreach (var obj in objects)
                        yield return obj;
                }
            }

            IEnumerable<Object> ToEnumerable(Object[] objects, string text)
            {
                if (objects == null)
                    yield break;

                var length = objects.Length;
                for (int i = 0; i < length; i++)
                {
                    var obj = objects[i];
                    DisplayProgress($"{text} {i}/{length} {obj}", i, length);
                    yield return obj;
                }
            }

            async UniTask CheckObjectsAsync(IEnumerable<Object> objects)
            {
                var time = EditorApplication.timeSinceStartup;
                Object last = null;

                foreach (var obj in objects)
                {
                    if (obj == null)
                    {
                        Debug.LogError($"Missing behaviour near {last}", last);
                        continue;
                    }

                    var type = obj.GetType();
                    CheckFields(obj, obj, type);
                    if (_canceled)
                        return;

                    var newTime = EditorApplication.timeSinceStartup;
                    if (newTime > time + _waitInterval)
                    {
                        time = newTime;
                        await UniTask.Yield();
                    }

                    last = obj;
                }
            }

            void CheckFields(Object parentObject, object obj, System.Type type)
            {
                var fields = GetAllFields(type);
                foreach (var field in fields)
                    CheckField(parentObject, obj, field);
            }

            IEnumerable<FieldInfo> GetAllFields(System.Type type)
            {
                if (type == null)
                    return Enumerable.Empty<FieldInfo>();
                else
                    return type.GetFields(_FLAGS).Concat(GetAllFields(type.BaseType));
            }

            void CheckField(Object parentObject, object obj, FieldInfo fieldInfo)
            {
                var fieldType = fieldInfo.FieldType;
                if (fieldType.IsValueType && !fieldType.IsPrimitive)
                {
                    var fieldObj = fieldInfo.GetValue(obj);
                    CheckFields(parentObject, fieldObj, fieldType);
                }
                else if (fieldType.IsClass)
                    CheckClass(parentObject, obj, fieldInfo);
            }

            void CheckClass(Object parentObject, object obj, FieldInfo field)
            {
                var requierdAttribute = field.GetCustomAttribute<RequireAttribute>();
                if (requierdAttribute != null)
                {
                    var value = field.GetValue(obj);
                    if (value == null)
                    {
                        if (EditorUtility.IsPersistent(parentObject) && parentObject is Component component)
                            parentObject = component.transform.root.gameObject;

                        _errorCount++;
                        Debug.LogError($"Required propery <b>{ObjectNames.NicifyVariableName(field.Name)}</b> of <b>{parentObject.name}</b> not set", parentObject);
                    }
                }
            }

            #endregion
        }
    }
#endif
}