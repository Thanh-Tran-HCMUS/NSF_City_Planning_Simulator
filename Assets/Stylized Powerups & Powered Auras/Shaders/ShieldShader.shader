// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Stylized Powerups/Shield Shader"
{
	Properties
	{
		_Brightness("Brightness", Float) = 1
		_MainTex1("Main Texture", 2D) = "white" {}
		_Mask("Mask", 2D) = "white" {}
		_FresnelPower("Fresnel Power", Float) = 5
		_FresnelColor("Fresnel Color", Color) = (1,1,1,0)
		_FresnelColorBrightness("Fresnel Color Brightness", Float) = 1
		_PanningSpeed("Panning Speed", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Overlay+0" "IsEmissive" = "true"  }
		Cull Back
		Blend OneMinusDstColor One , One One
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float _Brightness;
		uniform sampler2D _MainTex1;
		uniform float2 _PanningSpeed;
		uniform float4 _MainTex1_ST;
		uniform float _FresnelPower;
		uniform float4 _FresnelColor;
		uniform float _FresnelColorBrightness;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTex1 = i.uv_texcoord * _MainTex1_ST.xy + _MainTex1_ST.zw;
			float2 panner66 = ( 1.0 * _Time.y * _PanningSpeed + uv_MainTex1);
			float4 appendResult11 = (float4(i.vertexColor.r , i.vertexColor.g , i.vertexColor.b , 0.0));
			float4 ParticleColor14 = appendResult11;
			float4 temp_output_64_0 = ( ( _Brightness * tex2D( _MainTex1, panner66 ).g ) * ParticleColor14 );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV48 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode48 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV48, _FresnelPower ) );
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float fresnelNdotV71 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode71 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV71, 2.0 ) );
			float ParticleAlpha16 = i.vertexColor.a;
			float4 clampResult78 = clamp( ( ( fresnelNode71 * ParticleAlpha16 ) + temp_output_64_0 ) , float4( 0,0,0,0 ) , float4( 1,0,0,0 ) );
			o.Emission = ( ( temp_output_64_0 + ( ( fresnelNode48 * ParticleColor14 ) * ( _FresnelColor * _FresnelColorBrightness ) ) ) * ( tex2D( _Mask, uv_Mask ).r * clampResult78 ).x ).xyz;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18800
0;0;2560;1389;1240.305;392.0927;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;15;74.61517,-1288.186;Inherit;False;657.3822;308.1222;Register Particle Color And Alpha;4;10;11;14;16;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;70;-1449.796,-476.3606;Inherit;False;1343.689;433.841;Panning Texture Setup;8;66;63;67;13;64;25;65;77;;1,1,1,1;0;0
Node;AmplifyShaderEditor.VertexColorNode;10;124.6152,-1205.064;Inherit;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;67;-1392.796,-233.184;Inherit;False;Property;_PanningSpeed;Panning Speed;7;0;Create;True;0;0;0;False;0;False;0,0;0.3,-0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;77;-1381.998,-376.61;Inherit;False;0;25;2;3;2;SAMPLER2D;;False;0;FLOAT2;2,2;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;11;372.5228,-1214.152;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PannerNode;66;-1122.081,-280.5196;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;16;507.9973,-1114.06;Inherit;False;ParticleAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-826.7871,-426.3606;Inherit;False;Property;_Brightness;Brightness;1;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;25;-941.6357,-342.7973;Inherit;True;Property;_MainTex1;Main Texture;2;0;Create;False;0;0;0;False;0;False;-1;d570e72a4dbde124ca4865c0ca828532;d570e72a4dbde124ca4865c0ca828532;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;14;507.9975,-1238.186;Inherit;False;ParticleColor;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;62;-1303.626,-1236.632;Inherit;False;1072.601;686.342;Fresnel setup;8;53;61;52;58;49;48;51;60;;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;73;-934.2588,346.0526;Inherit;False;16;ParticleAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;-616.5939,-368.6736;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;71;-1038.35,133.8893;Inherit;True;Standard;WorldNormal;ViewDir;True;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;65;-597.0809,-158.5196;Inherit;False;14;ParticleColor;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-341.1074,-311.686;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-1336.626,-816.0569;Inherit;False;Property;_FresnelPower;Fresnel Power;4;0;Create;True;0;0;0;False;0;False;5;6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-720,144;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;74;-583.8743,131.9386;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;51;-1023.83,-1186.632;Inherit;False;Property;_FresnelColor;Fresnel Color;5;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,0.910524,0.4764151,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;48;-1059.372,-928.1486;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;58;-1072.025,-666.2902;Inherit;False;14;ParticleColor;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-1055.454,-1023.171;Inherit;False;Property;_FresnelColorBrightness;Fresnel Color Brightness;6;0;Create;True;0;0;0;False;0;False;1;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-661.025,-838.2902;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-790.404,-1115.448;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;79;-365.2471,460.6813;Inherit;True;Property;_Mask;Mask;3;0;Create;True;0;0;0;False;0;False;-1;d570e72a4dbde124ca4865c0ca828532;d570e72a4dbde124ca4865c0ca828532;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;78;-317.9041,141.9632;Inherit;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;1,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;-50.10791,267.9618;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-404.0251,-924.2902;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;76;265.2111,-284.3925;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;88;193.6952,408.9073;Inherit;True;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.TexCoordVertexDataNode;68;-1702.796,-422.184;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;429.3113,91.05794;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;81;637.4088,-59.46069;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Stylized Powerups/Shield Shader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;False;Transparent;;Overlay;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;5;4;False;-1;1;False;-1;4;1;False;-1;1;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;11;0;10;1
WireConnection;11;1;10;2
WireConnection;11;2;10;3
WireConnection;66;0;77;0
WireConnection;66;2;67;0
WireConnection;16;0;10;4
WireConnection;25;1;66;0
WireConnection;14;0;11;0
WireConnection;63;0;13;0
WireConnection;63;1;25;2
WireConnection;64;0;63;0
WireConnection;64;1;65;0
WireConnection;72;0;71;0
WireConnection;72;1;73;0
WireConnection;74;0;72;0
WireConnection;74;1;64;0
WireConnection;48;3;49;0
WireConnection;60;0;48;0
WireConnection;60;1;58;0
WireConnection;52;0;51;0
WireConnection;52;1;53;0
WireConnection;78;0;74;0
WireConnection;80;0;79;1
WireConnection;80;1;78;0
WireConnection;61;0;60;0
WireConnection;61;1;52;0
WireConnection;76;0;64;0
WireConnection;76;1;61;0
WireConnection;88;0;80;0
WireConnection;84;0;76;0
WireConnection;84;1;88;0
WireConnection;81;2;84;0
ASEEND*/
//CHKSM=23E21647C28E1896347B9DC172DC9E9CCE3A0FD9