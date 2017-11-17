Shader "Hidden/ScreenShader" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _noiseTex ("Noise (RGB)", 2D) = "white" {}
        _distortionValue ("Distortion Value", Range (-1, 1)) = 0
        _fadeColor ("Fade Color", Color) = (1,1,1,1)
    }
    SubShader {
        Pass {
        CGPROGRAM
        #pragma vertex vert_img
        #pragma fragment frag

        #include "UnityCG.cginc"

        uniform sampler2D _MainTex;
        uniform sampler2D _noiseTex;
        uniform float _distortionValue;
        uniform float4 _fadeColor;

        float4 frag(v2f_img i) : COLOR {
            float4 noiseVal = tex2D(_noiseTex, i.uv);

            float n = lerp(0, noiseVal.r, abs(_Time.w % 6-3));
            n += lerp(0, noiseVal.b, abs((_Time.w+2) % 6-3));
            n += lerp(0, noiseVal.g, abs((_Time.w+4) % 6-3));

            float2 _distortion = float2(n, 0) / _ScreenParams.xy;

            float4 c = tex2D(_MainTex, i.uv + 15 * _distortionValue * _distortion );
            
            c = lerp(c, _fadeColor, _distortionValue*_distortionValue);
            return c;
        }
        ENDCG
        }
    }
}