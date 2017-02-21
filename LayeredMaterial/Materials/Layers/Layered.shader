Shader "Unlit/Layered" {
	Properties {
		_MainTex ("Texture", 2D) = "black" {}
        _BlendTex ("Blend", 2D) = "black" {}

        _Tex1 ("Layer 1", 2D) = "black" {}
        _Tex2 ("Layer 2", 2D) = "black" {}
        _Tex3 ("Layer 3", 2D) = "black" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }

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
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

            sampler2D _BlendTex;
            sampler2D _Tex1;
            sampler2D _Tex2;
            sampler2D _Tex3;
			
			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target {
				float4 c = tex2D(_MainTex, i.uv);
                float4 b = tex2D(_BlendTex, i.uv);
                float bsum = saturate(dot(b.rgb, 1));
                float4x4 cmat = float4x4(tex2D(_Tex1, i.uv), tex2D(_Tex2, i.uv), tex2D(_Tex3, i.uv), c);

                return mul(float4(b.rgb,1.0-bsum), cmat);

                c = lerp(c, tex2D(_Tex1, i.uv), b.r);
                c = lerp(c, tex2D(_Tex2, i.uv), b.g);
                c = lerp(c, tex2D(_Tex3, i.uv), b.b);
                return c;
			}
			ENDCG
		}
	}
}
