using Dot.Core.UI;
using UnityEngine;
using UnityEngine.U2D;

public class TestAtlasImage : MonoBehaviour
{
    public SpriteAtlasImage atlasImage;
    public string spriteName;
    public SpriteAtlas newAtlas;

    private void OnGUI()
    {
        if(GUILayout.Button("Change Atlas"))
        {
            atlasImage.Atlas = newAtlas;
        }
        if (GUILayout.Button("Change SpriteName"))
        {
            atlasImage.SpriteName = spriteName;
        }
    }
}
