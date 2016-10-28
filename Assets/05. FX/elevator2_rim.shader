Shader "Custom/elevator2_rim" {
	Properties {
		_Ecolor("eEColor", Color)= (1,1,1,1)
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_MainTex2 ("Albedo" , 2D ) = "white"{} 
		_BumpTex ("Normal", 2D ) = "Bump"
		_Rimpow("rimpow" , float ) = 1

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM

		#pragma surface surf Lambert//Standard fullforwardshadows

		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _MainTex2;
		sampler2D _BumpTex;
		float4 _Ecolor;
		float _Rimpow;

		struct Input {

			float2 uv_MainTex;
			float2 uv_MainTex2;
			float2 uv_BumpTex;
			float3 _viewDir;
			float3 _worldNormal;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;


		void surf (Input IN, inout SurfaceOutput o) {// Standard o) {
			fixed4 c = tex2D (_MainTex , IN.uv_MainTex) * _Color;
			fixed4 d = tex2D (_MainTex2, IN.uv_MainTex2);
			o.Normal = UnpackNormal(tex2D(_BumpTex , IN.uv_BumpTex));
			float rim = 1- saturate(dot(normalize(IN._worldNormal), o.Normal));
			rim = pow( rim, _Rimpow);
			rim = rim * _Ecolor;


			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			o.Emission =rim;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
