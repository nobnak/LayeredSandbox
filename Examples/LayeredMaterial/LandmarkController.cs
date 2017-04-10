using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PointRegistrationSubmod {

    [ExecuteInEditMode]
    public class LandmarkController : MonoBehaviour {

        public PointRegistrationSubmod.BasePlane.TextureEvent OnUpdateMaskTexture;
        public Data data;

        RenderTexture maskTex;

        #region Unity
    	void Update () {
            CheckInitMaskTex();

            if (data.points.Registered != null) {
                var points = data.points.Registered.Latest;
                for (var ts = 0; ts < data.spots.Length; ts++) {
                    UpdateSpot (points, ts);
                }
            }

            if (data.spots.Length >= 2) {
                var s0 = data.spots [0];
                var s1 = data.spots [1];
                var p0 = (Vector2)s0.view.localPosition;
                var p1 = (Vector2)s1.view.localPosition;
                var activity = s0.view.gameObject.activeInHierarchy & s1.view.gameObject.activeInHierarchy;
                UpdateSpline (p0, p1, activity);
            }
    	}
        void OnDestroy() {
            ReleaseMaskTex ();
        }
        #endregion

        public Vector3 NormalizedToLocalPosition(Vector3 normalizedPosition) {
            var p = data.maskCam.ViewportToWorldPoint (normalizedPosition);
            return data.maskCam.transform.InverseTransformPoint (p);
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
            data.maskCam.targetTexture = null;
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
        void UpdateSpline (Vector2 p0, Vector2 p1, bool activity) {
            data.river.SetEndPoints (p0, p1);
            data.river.gameObject.SetActive (activity);
        }

        [System.Serializable]
        public class Data {
            public float masterSize = 1f;
            public Spot[] spots;

            public SimpleBentSplineVisualizer river;
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
}
