using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class SceneDirtyFlagChecker
{
#if UNITY_EDITOR

    static bool _enabled;

    [MenuItem("Tools/DirtyChecker/Enable")]
    static void Enable()
    {
        if (!_enabled)
        {
            Undo.postprocessModifications += OnPostProcessModifications;
            _enabled = true;
        }
    }

    [MenuItem("Tools/DirtyChecker/Enable", true)]
    static bool EnableValidation()
    {
        return !_enabled;
    }

    [MenuItem("Tools/DirtyChecker/Disable")]
    static void Disable()
    {
        if (_enabled)
        {
            Undo.postprocessModifications -= OnPostProcessModifications;
            _enabled = false;
        }
    }

    [MenuItem("Tools/DirtyChecker/Disable", true)]
    static bool DisableValidation()
    {
        return _enabled;
    }

    private static UndoPropertyModification[] OnPostProcessModifications(UndoPropertyModification[] propertyModifications)
    {
        Debug.LogWarning($"Scene was marked Dirty by number of objects = {propertyModifications.Length}");
        for (int i = 0; i < propertyModifications.Length; i++)
        {
            Debug.LogWarning($"currentValue.target = {propertyModifications[i].currentValue.target}", propertyModifications[i].currentValue.target);
        }
        return propertyModifications;
    }
#endif
}

