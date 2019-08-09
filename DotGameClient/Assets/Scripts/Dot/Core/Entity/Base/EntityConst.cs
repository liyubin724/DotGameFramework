namespace Dot.Core.Entity
{
    public static class EntityContextEventConst
    {

    }

    public static class EntityInnerEventConst
    {
        public static readonly int POSITION_ID = 101;
        public static readonly int DIRECTION_ID = 102;


        public static readonly int TRIGGER_ENTER_ID = 201;

        public static readonly int SKELETON_ADD_ID = 301;
        public static readonly int SKELETON_REMOVE_ID = 302;

        public static readonly int ARRIVED_TARGET_ID = 401;

        public static readonly int TIMELINE_ADD_ID = 501;
        public static readonly int TIMELINE_GROUP_START_ID = 502;
        public static readonly int TIMELINE_GROUP_CHANGED_ID = 503;
        public static readonly int TIMELINE_GROUP_FINISH_ID = 504;
        public static readonly int TIMELINE_END_ID = 505;

    }

    public static class EntityCategroyConst
    {
        public static readonly int PLAYER = 0;
        public static readonly int BULLET = 1;
        public static readonly int BUFF = 2;
        public static readonly int EFFECT = 3;
        public static readonly int SOUND = 4;
        public static readonly int SHIP = 5;

        private static string[] names = new string[]
        {
            "Player","Bullet","Buff","Effect","Sound","Ship",
        };

        public static string GetCategroyName(int categroy)
        {
            if (categroy >= 0 && categroy < names.Length)
                return names[categroy];

            return string.Empty;
        }
    }

    public static class EntityControllerConst
    {
        public static readonly int SKELETON_INDEX = 0;
        public static readonly int AVATAR_INDEX = 1;
        public static readonly int MOVE_INDEX = 2;
        public static readonly int EFFECT_INDEX = 3;
        public static readonly int PHYSICS_INDEX = 4;
        public static readonly int VIEW_INDEX = 5;
        public static readonly int TIMELINE_INDEX = 6;
    }
}
