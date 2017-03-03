using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PointRegistrationSubmod {
        
    [ExecuteInEditMode]
    public class BasePlane : MonoBehaviour {
        public string textureName = "_MainTex";
    	public Material planeMat;

    	protected QuadMesh quad;
    	protected MeshFilter mfilter;
    	protected MeshRenderer mrenderer;

    	#region Unity
    	protected virtual void OnEnable() {
    		quad = new QuadMesh ();

    		if ((mfilter = GetComponent<MeshFilter> ()) == null)
    			mfilter = gameObject.AddComponent<MeshFilter> ();
    		if ((mrenderer = GetComponent<MeshRenderer> ()) == null)
    			mrenderer = gameObject.AddComponent<MeshRenderer> ();

    		mfilter.sharedMesh = quad.M;
    		mrenderer.sharedMaterial = planeMat;
    	}
    	protected virtual void OnDisable() {
    		quad.Dispose ();
    	}
    	#endregion

        public virtual void SetTexture(Texture tex) {
            planeMat.SetTexture (textureName, tex);
        }
        public virtual void SetTexture(string textureName, Texture tex) {
            planeMat.SetTexture (textureName, tex);
        }
    }
}
