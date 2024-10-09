Shader "Building/AppearShader"
{
    Properties
    {
        _BeginFillColor ("Begin fill color", Color) = (0, 0, 1, 0.5)
        _EndFillColor ("End fill color", Color) = (0, 0, 1, 1)
        _EmptyColor  ("Unfilled color", Color) = (0, 0, 0, 0)
        _ObjectCenter ("Object center", Vector) = (0, 0, 0, 0)
        _FillAmount ("Fill amount", float) = 0.5
        _BorderRadius ("Object border radius", float) = 1
        // X = 0, Y = 1, Z = 2
        _AppearAxis ("Appear axis", Integer) = 1
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
                float4 vertex     : SV_POSITION;
                float4 frag_pos   : FRAGPOS;
                bool   isFilled   : FILLBUF;
            };

            fixed4 _BeginFillColor;
            fixed4 _EndFillColor;
            fixed4 _EmptyColor;
            fixed4 _ObjectCenter;
            float  _FillAmount;
            float  _BorderRadius;
            int    _AppearAxis;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float lowest_y = -_BorderRadius;
                float border_range = 2 * _BorderRadius;
                o.frag_pos = v.vertex;
                o.isFilled = v.vertex.y - _ObjectCenter.y < (lowest_y + border_range * _FillAmount);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float lowest_y = -_BorderRadius;
                float border_range = 2 * _BorderRadius;
                bool isFilled = false;
                if (_AppearAxis == 0) {
                    isFilled = i.frag_pos.x - _ObjectCenter.x < (lowest_y + border_range * _FillAmount);
                }
                else if (_AppearAxis == 1) {
                    isFilled = i.frag_pos.y - _ObjectCenter.y < (lowest_y + border_range * _FillAmount);
                }
                else {
                    isFilled = i.frag_pos.z - _ObjectCenter.z < (lowest_y + border_range * _FillAmount);
                }
                    
                if (isFilled) {
                    fixed4 color_interpol_vec = _EndFillColor - _BeginFillColor;
                    fixed4 interpol_dir       = normalize(color_interpol_vec);
                    float  color_interpol_mag = length(color_interpol_vec);
                    return _BeginFillColor + interpol_dir * (color_interpol_mag * _FillAmount);
                }

                return _EmptyColor;
            }
            ENDCG
        }
    }
}
