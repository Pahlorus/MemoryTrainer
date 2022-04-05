# if UNITY_EDITOR
namespace Utility.Custom2DColliders
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(RoundedBoxCollider2D))]
    public class RoundedBoxCollider_Editor : Editor
    {
        RoundedBoxCollider2D rb;
        EdgeCollider2D edgeCollider;
        Vector2 off;

        void OnEnable()
        {
            rb = (RoundedBoxCollider2D)target;

            edgeCollider = rb.GetComponent<EdgeCollider2D>();
            if (edgeCollider == null)
            {
                rb.gameObject.AddComponent<EdgeCollider2D>();
                edgeCollider = rb.GetComponent<EdgeCollider2D>();
            }

            Vector2[] pts = rb.getPoints(edgeCollider.offset);
            if (pts != null) edgeCollider.points = pts;
        }

        public override void OnInspectorGUI()
        {
            Helpers.EditorOnly();

            GUI.changed = false;
            DrawDefaultInspector();


            // automatically adjust the radius according to width and height
            float lesser = (rb.width > rb.height) ? rb.height : rb.width;
            lesser /= 2f;
            lesser = Mathf.Round(lesser * 100f) / 100f;
            rb.radius = EditorGUILayout.Slider("Radius", rb.radius, 0f, lesser);
            rb.radius = Mathf.Clamp(rb.radius, 0f, lesser);

            if (GUILayout.Button("Reset"))
            {
                rb.smoothness = 15;
                rb.width = 2;
                rb.height = 2;
                rb.trapezoid = 0.5f;
                rb.radius = 0.5f;
                edgeCollider.offset = Vector2.zero;
            }

            if (GUI.changed || !off.Equals(edgeCollider.offset))
            {
                Vector2[] pts = rb.getPoints(edgeCollider.offset);
                if (pts != null) edgeCollider.points = pts;
            }

            off = edgeCollider.offset;
        }

    }
}
#endif