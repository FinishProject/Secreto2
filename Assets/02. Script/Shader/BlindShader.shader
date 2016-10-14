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

		v2f vert(Input input)
		{
			v2f output;
			output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
			output.position_in_world_space = mul(unity_ObjectToWorld, input.vertex);
			output.tex = input.texcoord;
			return output;
		}

		float4 frag(v2f input) : COLOR
		{
			// 플레이어와 버텍스의 거리를 구한다.
			float dist = distance(input.position_in_world_space, _PlayerPosition);

			// 범위 안에 있다면 텍스쳐를 그린다.
			if (dist < _VisibleDistance) {
				return tex2D(_MainTex, float4(input.tex));
			}
			// 범위 밖에 있다면 텍스쳐의 알파값을 0으로하여 보이지 않도록 한다.
			else {
				float4 tex = tex2D(_MainTex, float4(input.tex));
				tex.a = 0.0;
				return tex;
			}
		}
		ENDCG
		}
	}
		//FallBack "Transparent/VertexLit"
}
