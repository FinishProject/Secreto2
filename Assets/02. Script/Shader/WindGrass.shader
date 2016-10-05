Shader "Custom/WindGrass" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
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
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord  : TEXCOORD0;
			};

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

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

				float num = IN.vertex.z;

				if ((num - _MinY) < 0.0) {
					float3 worldPos = mul(unity_ObjectToWorld, IN.vertex).xyz;
					float x = sin(worldPos.x / _WorldScale + (_Time.y*_Speed)) * -(num - _MinY) * _Scale * 0.01;
					float y = cos(worldPos.y / _WorldScale + (_Time.y*_Speed)) * -(num + _MinY) * _Scale * 0.01;

					OUT.vertex.x += x * _xScale;
					OUT.vertex.y += y * _yScale;
				}

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;

			fixed4 SampleSpriteTexture(float2 uv)
			{
				fixed4 color = tex2D(_MainTex, uv);

				#if ETC1_EXTERNAL_ALPHA
				color.a = tex2D(_AlphaTex, uv).r;
				#endif

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
				c.rgb *= c.a;
				return c;
			}
			
		ENDCG
		}
	}
}