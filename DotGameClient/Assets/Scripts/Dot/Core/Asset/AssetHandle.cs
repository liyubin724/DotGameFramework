using System;

namespace Dot.Core.Asset
{
    public class AssetHandle
    {
        internal long uniqueID;
        internal string[] addresses;
        internal bool isInstance = false;
        internal Action<AssetHandle> releaseAction = null;

        public string Address
        {
            get
            {
                if(addresses!=null && addresses.Length>0)
                {
                    return addresses[0];
                }
                return null;
            }
        }

        public string[] Addresses => addresses;

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
