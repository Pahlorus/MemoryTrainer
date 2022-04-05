#if UNITY_EDITOR
#endif

using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = nameof(ScriptableObjectCollection), menuName = "ScriptableObjects/Utility/Collections/" + nameof(ScriptableObjectCollection))]
    public class ScriptableObjectCollection : Collection<ScriptableObject>
    {
    }
}