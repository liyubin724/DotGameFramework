using Dot.Core.Avatar;
using Dot.Core.Entity;
using UnityEditor;

namespace DotEditor.Core.Avatar
{
    public class AvatarPreviewWindow : EditorWindow
    {
        [MenuItem("Game/Avatar/Preview",false,101)]
        public static void ShowWindow()
        {
            GetWindow<AvatarPreviewWindow>().Show();
        }

        public EntityNodeBehaviour skeletonNode = null;
        public AvatarPart[] aParts = new AvatarPart[0];

        private void OnGUI()
        {
            
        }
    }
}
