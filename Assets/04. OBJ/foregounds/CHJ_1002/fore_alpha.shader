Shader "Custom/fore_alpha" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"  = "Transparent" }


		  Pass
      { 
         Name "ShadowCaster"
         Tags { "LightMode" = "ShadowCaster" }
         
         CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            fixed _Cutoff;

            struct v2f { 
               V2F_SHADOW_CASTER; 
               float2 uv : TEXCOORD1;
            };
            
            v2f vert(appdata_full v)
            {
               v2f o;
               o.pos = mul( UNITY_MATRIX_MVP, v.vertex ); 
               o.uv = v.texcoord;
               TRANSFER_SHADOW_CASTER(o)
               return o;
            }
            
            float4 frag(v2f IN) : COLOR
            {
               fixed4 c = tex2D( _MainTex, IN.uv );
               clip( c.a - _Cutoff );
               SHADOW_CASTER_FRAGMENT(IN)
            }
         ENDCG 
      }    



		CGPROGRAM

		#pragma surface surf Lambert alpha:blend noambient noshadow

		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};


		void surf (Input IN, inout SurfaceOutput o) {

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;

			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
