// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/StandardWindShader" {
	Properties{
		_MainTex("Main Tex", 2D) = "white"{}
		_MainColor("Color", Color) = (1,1,1,1)
		_MinY("Minimum Y Value", float) = 0.0

		_xScale("X Amount", Range(-1,1)) = 0.5
		_yScale("Z Amount", Range(-1,1)) = 0.5

		_Scale("Effect Scale", float) = 1.0
		_Speed("Effect Speed", float) = 1.0

		_WorldScale("World Scale", float) = 1.0
			_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" "Queue" = "Transparent" }
		zwrite on cull off

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
	#pragma surface surf Standard fullforwardshadows vertex:vert           
	#pragma target 3.0
	#include "UnityCG.cginc"

		float4 _MainColor;
	float _MinY;
	float _xScale;
	float _yScale;
	float _Scale;
	float _WorldScale;
	float _Speed;
	float _Cutoff;

	struct Input {
		float2 uv_MainTex;
	};

	sampler2D _MainTex;
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

	void surf(Input IN, inout SurfaceOutputStandard o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = c.rgb * _MainColor;
		// Metallic and smoothness come from slider variables
		o.Metallic = 0.0f;
		o.Smoothness = 0.0f;
		
		if (c.a < _Cutoff)
			discard;
	}

	ENDCG
	}
		Fallback "Diffuse"
}
