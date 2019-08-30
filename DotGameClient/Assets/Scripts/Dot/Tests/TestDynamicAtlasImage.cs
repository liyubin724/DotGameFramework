using Dot.Core.UI;
using UnityEngine;

namespace Assets.Scripts.Dot.Tests
{
    public class TestDynamicAtlasImage : MonoBehaviour
    {
        public DynamicAtlasImage daImage;

        private void OnGUI()
        {
            if(GUILayout.Button("Change"))
            {
                int index = UnityEngine.Random.Range(0, 7);
                daImage.RawImagePath = "Test" + index;
            }
        }
    }
}
