# if UNITY_EDITOR
using UnityEditor;

namespace Utility.Custom2DColliders
{
    public static class Helpers
    {
        public static void EditorOnly()
        {
            EditorGUILayout.HelpBox("This component works only in editor", MessageType.Warning);
        }
    }
}
#endif