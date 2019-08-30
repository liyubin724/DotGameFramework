Shader "Hidden/Hidden-Wall"
{
    Properties
	{
		_MainColor("Color",Color) = (1,1,1,1)
	}
	SubShader
	{
		Cull Back
		ZWrite On
		ZTest Always

		Tags { "Queue" = "Background"}

		Pass
		{
			Stencil
			{
				Ref 9
				Comp Always
				Pass Replace
			}

			Color[_MainColor]
		}
	}
}
