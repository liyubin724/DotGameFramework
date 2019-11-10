using Dot.Core.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dot.Core.World
{
    public partial class WorldManager : Singleton<WorldManager>
    {
        protected override void DoInit()
        {
            DoInitEvent();
            DoInitAssetLoader();
        }
    }
}

