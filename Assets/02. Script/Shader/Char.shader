Shader "Custom/Char" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_UpperColor("위칼라", color) = (0,0,0,0)
		_Emitt ("Emitt", Range(0,1)) = 0.2
		_Cutoff ("cutoff", Range(0,1)) = 0.5
//		_LowerColor("아래칼라" ,color) = (0,0,0,0)

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		cull off
		
		CGPROGRAM

		#pragma surface surf Lambert alphatest:_Cutoff addshadow

		#pragma target 3.0

		sampler2D _MainTex;
		float4 _UpperColor;
		float _Emitt;
//		float4 _LowerColor;
		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
			float3 worldNormal;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed rim = abs(dot(IN.worldNormal, IN.viewDir));
			rim = pow(1-rim,2);

			float3 upper = pow(saturate(IN.worldNormal.y),2) * _UpperColor.rgb;
//			float3 lower =(1- saturate(IN.worldNormal.y)) * _LowerColor.rgb;

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Emission = (c.rgb*_Emitt ) + (rim*c.rgb* (sin(_Time.y*3)*0.5+0.5)) + upper ;
//o.Emission =1-IN.worldNormal.y;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
