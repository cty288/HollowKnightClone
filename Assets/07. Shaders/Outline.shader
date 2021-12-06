Shader "Custom/Outline"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Tint ("Tint", Color) = (1,1,1,1)
		_Brightness("Outline Brightness", Range(0,8)) = 3.2
		_Width("Outline Width", Range(0,0.05)) = 0.003	
		_OutlineColor("OutlineColor", Color) = (1,1,1,1)
		[MaterialToggle] Enabled ("Enabled", Float) = 0
	}

    SubShader
    {
		Tags
		{ 
			"Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ ENABLED_ON
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			sampler2D _OutlineTex;

			float4 _Tint;
			float4 _OutlineColor;

			float _Brightness;
			float _Width;
			float _SpeedX, _SpeedY;
			
            struct MeshData
            {
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
            };

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                float2 uv = i.uv;
                float4 color = tex2D(_MainTex, uv) * _Tint;

				float left = tex2D(_MainTex, uv + float2(_Width, 0)).a;
				float right = tex2D(_MainTex, uv - float2(_Width, 0)).a;
				float up = tex2D(_MainTex, uv + float2(0, _Width*2)).a;
				float down = tex2D(_MainTex, uv - float2(0, _Width*2)).a;

				float4 outline = (left + right + up + down) * (1-color.a) * _OutlineColor* _Brightness;
                
                #ifdef ENABLED_ON
				color = outline;
				#else
				//color += outline;
				color = float4(0.0,0.0,0.0, 0.0);
				#endif
				
                return color;
            }
            ENDCG
        }
    }
}
