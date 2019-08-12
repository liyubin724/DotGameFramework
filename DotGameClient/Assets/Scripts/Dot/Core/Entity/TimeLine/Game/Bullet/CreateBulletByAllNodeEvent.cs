using Dot.Core.Entity.Controller;
using Dot.Core.Entity.Data;
using Dot.Core.TimeLine;

namespace Dot.Core.Entity.TimeLine.Game
{
    [TimeLineMark("Event/Bullet", "All Node", TimeLineExportPlatform.ALL)]
    public class CreateBulletByAllNodeEvent : AEventItem
    {
        public int BulletConfigID { get; set; }
        public BindNodeType NodeType { get; set; } = BindNodeType.Main;
        public bool UseEntitySpeed { get; set; } = false;

        public override void DoRevert()
        {
            
        }

        public override void Trigger()
        {
            EntitySkeletonController skeletonController = entity.GetController<EntitySkeletonController>(EntityControllerConst.SKELETON_INDEX);
            if (skeletonController == null) return;

            BindNodeData[] nodeDatas = skeletonController.GetBindNodes(NodeType);
            if (nodeDatas != null && nodeDatas.Length > 0)
            {
                foreach (var nodeData in nodeDatas)
                {
                    EntityFactroy.CreateBullet(entity, nodeData, BulletConfigID, UseEntitySpeed);
                }
            }
        }
    }
}
