# if UNITY_EDITOR
namespace Utility.Custom2DColliders
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(ArcCollider2D))]
    public class ArcCollider_Editor : Editor
    {
        ArcCollider2D ac;
        PolygonCollider2D polygonCollider;
        Vector2 off;

        void OnEnable()
        {
            ac = (ArcCollider2D)target;

            polygonCollider = ac.GetComponent<PolygonCollider2D>();
            if (polygonCollider == null)
            {
                ac.gameObject.AddComponent<PolygonCollider2D>();
                polygonCollider = ac.GetComponent<PolygonCollider2D>();
            }
            polygonCollider.points = ac.getPoints(polygonCollider.offset);
        }

        public override void OnInspectorGUI()
        {
            Helpers.EditorOnly();

            GUI.changed = false;
            DrawDefaultInspector();

            if (GUI.changed || !off.Equals(polygonCollider.offset))
            {
                polygonCollider.points = ac.getPoints(polygonCollider.offset);
            }
            off = polygonCollider.offset;
        }

    }
}
#endif