using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineMesh : System.IDisposable {

    public Mesh mesh { get; }

    Vector3[] vertices;
    Vector2[] uvs;
    int[] triangles;

    public SplineMesh() {
        this.mesh = new Mesh ();
    }

    public Mesh Build(Spline s, float width) {
        mesh.Clear ();

        var l = s.tend;
        var m = 10;
        var n = l * m;
        var dt = 1f / m;
        var duv = 1f / n;

        ResizeArrays (2 * (n + 1), 6 * n);
        for (var i = 0; i < l; i++) {
            for (var j = 0; j < m; j++) {
                var k = m * i + j;
                UpdateSpineVertices (s, width, k, dt * k);
                UpdateSplineUVs (k, duv * k);
                UpdateSpineTriangles (k);
            }
        }
        UpdateSpineVertices (s, width, n, l);
        UpdateSplineUVs (n, 1f);

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateBounds ();

        return mesh;
    }

    void ResizeArrays (int nvertices, int ntriangles) {
        System.Array.Resize (ref vertices, nvertices);
        System.Array.Resize (ref uvs, nvertices);
        System.Array.Resize (ref triangles, ntriangles);
    }

    void UpdateSpineVertices (Spline s, float width, int k, float t) {
        var k2 = 2 * k;
        var p = s.Position (t);
        var tan = s.Tangent (t);
        var nor = new Vector2 (-tan.y, tan.x);
        vertices [k2] = p - width * nor;
        vertices [k2 + 1] = p + width * nor;
        uvs [k2] = new Vector2 (0f, 0f);
        uvs [k2 + 1] = new Vector2 (0f, 1f);
    }
    void UpdateSplineUVs(int k, float uv) {
        var k2 = 2 * k;
        uvs [k2] = new Vector2 (uv, 0f);
        uvs [k2 + 1] = new Vector2 (uv, 1f);
    }
    void UpdateSpineTriangles (int k) {
        var k2 = 2 * k;
        var k6 = 6 * k;
        triangles [k6] = k2;
        triangles [k6 + 1] = k2 + 1;
        triangles [k6 + 2] = k2 + 3;
        triangles [k6 + 3] = k2;
        triangles [k6 + 4] = k2 + 3;
        triangles [k6 + 5] = k2 + 2;
    }

    #region IDisposable implementation
    public void Dispose () {
        if (mesh != null) {
            if (Application.isPlaying)
                Object.Destroy (mesh);
            else
                Object.DestroyImmediate (mesh);
        }            
    }
    #endregion
}
