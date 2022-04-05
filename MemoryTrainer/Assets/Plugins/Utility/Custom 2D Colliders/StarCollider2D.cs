namespace Utility.Custom2DColliders
{
    using UnityEngine;
    using System.Collections.Generic;

    [AddComponentMenu("Physics 2D/Star Collider 2D")]
    [RequireComponent(typeof(EdgeCollider2D))]
    public class StarCollider2D : MonoBehaviour
    {

        [Range(1, 25)]
        public float radiusA = 1;

        [Range(1, 25)]
        public float radiusB = 2;

        [Range(3, 36)]
        public int points = 5;

        [HideInInspector]
        public int rotation = 0;

        Vector2 origin, center;

        public Vector2[] getPoints(Vector2 off)
        {
            List<Vector2> pts = new List<Vector2>();

            origin = transform.localPosition;
            center = origin + off;

            float ang = rotation;

            for (int i = 0; i <= points * 2; i++)
            {
                float radius = (i % 2 == 0) ? radiusA : radiusB;
                float x = center.x + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
                float y = center.y + radius * Mathf.Sin(ang * Mathf.Deg2Rad);

                pts.Add(new Vector2(x, y));
                ang += 360f / (points * 2f);
            }

            pts.Add(pts[0]);

            return pts.ToArray();
        }
    }
}