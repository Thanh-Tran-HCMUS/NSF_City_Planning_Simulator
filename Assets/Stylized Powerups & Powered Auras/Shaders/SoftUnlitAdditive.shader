// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Stylized Powerups/Soft Unlit Simple"
{
	Properties
	{
		_MainTex1("Main Texture", 2D) = "white" {}
		[KeywordEnum(RGBA,Red,Green,Blue,Alpha)] _MainTextureLayer("Main Texture Layer", Float) = 0
		[KeywordEnum(RGBA,Red,Green,Blue,Alpha,MaskTexture)] _AlphaLayer("Alpha Layer", Float) = 4
		_MainTexturePanning("Main Texture Panning", Vector) = (0,0,0,0)
		_Brightness("Brightness", Float) = 1
		[Toggle(_USEMASK_ON)] _UseMask("Use Mask?", Float) = 0
		_Mask("Mask", 2D) = "white" {}
		[KeywordEnum(RGBA,Red,Green,Blue,Alpha)] _MaskTextureLayer("Mask Texture Layer", Float) = 4
		_MaskPanning("Mask Panning", Vector) = (0,0,0,0)
		_DepthFade("Depth Fade", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Overlay+0" "IsEmissive" = "true"  }
		Cull Back
		ZWrite Off
		Blend OneMinusDstColor One , One One
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _MAINTEXTURELAYER_RGBA _MAINTEXTURELAYER_RED _MAINTEXTURELAYER_GREEN _MAINTEXTURELAYER_BLUE _MAINTEXTURELAYER_ALPHA
		#pragma shader_feature_local _ALPHALAYER_RGBA _ALPHALAYER_RED _ALPHALAYER_GREEN _ALPHALAYER_BLUE _ALPHALAYER_ALPHA _ALPHALAYER_MASKTEXTURE
		#pragma multi_compile_local __ _USEMASK_ON
		#pragma shader_feature_local _MASKTEXTURELAYER_RGBA _MASKTEXTURELAYER_RED _MASKTEXTURELAYER_GREEN _MASKTEXTURELAYER_BLUE _MASKTEXTURELAYER_ALPHA
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform sampler2D _MainTex1;
		uniform float2 _MainTexturePanning;
		uniform float _Brightness;
		uniform sampler2D _Mask;
		uniform float2 _MaskPanning;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _DepthFade;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 appendResult11 = (float4(i.vertexColor.r , i.vertexColor.g , i.vertexColor.b , 0.0));
			float4 ParticleColor14 = appendResult11;
			float2 panner37 = ( 1.0 * _Time.y * _MainTexturePanning + i.uv_texcoord);
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
			float4 temp_cast_5 = (tex2DNode25.a).xxxx;
			float4 temp_cast_6 = (tex2DNode25.r).xxxx;
			float4 temp_cast_7 = (tex2DNode25.g).xxxx;
			float4 temp_cast_8 = (tex2DNode25.b).xxxx;
			float4 temp_cast_9 = (tex2DNode25.a).xxxx;
			float2 panner42 = ( 1.0 * _Time.y * _MaskPanning + i.uv_texcoord);
			float4 tex2DNode43 = tex2D( _Mask, panner42 );
			float4 temp_cast_10 = (tex2DNode43.a).xxxx;
			float4 temp_cast_11 = (tex2DNode43.r).xxxx;
			float4 temp_cast_12 = (tex2DNode43.g).xxxx;
			float4 temp_cast_13 = (tex2DNode43.b).xxxx;
			float4 temp_cast_14 = (tex2DNode43.a).xxxx;
			#if defined(_MASKTEXTURELAYER_RGBA)
				float4 staticSwitch44 = tex2DNode43;
			#elif defined(_MASKTEXTURELAYER_RED)
				float4 staticSwitch44 = temp_cast_11;
			#elif defined(_MASKTEXTURELAYER_GREEN)
				float4 staticSwitch44 = temp_cast_12;
			#elif defined(_MASKTEXTURELAYER_BLUE)
				float4 staticSwitch44 = temp_cast_13;
			#elif defined(_MASKTEXTURELAYER_ALPHA)
				float4 staticSwitch44 = temp_cast_10;
			#else
				float4 staticSwitch44 = temp_cast_10;
			#endif
			#ifdef _USEMASK_ON
				float4 staticSwitch46 = staticSwitch44;
			#else
				float4 staticSwitch46 = float4( 0,0,0,0 );
			#endif
			float4 Mask45 = staticSwitch46;
			#if defined(_ALPHALAYER_RGBA)
				float4 staticSwitch27 = tex2DNode25;
			#elif defined(_ALPHALAYER_RED)
				float4 staticSwitch27 = temp_cast_6;
			#elif defined(_ALPHALAYER_GREEN)
				float4 staticSwitch27 = temp_cast_7;
			#elif defined(_ALPHALAYER_BLUE)
				float4 staticSwitch27 = temp_cast_8;
			#elif defined(_ALPHALAYER_ALPHA)
				float4 staticSwitch27 = temp_cast_5;
			#elif defined(_ALPHALAYER_MASKTEXTURE)
				float4 staticSwitch27 = Mask45;
			#else
				float4 staticSwitch27 = temp_cast_5;
			#endif
			float4 Alpha28 = staticSwitch27;
			float ParticleAlpha16 = i.vertexColor.a;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth48 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth48 = abs( ( screenDepth48 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthFade ) );
			float clampResult64 = clamp( distanceDepth48 , 0.0 , 1.0 );
			float4 temp_output_62_0 = ( ( Alpha28 * ParticleAlpha16 ) * clampResult64 );
			o.Emission = ( ( ( ParticleColor14 * Main24 ) * _Brightness ) * temp_output_62_0 ).xyz;
			o.Alpha = temp_output_62_0.r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18800
2560;6;1920;1023;575.8541;499.4384;1;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;41;-1525.619,-611.2283;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;40;-1481.597,-496.5421;Inherit;False;Property;_MaskPanning;Mask Panning;9;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;42;-1272.618,-485.2284;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;43;-1074.704,-491.0439;Inherit;True;Property;_Mask;Mask;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;44;-783.2813,-482.2463;Inherit;False;Property;_MaskTextureLayer;Mask Texture Layer;8;0;Create;True;0;0;0;False;0;False;0;4;4;True;;KeywordEnum;5;RGBA;Red;Green;Blue;Alpha;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;36;-467.3413,-999.797;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;38;-399.3413,-883.797;Inherit;False;Property;_MainTexturePanning;Main Texture Panning;4;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.StaticSwitch;46;-543.5445,-477.848;Inherit;False;Property;_UseMask;Use Mask?;6;0;Create;True;0;0;0;False;0;False;1;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-340.0981,-428.3606;Inherit;False;Mask;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;15;24.61517,-1616.186;Inherit;False;910.3822;541.1222;Register Particle Color And Alpha;4;14;11;16;10;;1,1,1,1;0;0
Node;AmplifyShaderEditor.PannerNode;37;-128,-880;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;29;74.15556,-949.6292;Inherit;False;850;486;Sets The Color & Alpha Channel chosen by the user.;6;25;27;20;28;24;47;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;25;124.1556,-787.6292;Inherit;True;Property;_MainTex1;Main Texture;1;0;Create;False;0;0;0;False;0;False;-1;de5ce1a6254c83b4db17e88ce430bee5;0378a2e7c4d9f90498e70a23c1b4ebf7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;10;74.6152,-1533.064;Inherit;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;47;228.4516,-574.6224;Inherit;False;45;Mask;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;20;476.1555,-899.6291;Inherit;False;Property;_MainTextureLayer;Main Texture Layer;2;0;Create;True;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;5;RGBA;Red;Green;Blue;Alpha;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;27;476.1555,-675.6292;Inherit;False;Property;_AlphaLayer;Alpha Layer;3;0;Create;True;0;0;0;False;0;False;0;4;0;True;;KeywordEnum;6;RGBA;Red;Green;Blue;Alpha;MaskTexture;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;11;322.5228,-1542.152;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;50;17.13452,324.3955;Inherit;False;Property;_DepthFade;Depth Fade;10;0;Create;True;0;0;0;False;0;False;0;50;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;16;688.9973,-1412.06;Inherit;False;ParticleAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;14;457.9975,-1566.186;Inherit;False;ParticleColor;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;24;700.1555,-899.6291;Inherit;False;Main;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;28;734.1555,-630.6292;Inherit;False;Alpha;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;33;91.99597,47.19214;Inherit;False;28;Alpha;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.DepthFade;48;132.8339,205.4467;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;34;71.99597,122.1921;Inherit;False;16;ParticleAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;17;106.7475,-370.644;Inherit;False;14;ParticleColor;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;31;88.56536,-300.1893;Inherit;True;24;Main;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;296.779,-367.8976;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;13;141.2774,-114.0063;Inherit;False;Property;_Brightness;Brightness;5;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;64;391.2059,267.806;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;350.996,52.19214;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;498.2059,122.806;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;464,-368;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;702.1459,-131.4384;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;65;900.6,-179.6;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Stylized Powerups/Soft Unlit Simple;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;Overlay;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;5;4;False;-1;1;False;-1;4;1;False;-1;1;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
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
WireConnection;37;0;36;0
WireConnection;37;2;38;0
WireConnection;25;1;37;0
WireConnection;20;1;25;0
WireConnection;20;0;25;1
WireConnection;20;2;25;2
WireConnection;20;3;25;3
WireConnection;20;4;25;4
WireConnection;27;1;25;0
WireConnection;27;0;25;1
WireConnection;27;2;25;2
WireConnection;27;3;25;3
WireConnection;27;4;25;4
WireConnection;27;5;47;0
WireConnection;11;0;10;1
WireConnection;11;1;10;2
WireConnection;11;2;10;3
WireConnection;16;0;10;4
WireConnection;14;0;11;0
WireConnection;24;0;20;0
WireConnection;28;0;27;0
WireConnection;48;0;50;0
WireConnection;9;0;17;0
WireConnection;9;1;31;0
WireConnection;64;0;48;0
WireConnection;35;0;33;0
WireConnection;35;1;34;0
WireConnection;62;0;35;0
WireConnection;62;1;64;0
WireConnection;12;0;9;0
WireConnection;12;1;13;0
WireConnection;66;0;12;0
WireConnection;66;1;62;0
WireConnection;65;2;66;0
WireConnection;65;9;62;0
ASEEND*/
//CHKSM=38B3C8C5C69B524405B333D73867B83BE816BF58