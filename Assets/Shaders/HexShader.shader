Shader "Unlit/HexShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (1, 1, 1, 1)
        _RowOffset ("Row Offset", Int) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "LightMode"="ForwardBase" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 diff: COLOR0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _TintColor;
            int _RowOffset;
            float4 _Weird;

            v2f vert (appdata_base v)
            {
                v2f o;
                if (v.vertex.y >= -0.1) {
                    v.vertex.y += 0.02 * sin((0.5 * _Time.y + (v.vertex.x / 0.866 + _RowOffset)) * 3.14);
                }
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = _TintColor * i.diff;;
                col.a = 0.8;
                return col;
            }
            ENDCG
        }
    }
}
