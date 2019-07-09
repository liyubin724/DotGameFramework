using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Core.Entity.Controller
{
    public enum EntityControllerType
    {
        Skeleton = 0,
        Avatar,
        Effect,
        Lua,
        Move,
        Max,
    }

    public abstract class EntityController
    {
        protected EntityObject Entity { get; }
        public bool Enable { get; set; }

        public EntityController(EntityObject entityObj)
        {
            Entity = entityObj;
            OnInit();
        }
        protected virtual void OnInit()
        {
            AddEventListeners();
        }
        protected abstract void AddEventListeners();
        protected abstract void RemoveEventListeners();
        public virtual void OnReset()
        {
            RemoveEventListeners();
        }
        public virtual void OnDispose()
        {
            OnReset();
        }
    }
}
