Shader "Custom/standard_rim" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_rimpow ("rimpow", float) = 1
		_rimcol ("rimcol", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_BumpTex ("normal" , 2D ) = "Bump"
	

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpTex;
		float _rimpow;
		float4 _rimcol;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpTex;
			float3 viewDir;
			float3 lightDir;
			float3 worldNormal;

			     INTERNAL_DATA

		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Normal = UnpackNormal(tex2D(_BumpTex , IN.uv_BumpTex));
			float3 worldNor = WorldNormalVector(IN,o.Normal);
			float rim;
			rim =  saturate(dot(normalize(IN.viewDir) , worldNor.y));
			rim = pow(1- rim, _rimpow);
			rim = rim * _rimcol;
		
			o.Albedo = IN.worldNormal.y ;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Emission = rim;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
