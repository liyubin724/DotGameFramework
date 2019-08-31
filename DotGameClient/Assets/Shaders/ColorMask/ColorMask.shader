﻿Shader "Custom/ColorMask"
{
    Properties
	{
		_ColorMask("Color Mask", Float) = 15
	}

	SubShader
	{
		Tags{"RenderType"="Opaque" "Queue"="Geometry"}

		Pass
		{
			ColorMask [_ColorMask]

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex:POSITION;
			};
			
			struct v2f
			{
				float4 vertex:SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i ):SV_Target
			{
				return fixed4(0.5,1,1,1);
			}

			ENDCG
		}
	}
}
