Shader "Custom/ele2_2call" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_lineColor ("linecolor", Color) = (1,1,1,1)
		_eColor ("Ecolor", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MainTex2 ("Emission", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }

		cull front
		
		CGPROGRAM

		#pragma surface surf Lambert vertex:vert

		sampler2D _MainTex;
		float4 _Color;
		float4 _lineColor;


		struct Input {
			float2 uv_MainTex;
		};

		void vert(inout appdata_full v)
		{

		v.vertex.xyz = v.vertex.xyz + (v.normal*0.01);
		}


		void surf (Input IN, inout SurfaceOutput o) {

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _lineColor;

			o.Albedo = _lineColor;
			o.Alpha = c.a;
		}
		ENDCG

		cull back
		
		CGPROGRAM

		#pragma surface surf  Lambert //  standard fullforwardshadows

		sampler2D _MainTex;
		float4 _Color;
		float4 _lineColor;
		float4 _eColor;
		sampler2D _MainTex2;
	


		struct Input {
			float2 uv_MainTex;
			float2 uv_MainTex2;
		};

		void surf (Input IN, inout SurfaceOutput o) {

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 d = tex2D (_MainTex2, IN.uv_MainTex2) * _eColor;

			o.Albedo = c.rgb;
			o.Emission = d;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
