# if UNITY_EDITOR
namespace Utility.Custom2DColliders
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(StarCollider2D))]
    public class StarCollider_Editor : Editor
    {
        StarCollider2D sc;
        EdgeCollider2D edgeCollider;
        Vector2 off;

        void OnEnable()
        {
            sc = (StarCollider2D)target;

            edgeCollider = sc.GetComponent<EdgeCollider2D>();
            if (edgeCollider == null)
            {
                sc.gameObject.AddComponent<EdgeCollider2D>();
                edgeCollider = sc.GetComponent<EdgeCollider2D>();
            }
            edgeCollider.points = sc.getPoints(edgeCollider.offset);
        }

        public override void OnInspectorGUI()
        {
            Helpers.EditorOnly();

            GUI.changed = false;
            DrawDefaultInspector();

            sc.rotation = EditorGUILayout.IntSlider("Rotation", sc.rotation, 0, 360 / sc.points);


            if (GUI.changed || !off.Equals(edgeCollider.offset))
            {
                edgeCollider.points = sc.getPoints(edgeCollider.offset);
            }

            off = edgeCollider.offset;
        }

    }
}
#endif