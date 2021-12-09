Shader "Custom/Distort"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_DistortTex("Distort Texture", 2D) = "grey" {}
		_MaskTex("Mask Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 0, 1, 1)
		_Multiplier("Sprite Multiplier", Range(-2,2)) = 1
		_Scale("Distort Scale", Range(0,10)) = 3.2
		_SpeedX("Speed X", Range(-1,1)) = 1
		_SpeedY("Speed Y", Range(-1,1)) = 1
		_EffectSize("Effect Size", Range(0,5)) = 1
		_EffectOffset("Effect Offset", Range(-0.1,0.1)) = 0
		_Brightness("Brightness", Range(0,2)) = 1.5
		_Opacity("Opacity", Range(0,1)) =1.0
		[Toggle(ENABLE)] _ENABLE ("Enable Effect", Float) = 1
		[Toggle(BEHIND)] _BEHIND("Effect Behind Sprite", Float) = 1
		[Toggle(ONLY)] _ONLY("Effect Only", Float) = 0
		[Toggle(MASK)] _MASK("Enable Mask", Float) = 1
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

			#pragma shader_feature ENABLE	
			#pragma shader_feature BEHIND			
			#pragma shader_feature ONLY	
			#pragma shader_feature MASK

			#include "UnityCG.cginc"
			
			sampler2D _MainTex, _DistortTex, _MaskTex;
			
			float4 _Color;
			float _Brightness;
			float _EffectSize;
			float _EffectOffset;
			float _Scale;
			float _SpeedY, _SpeedX;
			float _Opacity;		
			float _Multiplier;		

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
                float4 color = tex2D(_MainTex, uv);
				color *= color.a;

				float2 distortUV = (uv * 1/_EffectSize) - _EffectOffset;
				float timeX = _Time.y * _SpeedX;
				float timeY = _Time.y * _SpeedY;

				float distortion = tex2D(_DistortTex, float2(uv.x * _Scale + timeX, uv.y * _Scale + timeY)) * 0.1;
				float4 effect = tex2D(_MainTex, float2(distortUV.x + distortion, distortUV.y + distortion));

				effect *= effect.a;

				float4 tint = _Color + (color * _Multiplier);
				effect += (_Brightness * effect.a) * tint;
				effect = saturate(effect *_Opacity);

				float4 mask = tex2D(_MaskTex, uv);
				

                #if ENABLE
				#else
					return color;
				#endif
				
				#if BEHIND
					effect *= (1 - color.a);
				#endif

				#if ONLY
					color = 0;
				#endif

				#if MASK
					effect *= mask.r;
				#endif
				
				color += effect;

				return color;

            }
            ENDCG
        }
    }
}
