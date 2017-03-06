Shader "Layered/Marker/River" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _TimeShifting ("Time UV Shifting", Vector) = (0,0,0,0)
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uvMain : TEXCOORD0;
                float2 uvMask : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
            sampler2D _MaskTex;

			float4 _MainTex_ST;
            float4 _MaskTex_ST;

            float4 _Color;
            float4 _TimeShifting;
			
			v2f vert (appdata v) {
				v2f o;

                float2 uvOffset = _Time.y * _TimeShifting.xy;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uvMain = TRANSFORM_TEX(v.uv + uvOffset, _MainTex);
                o.uvMask = TRANSFORM_TEX(v.uv, _MaskTex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target {
				float4 c = tex2D(_MainTex, i.uvMain);
                float4 cmask = tex2D(_MaskTex, i.uvMask);
				return c * cmask * _Color;
			}
			ENDCG
		}
	}
}
