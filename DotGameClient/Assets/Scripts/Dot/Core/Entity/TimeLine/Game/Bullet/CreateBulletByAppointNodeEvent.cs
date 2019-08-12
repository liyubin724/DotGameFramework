using Dot.Core.Entity.Controller;
using Dot.Core.Entity.Data;
using Dot.Core.TimeLine;

namespace Dot.Core.Entity.TimeLine.Game
{
    [TimeLineMark("Event/Bullet", "Appoint Node", TimeLineExportPlatform.ALL)]
    public class CreateBulletByAppointNodeEvent : AEventItem
    {
        public int BulletConfigID { get; set; }
        public BindNodeType NodeType { get; set; } = BindNodeType.Main;
        public int NodeIndex { get; set; }
        public bool UseEntitySpeed { get; set; } = false;

        public override void DoRevert()
        {
           
        }

        public override void Trigger()
        {
            EntitySkeletonController skeletonController = entity.GetController<EntitySkeletonController>(EntityControllerConst.SKELETON_INDEX);
            if (skeletonController == null) return;

            BindNodeData nodeData = skeletonController.GetBindNodeData(NodeType, NodeIndex);
            if (nodeData != null)
            {
                EntityFactroy.CreateBullet(entity, nodeData, BulletConfigID, UseEntitySpeed);
            }
        }
    }
}
