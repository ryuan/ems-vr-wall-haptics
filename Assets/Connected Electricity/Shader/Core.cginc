#ifndef UTILS_CGINC
#define UTILS_CGINC

uniform float4 _Color;
uniform float _GlowStrength;
uniform float _Height;
uniform float _GlowFallOff;
uniform float _Speed;
uniform float _SampleDist;
uniform float _AmbientGlow;
uniform float _AmbientGlowHeightScale;
uniform float _VertNoise;
uniform sampler3D _NoiseTex;

float4 frag (v2f_img i) : SV_Target
{
	float2 uv = (i.uv - 0.5) * 2;
		
	float2 t = float2(_Speed * _Time.y * 0.6 - _VertNoise * abs(uv.y), _Speed * _Time.y);

	float xs0 = uv.x - _SampleDist;
	float xs1 = uv.x;
	float xs2 = uv.x + _SampleDist;

	float noise0 = tex3D(_NoiseTex, float3(xs0, t)).r;
	float noise1 = tex3D(_NoiseTex, float3(xs1, t)).r;
	float noise2 = tex3D(_NoiseTex, float3(xs2, t)).r;

	float mid0 = _Height * (noise0 * 2 - 1) * (1 - xs0 * xs0);
	float mid1 = _Height * (noise1 * 2 - 1) * (1 - xs1 * xs1);
	float mid2 = _Height * (noise2 * 2 - 1) * (1 - xs2 * xs2);

	float dist0 = abs(uv.y - mid0);
	float dist1 = abs(uv.y - mid1);
	float dist2 = abs(uv.y - mid2);

	float glow = 1.0 - pow(0.25 * (dist0 + 2 * dist1 + dist2), _GlowFallOff);

	float ambGlow = _AmbientGlow * (1 - xs1 * xs1) * (1 - abs(_AmbientGlowHeightScale * uv.y));

	return (_GlowStrength * glow * glow + ambGlow) * _Color;
}

#endif