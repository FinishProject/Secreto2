Shader "Custom/upper" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_UpperColor("위칼라", color) = (0,0,0,0)
		_Emitt ("Emitt", Range(0,1)) = 0.2
		_Cutoff ("cutoff", Range(0,1)) = 0.5
		_BumpPow("BUMP pow" , float ) = 1
		_BumpTex ("Bump" , 2d ) = "Bump"
		_rimpow ( "rimpow", float ) = 1
		_upperpow("upperpow", float) = 1 

//		_LowerColor("아래칼라" ,color) = (0,0,0,0)

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		cull off
		
		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows

		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpTex; 
		float4 _UpperColor;
		float _Emitt;
		float _BumpPow;
		float _rimpow;
		float _upperpow;
//		float4 _LowerColor;
		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpTex;
			float3 viewDir;
			float3 worldNormal;
			 INTERNAL_DATA
		};
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed rim = abs(dot(IN.worldNormal, IN.viewDir));
			//o.Normal = UnpackNormal(tex2D(_BumpTex,IN.uv_BumpTex)) * _BumpPow;
			rim = pow(1-rim,_rimpow);

			float3 upper = pow(saturate(IN.worldNormal.y),_upperpow) * _UpperColor.rgb;
//			float3 lower =(1- saturate(IN.worldNormal.y)) * _LowerColor.rgb;

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Emission = (c.rgb*_Emitt ) + (rim*c.rgb* (sin(_Time.y*3)*0.5+0.5)) + upper ;
		//o.Emission =1-IN.worldNormal.y;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
