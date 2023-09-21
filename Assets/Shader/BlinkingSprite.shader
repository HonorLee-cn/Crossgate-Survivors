Shader "Custom/BlinkingSprite"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _MinAlpha ("Min Alpha", Range(0, 1)) = 0.2
        _MaxAlpha ("Max Alpha", Range(0, 1)) = 1.0
        _BlinkFrequency ("Blink Frequency", Range(0.1, 10)) = 1.0
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            float _MinAlpha;
            float _MaxAlpha;
            float _BlinkFrequency;
            sampler2D _MainTex;
            
            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                
                // Add a pulsating effect to the alpha channel
                float blink = sin(_Time.y * _BlinkFrequency) * 0.5 + 0.5;
                half alpha = lerp(_MinAlpha, _MaxAlpha, blink);
                
                col.a *= alpha;
                
                return col;
            }
            ENDCG
        }
    }
}
