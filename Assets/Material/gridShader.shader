Shader "Custom/GridShader"
{
    Properties
    {
        _LineColor ("Line Color", Color) = (1,1,1,1)
        _LineThickness ("Line Thickness", Range(0.01, 0.1)) = 0.02
        _MainTex ("Base (RGB)", 2D) = "white" { }
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _LineColor;
            float _LineThickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float lineWidth = _LineThickness;
                float2 uv = i.uv * 100.0; // Scale UVs to 100x100 for 1cm grid in 1m
                float2 grid = abs(frac(uv - 0.5) - 0.5) / fwidth(uv);
                float minGrid = min(grid.x, grid.y);
                float lineAlpha = 1.0 - smoothstep(1.0 - lineWidth, 1.0, minGrid);
                fixed4 col = _LineColor;
                col.a = lineAlpha; // Make the line alpha match the line color alpha
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}