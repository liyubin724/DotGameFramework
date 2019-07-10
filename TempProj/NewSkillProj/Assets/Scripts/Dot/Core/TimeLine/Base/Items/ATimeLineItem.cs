using System;

namespace DotTimeLine.Base.Items
{
    public abstract class ATimeLineItem : ATimeLineEnv,IComparable<ATimeLineItem>
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

        public int CompareTo(ATimeLineItem other)
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
