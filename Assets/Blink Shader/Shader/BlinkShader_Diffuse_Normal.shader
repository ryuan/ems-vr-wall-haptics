// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Shader created with Shader Forge Beta 0.17 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.17;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:1,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,uamb:True,mssp:True,ufog:True,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:,rntp:1,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300;n:type:ShaderForge.SFN_Final,id:1,x:32510,y:32857|diff-4-OUT,normal-20-RGB,emission-33-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:33361,y:32229,ptlb:Diffuse,tex:b66bceaf0cc0ace4e9bdc92f14bba709,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:3,x:33361,y:32416,ptlb:MainColor,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:4,x:33164,y:32329|A-2-RGB,B-3-RGB;n:type:ShaderForge.SFN_Tex2d,id:11,x:33505,y:32797,ptlb:Emission,tex:e6e7abfd01541984db62a8d677d6b2d7,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:12,x:33505,y:33001,ptlb:Emission Color,c1:0.4779412,c2:0.4779412,c3:0.4779412,c4:1;n:type:ShaderForge.SFN_Multiply,id:13,x:33281,y:32879|A-11-RGB,B-12-RGB;n:type:ShaderForge.SFN_Tex2d,id:20,x:33361,y:32618,ptlb:Normal,tex:bbab0a6f7bae9cf42bf057d8ee2755f6,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Time,id:28,x:33560,y:33276;n:type:ShaderForge.SFN_Sin,id:29,x:33217,y:33338|IN-30-OUT;n:type:ShaderForge.SFN_Multiply,id:30,x:33380,y:33338|A-28-T,B-31-OUT;n:type:ShaderForge.SFN_ValueProperty,id:31,x:33560,y:33464,ptlb:Speed Blink,v1:10;n:type:ShaderForge.SFN_Abs,id:32,x:33061,y:33338|IN-29-OUT;n:type:ShaderForge.SFN_Multiply,id:33,x:32859,y:33151|A-43-OUT,B-32-OUT;n:type:ShaderForge.SFN_ValueProperty,id:42,x:33281,y:33102,ptlb:Emission Power,v1:1;n:type:ShaderForge.SFN_Multiply,id:43,x:33062,y:33003|A-13-OUT,B-42-OUT;proporder:2-3-20-11-12-42-31;pass:END;sub:END;*/

Shader "Langvv/BlinkShader_Diffuse_Normal" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _MainColor ("MainColor", Color) = (0.5,0.5,0.5,1)
        _Normal ("Normal", 2D) = "bump" {}
        _Emission ("Emission", 2D) = "white" {}
        _EmissionColor ("Emission Color", Color) = (0.4779412,0.4779412,0.4779412,1)
        _EmissionPower ("Emission Power", Float ) = 1
        _SpeedBlink ("Speed Blink", Float ) = 10
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float4 _MainColor;
            uniform sampler2D _Emission; uniform float4 _Emission_ST;
            uniform float4 _EmissionColor;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _SpeedBlink;
            uniform float _EmissionPower;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_52 = i.uv0;
                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_52.rg, _Normal))).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor + UNITY_LIGHTMODEL_AMBIENT.xyz;
////// Emissive:
                float4 node_28 = _Time + _TimeEditor;
                float3 emissive = (((tex2D(_Emission,TRANSFORM_TEX(node_52.rg, _Emission)).rgb*_EmissionColor.rgb)*_EmissionPower)*abs(sin((node_28.g*_SpeedBlink))));
                float3 finalColor = diffuse * (tex2D(_Diffuse,TRANSFORM_TEX(node_52.rg, _Diffuse)).rgb*_MainColor.rgb) + emissive;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float4 _MainColor;
            uniform sampler2D _Emission; uniform float4 _Emission_ST;
            uniform float4 _EmissionColor;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _SpeedBlink;
            uniform float _EmissionPower;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_53 = i.uv0;
                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_53.rg, _Normal))).rgb;
                float3 normalDirection = normalize( mul( normalLocal, tangentTransform ) );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
                float3 finalColor = diffuse * (tex2D(_Diffuse,TRANSFORM_TEX(node_53.rg, _Diffuse)).rgb*_MainColor.rgb);
/// Final Color:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
