Shader "Unlit/MagmaRayShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Speed ("Scroll Speed", Float) = 1.0
        _TilingX ("Tiling X", Float) = 6.0
        _TilingY ("Tiling Y", Float) = 1.0
        _EmissionStrength ("Emission Strength", Float) = 1.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color;
            float _Speed;
            float _TilingX;
            float _TilingY;
            float _EmissionStrength;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);

                float2 uv = TRANSFORM_TEX(v.uv, _MainTex);

                // tiling wzd³u¿ i w poprzek promienia
                uv.x *= _TilingX;
                uv.y *= _TilingY;

                o.uv = uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // przesuwanie UV w czasie – magma od d³oni do celu
                float2 uv = i.uv;
                uv.x += (-_Time.y * _Speed);

                fixed4 texCol = tex2D(_MainTex, uv);

                // kolor materia³u
                texCol *= _Color;

                // emisja
                texCol.rgb += texCol.rgb * _EmissionStrength;

                return texCol;
            }
            ENDCG
        }
    }
}
