#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Utility
{
    public static class UtilityExtensions
    {
        public static Vector2 GetResolution(this Texture texture)
        {
            return new Vector2(texture.width, texture.height);
        }

        public static Vector2 GetResolution()
        {
            return new Vector2(Screen.width, Screen.height);
        }

        #region Vector

        public static Vector3 ToVector3(this Vector2 vector2, float z = 0)
        {
            return new Vector3(vector2.x, vector2.y, z);
        }

        public static Vector3 Mult(this Vector3 lhs, Vector3 rhs)
        {
            lhs.x *= rhs.x;
            lhs.y *= rhs.y;
            lhs.z *= rhs.z;
            return lhs;
        }

        public static Vector2 ToVector2(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.y);
        }

        public static Vector2 Rotate(this Vector2 vector, float degrees)
        {
            var rads = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(rads);
            float cos = Mathf.Cos(rads);

            float tx = vector.x;
            float ty = vector.y;
            vector.x = (cos * tx) - (sin * ty);
            vector.y = (sin * tx) + (cos * ty);
            return vector;
        }

        #endregion

        public static int Sum(this int[] array)
        {
            var result = 0;
            for (int i = 0; i < array.Length; i++)
                result += array[i];
            return result;
        }

        public static void Fill<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = value;
        }

        public static TRes Sum<TVal, TRes>(this TVal[] array, Func<TRes, TVal, TRes> sum, TRes res = default)
        {
            for (int i = 0; i < array.Length; i++)
                res = sum(res, array[i]);
            return res;
        }

        public static T[] GetValues<T>() where T : Enum
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        #region List

        public static T First<T>(this List<T> list)
        {
            if (list == null || list.Count == 0)
                return default;

            return list[0];
        }

        public static T Last<T>(this List<T> list)
        {
            if (list == null || list.Count == 0)
                return default;

            return list[list.Count - 1];
        }

        #endregion

        #region Callback

        public static void AddSingleExecutionCallback(this UnityEvent unityEvent, Action action)
        {
            UnityAction wrapper = null;
            wrapper = () =>
            {
                unityEvent.RemoveListener(wrapper);
                action.Invoke();
            };
            unityEvent.AddListener(wrapper);
        }

        public static void AddSingleExecutionCallback<T0>(this UnityEvent<T0> unityEvent, Action<T0> action)
        {
            UnityAction<T0> wrapper = null;
            wrapper = (t0) =>
            {
                unityEvent.RemoveListener(wrapper);
                action.Invoke(t0);
            };
            unityEvent.AddListener(wrapper);
        }

        public static void AddSingleExecutionCallback<T0, T1>(this UnityEvent<T0, T1> unityEvent, Action<T0, T1> action)
        {
            UnityAction<T0, T1> wrapper = null;
            wrapper = (t0, t1) =>
            {
                unityEvent.RemoveListener(wrapper);
                action.Invoke(t0, t1);
            };
            unityEvent.AddListener(wrapper);
        }

        public static void AddSingleExecutionCallback(this Action @event, Action action)
        {
            Action wrapper = null;
            wrapper = () =>
            {
                @event -= wrapper;
                action.Invoke();
            };
            @event += wrapper;
        }

        public static void AddSingleExecutionCallback<T0>(this Action<T0> @event, Action<T0> action)
        {
            Action<T0> wrapper = null;
            wrapper = (t0) =>
            {
                @event -= wrapper;
                action.Invoke(t0);
            };
            @event += wrapper;
        }

        public static void AddSingleExecutionCallback<T0, T1>(this Action<T0, T1> @event, Action<T0, T1> action)
        {
            Action<T0, T1> wrapper = null;
            wrapper = (t0, t1) =>
            {
                @event -= wrapper;
                action.Invoke(t0, t1);
            };
            @event += wrapper;
        }

        #endregion

        public static void StopCoroutineChecked(this MonoBehaviour monoBehaviour, ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                monoBehaviour.StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        public static string ToHex(this Color32 color)
        {
            return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", color.r, color.g, color.b, color.a);
        }

        #region Random

        private static readonly System.Random _RANDOM = new System.Random((int)DateTime.Now.Ticks);

        /// <summary>
        /// [0..IntMax)
        /// </summary>
        public static int RandomInt()
        {
            return _RANDOM.Next();
        }

        /// <summary>
        /// [0..max)
        /// </summary>
        public static int RandomRange(int max)
        {
            return _RANDOM.Next(max);
        }

        /// <summary>
        /// [min..max)
        /// </summary>
        public static int RandomRange(int min, int max)
        {
            return _RANDOM.Next(min, max);
        }

        /// <summary>
        /// [min..max)
        /// </summary>
        public static int RandomRangeExcluding(int min, int max, int excludedValue)
        {
            if (min <= excludedValue && excludedValue < max)
            {
                if (max - min == 1)
                    return min;

                var lengthLeft = excludedValue - min;
                var lengthRight = max - 1 - excludedValue;
                var sum = lengthLeft + lengthRight;
                var leftSide = RandomRange(sum) < lengthLeft;
                return leftSide ? RandomRange(0, lengthLeft) : RandomRange(lengthLeft + 1, max);
            }
            else
                return RandomRange(min, max);
        }

        /// <summary>
        /// [0..1)
        /// </summary>
        public static float RandomFloat()
        {
            return (float)_RANDOM.NextDouble();
        }

        /// <summary>
        /// [0..max)
        /// </summary>
        public static float RandomRange(float max)
        {
            return (float)_RANDOM.NextDouble() * max;
        }

        /// <summary>
        /// [min..max)
        /// </summary>
        public static float RandomRange(float min, float max)
        {
            return (float)_RANDOM.NextDouble() * (max - min) + min;
        }

        public static bool RandomBool(double trueProbability = 0.5)
        {
            return _RANDOM.NextDouble() < trueProbability;
        }

        public static T Random<T>(this List<T> list)
        {
            if (list == null || list.Count == 0)
                return default;
            return list[RandomRange(list.Count)];
        }

        #endregion

        public static T Nullify<T>(this T obj) where T : UnityEngine.Object
        {
            return obj == null ? null : obj;
        }

        public static void DestroyChilds(this Transform transform)
        {
            foreach (Transform child in transform)
                UnityEngine.Object.Destroy(child.gameObject);
        }

        #region EDITOR
#if UNITY_EDITOR

        [MenuItem("CONTEXT/RectTransform/Revert")]
        private static void RevertRectTransform(MenuCommand menuCommand)
        {
            if (menuCommand.context is RectTransform rectTransform)
            {
                var obj = new SerializedObject(rectTransform);
                var prop = obj.GetIterator();
                while (prop.Next(true))
                {
                    PrefabUtility.RevertPropertyOverride(prop, InteractionMode.UserAction);
                }
            }
        }
#endif
        #endregion
    }
}