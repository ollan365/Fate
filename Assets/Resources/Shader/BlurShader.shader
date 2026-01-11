Shader "Custom/BlursShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _BlurSizeX ("Blur Size X", Float) = 10
        _BlurSizeY ("Blur Size Y", Float) = 10
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        // -------- Pass 1: Horizontal blur --------
        GrabPass { } // capture screen BEFORE pass 1
        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                fixed4 color  : COLOR;
            };
            struct v2f {
                float4 pos     : SV_POSITION;
                float4 grabPos : TEXCOORD0; // projective coords
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos     = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            sampler2D _GrabTexture;
            float4    _GrabTexture_TexelSize; // (1/w,1/h,w,h)
            float     _BlurSizeX;

            half4 frag (v2f i) : SV_Target
            {
                float blur = max(1.0, _BlurSizeX);
                half4 color = 0;
                float  wsum = 0;

                for (float x = -blur; x <= blur; x++)
                {
                    float dn = abs(x / blur);
                    float w  = exp(-0.5 * dn * dn * 5.0); // gaussian-ish
                    wsum += w;

                    // scale by i.grabPos.w for projective coords
                    float4 proj = i.grabPos + float4(x * _GrabTexture_TexelSize.x * i.grabPos.w, 0, 0, 0);
                    color += tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(proj)) * w;
                }
                return color / wsum;
            }
            ENDCG
        }

        // -------- Pass 2: Vertical blur on result of pass 1 --------
        GrabPass { } // capture screen again (now includes pass 1 result)
        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                fixed4 color  : COLOR;
            };
            struct v2f {
                float4 pos     : SV_POSITION;
                float4 grabPos : TEXCOORD0;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos     = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            sampler2D _GrabTexture;
            float4    _GrabTexture_TexelSize;
            float     _BlurSizeY;

            half4 frag (v2f i) : SV_Target
            {
                float blur = max(1.0, _BlurSizeY);
                half4 color = 0;
                float  wsum = 0;

                for (float y = -blur; y <= blur; y++)
                {
                    float dn = abs(y / blur);
                    float w  = exp(-0.5 * dn * dn * 5.0);
                    wsum += w;

                    float4 proj = i.grabPos + float4(0, y * _GrabTexture_TexelSize.y * i.grabPos.w, 0, 0);
                    color += tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(proj)) * w;
                }
                return color / wsum;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
