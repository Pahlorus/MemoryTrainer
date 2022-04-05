namespace Utility.Custom2DColliders
{
    using UnityEngine;
    using System.Collections.Generic;

    [AddComponentMenu("Physics 2D/Capsule Collider 2D")]

    [RequireComponent(typeof(EdgeCollider2D))]
    public class CapsuleCollider2D : MonoBehaviour
    {

        [HideInInspector]
        public bool bullet = false, flip = false;

        [HideInInspector]
        [Range(.5f, 25)]
        public float radius = 1;

        [Range(1, 25)]
        public float height = 4;

        [Range(10, 90)]
        public int smoothness = 20;

        [Range(0, 180)]
        public int rotation = 0;

        Vector2 origin, center, center1, center2;
        List<Vector2> points;
        float ang = 0;

        public Vector2[] getPoints(Vector2 off)
        {
            points = new List<Vector2>();

            origin = transform.localPosition;
            center = origin + off;

            float r = (height / 2f) - (radius);

            if (bullet && flip) r += radius;

            center1.x = center.x + r * Mathf.Sin(rotation * Mathf.Deg2Rad);
            center1.y = center.y + r * Mathf.Cos(rotation * Mathf.Deg2Rad);

            if (bullet)
            {
                if (!flip) r += radius;
                else r -= radius;
            }

            center2.x = center.x + r * Mathf.Sin((rotation + 180f) * Mathf.Deg2Rad);
            center2.y = center.y + r * Mathf.Cos((rotation + 180f) * Mathf.Deg2Rad);




            ang = 360f - rotation;
            ang %= 360;

            // top semi circle
            for (int i = 0; i <= smoothness; i++)
            {
                if (bullet && flip)
                {
                    calcPointLocation(radius, center1);
                    ang += 180f;
                    calcPointLocation(radius, center1);
                    i = smoothness + 1;
                }
                else
                {
                    calcPointLocation(radius, center1);
                    ang += 180f / smoothness;
                }
            }

            ang -= 180f / smoothness;
            ang %= 360;

            // bottom semi circle
            for (int i = 0; i <= smoothness; i++)
            {
                if (bullet && !flip)
                {
                    calcPointLocation(radius, center2);
                    ang += 180f;
                    calcPointLocation(radius, center2);
                    i = smoothness + 1;
                }
                else
                {
                    calcPointLocation(radius, center2);
                    ang += 180f / smoothness;
                }
            }

            points.Add(points[0]);
            return points.ToArray();
        }

        void calcPointLocation(float r, Vector2 centerPt)
        {
            float a = ang * Mathf.Deg2Rad;
            float x = centerPt.x + r * Mathf.Cos(a);
            float y = centerPt.y + r * Mathf.Sin(a);

            points.Add(new Vector2(x, y));
        }
    }
}