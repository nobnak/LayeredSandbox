using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSplineVisualizer : MonoBehaviour {
    protected Spline spline;
    protected SplineMesh spmesh;
    protected MeshFilter mfilter;

    protected abstract float GetSplineWidth ();
    protected abstract Vector2[] GetControlPoints ();

    #region Unity
    protected virtual void OnEnable() {
        spline = new Spline();
        spmesh = new SplineMesh ();
        mfilter = GetComponent<MeshFilter> ();

        mfilter.sharedMesh = spmesh.mesh;
    }
    protected virtual void Update() {
        spline.Reset (GetControlPoints());
        spmesh.Build (spline, GetSplineWidth());
    }
    protected virtual void OnDisable() {
        spmesh.Dispose ();
    }
    #endregion
}
