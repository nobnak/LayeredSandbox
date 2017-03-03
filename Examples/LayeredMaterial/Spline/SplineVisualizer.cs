using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SplineVisualizer : MonoBehaviour {
    public Link link;
    public Data data;

    Spline spline;
    SplineMesh spmesh;

    void OnEnable() {
        spline = new Spline();
        spmesh = new SplineMesh ();
        link.mf.sharedMesh = spmesh.mesh;
    }
    void Update() {
        spline.Reset (data.controls);
        spmesh.Build (spline, data.width);
    }
    void OnDisable() {
        spmesh.Dispose ();
    }

    void OnDrawGizmos() {
        if (spline.valid && link.cam != null)
            spline.DrawGizmos (link.cam, data.depth);
    }

    [System.Serializable]
    public class Link {
        public Camera cam;
        public MeshFilter mf;
    }

    [System.Serializable]
    public class Data {
        public float width;
        public float depth;
        public Vector2[] controls;
    }
}
