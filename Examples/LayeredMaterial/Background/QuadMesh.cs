using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PointRegistrationSubmod {
        
    public class QuadMesh : System.IDisposable {
    	public readonly Mesh M;

    	public QuadMesh() {
    		M = new Mesh ();
    		M.vertices = new Vector3[] {
    			new Vector3 (-0.5f, -0.5f, 0f), 
    			new Vector3 (-0.5f, 0.5f, 0f),
    			new Vector3 (0.5f, -0.5f, 0f), 
    			new Vector3 (0.5f, 0.5f, 0f)
    		};
    		M.uv = new Vector2[] {
    			new Vector2 (0f, 0f), new Vector2 (0f, 1f), new Vector2 (1f, 0f), new Vector2 (1f, 1f)
    		};
    		M.triangles = new int[] {
    			0, 1, 3, 0, 3, 2
    		};
    		M.RecalculateBounds ();
    	}

    	#region IDisposable implementation
    	public void Dispose () {
    		if (Application.isPlaying)
    			Object.Destroy (M);
    		else
    			Object.DestroyImmediate (M);
    	}
    	#endregion
    }
}
