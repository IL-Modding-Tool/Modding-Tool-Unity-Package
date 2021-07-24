// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "RMAShaderOpaqueAccessory"
{
	Properties
	{
		_Metallic("Metallic", Range( 0 , 2)) = 1
		_Glossiness("Glossiness", Range( 0 , 2)) = 1
		_EmissionColor("EmissionColor", Color) = (0,0,0,0)
		_Color("Color", Color) = (1,1,1,1)
		_EmissionStrength("EmissionStrength", Float) = 0.1
		_ColorMask("ColorMask", 2D) = "white" {}
		_Color2("Color2", Color) = (0,0,0,0)
		_Color4("Color4", Color) = (0,0,0,0)
		_Color3("Color 3", Color) = (0,0,0,0)
		_NameArea("NameArea", 2D) = "white" {}
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
		uniform float4 _Color2;
		uniform sampler2D _ColorMask;
		uniform float4 _ColorMask_ST;
		uniform float4 _Color3;
		uniform float4 _Color4;
		uniform sampler2D _NameArea;
		uniform float4 _NameArea_ST;
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
			float4 tex2DNode6_g23 = tex2D( _MainTex, uv_MainTex );
			float2 uv_ColorMask = i.uv_texcoord * _ColorMask_ST.xy + _ColorMask_ST.zw;
			float4 tex2DNode62 = tex2D( _ColorMask, uv_ColorMask );
			float4 break17_g22 = tex2DNode62;
			float4 lerpResult29_g22 = lerp( float4( 1,1,1,1 ) , _Color2 , break17_g22.x);
			float4 lerpResult30_g22 = lerp( float4( 1,1,1,1 ) , _Color3 , break17_g22.y);
			float4 lerpResult31_g22 = lerp( float4( 1,1,1,1 ) , _Color4 , break17_g22.z);
			float2 uv_NameArea = i.uv_texcoord * _NameArea_ST.xy + _NameArea_ST.zw;
			float4 lerpResult69 = lerp( saturate( ( ( tex2DNode6_g23 * _Color ) * lerpResult29_g22 * lerpResult30_g22 * lerpResult31_g22 ) ) , tex2D( _NameArea, uv_NameArea ) , tex2DNode62.b);
			o.Albedo = lerpResult69.rgb;
			float2 uv_EmissionTex = i.uv_texcoord * _EmissionTex_ST.xy + _EmissionTex_ST.zw;
			o.Emission = ( tex2D( _EmissionTex, uv_EmissionTex ) * _EmissionColor * ( _EmissionStrength * 10.0 ) ).rgb;
			float2 uv_RMA = i.uv_texcoord * _RMA_ST.xy + _RMA_ST.zw;
			float4 tex2DNode1_g23 = tex2D( _RMA, uv_RMA );
			o.Metallic = saturate( ( tex2DNode1_g23.g * _Metallic ) );
			o.Smoothness = saturate( ( ( 1.0 - tex2DNode1_g23.r ) * _Glossiness ) );
			o.Occlusion = tex2DNode1_g23.b;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17400
2683;3830;1662;734;2158.354;1047.287;1;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;31;-2105.4,-302.892;Inherit;True;Property;_RMA;RMA;13;0;Create;True;0;0;False;0;None;6e6b99164daf27044ac208b7652f7367;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;32;-2037.1,-108.592;Inherit;True;Property;_EmissionTex;EmissionTex;14;0;Create;True;0;0;False;0;None;None;False;black;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;33;-2112.4,-499.892;Inherit;True;Property;_NormalMap;NormalMap;12;0;Create;True;0;0;False;0;None;a7160a0636d721047b20c66db14198d5;True;bump;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;34;-2123.4,-698.892;Inherit;True;Property;_MainTex;MainTex;11;0;Create;True;0;0;False;0;None;92e03bc5d2fccff4f80d98749021a218;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;62;-2199,-1470.498;Inherit;True;Property;_ColorMask;ColorMask;6;0;Create;True;0;0;False;0;-1;None;f823378d33b80d84396fec3b38ff1d14;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;65;-2121.973,-1092.949;Inherit;False;Property;_Color2;Color2;7;0;Create;False;0;0;False;0;0,0,0,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;64;-2125.196,-939.3729;Inherit;False;Property;_Color3;Color 3;9;0;Create;False;0;0;False;0;0,0,0,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;66;-1577.776,-539.8482;Inherit;True;RMAShaderFunction;0;;23;00e30437c5cfabf448090be5d03d8fd0;0;4;17;SAMPLER2D;0;False;18;SAMPLER2D;0;False;16;SAMPLER2D;0;False;15;SAMPLER2D;0;False;7;COLOR;22;FLOAT3;21;COLOR;20;FLOAT;0;FLOAT;19;FLOAT;31;FLOAT;28
Node;AmplifyShaderEditor.ColorNode;63;-2123.797,-1254.761;Inherit;False;Property;_Color4;Color4;8;0;Create;False;0;0;False;0;0,0,0,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;60;-1388.671,-1053.572;Inherit;False;ColorMaskThreeChannel;-1;;22;9c22a2c33dcb85b479d49d4f1e541416;0;5;16;FLOAT4;0,0,0,0;False;18;FLOAT4;0,0,0,0;False;19;FLOAT4;0,0,0,0;False;20;FLOAT4;0,0,0,0;False;21;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;68;-1437.622,-861.9863;Inherit;True;Property;_NameArea;NameArea;10;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;69;-996.1758,-1049.038;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-708.125,-563.1862;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;RMAShaderOpaqueAccessory;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;66;17;34;0
WireConnection;66;18;33;0
WireConnection;66;16;31;0
WireConnection;66;15;32;0
WireConnection;60;16;62;0
WireConnection;60;18;66;22
WireConnection;60;19;65;0
WireConnection;60;20;64;0
WireConnection;60;21;63;0
WireConnection;69;0;60;0
WireConnection;69;1;68;0
WireConnection;69;2;62;3
WireConnection;0;0;69;0
WireConnection;0;1;66;21
WireConnection;0;2;66;20
WireConnection;0;3;66;0
WireConnection;0;4;66;19
WireConnection;0;5;66;31
ASEEND*/
//CHKSM=1C3EF0EECD2FACFE7FF2F07B48D38A9BD576146A