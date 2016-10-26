Shader "Custom/elevator2" {
	Properties {
		_Ecolor("eEColor", Color)= (1,1,1,1)
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_MainTex2 ("Albedo" , 2D ) = "white"{} 
		_BumpTex ("Normal", 2D ) = "Bump"
	
	}
	SubShader {
		Tags { "RenderType"="Opaque" }

		CGPROGRAM
	
		#pragma surface surf standard fullforwardshadows
		#pragma target 3.0

		float4 _Ecolor;
		sampler2D _MainTex;
		sampler2D _MainTex2;
		sampler2D _BumpTex;
		struct Input {
			float2 uv_MainTex;
			float2 uv_MainTex2;
			float2 uv_BumpTex;
			float3 viewDir;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			fixed4 d = tex2D (_MainTex2, IN.uv_MainTex2);

			o.Normal = UnpackNormal(tex2D(_BumpTex , IN.uv_BumpTex));
			float rim = 1- abs(dot(normalize(IN.viewDir), o.Normal));
			rim = pow( rim, 1);

			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Emission = d * _Ecolor * rim;
			o.Alpha = c.a;

		}
		ENDCG
	}
	FallBack "Diffuse"
}
