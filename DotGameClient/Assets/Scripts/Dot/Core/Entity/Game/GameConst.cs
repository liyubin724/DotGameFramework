using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Battle.Entity
{
    public static class GameConst
    {
    }

    public static class SkillConst
    {
        public static readonly string TIMELINE_BEGIN = "SkillBegin";
        public static readonly string TIMELINE_CAST = "SkillCast";
        public static readonly string TIMELINE_END = "SkillEnd";
    }

    public static class BulletConst
    {
        public static readonly string TIMELINE_BEGIN = "BulletBegin";
        public static readonly string TIMELINE_FLY = "BulletFly";
        public static readonly string TIMELINE_END = "BulletEnd";

    }
}
