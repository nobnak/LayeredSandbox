using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SimpleBentSplineVisualizer : AbstractSplineVisualizer {
    public const float TWO_PI = 2f * Mathf.PI;

    public float splineWidth = 1f;
    public Vector2 splineBending = new Vector2 (0.1f, 2f);

    protected Vector2[] controlPoints;

    #region Unity
    protected override void OnEnable () {
        base.OnEnable ();
        controlPoints = new Vector2[4];
    }
    #endregion

    #region implemented abstract members of AbstractSplineVisualizer
    protected override float GetSplineWidth () { return splineWidth; }
    protected override Vector2[] GetControlPoints () { return controlPoints; }
    #endregion


    public static Vector2 Rotate(Vector2 v, float angle, float scale = 1f) {
        var c = Mathf.Cos (angle);
        var s = Mathf.Sin (angle);
        return new Vector2 (scale * (c * v.x - s * v.y), scale * (s * v.x + c * v.y));
    }

    public void SetEndPoints (Vector2 p0, Vector2 p1) {
        var bending = Rotate (p1 - p0, TWO_PI * splineBending.x, splineBending.y);
        controlPoints [0] = p0 - bending;
        controlPoints [1] = p0;
        controlPoints [2] = p1;
        controlPoints [3] = p1 + bending;
    }
}
