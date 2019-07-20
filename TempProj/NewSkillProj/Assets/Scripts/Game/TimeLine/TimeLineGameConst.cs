namespace Game.TimeLine
{
    public static class SkillTimeLineConst
    {
        public static readonly string TIMELINE_BEGIN = "Begin";
        public static readonly string TIMELINE_CAST = "Cast";
        public static readonly string TIMELINE_END = "End";

        public static string GetNextGroupName(string curGroupName)
        {
            if(string.IsNullOrEmpty(curGroupName))
            {
                return TIMELINE_BEGIN;
            }else if(curGroupName == TIMELINE_BEGIN)
            {
                return TIMELINE_CAST;
            }else if(curGroupName == TIMELINE_CAST)
            {
                return TIMELINE_END;
            }else
            {
                return null;
            }
        }
    }

    public static class BulletTimeLineConst
    {
        public static readonly string TIMELINE_BEGIN = "Begin";
        public static readonly string TIMELINE_FLY = "Fly";
        public static readonly string TIMELINE_END = "End";

        public static string GetNextGroupName(string curGroupName)
        {
            if (string.IsNullOrEmpty(curGroupName))
            {
                return TIMELINE_BEGIN;
            }
            else if (curGroupName == TIMELINE_BEGIN)
            {
                return TIMELINE_FLY;
            }
            else if (curGroupName == TIMELINE_FLY)
            {
                return TIMELINE_END;
            }
            else
            {
                return null;
            }
        }
    }
}
