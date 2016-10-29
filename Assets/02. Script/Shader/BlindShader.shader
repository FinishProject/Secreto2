Shader "Custom/BlindShader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_PlayerPosition("Player Position", vector) = (0,0,0,0)
		_VisibleDistance("Visible Distance", float) = 10.0
		_OutlineWidth("Outlint Width", float) = 3.0
		_OutlineColor("Outline Color", color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
		Pass{
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		// Access the shaderlab properties
		sampler2D _MainTex;
		float4 _PlayerPosition;
		float _VisibleDistance;
		float _OutlineWidth;
		fixed4 _OutlineColor;

		// Input to vertex shader
		struct Input {
			float4 vertex : POSITION;
			float4 texcoord : TEXCOORD0;
		};
		// Input to fragment shader
		struct v2f {
			float4 pos : SV_POSITION;
			float4 position_in_world_space : TEXCOORD0;
			float4 tex : TEXCOORD1;
		};

		// VERTEX SHADER
		v2f vert(Input input)
		{
			v2f output;
			output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
			output.position_in_world_space = mul(unity_ObjectToWorld, input.vertex);
			output.tex = input.texcoord;
			return output;
		}

		// FRAGMENT SHADER
		float4 frag(v2f input) : COLOR
		{
			// Calculate distance to player position
			float dist = distance(input.position_in_world_space, _PlayerPosition);

			// Return appropriate colour
			if (dist < _VisibleDistance) {
				return tex2D(_MainTex, float4(input.tex)); // Visible
			}
			else {
				float4 tex = tex2D(_MainTex, float4(input.tex)); // Outside visible range
				tex.a = 0.0;
				return tex;
			}
		}
		ENDCG
		}
	}
		//FallBack "Transparent/VertexLit"
}
