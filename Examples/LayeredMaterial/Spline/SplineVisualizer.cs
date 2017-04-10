using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SplineVisualizer : MonoBehaviour {
    public Data data;

    Spline spline;
    SplineMesh spmesh;

    #region Unity
    void OnEnable() {
        spline = new Spline();
        spmesh = new SplineMesh ();
    }
    void Update() {
        spline.Reset (data.controls);
        spmesh.Build (spline, data.width);
    }
    void OnDisable() {
        spmesh.Dispose ();
    }
    #endregion

    [System.Serializable]
    public class Data {
        public float width;
        public float depth;
        public Vector2[] controls;
    }
}
