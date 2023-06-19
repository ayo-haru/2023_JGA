Shader "UI/UIWriteShader"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}
		_DissolveTex("Dissolution texture", 2D) = "gray" {}
		_Threshold("Threshold", Range(0., 1.0)) = 0.
		_BaseColor("BaseColor",Color) = (1,1,1,0)
		_ChangeColor("ChangeColor",Color) = (1,1,1,1)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

		Tags { "Queue" = "Transparent" }
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

			sampler2D _MainTex;
			sampler2D _DissolveTex;
			float _Threshold;
			float4 _ChangeColor;
			float4 _BaseColor;

            fixed4 frag (v2f i) : SV_Target
            {
				float4 col = tex2D(_MainTex, i.uv);
				float  val = tex2D(_DissolveTex, i.uv).r;
				col *= lerp(_BaseColor, _ChangeColor, step(_Threshold, val));
				return col;
            }
            ENDCG
        }
    }
}
