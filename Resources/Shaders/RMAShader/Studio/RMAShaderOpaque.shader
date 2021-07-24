// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "RMAShaderOpaque"
{
	Properties
	{
		_Metallic("Metallic", Range( 0 , 2)) = 1
		_Glossiness("Glossiness", Range( 0 , 2)) = 1
		_EmissionColor("EmissionColor", Color) = (0,0,0,0)
		_Color("Color", Color) = (1,1,1,1)
		_EmissionStrength("EmissionStrength", Float) = 0.1
		_MainTex("MainTex", 2D) = "white" {}
		_NormalMap("NormalMap", 2D) = "bump" {}
		_RMA("RMA", 2D) = "white" {}
		_EmissionTex("EmissionTex", 2D) = "black" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
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

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			o.Normal = UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode6_g21 = tex2D( _MainTex, uv_MainTex );
			o.Albedo = ( tex2DNode6_g21 * _Color ).rgb;
			float2 uv_EmissionTex = i.uv_texcoord * _EmissionTex_ST.xy + _EmissionTex_ST.zw;
			o.Emission = ( tex2D( _EmissionTex, uv_EmissionTex ) * _EmissionColor * ( _EmissionStrength * 10.0 ) ).rgb;
			float2 uv_RMA = i.uv_texcoord * _RMA_ST.xy + _RMA_ST.zw;
			float4 tex2DNode1_g21 = tex2D( _RMA, uv_RMA );
			o.Metallic = saturate( ( tex2DNode1_g21.g * _Metallic ) );
			o.Smoothness = saturate( ( ( 1.0 - tex2DNode1_g21.r ) * _Glossiness ) );
			o.Occlusion = tex2DNode1_g21.b;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17400
168;64;1662;938;2581.677;854.7449;1.3;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;31;-2105.4,-302.892;Inherit;True;Property;_RMA;RMA;8;0;Create;True;0;0;False;0;None;64a3fb9a4e2ebca4b9ad8014547d17e4;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;32;-2037.1,-108.592;Inherit;True;Property;_EmissionTex;EmissionTex;9;0;Create;True;0;0;False;0;None;None;False;black;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;34;-2123.4,-698.892;Inherit;True;Property;_MainTex;MainTex;6;0;Create;True;0;0;False;0;None;fba0d1d988d0cae4993865dcf0326793;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;33;-2112.4,-499.892;Inherit;True;Property;_NormalMap;NormalMap;7;0;Create;True;0;0;False;0;None;a11caeb1b363ae84fa0c88cd17b0e661;True;bump;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.FunctionNode;59;-1484.208,-490.5861;Inherit;True;RMAShaderFunction;0;;21;00e30437c5cfabf448090be5d03d8fd0;0;4;17;SAMPLER2D;0;False;18;SAMPLER2D;0;False;16;SAMPLER2D;0;False;15;SAMPLER2D;0;False;7;COLOR;22;FLOAT3;21;COLOR;20;FLOAT;0;FLOAT;19;FLOAT;31;FLOAT;28
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-883.125,-486.1862;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;RMAShaderOpaque;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;60;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;59;17;34;0
WireConnection;59;18;33;0
WireConnection;59;16;31;0
WireConnection;59;15;32;0
WireConnection;0;0;59;22
WireConnection;0;1;59;21
WireConnection;0;2;59;20
WireConnection;0;3;59;0
WireConnection;0;4;59;19
WireConnection;0;5;59;31
ASEEND*/
//CHKSM=43C4364E1D188E632ACF7D1FE7A221ABA59A804A