using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PointRegistrationSubmod;

[ExecuteInEditMode]
public class LandmarkController : MonoBehaviour {
    public const float TWO_PI = 2f * Mathf.PI;

    public TextureEvent OnUpdateMaskTexture;
    public Data data;

    RenderTexture maskTex;

	void Update () {
        CheckInitMaskTex();

        if (data.points.Registered != null) {
            var points = data.points.Registered.Latest;
            for (var ts = 0; ts < data.spots.Length; ts++) {
                UpdateSpot (points, ts);
            }
        }

        if (data.spots.Length >= 2) {
            var p0 = (Vector2)data.spots [0].view.localPosition;
            var p1 = (Vector2)data.spots [1].view.localPosition;
            UpdateSpline(p0, p1);
        }
	}
    void OnDestroy() {
        ReleaseMaskTex ();
    }

    public Vector3 NormalizedToLocalPosition(Vector3 normalizedPosition) {
        var p = data.maskCam.ViewportToWorldPoint (normalizedPosition);
        return data.maskCam.transform.InverseTransformPoint (p);
    }

    public static Vector2 Rotate(Vector2 v, float angle, float scale = 1f) {
        var c = Mathf.Cos (angle);
        var s = Mathf.Sin (angle);
        return new Vector2 (scale * (c * v.x - s * v.y), scale * (s * v.x + c * v.y));
    }
    public static void Release(Object o) {
        if (Application.isPlaying)
            Destroy (o);
        else
            DestroyImmediate (o);
        
    }

    void CheckInitMaskTex () {
        var w = data.referenceCam.pixelWidth >> data.maskLOD;
        var h = data.referenceCam.pixelHeight >> data.maskLOD;
        if (maskTex == null || maskTex.width != w || maskTex.height != h) {
            ReleaseMaskTex ();
            maskTex = new RenderTexture (w, h, 24, RenderTextureFormat.ARGB32);
            maskTex.filterMode = FilterMode.Bilinear;
            maskTex.wrapMode = TextureWrapMode.Clamp;
            data.maskCam.targetTexture = maskTex;
            OnUpdateMaskTexture.Invoke (maskTex);
        }
    }

    void ReleaseMaskTex () {
        Release (maskTex);
    }

    void UpdateSpot (List<Point> points, int ts) {
        var s = data.spots [ts];
        var tp = points.FindIndex (p => p.type == ts);
        if (tp < 0) {
            s.view.gameObject.SetActive (false);
        }
        else {
            s.view.gameObject.SetActive (true);
            var p = (Vector3)points [tp].rect.center;
            p.z = s.view.localPosition.z;
            s.view.localPosition = NormalizedToLocalPosition (p);
            s.view.localScale = data.masterSize * s.size * Vector3.one;
        }
    }

    void UpdateSpline (Vector2 p0, Vector2 p1) {
        var controls = data.river.data.controls;
        if (controls == null || controls.Length < 4)
            controls = new Vector2[4];
        var bending = Rotate (p1 - p0, TWO_PI * data.riverBending.x, data.riverBending.y);
        controls [0] = p0 - bending;
        controls [1] = p0;
        controls [2] = p1;
        controls [3] = p1 + bending;
        data.river.data.controls = controls;
    }

    [System.Serializable]
    public class Data {
        public float masterSize = 1f;
        public Spot[] spots;

        public SplineVisualizer river;
        public Vector2 riverBending;

        public int maskLOD = 1;
        public Camera referenceCam;
        public Camera maskCam;
        public LandmarkRegistration points;

        [System.Serializable]
        public class Spot {
            public Vector2 normalizedPosition;
            public float size = 1f;

            public Transform view;
        }
    }
}
