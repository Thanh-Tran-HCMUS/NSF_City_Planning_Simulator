// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Stylized Powerups/Unlit Fresnel Vertex Color Blend"
{
	Properties
	{
		_MainTex1("Main Texture", 2D) = "white" {}
		[KeywordEnum(RGBA,Red,Green,Blue,Alpha)] _MainTextureLayer("Main Texture Layer", Float) = 0
		_MainTexturePanning("Main Texture Panning", Vector) = (0,0,0,0)
		_Brightness("Brightness", Float) = 1
		_FresnelPower("Fresnel Power", Float) = 5
		_FresnelColor("Fresnel Color", Color) = (1,1,1,0)
		_FresnelColorBrightness("Fresnel Color Brightness", Float) = 0
		_MainTexTiling("Main Tex Tiling", Vector) = (1,1,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _MAINTEXTURELAYER_RGBA _MAINTEXTURELAYER_RED _MAINTEXTURELAYER_GREEN _MAINTEXTURELAYER_BLUE _MAINTEXTURELAYER_ALPHA
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform sampler2D _MainTex1;
		uniform float2 _MainTexturePanning;
		uniform float2 _MainTexTiling;
		uniform float _Brightness;
		uniform float _FresnelPower;
		uniform float4 _FresnelColor;
		uniform float _FresnelColorBrightness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 appendResult11 = (float4(i.vertexColor.r , i.vertexColor.g , i.vertexColor.b , 0.0));
			float4 ParticleColor14 = appendResult11;
			float2 uv_TexCoord36 = i.uv_texcoord * _MainTexTiling;
			float2 panner37 = ( 1.0 * _Time.y * _MainTexturePanning + uv_TexCoord36);
			float4 tex2DNode25 = tex2D( _MainTex1, panner37 );
			float4 temp_cast_0 = (tex2DNode25.r).xxxx;
			float4 temp_cast_1 = (tex2DNode25.g).xxxx;
			float4 temp_cast_2 = (tex2DNode25.b).xxxx;
			float4 temp_cast_3 = (tex2DNode25.a).xxxx;
			#if defined(_MAINTEXTURELAYER_RGBA)
				float4 staticSwitch20 = tex2DNode25;
			#elif defined(_MAINTEXTURELAYER_RED)
				float4 staticSwitch20 = temp_cast_0;
			#elif defined(_MAINTEXTURELAYER_GREEN)
				float4 staticSwitch20 = temp_cast_1;
			#elif defined(_MAINTEXTURELAYER_BLUE)
				float4 staticSwitch20 = temp_cast_2;
			#elif defined(_MAINTEXTURELAYER_ALPHA)
				float4 staticSwitch20 = temp_cast_3;
			#else
				float4 staticSwitch20 = tex2DNode25;
			#endif
			float4 Main24 = staticSwitch20;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV48 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode48 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV48, _FresnelPower ) );
			float4 blendOpSrc56 = ParticleColor14;
			float4 blendOpDest56 = ( fresnelNode48 * ( _FresnelColor * _FresnelColorBrightness ) );
			o.Emission = ( ( ( ParticleColor14 * Main24 ) * _Brightness ) + ( saturate( 	max( blendOpSrc56, blendOpDest56 ) )) ).xyz;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				half4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				surfIN.vertexColor = IN.color;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18800
2561;4;1920;1029;86.50842;379.3462;1;True;False
Node;AmplifyShaderEditor.Vector2Node;57;-670.9514,-1014.392;Inherit;False;Property;_MainTexTiling;Main Tex Tiling;12;0;Create;True;0;0;0;False;0;False;1,1;2,2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;36;-467.3413,-999.797;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;38;-399.3413,-883.797;Inherit;False;Property;_MainTexturePanning;Main Texture Panning;3;0;Create;True;0;0;0;False;0;False;0,0;0.4,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;15;74.61517,-1288.186;Inherit;False;657.3822;308.1222;Register Particle Color And Alpha;4;10;11;14;16;;1,1,1,1;0;0
Node;AmplifyShaderEditor.PannerNode;37;-176.9512,-819.9112;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;29;74.15556,-949.6292;Inherit;False;850;486;Sets The Color & Alpha Channel chosen by the user.;6;25;27;20;28;24;47;;1,1,1,1;0;0
Node;AmplifyShaderEditor.VertexColorNode;10;124.6152,-1205.064;Inherit;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;25;124.1556,-787.6292;Inherit;True;Property;_MainTex1;Main Texture;0;0;Create;False;0;0;0;False;0;False;-1;de5ce1a6254c83b4db17e88ce430bee5;fb89fa2456f093640add0b7befb48d51;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;11;372.5228,-1214.152;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StaticSwitch;20;476.1555,-899.6291;Inherit;False;Property;_MainTextureLayer;Main Texture Layer;1;0;Create;True;0;0;0;False;0;False;0;0;1;True;;KeywordEnum;5;RGBA;Red;Green;Blue;Alpha;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;51;-626.2964,-121.7756;Inherit;False;Property;_FresnelColor;Fresnel Color;10;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;24;731.3694,-878.0195;Inherit;False;Main;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;14;507.9975,-1238.186;Inherit;False;ParticleColor;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-657.9203,41.68527;Inherit;False;Property;_FresnelColorBrightness;Fresnel Color Brightness;11;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-584.2401,-274.2615;Inherit;False;Property;_FresnelPower;Fresnel Power;9;0;Create;True;0;0;0;False;0;False;5;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;31;88.56536,-300.1893;Inherit;True;24;Main;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;48;-389.9864,-265.3532;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;17;106.7475,-370.644;Inherit;False;14;ParticleColor;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-392.8705,-50.59142;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;55;126.222,-10.84846;Inherit;False;14;ParticleColor;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;296.779,-367.8976;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-43.68991,-69.68632;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;13;141.2774,-114.0063;Inherit;False;Property;_Brightness;Brightness;4;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;56;129.741,64.44461;Inherit;True;Lighten;True;3;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;440.7243,-370.4501;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;40;-1481.597,-496.5421;Inherit;False;Property;_MaskPanning;Mask Panning;8;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;42;-1272.618,-485.2284;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;43;-1074.704,-491.0439;Inherit;True;Property;_Mask;Mask;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;44;-783.2813,-482.2463;Inherit;False;Property;_MaskTextureLayer;Mask Texture Layer;7;0;Create;True;0;0;0;False;0;False;0;4;3;True;;KeywordEnum;5;RGBA;Red;Green;Blue;Alpha;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;46;-543.5445,-477.848;Inherit;False;Property;_UseMask;Use Mask?;5;0;Create;True;0;0;0;False;0;False;1;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-340.0981,-428.3606;Inherit;False;Mask;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;47;228.4516,-574.6224;Inherit;False;45;Mask;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;27;476.1555,-675.6292;Inherit;False;Property;_AlphaLayer;Alpha Layer;2;0;Create;True;0;0;0;False;0;False;0;4;4;True;;KeywordEnum;6;RGBA;Red;Green;Blue;Alpha;MaskTexture;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;28;717.3777,-673.2281;Inherit;False;Alpha;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;16;507.9973,-1114.06;Inherit;False;ParticleAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;33;629.7853,119.469;Inherit;False;28;Alpha;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;54;649.7205,-148.6934;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;34;609.7853,194.469;Inherit;False;16;ParticleAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;41;-1525.619,-611.2283;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;888.7856,124.469;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;58;1063.809,-52.02347;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Stylized Powerups/Unlit Fresnel Vertex Color Blend;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;36;0;57;0
WireConnection;37;0;36;0
WireConnection;37;2;38;0
WireConnection;25;1;37;0
WireConnection;11;0;10;1
WireConnection;11;1;10;2
WireConnection;11;2;10;3
WireConnection;20;1;25;0
WireConnection;20;0;25;1
WireConnection;20;2;25;2
WireConnection;20;3;25;3
WireConnection;20;4;25;4
WireConnection;24;0;20;0
WireConnection;14;0;11;0
WireConnection;48;3;49;0
WireConnection;52;0;51;0
WireConnection;52;1;53;0
WireConnection;9;0;17;0
WireConnection;9;1;31;0
WireConnection;50;0;48;0
WireConnection;50;1;52;0
WireConnection;56;0;55;0
WireConnection;56;1;50;0
WireConnection;12;0;9;0
WireConnection;12;1;13;0
WireConnection;42;0;41;0
WireConnection;42;2;40;0
WireConnection;43;1;42;0
WireConnection;44;1;43;0
WireConnection;44;0;43;1
WireConnection;44;2;43;2
WireConnection;44;3;43;3
WireConnection;44;4;43;4
WireConnection;46;0;44;0
WireConnection;45;0;46;0
WireConnection;27;1;25;0
WireConnection;27;0;25;1
WireConnection;27;2;25;2
WireConnection;27;3;25;3
WireConnection;27;4;25;4
WireConnection;27;5;47;0
WireConnection;28;0;27;0
WireConnection;16;0;10;4
WireConnection;54;0;12;0
WireConnection;54;1;56;0
WireConnection;35;0;33;0
WireConnection;35;1;34;0
WireConnection;58;2;54;0
ASEEND*/
//CHKSM=158C3CAC6E36BCC83DABED238BEA5D2B9252B352