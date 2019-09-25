using Dot.Core.UI;
using UnityEngine;
using UnityEngine.U2D;

public class TestAtlasImage : MonoBehaviour
{
    public SpriteAtlasImage atlasImage;
    public SpriteAtlas newAtlas;
    public string atlasSpriteName;

    public DynamicAtlasImage dynamicAtlasImage;
    public string dynamicSpriteName;

    public SpriteAtlasImageAnimation atlasImageAnimation;
    public int animationIndex = 0;

    private void OnGUI()
    {
        if(GUILayout.Button("Change Atlas"))
        {
            atlasImage.Atlas = newAtlas;
        }
        if (GUILayout.Button("Change SpriteName"))
        {
            atlasImage.SpriteName = atlasSpriteName;
        }

        if (GUILayout.Button("Animation Play"))
        {
            atlasImageAnimation.Play();
        }
        if (GUILayout.Button("Animation Play At"))
        {
            atlasImageAnimation.PlayAt(animationIndex);
        }
        if (GUILayout.Button("Animation Stop"))
        {
            atlasImageAnimation.Stop();
        }
        if (GUILayout.Button("Animation StopAt"))
        {
            atlasImageAnimation.StopAt(animationIndex);
        }
    }
}
