using UnityEngine;

namespace DotEditor.Core.TimeLine
{
    public class EditorSetting
    {
        public float timeStep = 0.1f;
        public int pixelForSecond = 100;
        public int timeHeight = 20;
        public int propertyWidth = 280;
        public int trackWidth = 100;
        public int groupWidth = 100;
        public int trackHeight = 40;
        public int groupHeight = 60;

        //runtime setting
        public Vector2 scrollPos = Vector2.zero;
        public bool isChanged = false;

        public float PixelForStep { get { return pixelForSecond * timeStep; } }
        
    }
}
