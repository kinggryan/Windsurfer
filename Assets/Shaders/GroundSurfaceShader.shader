Shader "Custom/GroundSurfaceShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_DesertTex ("Desert (RGB)", 2D) = "white" {}
		_ForestTex ("Forest (RGB)", 2D) = "white" {}
		_ForestAmount ("Forest Amount",Range(0,1)) = 0.0
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _DesertTex;
		sampler2D _ForestTex;

		struct Input {
			float2 uv_DesertTex;
			float2 uv_ForestTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		half _ForestAmount;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 dc = tex2D (_DesertTex, IN.uv_DesertTex) * _Color;
			fixed4 fc = tex2D (_ForestTex, IN.uv_ForestTex) * _Color;
			o.Albedo = dc.rgb * (1 - _ForestAmount) + fc.rgb * (_ForestAmount);
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = dc.a *(1 - _ForestAmount) + fc.a * (_ForestAmount);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
