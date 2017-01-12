Shader "PenBlit"
{
    Properties
	{
		_MainTex("Base(Not used)", 2D) = "white" {}
		_penColor("Color", Color) = (1,1,1,1)
		_penX("Pen X", Range(0,1)) = 0.0
		_penY("Pen Y", Range(0,1)) = 0.0
		_penSize("Pen Radius", Range(0,0.2)) = 0.05
	}
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
         
            #include "UnityCG.cginc"
			sampler2D _MainTex;
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
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = v.uv;
                return o;
            }
 
            fixed4 _penColor;
            float _penX;
            float _penY;
            float _penSize;
			float2 pos : TEXCOORD0;
 
            fixed4 frag (v2f i) : SV_Target
            {
				pos.x = _penX;
				pos.y = _penY;
            //    float x = i.uv.x;
            //    float y = i.uv.y;
				float dist = distance(pos, i.uv);
            //    float halfPen = _penSize / 2;
				if (dist < _penSize)
				{
					return _penColor;
				}

              //  if (x > (_penX - halfPen) && x < _penX + halfPen && y >(_penY - halfPen) && y < (_penY + halfPen)) {
              //      return _penColor;
              //  }
 
				half4 color = tex2D(_MainTex, i.uv);
                return color;
                //return fixed4(_penColor.rgb, 0.0);
            }
            ENDCG
        }
    }
}