Shader "Hidden/Hidden-InfrontWall"
{
	Properties
	{
		_MainColor("Color",Color) = (1,1,1,1)
	}
	SubShader
	{
		Cull Off
		ZWrite Off
		ZTest Always

		Tags{"Queue"="Geometry"}

		Pass
		{
			Stencil
			{
				Ref 2
				Comp NotEqual
				Pass Replace
			}
			Color[_MainColor]
		}

	}
}
