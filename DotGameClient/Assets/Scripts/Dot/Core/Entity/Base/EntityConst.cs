namespace Dot.Core.Entity
{
    public static class EntityEventConst
    {
        public static readonly int POSITION_ID = 101;
        public static readonly int DIRECTION_ID = 102;

        public static readonly int TRIGGER_ENTER_SENDER_ID = 201;
        public static readonly int TRIGGER_ENTER_RECEIVER_ID = 202;

        public static readonly int SKELETON_ADD_ID = 301;
        public static readonly int SKELETON_REMOVE_ID = 302;

    }

    public static class EntityCategroyConst
    {
        public static readonly int PLAYER = 0;
        public static readonly int BULLET = 1;
        public static readonly int BUFF = 2;
        public static readonly int EFFECT = 3;
        public static readonly int SOUND = 4;
    }

    public static class EntityControllerConst
    {
        public static readonly int SKELETON_INDEX = 1;
        public static readonly int AVATAR_INDEX = 2;

        public static readonly int MAX_INDEX = 3;
    }
}
