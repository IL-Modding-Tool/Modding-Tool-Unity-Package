// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "RMAShaderDither"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Metallic("Metallic", Range( 0 , 2)) = 1
		_Glossiness("Glossiness", Range( 0 , 2)) = 1
		_EmissionColor("EmissionColor", Color) = (0,0,0,0)
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}
		[NoScaleOffset]_BlueNoise2("Blue Noise", 2D) = "white" {}
		_NormalMap("NormalMap", 2D) = "white" {}
		_OpacityMap("OpacityMap", 2D) = "white" {}
		_RMA("RMA", 2D) = "white" {}
		_EmissionTex("EmissionTex", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPosition;
		};

		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _Color;
		uniform sampler2D _EmissionTex;
		uniform float4 _EmissionTex_ST;
		uniform float4 _EmissionColor;
		uniform sampler2D _RMA;
		uniform float4 _RMA_ST;
		uniform float _Glossiness;
		uniform float _Metallic;
		uniform sampler2D _OpacityMap;
		uniform float4 _OpacityMap_ST;
		uniform sampler2D _BlueNoise2;
		uniform float4 _BlueNoise2_TexelSize;
		uniform float _Cutoff = 0.5;


		inline float DitherNoiseTex( float4 screenPos, sampler2D noiseTexture, float4 noiseTexelSize )
		{
			float dither = tex2Dlod( noiseTexture, float4( screenPos.xy * _ScreenParams.xy * noiseTexelSize.xy, 0, 0 ) ).g;
			float ditherRate = noiseTexelSize.x * noiseTexelSize.y;
			dither = ( 1 - ditherRate ) * dither + ditherRate;
			return dither;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			o.screenPosition = ase_screenPos;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			o.Normal = UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode6_g8 = tex2D( _MainTex, uv_MainTex );
			o.Albedo = ( tex2DNode6_g8 * _Color ).rgb;
			float2 uv_EmissionTex = i.uv_texcoord * _EmissionTex_ST.xy + _EmissionTex_ST.zw;
			o.Emission = ( tex2D( _EmissionTex, uv_EmissionTex ) * _EmissionColor ).rgb;
			float2 uv_RMA = i.uv_texcoord * _RMA_ST.xy + _RMA_ST.zw;
			float4 tex2DNode1_g8 = tex2D( _RMA, uv_RMA );
			o.Metallic = saturate( ( 1.0 - ( tex2DNode1_g8.r * _Glossiness ) ) );
			o.Smoothness = saturate( ( tex2DNode1_g8.g * _Metallic ) );
			o.Occlusion = tex2DNode1_g8.b;
			o.Alpha = 1;
			float2 uv_OpacityMap = i.uv_texcoord * _OpacityMap_ST.xy + _OpacityMap_ST.zw;
			float4 ase_screenPos = i.screenPosition;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float dither42 = DitherNoiseTex( ase_screenPosNorm, _BlueNoise2, _BlueNoise2_TexelSize);
			clip( ( tex2D( _OpacityMap, uv_OpacityMap ) * dither42 ).r - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17400
2576;3672.333;1897;871;2246.707;522.1917;1.3;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;41;-1505.244,338.409;Float;True;Property;_BlueNoise2;Blue Noise;7;1;[NoScaleOffset];Create;True;0;0;False;0;None;16d574e53541bba44a84052fa38778df;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;34;-1777.4,-687.892;Inherit;True;Property;_MainTex;MainTex;6;0;Create;True;0;0;False;0;None;6d734272843903b48acdaa0d55251f3f;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;33;-1776.4,-488.892;Inherit;True;Property;_NormalMap;NormalMap;8;0;Create;True;0;0;False;0;None;303c26a08ce4af140b75a6b4b7740b5a;True;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;31;-1773.4,-290.892;Inherit;True;Property;_RMA;RMA;10;0;Create;True;0;0;False;0;None;7ac73147f1a2b4841af48f7b197a6bdb;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;32;-1770.7,-95.59199;Inherit;True;Property;_EmissionTex;EmissionTex;11;0;Create;True;0;0;False;0;None;385ad64a9a9e4cd44a181eaf3a7807d0;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.DitheringNode;42;-1233.244,338.409;Inherit;False;2;False;3;0;FLOAT;0;False;1;SAMPLER2D;;False;2;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;44;-1562.908,125.2081;Inherit;True;Property;_OpacityMap;OpacityMap;9;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;43;-1484.208,-490.5861;Inherit;False;RMAShaderFunction;1;;8;00e30437c5cfabf448090be5d03d8fd0;0;4;17;SAMPLER2D;0;False;18;SAMPLER2D;0;False;16;SAMPLER2D;0;False;15;SAMPLER2D;0;False;7;COLOR;22;FLOAT3;21;COLOR;20;FLOAT;0;FLOAT;19;FLOAT;31;FLOAT;28
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-1071.507,91.40826;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-746.125,-485.1862;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;RMAShaderDither;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;42;1;41;0
WireConnection;43;17;34;0
WireConnection;43;18;33;0
WireConnection;43;16;31;0
WireConnection;43;15;32;0
WireConnection;45;0;44;0
WireConnection;45;1;42;0
WireConnection;0;0;43;22
WireConnection;0;1;43;21
WireConnection;0;2;43;20
WireConnection;0;3;43;0
WireConnection;0;4;43;19
WireConnection;0;5;43;31
WireConnection;0;10;45;0
ASEEND*/
//CHKSM=E2209CDB987C2D80969B34F5B276F5746277E777