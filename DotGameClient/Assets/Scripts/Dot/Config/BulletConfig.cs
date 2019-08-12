using Dot.Core.Entity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Config
{
    [Serializable]
   public class BulletConfigData
    {
        public int id;
        public string address;
        public string timelineAddress;
        public TargetType targetType = TargetType.None;
    }

    [Serializable]
    public class BulletConfig
    {
        public List<BulletConfigData> configs = new List<BulletConfigData>();
    }
}
