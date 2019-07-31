using Dot.Core.Pool;
using Dot.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Core.Effect
{
    public class EffectManager : Singleton<EffectManager>
    {
        private Dictionary<string, SpawnPool> spawnPoolDic = new Dictionary<string, SpawnPool>();

        public void CreateSpawnPool(string name)
        {
            if(spawnPoolDic.TryGetValue(name,out SpawnPool spawnPool))
            {

            }
        }
    }
}
