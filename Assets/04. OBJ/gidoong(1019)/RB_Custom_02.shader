Shader "RealBright/RB_Custom_02" {
	Properties{
		_Color("Albedo Color", Color) = (1,1,1,1)
		_Light_Color("Light Color", Color) = (1,1,1,1)
		_Light_Pow("Light Pow" , Range(0.01,5)) = 1
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	_WarpTex("_WarpTex" , 2D) = "white" {}

	_Spec_Map("Specular_Mask", 2D) = "white" {}
	_Specular_Color("Specular Color", Color) = (1,1,1,1)
		_Spec_Power("Specular Power" , Range(1,100)) = 30.0

		//      카메라 방향 빛 (ViewDir Light)
		_Second_Specular_Color("Second Specular(View) Color", Color) = (1,1,1,1)
		_Second_Spec_Power("Second Specular(View) Power" , Range(1,100)) = 30

		_Occlusion_Map("Occlussion", 2D) = "white" {}
	_Occlusion_Color("Occlusion Color", Color) = (1,1,1,1)
		_Occlusion_Power("Occlusion Multiply" , Range(0,5)) = 0.0

		_Emission_Color("Emission Color", Color) = (1,1,1,1)
		_Emission_Intencity("Emission Intencity" , Range(0,3)) = 0.0
		_Emission_Map("Emission", 2D) = "black" {}

	//      흐르는 Emission
	_Emission_Mask("Emission Mask", 2D) = "white" {}
	_UV_Offset_X("x offset" , int) = 0.0
		_UV_Offset_Y("y offset" , int) = 0.0

		_BumpMap("Normal" , 2D) = "bump" {}
	_Bump_Intencity("Bump" , Range(0,1)) = 0.0

		_Rim_Color("Rim Color", Color) = (1,1,1,1)
		_Rim_Pow("Rim Multiply" , Range(0.01,10)) = 0.0
		_Rim_Power("Rim Power" , Range(0.01,10)) = 0.0
		_Rim_Intencity("Rim Intencity" , Range(0,1)) = 0.0

	}

		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		cull off
		zwrite on
		ztest LEqual

		CGPROGRAM

#pragma surface surf RealBright fullforwardshadow   noforwardadd
#pragma target 4.0

		sampler2D _MainTex;
	sampler2D _Spec_Map;
	sampler2D _Occlusion_Map;
	sampler2D _Emission_Map;
	sampler2D _Emission_Mask;
	sampler2D _BumpMap;
	sampler2D _WarpTex;

	fixed4 _Color;
	fixed4 _Light_Color;
	fixed4 _Reflection_Color;
	fixed4 _Specular_Color;
	fixed4 _Second_Specular_Color;
	fixed4 _Occlusion_Color;
	fixed4 _Emission_Color;
	fixed4 _Rim_Color;


	float _Light_Pow;
	float _Reflection_Power;
	float _Spec_Power;
	float _Second_Spec_Power;
	float _Occlusion_Power;
	float _UV_Offset_X;
	float _UV_Offset_Y;
	float _Emission_Intencity;
	float _Bump_Intencity;
	float _Rim_Power;
	float _Rim_Intencity;
	float _Rim_Pow;

	struct Input {
		float2 uv_MainTex;
		float2 uv_Spec_Map;
		float2 uv_Occlusion_Map;
		float2 uv_Emission_Map;
		float2 uv_Emission_Mask;
		float2 uv_BumpMap;


		float2 uv_AnsioTex;

		float3 lightDir;
		float3 viewDir;
		float3 worldNormal;

		INTERNAL_DATA
	};

	float _Spec_Mask;



	void surf(Input IN, inout SurfaceOutput o) {

		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);// *_Color;
		float4 Spec_Map = tex2D(_Spec_Map, IN.uv_Spec_Map);

		float3 ao = tex2D(_Occlusion_Map,IN.uv_Occlusion_Map);

		float3 e = tex2D(_Emission_Map, IN.uv_Emission_Map) * _Emission_Color;
		float3 em = tex2D(_Emission_Mask, float2(IN.uv_Emission_Mask.x + (_Time.y * _UV_Offset_X),
			IN.uv_Emission_Mask.y + (_Time.y * _UV_Offset_Y)));

		float3 n = UnpackNormal(tex2D(_BumpMap,IN.uv_BumpMap));
		o.Normal = float3(n.r * _Bump_Intencity, n.g *_Bump_Intencity, n.b);

		float3 worldNor = WorldNormalVector(IN,o.Normal);

		float3 Up_Color = _Light_Color.rgb * pow(saturate(worldNor.y), _Light_Pow);

		float3 occlusion = (1,1,1);
		occlusion = ao.r + ((1 - ao.r) * _Occlusion_Color);

		//      Occulsion의 검은부분을 반전시켜, Occulsion에 더하는 방법으로, Occulsion 컬러를 조절 할 수 있게끔 만들어 주고,
		//      Occlusion 을 Albedo 와 Gloss에 곱해주어, 빛의 영향을 안 받는 것 처럼 보이게끔 해준다.

		o.Albedo = (c.rgb + Up_Color) * pow(occlusion,_Occlusion_Power);
		//		o.Albedo= c.rgb;

		o.Gloss = Spec_Map.a * pow(occlusion,_Occlusion_Power);
		o.Emission = (e.rgb * _Emission_Intencity) * em.rgb + Up_Color;
		o.Specular = ao.g;

		o.Alpha = c.a;
	}








	float4 LightingRealBright(SurfaceOutput s , float3 lightDir , float3 viewDir , float atten) {


		// - - - - Diffuse - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
		float NdotL = (dot(normalize(s.Normal) , normalize(lightDir)));
		float R_NdotL = 1 - (NdotL * 0.5 + 0.5);
		float Half_Lambert = NdotL * 0.5 + 0.5;


		float4 wrap = tex2D(_WarpTex , float2(Half_Lambert,0.5));


		float3 Diffuse_Color = (1,1,1);
		float3 Reflection_Color = (1,1,1);
		Diffuse_Color = s.Albedo  * _LightColor0.rgb *wrap;
		//Reflection_Color = saturate(pow(R_NdotL,11 - _Reflection_Power)) * _Reflection_Color;

		//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 


		//  - - - - Specular - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  
		float3 Half = normalize(lightDir + viewDir);
		float Spec = saturate(dot(Half,s.Normal));
		float3 Specular_Color = (1,1,1);
		Specular_Color = pow(Spec,  _Spec_Power) * _Specular_Color * s.Gloss;

		float3 Second_Specular = saturate(dot(s.Normal,viewDir));
		float3 Second_Specular_Color = (1,1,1);
		Second_Specular_Color = pow(Second_Specular, _Second_Spec_Power) * _Second_Specular_Color * s.Gloss;

		//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

		//      Spec 과 Second_Specular 에 saturate 를 걸어서 0~1 이상 및 이하로 나오지 않게끔 잘라주어 최종결과가 이상하지 않게끔 해준다.

		//  - - - - Rim - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		float3 Rim_Color = (1,1,1);
		float3 Rim_Dot = saturate(dot(s.Normal,viewDir));
		float3 Rim_Mask = s.Specular;
		float Rim = pow(1 - Rim_Dot,10.01 - _Rim_Power);

		Rim_Color = Rim * _Rim_Color * _Rim_Intencity * (Rim_Mask * _Rim_Pow);
		//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

		//      Rim Dot에 saturate를 걸어서, 뒷면에 연산이 0이하로 넘어가 밝아지는 것을 방지해준다.

		//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

		//      최종 결과 산출 

		float4 FinalAlbedo = 1;
		//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

		FinalAlbedo.rgb = Diffuse_Color
			+ Specular_Color
			+ Second_Specular_Color
			+ Rim_Color;

		FinalAlbedo.a = s.Alpha;

		return FinalAlbedo;
	}

	ENDCG
	}
		FallBack "Diffuse"
}