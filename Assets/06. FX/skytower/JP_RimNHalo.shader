Shader "jpshader/JP_RimNHalo" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("텍스쳐", 2D) = "white" {}
		_BumpMap ("노말맵 (_BumpMap)", 2D) = "bump"{}
		[PowerSlider(3.0)] _Rimpower ("두께 조절",Range(0,20)) = 1
		[Toggle]_RimInverse ("림 효과로 만들까요?",float) = 0	
		_Blink("깜빡거리는 속도",float) = 0
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 200
		
		blend SrcAlpha One
		
		CGPROGRAM
		#pragma surface surf jp keepalpha noambient


		sampler2D _MainTex;
		sampler2D _BumpMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;
		};


		fixed4 _Color;
		float _Rimpower;
		float _RimInverse;
		float _Blink;

		void surf (Input IN, inout SurfaceOutput o) {

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Emission = c.rgb;
			o.Normal = UnpackNormal( tex2D (_BumpMap, IN.uv_BumpMap));
			
			float rim =  abs(_RimInverse-abs(dot( o.Normal , IN.viewDir)));
			float blink = saturate(pow(rim,_Rimpower) *_Color.a * (cos(_Time.y*_Blink)*0.5+0.5));

			o.Alpha = c.a * blink;
		}
		
		float4 Lightingjp( SurfaceOutput s , float3 lightDir, float atten){
		return float4(0,0,0,s.Alpha);
		}
		
		ENDCG
	} 
	FallBack "Transparent"
}
