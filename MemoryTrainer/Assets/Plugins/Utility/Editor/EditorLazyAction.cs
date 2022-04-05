using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Utility
{
    public static class EditorLazyAction
    {
        private static List<Action> Actions = new List<Action>();

        static EditorLazyAction()
        {
#if UNITY_EDITOR
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
#endif
        }

        private static void Update()
        {
            if (Actions.Count == 0)
                return;
            var actions = Actions.ToArray();
            Actions.Clear();
            for (int i = 0; i < actions.Length; i++)
            {
                var action = actions[i];
                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        public static void Add(Action action)
        {
            if (action != null)
                Actions.Add(action);
        }
    }
}
