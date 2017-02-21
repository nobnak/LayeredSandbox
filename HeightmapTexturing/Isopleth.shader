Shader "Unlit/Isopleth" {
	Properties 	{
		_MainTex ("Texture", 2D) = "white" {}

        _Modulo ("Modulo", Range(0, 1)) = 0.5
        _ContourWidth ("Contour Width", Float) = 1.5
        _ContourColor ("Contour Color", Color) = (1,1,1,1)
	}
	SubShader 	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass 		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

            float _Modulo;
            float _ContourWidth;
            float4 _ContourColor;
			
			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

            float contour(float v) {
                float m = saturate(fmod(v, _Modulo) / _Modulo);
                return saturate(smoothstep(0, _ContourWidth * fwidth(m), m));
            }
			
			float4 frag (v2f i) : SV_Target {
				float c = tex2D(_MainTex, i.uv).r;
				float m = contour(c);
                return lerp(_ContourColor, c, m);
			}
			ENDCG
		}
	}
}
