Shader "Custom/BlursShader"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _BlurSize("Blur Size", Float) = 10
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" }

        GrabPass {}

        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
                float2 vertColor : COLOR;
            };

            v2f vert(appdata input)
            {
                v2f output;
                output.pos = UnityObjectToClipPos(input.vertex);
                output.grabPos = ComputeGrabScreenPos(output.pos);
                output.vertColor = input.color;
                return output;
            }

            sampler2D _GrabTexture;
            fixed4 _GrabTexture_TexelSize;
            float _BlurSize;

            half4 frag(v2f input) : SV_Target
            {
                float blur = _BlurSize;
                blur = max(1, blur);

                fixed4 color = (0, 0, 0, 0);
                float weight_total = 0;

                for (float x = -blur; x <= blur; x++) {
                    float distance_normalized = abs(x / blur);
                    float weight = exp(-0.5 * pow(distance_normalized, 2) * 5.0);
                    weight_total += weight;
                    color += tex2Dproj(_GrabTexture, input.grabPos + float4(x * _GrabTexture_TexelSize.x, 0, 0, 0)) * weight;
                }

                color /= weight_total;
                return color;
            }
            ENDCG
        }

        GrabPass {}

        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
                float2 vertColor : COLOR;
            };

            v2f vert(appdata input)
            {
                v2f output;
                output.pos = UnityObjectToClipPos(input.vertex);
                output.grabPos = ComputeGrabScreenPos(output.pos);
                output.vertColor = input.color;
                return output;
            }

            sampler2D _GrabTexture;
            fixed4 _GrabTexture_TexelSize;
            float _BlurSize;

            half4 frag(v2f input) : SV_Target
            {
                float blur = _BlurSize;
                blur = max(1, blur);

                fixed4 color = (0, 0, 0, 0);
                float weight_total = 0;

                [loop]
                for (float y = -blur; y <= blur; y++) {
                    float distance_normalized = abs(y / blur);
                    float weight = exp(-0.5 * pow(distance_normalized, 2) * 5.0);
                    weight_total += weight;
                    color += tex2Dproj(_GrabTexture, input.grabPos + float4(0, y * _GrabTexture_TexelSize.y, 0, 0)) * weight;
                }

                color /= weight_total;
                return color;
            }
            ENDCG
        }

    }
    FallBack "Diffuse"
}
