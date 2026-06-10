Shader "Image/Shadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FishTex ("Fish Texture", 2D) = "white" {}
        _VerticalFlip ("Vertical Flip", Range(0,1)) = 0   // 0=²»·­×ª£¬1=·­×ª
       // _Brightness("Shadow Brightness",Range(0,1.0))=0;
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
            sampler2D _FishTex;
            int _VerticalFlip;
            // float _Brightness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                // o.uv.y = 1 - v.uv.y;//¾µÏñ
                if (_VerticalFlip > 0.5)
                o.uv.y = 1 - v.uv.y;
                return o;
             }

           

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_FishTex, i.uv);
                // just invert the colors
                if(col.a >= 0 )
                {
                    col.rgb=0.1;
                }
               // col.rgb = 1 - col.rgb;
                return col;
            }
            ENDCG
        }
    }
}
