using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PointRegistrationSubmod {

    public class Rope : MonoBehaviour {
        public Camera viewPointCam;
        public Transform[] colliders;

        public Transform p0;
        public Transform p1;

        public float restoringForce = 1f;
        public float collidingDistance = 1f;
        public int numberOfIntermediatePoints = 10;

        Spline ropeSpline;
        Point[] points;
        Vector2[] splineControlPositions;
        Vector2[] colliderPositions;

        float restLength;

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
            var dt = 1f / numberOfLines;
            var dTime = Time.deltaTime;
            restLength = Vector2.Distance (points [0].pos, points [points.Length - 1].pos) / numberOfLines;
            for (var i = 0; i < points.Length; i++) {
                var p = points [i];
                points [i] = new Point (p.mobility, Vector2.Lerp (p.pos, RestPosition (dt * i), dt * restoringForce));
            }
            for (var i = 0; i < colliders.Length; i++)
                colliderPositions [i] = PositionOnViewplane (colliders [i].position);

            for (var i = 0; i < numberOfLines; i++) {
                var p0 = points [i];
                var p1 = points [i + 1];
                var totalMobility = p0.mobility + p1.mobility;
                if (totalMobility > 0f) {
                    var p0To1 = p1.pos - p0.pos;
                    var len = p0To1.magnitude;
                    var dx = (dt * restoringForce * (len - restLength) / totalMobility) * p0To1.normalized;
                    points[i] = new Point (p0.mobility, p0.pos + p0.mobility * dx);
                    points[i+1] = new Point (p1.mobility, p1.pos - p1.mobility * dx);
                }
            }
            
            for (var i = 0; i < numberOfLines; i++) {
                var p0 = points [i];
                var p1 = points [i + 1];
                var line = new Line (p0, p1);
                for (var j = 0; j < colliders.Length; j++)
                    SolveCollision (ref points[i], ref points[i+1], line, j);
            }

            UpdateSpline ();
        }
        #endregion

        bool Prepared() {
            return viewPointCam != null && ropeSpline != null && ropeSpline.valid;
        }
        void CheckInit () {
            if (points == null || points.Length != (numberOfIntermediatePoints + 2)) {
                System.Array.Resize (ref points, numberOfIntermediatePoints + 2);
                System.Array.Resize (ref splineControlPositions, points.Length + 2);
                var first = points [0] = new Point (0f, PositionOnViewplane (p0.position));
                var last = points [points.Length - 1] = new Point (first.mobility, PositionOnViewplane (p1.position));
                var numOfLines = points.Length - 1;
                for (var i = 1; i < numOfLines; i++)
                    points [i] = new Point (1f, Vector2.Lerp (first.pos, last.pos, (float)i / numOfLines));
            }
            if (colliderPositions == null || colliderPositions.Length != colliders.Length)
                System.Array.Resize (ref colliderPositions, colliders.Length);
        }
        void SolveCollision (ref Point p0, ref Point p1, Line line, int j) {
            var q = PositionOnViewplane (colliderPositions [j]);
            var p = line.ClosestPointOnLine (q);
            var positionFromCollider = p - q;
            if (positionFromCollider.sqrMagnitude < (collidingDistance * collidingDistance)) {
                var dx = (collidingDistance - positionFromCollider.magnitude) * positionFromCollider.normalized;
                p0 = new Point (p0.mobility, p0.pos + p0.mobility * dx);
                p1 = new Point (p1.mobility, p1.pos + p1.mobility * dx);
            }
        }

        void UpdateSpline () {
            for (var i = 0; i < points.Length; i++)
                splineControlPositions [i + 1] = points [i].pos;
            splineControlPositions [0] = 2f * splineControlPositions [1] - splineControlPositions [2];
            splineControlPositions [splineControlPositions.Length - 1] = 
                2f * splineControlPositions [splineControlPositions.Length - 2] - splineControlPositions [splineControlPositions.Length - 3];
            ropeSpline.Reset (splineControlPositions);
        }

        Vector2 PositionOnViewplane(Vector3 worldPos) {
            return viewPointCam.transform.InverseTransformPoint(worldPos);
        }
        Vector3 WorldPosition(Vector2 viewplanePos, float z = 0f) {
            return viewPointCam.transform.TransformPoint (new Vector3 (viewplanePos.x, viewplanePos.y, z));
        }
        Vector2 RestPosition(float t) {
            return Vector2.Lerp (points [0].pos, points [points.Length - 1].pos, t);
        }

        public struct Point {
            public readonly float mobility;
            public readonly Vector2 pos;

            public Point(float mass, Vector2 pos) {
                this.mobility = mass;
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
