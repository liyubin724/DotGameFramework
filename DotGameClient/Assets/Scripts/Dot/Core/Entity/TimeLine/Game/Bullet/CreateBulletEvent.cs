using Dot.Core.Entity.Controller;
using Dot.Core.Entity.Data;
using Dot.Core.TimeLine;

namespace Dot.Core.Entity.TimeLine.Game
{
    [TimeLineMark("Event/Bullet", "Normal", TimeLineExportPlatform.Client)]
    public class CreateBulletEvent : AEventItem
    {
        public int ConfigID { get; set; }
        public BindNodeType NodeType { get; set; } = BindNodeType.Main;
        public bool IsAllNode { get; set; } = false;
        public int NodeIndex { get; set; }

        public bool UseEntitySpeed { get; set; } = false;
        
        public override void DoRevert()
        {
            
        }

        public override void Trigger()
        {
            EntitySkeletonController skeletonController = entity.GetController<EntitySkeletonController>(EntityControllerConst.SKELETON_INDEX);
            if (skeletonController == null) return;

            if (IsAllNode)
            {
                BindNodeData[] nodeDatas = skeletonController.GetBindNodes(NodeType);
                if (nodeDatas != null && nodeDatas.Length > 0)
                {
                    foreach (var nodeData in nodeDatas)
                    {
                        CreateBullet(nodeData);
                    }
                }
            }
            else
            {
                BindNodeData nodeData = skeletonController.GetBindNodeData(NodeType, NodeIndex);
                if (nodeData != null)
                {
                    CreateBullet(nodeData);
                }
            }
        }
        
        private EntityObject CreateBullet(BindNodeData nodeData)
        {
            EntityObject bulletEntity = EntityFactroy.CreateBullet(ConfigID, nodeData.transform.position, nodeData.transform.forward);
            if (UseEntitySpeed)
            {
                BulletEntityData entityData = bulletEntity.EntityData as BulletEntityData;
                EntityMoveData moveData = entityData.GetMoveData();

                EntityMoveData shipMoveData = (entity.EntityData as ShipEntityData).GetMoveData();

                moveData.SetOriginSpeed(shipMoveData.GetSpeed());
            }
            bulletEntity.EntityData.OwnerUniqueID = entity.UniqueID;

            return bulletEntity;
        }
    }
}
