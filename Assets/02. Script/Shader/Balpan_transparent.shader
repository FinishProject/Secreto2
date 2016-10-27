Shader "Custom/balpan_trans" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader{
		Tags{ "Queue" = "Transparent""RenderType" = "Transparent" }
		LOD 200


		zwrite on
		COLORMASK 0
		CGPROGRAM

#pragma surface surf Lambert 

		sampler2D _MainTex;

	struct Input {
		float2 uv_MainTex;
	};


	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG

		zwrite off
		CGPROGRAM

		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;

	struct Input {
		float2 uv_MainTex;
	};

	half _Glossiness;
	half _Metallic;
	fixed4 _Color;

	void surf(Input IN, inout SurfaceOutputStandard o) {
		// Albedo comes from a texture tinted by color
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		// Metallic and smoothness come from slider variables
		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
		o.Alpha = _Color.a;
	}
	ENDCG


	}
		FallBack "Diffuse"
}
