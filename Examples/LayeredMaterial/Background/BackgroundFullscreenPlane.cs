using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PointRegistrationSubmod {
        
    [ExecuteInEditMode]
    public class BackgroundFullscreenPlane : BasePlane {
    	[Range(0f, 0.99f)]
    	public float normalizedDepth = 0f;
    	public Camera targetCam;

        void Update() {
            var depth = Mathf.Lerp (targetCam.nearClipPlane, targetCam.farClipPlane, normalizedDepth);
    		var scale = targetCam.ViewportToWorldPoint (new Vector3 (1f, 1f, depth))
    			- targetCam.ViewportToWorldPoint (new Vector3 (0f, 0f, depth));
    		scale = targetCam.transform.InverseTransformDirection (scale);

            transform.position = targetCam.ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, depth));
            transform.rotation = targetCam.transform.rotation;
    		transform.localScale = scale;
        }
    }
}
