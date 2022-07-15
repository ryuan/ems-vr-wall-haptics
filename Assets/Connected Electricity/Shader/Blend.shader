Shader "Connected Electricity/Blend" {
	Properties {
		_Color ("Color", Color) = (1, 1, 1, 1)
		_GlowStrength ("Glow Strength", Range(0, 200)) = 144
		_Height ("Height", Range(0, 2)) = 0.44
		_GlowFallOff ("Glow FallOff", Range(0, 0.1)) = 0.01
		_Speed ("Speed", Range(0, 3)) = 1.86
		_SampleDist ("Sample Dist", Range(0, 0.04)) = 0.0076
		_AmbientGlow ("Ambient Glow", Range(0, 1)) = 0.5
		_AmbientGlowHeightScale ("Ambient Glow Height Scale", Range(0, 8)) = 1.68
		_VertNoise ("Vert Noise", Range(0, 1)) = 0.78
		_NoiseTex ("Noise", 3D) = "black" {}
	}
	SubShader {
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Core.cginc"
			ENDCG
		}
	}
	FallBack Off
}