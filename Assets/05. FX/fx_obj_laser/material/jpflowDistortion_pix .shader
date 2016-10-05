Shader "jpshader/JP_FlowDistortion" {
	Properties {
	
		_Color("전체 칼라,알파도 지원됨",color) = (1,1,1,1)
		_MainTex ("첫번째 텍스쳐", 2D) = "white" {}
		_tex1U("첫번째 텍스쳐 가로 스크롤 속도",float) = 0 
		_tex1V("첫번째 텍스쳐 세로 스크롤 속도",float) = 0
		_rot1("첫번째 텍스쳐 회전 속도",float) = 0
		
		_MainTex2 ("두번째 텍스쳐", 2D) = "white" {}
		_tex2U("두번째 텍스쳐 가로 스크롤 속도",float) = 0 
		_tex2V("두번째 텍스쳐 세로 스크롤 속도",float) = 0
		_rot2("두번째 텍스쳐 회전 속도",float) = 0
		
		_MainTex3("이미지 전체를 구기는 텍스쳐, 안써도 됨.", 2D) = "black" {}
		_tex3pow("텍스쳐의 구김영향력", Range(0,1)) = 0
		_tex3U("세번째 텍스쳐 가로 스크롤 속도",float) = 0 
		_tex3V("세번째 텍스쳐 세로 스크롤 속도",float) = 0
		
		[Space(50)]
		[Header(Add_ SrcAlpha_One)]
		[Header(Add_ One_One)]
		[Header(AlphaBlend_ SrcAlpha_OneMinusSrcAlpha)]
		[Header(Multiply_ DstColor_Zero)]
		
		
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("SrcBlend mode", Float) = 1
		 [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("DstBlend mode", Float) = 1
		 [HideInInspector][Toggle] _Zwrite("Zwrite", float) = 0	

		
	}
	
	
	Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend [_SrcBlend] [_DstBlend]

	ZWrite [_Zwrite]

	AlphaTest Greater .01
	ColorMask RGB
	Cull Off Lighting Off 
	
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			fixed4 _TintColor;
			sampler2D _MainTex;
			sampler2D _MainTex2;
			sampler2D _MainTex3;
			float _tex1U;
			float _tex1V;
			float _rot1;
			float _tex2U;
			float _tex2V;
			float _rot2;
			float _tex3U;
			float _tex3V;
			float4 _Color;
			float _tex3pow;
			
			float4 _MainTex_ST;
			float4 _MainTex2_ST;
			float4 _MainTex3_ST;
			
			float2 rotateUVs(float2 Texcoords, float2 center, float theta) { 
			 // compute sin and cos for this angle 
			 float2 sc; 
			 sincos( (theta/180.0f*3.14159f), sc.x, sc.y ); 
			// pi to dgree
			//sincos(x,s,c) : sin(x)와 cos(x)를 동시에 s, c로 리턴한다. 여기서 s, c는 x와 동일한 차원의 타입이어야 한다.
			// move the rotation center to the origin : 중점이동 (center는 기초값을 0.5로 하면 중심이 되것지)
			 float2 uv = Texcoords - center;  
			  
			 // rotate the uv : 기본 UV 좌표와의 dot연산 
			 float2 rotateduv; 
			 rotateduv.x = dot( uv, float2( sc.y, -sc.x ) ); 
			 rotateduv.y = dot( uv, sc.xy );  
			  
			 // move the uv's back to the correct place 
			 rotateduv += center; 
			  
			 return rotateduv; 
			} 
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord2 : TEXCOORD1;
				float2 texcoord3 : TEXCOORD2;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord2 : TEXCOORD1;
				float2 texcoord3 : TEXCOORD2;
				UNITY_FOG_COORDS(3)
	
			};
			
			

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.texcoord2 = TRANSFORM_TEX(v.texcoord2,_MainTex2);
				o.texcoord3 = TRANSFORM_TEX(v.texcoord3,_MainTex3);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}


			
			fixed4 frag (v2f i) : SV_Target
			{
			
				//rotate UV 
				float2 firstUVRotate = rotateUVs(i.texcoord,0.5,_Time.y*_rot1);
				float2 secondUVRotate = rotateUVs(i.texcoord2,0.5,_Time.y*_rot2);
				
				half4 e = tex2D (_MainTex3, float2(i.texcoord3.x-_Time.y*_tex3U ,i.texcoord3.y -_Time.y*_tex3V)) * 2-1;
				e *= _tex3pow;
				
				
				half4 c = tex2D (_MainTex, float2((firstUVRotate.x+e.r-_Time.y*_tex1U),(firstUVRotate.y-_Time.y*_tex1V)));
				half4 d = tex2D (_MainTex2, float2 ((secondUVRotate.x+e.r-_Time.y*_tex2U),(secondUVRotate.y-_Time.y*_tex2V)));
				float4 f;
				f.rgb = c.rgb * d.rgb;
				f.rgb = saturate(f.rgb *_Color.rgb) * i.color.rgb;
//				f.a = (f.r+f.g+f.b)/3* _Color.a  ;
				f.a = c.a * d.a* _Color.a * i.color.a ;						
												
																				
												
																				
//				fixed4 col = 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord);
				UNITY_APPLY_FOG_COLOR(i.fogCoord, f, fixed4(0,0,0,0)); // fog towards black due to our blend mode
				return f;
			}
			ENDCG 
		}
	}	
}
}

	
	
	
//	SubShader {
//		Tags { "RenderType"="Transparent" "Queue"="Transparent" "Queue"="Transparent" }
//		LOD 200
//		blend SrcAlpha One
//		zwrite off
//		cull off
//		AlphaTest Greater .01
//		ColorMask RGB
//		 Lighting Off
//
//		
//		CGPROGRAM
//		#pragma surface surf Nolight keepalpha
//		#pragma target 3.0
//		
//		float4 LightingNolight ( SurfaceOutput s, float3 lightDir, float atten){
//		float4 c;
//		c.rgb = 0;
//		c.a = s.Alpha;
//		return c;		
//		}
//		
//			float2 rotateUVs(float2 Texcoords, float2 center, float theta) { 
//			 // compute sin and cos for this angle 
//			 float2 sc; 
//			 sincos( (theta/180.0f*3.14159f), sc.x, sc.y ); 
//			// pi to dgree
//			//sincos(x,s,c) : sin(x)와 cos(x)를 동시에 s, c로 리턴한다. 여기서 s, c는 x와 동일한 차원의 타입이어야 한다.
//			// move the rotation center to the origin : 중점이동 (center는 기초값을 0.5로 하면 중심이 되것지)
//			 float2 uv = Texcoords - center;  
//			  
//			 // rotate the uv : 기본 UV 좌표와의 dot연산 
//			 float2 rotateduv; 
//			 rotateduv.x = dot( uv, float2( sc.y, -sc.x ) ); 
//			 rotateduv.y = dot( uv, sc.xy );  
//			  
//			 // move the uv's back to the correct place 
//			 rotateduv += center; 
//			  
//			 return rotateduv; 
//			} 
//				
//
//		sampler2D _MainTex;
//		sampler2D _MainTex2;
//		sampler2D _MainTex3;
//		float _tex1U;
//		float _tex1V;
//		float _rot1;
//		float _tex2U;
//		float _tex2V;
//		float _rot2;
//		float _tex3U;
//		float _tex3V;
//		float4 _Color;
//		float _tex3pow;
////		float _Cutoff;
//
//		struct Input {
//			float2 uv_MainTex;
//			float2 uv_MainTex2;
//			float2 uv_MainTex3;
//		};
//
//		void surf (Input IN, inout SurfaceOutput o) {
//			half4 e = tex2D (_MainTex3, float2(IN.uv_MainTex3.x-_Time.y*_tex3U ,IN.uv_MainTex3.y -_Time.y*_tex3V)) * 2-1;
//			e *= _tex3pow;
//			
//			//rotate UV 
//			float2 firstUVRotate = rotateUVs(IN.uv_MainTex,0.5,_Time.y*_rot1);
//			float2 secondUVRotate = rotateUVs(IN.uv_MainTex2,0.5,_Time.y*_rot2);
//			
//			half4 c = tex2D (_MainTex, float2((firstUVRotate.x+e.r-_Time.y*_tex1U),(firstUVRotate.y-_Time.y*_tex1V)));
//			half4 d = tex2D (_MainTex2, float2 ((secondUVRotate.x+e.r-_Time.y*_tex2U),(secondUVRotate.y-_Time.y*_tex2V)));
//			float4 f;
//			f.rgb = c.rgb * d.rgb;
//			//f.a = c.a*d.a;
//			o.Emission = saturate(f.rgb * _Color.rgb);
////			o.Alpha =  _Color.a ;
//			o.Alpha = (f.r+f.g+f.b)/3* _Color.a  ;
//
//		}
//		ENDCG
//	} 
//	FallBack "Particles/Additive"
//}
