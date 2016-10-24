Shader "Custom/test" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Cutoff("cutoff",float) = 0.5

		_MinY("Minimum Y Value", float) = 0.0

		_xScale("X Amount", Range(-1,1)) = 0.5
		_yScale("Z Amount", Range(-1,1)) = 0.5

		_Scale("Effect Scale", float) = 1.0
		_Speed("Effect Speed", float) = 1.0

		_WorldScale("World Scale", float) = 1.0

	}
		SubShader{
		Tags{ "RenderType" = "TransparentCutout" "Queue" = "AlphaTest" }
		LOD 200
		//cull off

		CGPROGRAM
#pragma surface surf Standard fullforwardshadows alphatest:_Cutoff vertex:vert
#pragma target 3.0

		sampler2D _MainTex;
	float _MinY;
	float _xScale;
	float _yScale;
	float _Scale;
	float _WorldScale;
	float _Speed;

	void vert(inout appdata_full v) {
		/*float num = v.vertex.z;

		if ((num - _MinY) < 0.0) {
		float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		float x = sin(worldPos.x / _WorldScale + (_Time.y*_Speed)) * (num - _MinY) * _Scale * 0.01;
		float y = cos(worldPos.y / _WorldScale + (_Time.y*_Speed)) * (num - _MinY) * _Scale * 0.01;

		v.vertex.x += x * _xScale;
		v.vertex.y += y * _yScale;*/

		//v.vertex.x += _xScale;
	}
	}

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
		o.Alpha = c.a;
	}
	ENDCG
}
FallBack "Transparent/Cutout/VertexLit"
}
