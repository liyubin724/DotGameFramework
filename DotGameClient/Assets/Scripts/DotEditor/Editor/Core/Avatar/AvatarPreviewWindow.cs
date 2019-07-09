using Dot.Core.Avator;
using Dot.Core.Entity;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace DotEditor.Core.Avatar
{
    public class AvatarPreviewWindow : OdinEditorWindow
    {
        [MenuItem("Game/Avatar/Preview",false,101)]
        public static void ShowWindow()
        {
            var win = EditorWindow.GetWindow<AvatarPreviewWindow>();
            win.Show();
        }

        public EntityNodeBehaviour skeletonNode = null;
        public AvatarPart[] aParts = new AvatarPart[0];

        [Button]
        public void Preview()
        {

        }
    }
}
