using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PointRegistrationSubmod {

    public class Rope : MonoBehaviour {
        public Camera viewPointCam;
        public Transform[] colliders;

        public Transform p0;
        public Transform p1;

        public float collidingDistance = 1f;
        public int numberOfIntermediatePoints = 10;

        Spline ropeSpline;
        Point[] points;
        Vector2[] pointPositions;
        Vector2[] colliderPositions;

        #region Unity
        void OnEnable() {
            ropeSpline = new Spline ();
        }
        void OnDrawGizmos() {
            if (isActiveAndEnabled && Prepared())
                ropeSpline.DrawGizmos (viewPointCam, 10f);
        }
        void Update() {
            CheckInit();

            var numberOfLines = points.Length - 1;
            for (var i = 0; i < colliders.Length; i++)
                colliderPositions [i] = PositionOnViewplane (colliders [i].position);
            for (var i = 0; i < numberOfLines; i++) {
                var p0 = points [i];
                var p1 = points [i + 1];
                var line = new Line (p0, p1);
                for (var j = 0; j < colliders.Length; j++) {
                    var q = PositionOnViewplane (colliderPositions[j]);
                    var p = line.ClosestPointOnLine (q);
                    var positionFromCollider = p - q;
                    if (positionFromCollider.sqrMagnitude < (collidingDistance * collidingDistance)) {
                        var dx = (collidingDistance - positionFromCollider.magnitude) * positionFromCollider.normalized;
                        p0 = new Point (p0.mass, p0.pos + dx);
                        p1 = new Point (p1.mass, p1.pos + dx);
                        points [i] = p0;
                        points [i + 1] = p1;
                    }
                }
            }

            for (var i = 0; i < points.Length; i++)
                pointPositions [i] = points [i].pos;
            ropeSpline.Reset (pointPositions);
        }
        #endregion

        bool Prepared() {
            return viewPointCam != null && ropeSpline != null && ropeSpline.valid;
        }
        void CheckInit () {
            if (points == null || points.Length != (numberOfIntermediatePoints + 2)) {
                System.Array.Resize (ref points, numberOfIntermediatePoints + 2);
                System.Array.Resize (ref pointPositions, points.Length);
                var first = points [0] = new Point (float.MaxValue, PositionOnViewplane (p0.position));
                var last = points [points.Length - 1] = new Point (first.mass, PositionOnViewplane (p1.position));
                var numOfLines = points.Length - 1;
                for (var i = 1; i < numOfLines; i++)
                    points [i] = new Point (1f, Vector2.Lerp (first.pos, last.pos, (float)i / numOfLines));
            }
            if (colliderPositions == null || colliderPositions.Length != colliders.Length)
                System.Array.Resize (ref colliderPositions, colliders.Length);
        }

        Vector2 PositionOnViewplane(Vector3 worldPos) {
            return viewPointCam.transform.InverseTransformPoint(worldPos);
        }

        public struct Point {
            public readonly float mass;
            public readonly Vector2 pos;

            public Point(float mass, Vector2 pos) {
                this.mass = mass;
                this.pos = pos;
            }
        }
        public struct Line {
            public readonly Point p0;
            public readonly Point p1;

            public Line(Point p0, Point p1) {
                this.p0 = p0;
                this.p1 = p1;
            }

            public Vector2 ClosestPointOnLine(Point q) {
                return ClosestPointOnLine (q.pos);
            }
            public Vector2 ClosestPointOnLine(Vector2 q) {
                var lineVec = p1.pos - p0.pos;
                var p0toQVec = q - p0.pos;

                var lineSqrLen = lineVec.sqrMagnitude;
                if (lineSqrLen <= Mathf.Epsilon)
                    return p0.pos;
                else {
                    var t = Vector2.Dot (lineVec, p0toQVec) / lineSqrLen;
                    if (t < 0f)
                        return p0.pos;
                    else if (t <= 1f)
                        return t * lineVec + p0.pos;
                    else
                        return p0.pos;
                } 
            }
        }
    }
}
