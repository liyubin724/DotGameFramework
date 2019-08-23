Shader "Custom/RoundRectShader"
{
	Properties
	{
		_MainTex("Base (RGBA)",2D) = "white" {}
		_RADIUSBUCE("RADIUSBUCE",Range(0,0.5)) = 0.1
	}

	SubShader
	{
		Tags{"PreviewType" = "Plane"}
		pass
		{
			CGPROGRAM

			#pragma exclude_renderers gles
			#pragma vertex vert
			#pragma fragment frag

			#include "unitycg.cginc"
			# include "RoundRS.cginc"

			float _RADIUSBUCE;
			sampler2D _MainTex;

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.modelUV = v.texcoord;
				o.radiusBuceUV = v.texcoord - float2(0.5,0.5);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col=float4(0,0,0,1);
				if(abs(i.radiusBuceUV.x) <0.5 - _RADIUSBUCE || abs(i.radiusBuceUV.y)<0.5 - _RADIUSBUCE)
				{
					col = tex2D(_MainTex,i.modelUV);
				}else
				{
					if(length(abs(i.radiusBuceUV) - float2(0.5 - _RADIUSBUCE,0.5 - _RADIUSBUCE))<_RADIUSBUCE)
					{
						col = tex2D(_MainTex,i.modelUV);
					}else
					{
						discard;
					}
				}

				return col;
			}

			ENDCG
		}
	}

}