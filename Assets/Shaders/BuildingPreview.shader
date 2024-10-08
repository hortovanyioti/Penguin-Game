Shader "Building/BuildingPreview"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PreviewColor ("Preview Color", Color) = (0, 0, 1, 0.5)
        _PulseFrequency ("Pulse Frequency", float) = 1
        _MaxOpacity ("Max opacity", float) = 1
        _MinOpacity ("Min opacity", float) = 0.5
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert alpha
            #pragma fragment frag alpha
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4    _MainTex_ST;
            fixed4    _PreviewColor;
            float     _PulseFrequency;
            float     _MaxOpacity;
            float     _MinOpacity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed get_transparency() {
                float maxmin_diff = _MaxOpacity - _MinOpacity;
                return _MinOpacity + abs(sin(_Time.y * _PulseFrequency * maxmin_diff)) * maxmin_diff;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                col = col * _PreviewColor;

                col.w = get_transparency();
                return col;
            }
            ENDCG
        }
    }
}
