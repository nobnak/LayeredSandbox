﻿Shader "Unlit/HightBasedColoring" {
	Properties 	{
		_MainTex ("Texture", 2D) = "white" {}
        _DistTex ("Distribution", 2D) = "black" {}
        _DistScale ("Dist Scale", Float) = 1
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

            sampler2D _DistTex;
            float _DistScale;
			
			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target {
				float c = tex2D(_MainTex, i.uv).r;
				float dc = fwidth(c);
                return tex2D(_DistTex, float2(_DistScale * dc, c));
			}
			ENDCG
		}
	}
}
