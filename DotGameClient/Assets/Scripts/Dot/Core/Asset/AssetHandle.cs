using System;

namespace Dot.Core.Asset
{
    public class AssetHandle
    {
        internal long uniqueID;
        internal string[] addresses;

        internal Action<AssetHandle> releaseAction = null;

        private bool isValid = true;
        public bool IsValid
        {
            get
            {
                return isValid;
            }
            set
            {
                isValid = value;
                if(!isValid)
                {
                    uniqueID = -1;
                    addresses = null;
                    releaseAction = null;
                }
            }
        }

        public void Release()
        {
            releaseAction?.Invoke(this);
        }
    }
}
