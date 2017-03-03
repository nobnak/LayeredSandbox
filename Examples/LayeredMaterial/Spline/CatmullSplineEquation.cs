using UnityEngine;
using System.Collections;

public static class CatmullSplineEquation {
    #region Base Equations
    public static float Position(float t, float p0, float p1, float p2, float p3) {
        var tm1 = t - 1f;
        var tm2 = tm1 * tm1;
        var t2 = t * t;

        var m1 = 0.5f * (p2 - p0);
        var m2 = 0.5f * (p3 - p1);

        return (1f + 2f * t) * tm2 * p1 + t * tm2 * m1 + t2 * (3 - 2f * t) * p2 + t2 * tm1 * m2;
    }
    public static float Velosity(float t, float p0, float p1, float p2, float p3) {
        var tm1 = (t - 1f);
        var t6tm1 = 6f * t * tm1;

        var m1 = 0.5f * (p2 - p0);
        var m2 = 0.5f * (p3 - p1);

        return t6tm1 * p1 + (3f * t - 1f) * tm1 * m1 - t6tm1 * p2 + t * (3f * t - 2f) * m2;
    }
    public static float Acceleration(float t, float p0, float p1, float p2, float p3) {
        var t2m1 = 2f * t - 1f;
        var t3 = 3f * t;

        var m1 = 0.5f * (p2 - p0);
        var m2 = 0.5f * (p3 - p1);
        return 6f * t2m1 * p1 + 2f * (t3 - 2f) * m1 - 6f * t2m1 * p2 + 2f * (t3 - 1f) * m2;
    }
    #endregion

    public static Vector2 Position(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3) {
        return new Vector2 (Position (t, p0.x, p1.x, p2.x, p3.x), Position (t, p0.y, p1.y, p2.y, p3.y));
    }
    public static Vector3 Position(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
        return new Vector3 (Position (t, p0.x, p1.x, p2.x, p3.x),
            Position (t, p0.y, p1.y, p2.y, p3.y), Position (t, p0.z, p1.z, p2.z, p3.z));
    }

    public static Vector2 Velosity(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3) {
        return new Vector2 (Velosity (t, p0.x, p1.x, p2.x, p3.x), Velosity (t, p0.y, p1.y, p2.y, p3.y));
    }
    public static Vector3 Velosity(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
        return new Vector3 (Velosity (t, p0.x, p1.x, p2.x, p3.x),
            Velosity (t, p0.y, p1.y, p2.y, p3.y), Velosity (t, p0.z, p1.z, p2.z, p3.z));
    }

    public static Vector2 Acceleration(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3) {
        return new Vector2 (Acceleration (t, p0.x, p1.x, p2.x, p3.x), Acceleration (t, p0.y, p1.y, p2.y, p3.y));
    }
	public static Vector3 Acceleration(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
        return new Vector3 (Acceleration (t, p0.x, p1.x, p2.x, p3.x),
            Acceleration (t, p0.y, p1.y, p2.y, p3.y), Acceleration (t, p0.z, p1.z, p2.z, p3.z));
	}

	public static float Curvature(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
		var a = Acceleration(t, p0, p1, p2, p3);
		var v = Velosity(t, p0, p1, p2, p3);
		var vmag = v.magnitude;
		return Vector3.Cross(v, a).magnitude / (vmag * vmag * vmag);
	}

}