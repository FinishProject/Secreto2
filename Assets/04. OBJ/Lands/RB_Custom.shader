Shader "RealBright/RB_Custom" {

	Properties {
	
		_MainTex ("Albedo (R)", 2D) = "white" {}

		_MetallicRange ("Metallic" , Range(0,1)) = 0.0
		_SmoothnessRange ("Smoothness" , Range(0,1)) = 0.0
		_BumpRangeTex ("Bump" , Range(0,1)) = 0.0



		_MainTex2 ("Albedo (G)", 2D) = "white" {}

		_MetallicRange2 ("Metallic" , Range(0,1)) = 0.0
		_SmoothnessRange2 ("Smoothness" , Range(0,1)) = 0.0
		_BumpRangeTex2 ("Bump" , Range(0,1)) = 0.0



		_MainTex3 ("Albedo (B)", 2D) = "white" {}

		_MetallicRange3 ("Metallic" , Range(0,1)) = 0.0
		_SmoothnessRange3 ("Smoothness" , Range(0,1)) = 0.0
		_BumpRangeTex3 ("Bump" , Range(0,1)) = 0.0



		_MainTex4 ("Albedo (A) / No Vertex Color = D", 2D) = "white" {}

//		버텍스컬러가 없는 경우에는 버텍스 컬러가 흰색이기 때문에 반전이 필요하다.
//		맥스에서 기본 디폴트로 넘어 올 때, RGBA 가 모두 칠해져 오기 때문에 1개의 텍스쳐만 쓰고 싶을 때는 나머지를 간섭 안받게 꺼줘야 한다.
		[Toggle]_NoVertexColor ("No Vertex Color This Check (1 Albedo Setting)",float) = 0

		_Color ("Color", Color) = (1,1,1,1)
		_MetallicRange4 ("Metallic" , Range(0,1)) = 0.0
		_SmoothnessRange4 ("Smoothness" , Range(0,1)) = 0.0
		_BumpRangeTex4 ("Bump" , Range(0,1)) = 0.0


		_RGBA_Tex ("R=Metal / G=AO / B=Emi / A=Smooth)", 2D) = "white" {}

		_AORange ("Occlusion" , Range(0,1)) = 0.0
		_EmissionRange ("Emission" , Range(0,3)) = 0.0
		_EmissionColor ("Emission Color", Color) = (1,1,1,1)

		_BumpMap ("Detail Normal", 2D) = "bump" {}

		

	}


	SubShader {

		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull Off
		CGPROGRAM
 		#pragma surface surf Standard fullforwardshadows // noambient


		//텍스쳐 연산이 많아지면서 드디어 2.0 셰이더를 탈피 했다.
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _MainTex2;
		sampler2D _MainTex3;
		sampler2D _MainTex4;
		sampler2D _BumpMap;
		sampler2D _RGBA_Tex;		

		float _MetallicRange;
		float _MetallicRange2;
		float _MetallicRange3;
		float _MetallicRange4;

		float _SmoothnessRange;
		float _SmoothnessRange2;
		float _SmoothnessRange3;
		float _SmoothnessRange4;

		float _EmissionRange;
		fixed4 _EmissionColor;

		float _BumpRangeTex;
		float _BumpRangeTex2;
		float _BumpRangeTex3;
		float _BumpRangeTex4;

		float _AORange;

		float _NoVertexColor;
		fixed4 _Color;

	//	이것은 Input Struct (Input 구조체) 
		struct Input {
			float2 uv_MainTex;
			float2 uv_MainTex2;
			float2 uv_MainTex3;
			float2 uv_MainTex4;
			float4 color:COLOR;

			float2 uv_RGBA_Tex;
			float2 uv_BumpMap;


		};


		void surf (Input IN, inout SurfaceOutputStandard o) {


			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			fixed4 d = tex2D (_MainTex2, IN.uv_MainTex2);
			fixed4 e = tex2D (_MainTex3, IN.uv_MainTex3);
			fixed4 f = tex2D (_MainTex4, IN.uv_MainTex4);
			fixed3 n = UnpackNormal(tex2D (_BumpMap, IN.uv_BumpMap));
			fixed4 m = tex2D (_RGBA_Tex, IN.uv_RGBA_Tex);



			// lerp TexTure Blending
//			o.Albedo = lerp(lerp(lerp(c.rgb, d.rgb, IN.color.r),e.rgb,IN.color.g),f.rgb,IN.color.b);


			//Not lerp TexTure Blending
			o.Albedo =  ((c.rgb * IN.color.r) 
					   + (d.rgb * IN.color.g)
					   + (e.rgb * IN.color.b)
					   + (f.rgb * (1 - IN.color.r - IN.color.g - IN.color.b)));			

			// 버텍스 컬러가 없을 경우, f 텍스쳐 하나로만 컨트롤 할 수 있게끔 해 준다.
			// 토글을 껏을때는 알베도에 1을 곱해 원래 텍스쳐가 나오게 끔 해주고, 토글이 켜지면, RGB 텍스쳐를 모두 0으로 만든 뒤, A 텍스쳐만 표기하게끔 한다.
			o.Albedo = o.Albedo * (1 - _NoVertexColor) + (_NoVertexColor * f.rgb * _Color);
//			o.Albedo = f.rgb;


//			RGBA 텍스쳐에서 메탈릭맵(R채널)만 뽑아쓴다. 
			o.Metallic =  (IN.color.r * m.r * _MetallicRange)
						 +(IN.color.g * m.r * _MetallicRange2)
						 +(IN.color.b * m.r * _MetallicRange3)
						 +(IN.color.a * m.r * _MetallicRange4);

			// 텍스쳐를 한개만 쓸 경우에 RGB 쪽에 슬라이드에 영향을 안받게끔 텍스쳐를 꺼준다.
			o.Metallic = o.Metallic * (1 - _NoVertexColor) + (_NoVertexColor * (IN.color.a * m.r * _MetallicRange4));

//			RGBA 텍스쳐에서 스무스니스맵(A채널)만 뽑아쓴다. 
			o.Smoothness = (IN.color.r * m.a * _SmoothnessRange)
						  +(IN.color.g * m.a * _SmoothnessRange2)
						  +(IN.color.b * m.a * _SmoothnessRange3)
						  +(IN.color.a * m.a * _SmoothnessRange4);

			// 텍스쳐를 한개만 쓸 경우에 RGB 쪽에 슬라이드에 영향을 안받게끔 텍스쳐를 꺼준다.
			o.Smoothness = o.Smoothness * (1 - _NoVertexColor) + (_NoVertexColor * (IN.color.a * m.a * _SmoothnessRange4));



			o.Normal =  lerp
							(lerp
								(lerp
									(lerp(float3(0,0,1),n,IN.color.r * _BumpRangeTex),
										   lerp(float3(0,0,1),n,IN.color.g * _BumpRangeTex2),IN.color.g),
										   lerp(float3(0,0,1),n,IN.color.b * _BumpRangeTex3),IN.color.b),
										   lerp(float3(0,0,1),n,IN.color.a * _BumpRangeTex4),IN.color.a);


//			아래 방법대로 해도 나오기는 하지만, 노말들이 서로 겹쳐지는 부위는 노말이 증폭되어 어두운곳이 밝아지는 둥 이상한 결과가 나온다.
//			o.Normal =  lerp(float3(0,0,1),n,IN.color.r * _BumpRangeTex)
//					  + lerp(float3(0,0,1),n,IN.color.g * _BumpRangeTex2)
//					  + lerp(float3(0,0,1),n,IN.color.b * _BumpRangeTex3)
//					  + lerp(float3(0,0,1),n,IN.color.a * _BumpRangeTex4);


//			1이 AO 가 없는 상태에서 AO에 검은 부분을 반전시켜 그만큼을 흰색에서 빼주어 검정으로 다시 만들어준다.
			o.Occlusion = 1-(1-m.g)*_AORange;	

//			RGBA 텍스쳐에서 이미션맵(B채널)만 뽑아쓴다. 	
			o.Emission =  m.b * _EmissionRange * _EmissionColor;

			o.Alpha = c.a;
		}

		ENDCG

	}

	FallBack "Diffuse"
}
