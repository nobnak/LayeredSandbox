using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spline {
    public const int SUBDIVISION = 10;
    public const float CONTROL_RADIUS = 0.2f;
    public static readonly Color COLOR_POINT = (Color)new Color32 (226, 0, 255, 255);
    public static readonly Color COLOR_LINE = (Color)new Color32 (126, 255, 0, 255);

    public Vector2[] controls;
    public int tend;
    public bool valid;

    public void Reset (Vector2[] controls) {
        this.controls = controls;
        this.tend = Mathf.Max (controls.Length - 3, 0);
        this.valid = Valid ();
    }
    public bool Valid() {
        return controls != null && controls.Length >= 4;
    }

    public void DrawGizmos(Camera c, float z) {
        Gizmos.color = COLOR_POINT;
        for (var i = 0; i < controls.Length; i++) {
            var p = LocalToWorldPosition (c, z, controls [i]);
            Gizmos.DrawSphere (p, CONTROL_RADIUS * GizmoScale (p));
        }

        Gizmos.color = COLOR_LINE;
        var dt = 1f / SUBDIVISION;
        for (var i = 0; i < tend; i++) {
            for (var j = 0; j < SUBDIVISION; j++) {
                var k = i * SUBDIVISION + j;
                Gizmos.DrawLine (
                    LocalToWorldPosition(c, z, Position (k * dt)), 
                    LocalToWorldPosition(c, z, Position ((k + 1) * dt))
                );
            }
        }
    }

    public Vector2 Position(float t) {
        float toffset;
        int tfloor;
        CalculateSplineIndex (t, out tfloor, out toffset);
        return CatmullSplineEquation.Position (toffset, 
            controls [tfloor], controls [tfloor + 1], controls [tfloor + 2], controls [tfloor + 3]);
    } 
    public Vector2 Tangent(float t) {
        float toffset;
        int tfloor;
        CalculateSplineIndex (t, out tfloor, out toffset);
        var v = CatmullSplineEquation.Velosity (toffset, 
            controls [tfloor], controls [tfloor + 1], controls [tfloor + 2], controls [tfloor + 3]);
        return v.normalized;
    }

    public void CalculateSplineIndex (float t, out int tfloor, out float toffset) {
        t = Mathf.Clamp (t, 0f, tend);
        tfloor = Mathf.Min (Mathf.FloorToInt (t), tend - 1);
        toffset = t - tfloor;
    }

    public static Vector3 LocalToWorldPosition(Camera c, float z, Vector2 localPos) {
        return c.transform.TransformPoint (new Vector3 (localPos.x, localPos.y, z));
    }
    public static float GizmoScale(Vector3 p) {
        #if UNITY_EDITOR
        return UnityEditor.HandleUtility.GetHandleSize(p);
        #else
        return 1f;
        #endif
    }
}
