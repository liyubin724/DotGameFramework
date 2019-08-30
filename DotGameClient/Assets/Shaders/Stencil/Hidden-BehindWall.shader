Shader "Hidden/Hidden-BehindWall"
{
    Properties
	{
		[NoScaleOffset]_MainTex("Texture",2D) = "white"{}
	}

	SubShader
	{
		Cull Off
		ZWrite Off
		ZTest Always

		Tags{"Queue"="Transparent"}

		Pass
		{
			Stencil
			{
				Ref 2
				Comp Equal
			}
			SetTexture[_MainTex]
		}
	}
}
