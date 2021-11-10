Shader "CameraFog"
{
    Properties
    {
        _MainTex ("render texture", 2D) = "white"{}
        _MaskTex("Mask Texture", 2D) = "white"{}
        _lerp("Lerp", Float) = 0.6
        
    }

    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex; float4 _MainTex_TexelSize;
            sampler2D _MaskTex;
            float _lerp;

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float3 sample(float2 uv){
                return tex2D(_MainTex,uv).rgb;
            }


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
                uv.x *= 1.4;
                uv.x-=0.2;
                float3 color = tex2D(_MainTex, uv).rgb;

				float3 maskColor =1 - tex2D(_MaskTex, uv).rgb;
				
                
				color *= maskColor * _lerp;

                return float4(color, 1.0);
            }
            ENDCG
        }
    }
}
