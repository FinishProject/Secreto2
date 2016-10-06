// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/SurfaceWindShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_MinY("Minimum Y Value", float) = 0.0

		_xScale("X Amount", Range(-1,1)) = 0.5
		_yScale("Z Amount", Range(-1,1)) = 0.5

		_Scale("Effect Scale", float) = 1.0
		_Speed("Effect Speed", float) = 1.0

		_WorldScale("World Scale", float) = 1.0
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}
		SubShader{
			Tags{ "RenderType" = "Opaque" "Queue" = "Geometry+1" "ForceNoShadowCasting" = "True" }
			LOD 200
			CGPROGRAM
			//#pragma surface surf Standard fullforwardshadows
			#pragma surface surf Lambert vertex:vert
			//#pragma vertex vert
			#pragma target 3.0

			struct Input {
				float2 uv_MainTex;
			};

			sampler2D _MainTex;
			fixed4 _Color;
			half _Glossiness;
			half _Metallic;
			float _Cutoff;
			float _MinY;
			float _xScale;
			float _yScale;
			float _Scale;
			float _WorldScale;
			float _Speed;
			float _Amount;

			void vert(inout appdata_full v) {
				float num = v.vertex.z;

				if ((num - _MinY) > 0.0) {
					float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					float x = sin(worldPos.x / _WorldScale + (_Time.y*_Speed)) * (num - _MinY) * _Scale * 0.01;
					float y = cos(worldPos.y / _WorldScale + (_Time.y*_Speed)) * (num - _MinY) * _Scale * 0.01;

					v.vertex.x += x * _xScale;
					v.vertex.y += y * _yScale;
				}
			}

			void surf(Input IN, inout SurfaceOutput o) {
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = c.rgb;
				o.Gloss = _Glossiness;

				if(c.a < _Cutoff){
					discard;
				}
				//o.Alpha = c.a;
			}
		ENDCG
	}
	SubShader{
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry+2" "ForceNoShadowCasting" = "True" }
		LOD 200
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows

		struct Input {
			float2 uv_MainTex;
		};

		sampler2D _MainTex;
		fixed4 _Color;
		half _Glossiness;
		half _Metallic;

		void surf(Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
			//Cull Off

		//Pass{
		//	CGPROGRAM
		//	// Physically based Standard lighting model, and enable shadows on all light types
		////#pragma surface surf Lambert vertex:vert
		//	#pragma vertex vert  
		//	#pragma fragment frag 
		//	
		//	//#pragma surface surf Standard fullforwardshadows
		//	//#pragma surface surf Lambert alpha
		//	#pragma target 3.0
		//	#include "UnityCG.cginc"

		//	sampler2D _MainTex;
		//	float _MinY;
		//	float _xScale;
		//	float _yScale;
		//	float _Scale;
		//	float _WorldScale;
		//	float _Speed;
		//	float _Amount;
		//	fixed4 _Color;
		//	float _Cutoff;

		//	struct Input {
		//		float2 uv_MainTex;
		//	};

		//	struct vertexInput {
		//		float4 vertex : POSITION;
		//		float4 texcoord : TEXCOORD0;
		//	};
		//	struct vertexOutput {
		//		float4 pos : SV_POSITION;
		//		float4 tex : TEXCOORD0;
		//	};

		//	vertexOutput vert(vertexInput input)
		//	{
		//		vertexOutput output;

		//		float num = input.vertex.z;

		//		if ((num - _MinY) > 0.0) {
		//			float3 worldPos = mul(_Object2World, input.vertex).xyz;
		//			float x = sin(worldPos.x / _WorldScale + (_Time.y*_Speed)) * (num - _MinY) * _Scale * 0.01;
		//			float y = sin(worldPos.y / _WorldScale + (_Time.y*_Speed)) * (num - _MinY) * _Scale * 0.01;

		//			input.vertex.x += x * _xScale;
		//			input.vertex.y += y * _yScale;
		//		}

		//		output.tex = input.texcoord;
		//		output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
		//		return output;
		//	}

		//	float4 frag(vertexOutput input) : COLOR
		//	{
		//		float4 textureColor = tex2D(_MainTex, input.tex.xy);
		//		if (textureColor.a < _Cutoff)
		//		{
		//			discard;
		//		}
		//		return textureColor;
		//	}

		//	
		//	ENDCG
		//}
		
	Fallback "Transparent/VertexLit"
	//FallBack "Diffuse"
}
