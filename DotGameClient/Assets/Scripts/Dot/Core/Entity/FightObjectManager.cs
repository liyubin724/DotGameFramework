using Dot.Core.Entity;
using Dot.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Core.Entity
{
    public class FightObjectManager : IGlobalUpdateManager
    {
        private Dictionary<int, EntityObject> fightObjectDic = new Dictionary<int, EntityObject>();
        private Dictionary<int, UpdateFightObject> updateFightObjectDic = new Dictionary<int, UpdateFightObject>();

        public EntityObject GetFightObject(int id)
        {
            if(fightObjectDic.TryGetValue(id,out EntityObject fightObj))
            {
                return fightObj;
            }
            return null;
        }

        public void AddFightObject(int id)
        {

        }

        public int Priority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void DoDispose()
        {
            throw new NotImplementedException();
        }

        public void DoInit()
        {
            throw new NotImplementedException();
        }

        public void DoReset()
        {
            throw new NotImplementedException();
        }

        public void DoUpdate(float deltaTime)
        {
            throw new NotImplementedException();
        }
    }
}
