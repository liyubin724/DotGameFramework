using Dot.Core.Entity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Config
{
    [Serializable]
    public class SkillConfigData
    {
        public int id;
        public bool isNeedTarget = false;
        public string timelinePath;
        public TargetType targetType = TargetType.None;
    }
    [Serializable]
    public class SkillConfig
    {
        public List<SkillConfigData> configs = new List<SkillConfigData>();
    }
}
