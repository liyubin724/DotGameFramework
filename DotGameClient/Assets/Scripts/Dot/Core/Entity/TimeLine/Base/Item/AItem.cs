using System;

namespace Dot.Core.TimeLine
{
    public abstract class AItem : AEntityEnv,IComparable<AItem>
    {
        public int Index { get; set; }

        private float fireTime = 0f;
        public float FireTime
        {
            get { return fireTime; }
            set
            {
                fireTime = value;
                if (fireTime < 0)
                {
                    fireTime = 0;
                }
            }
        }

        public virtual void SetDefaults()
        {
        }

        public int CompareTo(AItem other)
        {
            if (other == null)
                return -1;
            if(FireTime>other.FireTime)
            {
                return 1;
            }else if(FireTime<other.FireTime)
            {
                return -1;
            }else
            {
                return 0;
            }
        }
    }
}
