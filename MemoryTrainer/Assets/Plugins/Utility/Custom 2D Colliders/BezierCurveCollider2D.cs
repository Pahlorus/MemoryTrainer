﻿namespace Utility.Custom2DColliders
{
    using UnityEngine;
    using System.Collections.Generic;

    [AddComponentMenu("Physics 2D/Bezier Curve Collider 2D")]

    [RequireComponent(typeof(EdgeCollider2D))]
    public class BezierCurveCollider2D : MonoBehaviour
    {

        public List<Vector2> controlPoints, handlerPoints;

        [Range(3, 36)]
        public int smoothness = 15;

        Vector2 origin, center;

        [HideInInspector]
        public bool initialized;

        public bool continous = true;

        [HideInInspector]
        public EdgeCollider2D edge;
        List<Vector2> pts;


        public void Init()
        {
            if (initialized) return;
            initialized = true;
            continous = true;
            smoothness = 15;

            controlPoints = new List<Vector2>();
            handlerPoints = new List<Vector2>();

            controlPoints.Clear();
            handlerPoints.Clear();

            Vector2 pos = transform.localPosition;
            controlPoints.Add(pos);

            pos.x += 4;
            controlPoints.Add(pos);

            pos.x -= 4;
            pos.y += 4;
            handlerPoints.Add(pos);

            pos.x += 4;
            handlerPoints.Add(pos);

            drawCurve();
        }

        public void drawCurve()
        {
            pts = new List<Vector2>();
            pts.Clear();

            edge = GetComponent<EdgeCollider2D>();
            if (edge == null)
            {
                gameObject.AddComponent<EdgeCollider2D>();
            }

            if (controlPoints.Count == 2)
            {
                drawSegment(controlPoints[0], controlPoints[1], handlerPoints[0], handlerPoints[1]);
            }
            else if (controlPoints.Count > 2)
            {
                int h = 0;
                for (int i = 0; i < controlPoints.Count - 1; i++)
                {
                    drawSegment(controlPoints[i], controlPoints[i + 1], handlerPoints[h], handlerPoints[h + 1]);
                    h += 2;
                }
            }


            edge.points = pts.ToArray();

        }

        void drawSegment(Vector3 cPt1, Vector3 cPt2, Vector3 hPt1, Vector3 hPt2)
        {

            pts.Add(cPt1);
            for (int i = 1; i < smoothness; i++)
            {
                pts.Add(CalculateBezierPoint((1f / smoothness) * i, cPt1, hPt1, hPt2, cPt2));
            }
            pts.Add(cPt2);
        }

        Vector3 CalculateBezierPoint(float t, Vector3 controlP0, Vector3 handlerP0, Vector3 handlerP1, Vector3 controlP1)
        {
            //http://devmag.org.za/2011/04/05/bzier-curves-a-tutorial/
            Vector3 p = (Mathf.Pow((1.0f - t), 3) * controlP0) + (3 * Mathf.Pow((1 - t), 2) * t * handlerP0) + (3 * (1.0f - t) * Mathf.Pow(t, 2) * handlerP1) + (Mathf.Pow(t, 3) * controlP1);
            return p;
        }


        public void addControlPoint()
        {
            Vector2 pos = controlPoints[controlPoints.Count - 1];
            float hPosY = handlerPoints[handlerPoints.Count - 1].y;

            float mul = (hPosY > pos.y) ? -1 : 1; // check if the handler point was below or top of the control point and use that info to make sure that the next handler point is in the opposite direction

            handlerPoints.Add(new Vector2(pos.x, pos.y + (4 * mul)));

            pos.x += 4;
            controlPoints.Add(pos);

            pos.y += 4 * mul;
            handlerPoints.Add(pos);

            drawCurve();
        }


        public void removeControlPoint()
        {
            if (controlPoints.Count > 2)
            {
                controlPoints.RemoveAt(controlPoints.Count - 1);
                handlerPoints.RemoveAt(handlerPoints.Count - 1);
                handlerPoints.RemoveAt(handlerPoints.Count - 1);
            }
        }
    }
}