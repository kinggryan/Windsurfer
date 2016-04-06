Shader "Custom/GroundCelShader" {
	Properties{
		_DesertColor("Desert Color", Color) = (1, 1, 1, 1)
		_ForestColor("Forest Color", Color) = (1, 1, 1, 1)
		_ForestAmount("Forest Amount",Range(0,1)) = 0.0
		_DesertTex("Albedo (RGB)", 2D) = "white" {}
		_ForestTex("Albedo (RGB)", 2D) = "white" {}
	}
		SubShader{
		Tags{
		"RenderType" = "Opaque"
	}
		LOD 200

		CGPROGRAM
		#pragma surface surf CelShadingForward
		#pragma target 3.0

		half4 LightingCelShadingForward(SurfaceOutput s, half3 lightDir, half atten) {
			half NdotL = dot(s.Normal, lightDir);
			if (NdotL <= 0.0) NdotL = 0;
			else if (NdotL <= 0.25) NdotL = 0.25;
			else if (NdotL <= 0.5) NdotL = 0.5;
			else NdotL = 1;
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten * 2);
			c.a = s.Alpha;
			return c;
		}

		sampler2D _DesertTex;
		sampler2D _ForestTex;

		struct Input {
			float2 uv_DesertTex;
			float2 uv_ForestTex;
		};

		fixed4 _DesertColor;
		fixed4 _ForestColor;
		half _ForestAmount;

		void surf(Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 dc = tex2D(_DesertTex, IN.uv_DesertTex) * _DesertColor;
			fixed4 fc = tex2D(_ForestTex, IN.uv_ForestTex) * _ForestColor;
			o.Albedo = dc.rgb * (1 - _ForestAmount) + fc.rgb * (_ForestAmount);
			// Metallic and smoothness come from slider variables
			o.Alpha = dc.a *(1 - _ForestAmount) + fc.a * (_ForestAmount);
		}
	ENDCG
	}
	FallBack "Diffuse"
}
