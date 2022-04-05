# if UNITY_EDITOR
namespace Utility.Custom2DColliders
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(CapsuleCollider2D))]
    public class CapsuleCollider_Editor : Editor
    {
        CapsuleCollider2D capCol;
        EdgeCollider2D edgeCollider;
        Vector2 off;

        void OnEnable()
        {
            capCol = (CapsuleCollider2D)target;

            edgeCollider = capCol.GetComponent<EdgeCollider2D>();
            if (edgeCollider == null)
            {
                capCol.gameObject.AddComponent<EdgeCollider2D>();
                edgeCollider = capCol.GetComponent<EdgeCollider2D>();
            }

            edgeCollider.points = capCol.getPoints(edgeCollider.offset);
        }

        public override void OnInspectorGUI()
        {
            Helpers.EditorOnly();

            GUI.changed = false;
            DrawDefaultInspector();

            capCol.radius = Mathf.Clamp(capCol.radius, 0.5f, capCol.height / 2);
            capCol.radius = EditorGUILayout.Slider("Radius", capCol.radius, 0.25f, capCol.height / 2f);

            GUILayout.Space(8);
            capCol.bullet = EditorGUILayout.Toggle("Bullet", capCol.bullet);
            if (capCol.bullet) capCol.flip = EditorGUILayout.Toggle("Flip", capCol.flip);


            if (GUI.changed || !off.Equals(edgeCollider.offset))
            {
                edgeCollider.points = capCol.getPoints(edgeCollider.offset);
            }

            off = edgeCollider.offset;
        }

    }
}
#endif