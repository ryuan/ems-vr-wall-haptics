// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:32735,y:33011,varname:node_3138,prsc:2|emission-3079-OUT;n:type:ShaderForge.SFN_Tex2d,id:4778,x:32418,y:32995,ptovrint:False,ptlb:PatternTex,ptin:_PatternTex,varname:node_4778,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:1afddf56b0097e94380332af4dd63371,ntxv:0,isnm:False|UVIN-6448-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:7175,x:32107,y:32995,varname:node_7175,prsc:2,uv:0;n:type:ShaderForge.SFN_Panner,id:6448,x:32263,y:32995,varname:node_6448,prsc:2,spu:0,spv:1|UVIN-7175-UVOUT,DIST-3237-OUT;n:type:ShaderForge.SFN_Time,id:9189,x:31949,y:33086,varname:node_9189,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3237,x:32107,y:33142,varname:node_3237,prsc:2|A-9189-T,B-4475-OUT;n:type:ShaderForge.SFN_Multiply,id:3079,x:32580,y:33111,varname:node_3079,prsc:2|A-4778-RGB,B-1011-OUT;n:type:ShaderForge.SFN_Vector1,id:4475,x:31949,y:33212,varname:node_4475,prsc:2,v1:0.15;n:type:ShaderForge.SFN_Vector3,id:1011,x:32418,y:33155,varname:node_1011,prsc:2,v1:0,v2:0.7058824,v3:1;proporder:4778;pass:END;sub:END;*/

Shader "Gater/Recursion mask" {
    Properties {
        _PatternTex ("PatternTex", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _PatternTex; uniform float4 _PatternTex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_9189 = _Time + _TimeEditor;
                float2 node_6448 = (i.uv0+(node_9189.g*0.15)*float2(0,1));
                float4 _PatternTex_var = tex2D(_PatternTex,TRANSFORM_TEX(node_6448, _PatternTex));
                float3 emissive = (_PatternTex_var.rgb*float3(0,0.7058824,1));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
