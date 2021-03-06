# if UNITY_EDITOR
namespace Utility.Custom2DColliders
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(EllipseCollider2D))]
    public class EllipseCollider_Editor : Editor
    {
        EllipseCollider2D ec;
        EdgeCollider2D edgeCollider;
        Vector2 off;

        void OnEnable()
        {
            ec = (EllipseCollider2D)target;

            edgeCollider = ec.GetComponent<EdgeCollider2D>();
            if (edgeCollider == null)
            {
                ec.gameObject.AddComponent<EdgeCollider2D>();
                edgeCollider = ec.GetComponent<EdgeCollider2D>();
            }
            edgeCollider.points = ec.getPoints(edgeCollider.offset);
        }

        public override void OnInspectorGUI()
        {
            Helpers.EditorOnly();

            GUI.changed = false;
            DrawDefaultInspector();

            if (GUI.changed || !off.Equals(edgeCollider.offset))
            {
                edgeCollider.points = ec.getPoints(edgeCollider.offset);
            }

            off = edgeCollider.offset;
        }

    }
}
#endif