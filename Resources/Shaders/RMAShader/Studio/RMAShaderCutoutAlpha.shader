// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "RMAShaderCutout"
{
	Properties
	{
		_Metallic("Metallic", Range( 0 , 2)) = 1
		_Glossiness("Glossiness", Range( 0 , 2)) = 1
		_EmissionColor("EmissionColor", Color) = (0,0,0,0)
		_Color("Color", Color) = (1,1,1,1)
		_EmissionStrength("EmissionStrength", Float) = 0.1
		_MainTex("MainTex", 2D) = "white" {}
		_NormalMap("NormalMap", 2D) = "white" {}
		_RMA("RMA", 2D) = "white" {}
		_EmissionTex("EmissionTex", 2D) = "white" {}
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _Color;
		uniform sampler2D _EmissionTex;
		uniform float4 _EmissionTex_ST;
		uniform float4 _EmissionColor;
		uniform float _EmissionStrength;
		uniform sampler2D _RMA;
		uniform float4 _RMA_ST;
		uniform float _Metallic;
		uniform float _Glossiness;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			o.Normal = UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode6_g9 = tex2D( _MainTex, uv_MainTex );
			o.Albedo = ( tex2DNode6_g9 * _Color ).rgb;
			float2 uv_EmissionTex = i.uv_texcoord * _EmissionTex_ST.xy + _EmissionTex_ST.zw;
			o.Emission = ( tex2D( _EmissionTex, uv_EmissionTex ) * _EmissionColor * ( _EmissionStrength * 10.0 ) ).rgb;
			float2 uv_RMA = i.uv_texcoord * _RMA_ST.xy + _RMA_ST.zw;
			float4 tex2DNode1_g9 = tex2D( _RMA, uv_RMA );
			o.Metallic = saturate( ( tex2DNode1_g9.g * _Metallic ) );
			o.Smoothness = saturate( ( ( 1.0 - tex2DNode1_g9.r ) * _Glossiness ) );
			o.Occlusion = tex2DNode1_g9.b;
			o.Alpha = 1;
			clip( ( 1.0 * tex2DNode6_g9.a ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17400
2576;3672.333;1897;871;1225.862;148.5483;1.030534;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;27;-620.0388,-63.51938;Inherit;True;Property;_MainTex;MainTex;6;0;Create;True;0;0;False;0;None;687a5ddac29433d49a7bfaaa25fcfc08;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;28;-619.0388,135.4806;Inherit;True;Property;_NormalMap;NormalMap;7;0;Create;True;0;0;False;0;None;b6329c4f425e9364ea20e32b743e610c;True;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;30;-616.0388,333.4806;Inherit;True;Property;_RMA;RMA;8;0;Create;True;0;0;False;0;None;60f68fba4dafaef46a7da9a13eb9bbdd;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;29;-612.0388,540.4807;Inherit;True;Property;_EmissionTex;EmissionTex;9;0;Create;True;0;0;False;0;None;c022fccd6bccf3b4883f6e689bcbfc0e;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.FunctionNode;35;-326.8468,133.7865;Inherit;False;RMAShaderFunction;0;;9;00e30437c5cfabf448090be5d03d8fd0;0;4;17;SAMPLER2D;0;False;18;SAMPLER2D;0;False;16;SAMPLER2D;0;False;15;SAMPLER2D;0;False;7;COLOR;22;FLOAT3;21;COLOR;20;FLOAT;0;FLOAT;19;FLOAT;31;FLOAT;28
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;81.5366,134.2159;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;RMAShaderCutout;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;10;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;35;17;27;0
WireConnection;35;18;28;0
WireConnection;35;16;30;0
WireConnection;35;15;29;0
WireConnection;0;0;35;22
WireConnection;0;1;35;21
WireConnection;0;2;35;20
WireConnection;0;3;35;0
WireConnection;0;4;35;19
WireConnection;0;5;35;31
WireConnection;0;10;35;28
ASEEND*/
//CHKSM=111577DFC3BA3C425E4D33254DDED92627B3C829