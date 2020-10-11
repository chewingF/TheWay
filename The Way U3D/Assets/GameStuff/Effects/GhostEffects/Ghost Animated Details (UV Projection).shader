// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Ciconia Studio/Effects/Ghost/Ghost Animated Details (UV Projection)" {
    Properties {
        [Space(15)][Header(Main Properties)]
        [Space(10)]_Color ("Color", Color) = (1,1,1,1)
        _EmissiveIntensity ("Emissive Intensity", Range(0, 5)) = 0.5
        [Space(10)]_FresnelStrength ("Fresnel Strength", Range(0, 10)) = 0.5
        [MaterialToggle] _InvertFresnel ("Invert Fresnel", Float ) = 0

        [Space25)]_BumpMap ("Normal map", 2D) = "bump" {}
        _NormalIntensity ("Normal Intensity", Range(0, 2)) = 1

        [Space(25)][Header(Animation Properties)]
        [Space(10)][MaterialToggle] _InvertEffect ("Invert Effect", Float ) = 0
        [Space(10)]_AnimatedNormalmapCloud ("Animated Normal map (Cloud)", 2D) = "bump" {}
        _NormalIntensityCloud ("Normal Intensity", Range(0, 2)) = 1
        [Space(25)]_AnimationSpeed ("Animation Speed", Range(0, 1)) = 0.2
        _RotationDegree ("Rotation (Degree)", Float ) = 0
        [MaterialToggle] _SwitchAnimationFlow ("Switch Animation Flow", Float ) = 1.171035
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
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
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform float _FresnelStrength;
            uniform float4 _Color;
            uniform sampler2D _AnimatedNormalmapCloud; uniform float4 _AnimatedNormalmapCloud_ST;
            uniform float _NormalIntensityCloud;
            uniform float _EmissiveIntensity;
            uniform float _AnimationSpeed;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _NormalIntensity;
            uniform fixed _InvertEffect;
            uniform fixed _SwitchAnimationFlow;
            uniform fixed _InvertFresnel;
            uniform float _RotationDegree;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
                UNITY_FOG_COORDS(6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_9568 = _Time + _TimeEditor;
                float node_7384 = lerp(node_9568.r,node_9568.g,_AnimationSpeed);
                float node_7021_ang = (((_RotationDegree*3.141592654)/180.0)+90.0);
                float node_7021_spd = 1.0;
                float node_7021_cos = cos(node_7021_spd*node_7021_ang);
                float node_7021_sin = sin(node_7021_spd*node_7021_ang);
                float2 node_7021_piv = float2(0.5,0.5);
                float2 node_7021 = (mul(i.uv0-node_7021_piv,float2x2( node_7021_cos, -node_7021_sin, node_7021_sin, node_7021_cos))+node_7021_piv);
                float2 _SwitchAnimationFlow_var = lerp( (node_7021+node_7384*float2(1,1)), (node_7021+node_7384*float2(-1,-1)), _SwitchAnimationFlow );
                float3 _AnimatedNormalmapCloud_var = UnpackNormal(tex2D(_AnimatedNormalmapCloud,TRANSFORM_TEX(_SwitchAnimationFlow_var, _AnimatedNormalmapCloud)));
                float3 node_5650 = lerp(float3(0,0,1),_AnimatedNormalmapCloud_var.rgb,_NormalIntensityCloud);
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float3 normalLocal = (lerp( node_5650, (node_5650*2.0+-1.0), _InvertEffect )+lerp(float3(0,0,1),_BumpMap_var.rgb,_NormalIntensity));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5;
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
////// Emissive:
                float node_5849 = pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelStrength);
                float node_1046 = (node_5849*node_5849*node_5849);
                float3 node_7250 = ((_Color.rgb*lerp( node_1046, (node_1046*-1.0+1.0), _InvertFresnel ))*_EmissiveIntensity);
                float3 emissive = node_7250;
                float3 finalColor = emissive + tex2D( _GrabTexture, sceneUVs.rg).rgb;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
