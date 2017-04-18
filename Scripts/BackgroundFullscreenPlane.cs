using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PointRegistrationSubmod {
        
    [ExecuteInEditMode]
    public class BackgroundFullscreenPlane : BasePlane {
        public enum DepthModeEnum { Normalized = 0, Exact }

        public Camera targetCam;

        public DepthModeEnum depthMode;
        [Header("Normalized Depth")]
    	[Range(0f, 0.99f)]
    	public float normalizedDepth = 0f;
        [Header("Exact Depth")]
        public float depth = 10f;

        void Update() {
            var depth = Depth ();
    		var scale = targetCam.ViewportToWorldPoint (new Vector3 (1f, 1f, depth))
    			- targetCam.ViewportToWorldPoint (new Vector3 (0f, 0f, depth));
    		scale = targetCam.transform.InverseTransformDirection (scale);
            scale.z = 1f;

            transform.position = targetCam.ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, depth));
            transform.rotation = targetCam.transform.rotation;
    		transform.localScale = scale;
        }

        float Depth() {
            switch (depthMode) {
            case DepthModeEnum.Normalized:
                return Mathf.Lerp (targetCam.nearClipPlane, targetCam.farClipPlane, normalizedDepth);
            default:
                return depth;
            }
        }
    }
}
