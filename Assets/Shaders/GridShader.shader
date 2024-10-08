Shader "Building/GridShader"
{
    Properties
    {
        _LineWidth ("Line width", float)  = 0.01
        _GridColor ("Grid color", Color) = (0, 0, 1, 0.5)
        _CellColor ("Cell color", Color) = (0, 0, 0, 0)
        _CellSizeX ("Cell size X", float) = 3.0
        _CellSizeZ ("Cell size Z", float) = 3.0
        _GridStartPos ("Start position of grid", Vector) = (0, 0, 0, 0)
        _GridRadius ("Grid radius", float) = 10
        _InvalidColor ("Invalid color", Color) = (1, 0, 0, 0.5)
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            float  _LineWidth;
            fixed4 _GridColor;
            fixed4 _CellColor;
            float  _CellSizeX;
            float  _CellSizeZ;
            fixed4 _GridStartPos;
            float  _GridRadius;
            fixed4 _InvalidColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Apply offset to world position
                float2 wP = _GridStartPos.xz + i.worldPos.xz;
                
                // Scale point by cell size
                wP.x /= _CellSizeX;
                wP.y /= _CellSizeZ;

                // Calculate half of line width
                float lw_h = _LineWidth / 2.0;

                if (frac(wP.x) <= lw_h / 2.0
                 || frac(wP.x) >= 1.0 - lw_h / 2.0
                 || frac(wP.y) <= lw_h / 2.0
                 || frac(wP.y) >= 1.0 - lw_h / 2.0) {
                    // Check if the point is in the valid radius
                    if (length(i.worldPos.xz - _GridStartPos.xz) < _GridRadius) {
                        return _GridColor;
                    }
                    return _InvalidColor;
                }

                // No grid, return cell color
                return _CellColor;
            }
            ENDCG
        }
    }
}
